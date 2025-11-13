using Api.Data;
using Api.AwModels;
using Api.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AwDb>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AdventureWorks2022")));

// Swagger
var cors = "_awFrontend";
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => {
    o.AddPolicy(cors, p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod());
});
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(cors); // dodaj PRZED Mapami, najlepiej zaraz po UseSwagger/UseSwaggerUI
app.MapGet("/", () => Results.Redirect("/swagger"));

// ----------------------- CREATE CUSTOMER -----------------------
app.MapPost("/aw/customers", async (AwDb db, CreateAwCustomerDto dto) =>
{
    var territoryId = dto.TerritoryId ?? await db.SalesTerritories
        .Select(t => t.TerritoryID).FirstAsync();

    // 1) BusinessEntity
    var be = new BusinessEntity();
    db.BusinessEntities.Add(be);
    await db.SaveChangesAsync(); // BusinessEntityID

    // 2) Person.Person (ten sam BusinessEntityID)
    var person = new Person
    {
        BusinessEntityID = be.BusinessEntityID,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        PersonType = "IN",
        NameStyle = false
    };
    db.People.Add(person);
    await db.SaveChangesAsync();

    // 3) Sales.Customer (AccountNumber wyliczane w bazie)
    var cust = new SalesCustomer
    {
        PersonID = be.BusinessEntityID,
        TerritoryID = territoryId
    };
    db.SalesCustomers.Add(cust);
    await db.SaveChangesAsync();

    return Results.Created($"/aw/customers/{cust.CustomerID}",
        new { cust.CustomerID, PersonId = be.BusinessEntityID, dto.FirstName, dto.LastName, TerritoryId = territoryId });
});

// ----------------------- CREATE INVOICE -----------------------
app.MapPost("/aw/invoices", async (AwDb db, CreateAwInvoiceDto dto) =>
{
    if (dto.Lines == null || dto.Lines.Count == 0)
        return Results.BadRequest("Lines required.");

    var customer = await db.SalesCustomers.FindAsync(dto.CustomerId);
    if (customer == null)
        return Results.BadRequest($"Customer {dto.CustomerId} not found.");

    var addr = await db.Addresses.OrderBy(a => a.AddressID).FirstOrDefaultAsync();
    if (addr == null)
    {
        var sp = await db.StateProvinces.OrderBy(s => s.StateProvinceID).FirstAsync();
        addr = new Address
        {
            AddressLine1 = "Demo 1",
            City = "DemoCity",
            PostalCode = "00-000",
            StateProvinceID = sp.StateProvinceID
        };
        db.Addresses.Add(addr);
        await db.SaveChangesAsync();
    }

    var shipMethodId = await db.ShipMethods
        .OrderBy(s => s.ShipMethodID).Select(s => s.ShipMethodID).FirstAsync();

    // (opcjonalnie) weryfikacja wszystkich ProductID przed transakcją
    var productIds = dto.Lines.Select(l => l.ProductId).Distinct().ToList();
    var existingIds = await db.Products
        .Where(p => productIds.Contains(p.ProductID))
        .Select(p => p.ProductID).ToListAsync();
    var missing = productIds.Except(existingIds).ToList();
    if (missing.Count > 0)
        return Results.BadRequest($"Products not found: {string.Join(", ", missing)}");

    using var tx = await db.Database.BeginTransactionAsync();
    try
    {
        // 1) Header przez EF (OK z triggerami)
        var issueDate = dto.IssueDate.ToDateTime(TimeOnly.MinValue);
        var header = new SalesOrderHeader
        {
            CustomerID      = dto.CustomerId,
            OrderDate       = issueDate,
            DueDate         = issueDate.AddDays(7),
            ShipDate        = null,
            Status          = 1,
            OnlineOrderFlag = true,
            BillToAddressID = addr.AddressID,
            ShipToAddressID = addr.AddressID,
            ShipMethodID    = shipMethodId,
            SubTotal        = 0m,
            TaxAmt          = 0m,
            Freight         = 0m
        };
        db.SalesOrderHeaders.Add(header);
        await db.SaveChangesAsync(); // SalesOrderID gotowy

        // 2) Detale: czysty SQL (bez OUTPUT) + zapewnienie SpecialOfferProduct(1, ProductID)
        decimal subTotal = 0m;

        foreach (var l in dto.Lines)
        {
            // dopilnuj SpecialOfferProduct: (No Discount = 1, ProductID)
            await db.Database.ExecuteSqlRawAsync(@"
IF NOT EXISTS (
    SELECT 1
    FROM [Sales].[SpecialOfferProduct]
    WHERE [SpecialOfferID] = 1 AND [ProductID] = @p0
)
BEGIN
    INSERT INTO [Sales].[SpecialOfferProduct] 
        ([SpecialOfferID], [ProductID], [rowguid], [ModifiedDate])
    VALUES 
        (1, @p0, NEWID(), SYSDATETIME());
END
", new SqlParameter("@p0", l.ProductId));

            await db.Database.ExecuteSqlRawAsync(@"
INSERT INTO [Sales].[SalesOrderDetail]
    ([SalesOrderID], [OrderQty], [ProductID], [SpecialOfferID], [UnitPrice], [UnitPriceDiscount])
VALUES
    (@p0, @p1, @p2, 1, @p3, @p4);",
                new SqlParameter("@p0", header.SalesOrderID),
                new SqlParameter("@p1", (short)l.Quantity),
                new SqlParameter("@p2", l.ProductId),
                new SqlParameter("@p3", l.UnitPrice),
                new SqlParameter("@p4", l.UnitPriceDiscount)
            );

            var effectiveUnit = l.UnitPrice * (1 - l.UnitPriceDiscount);
            subTotal += effectiveUnit * l.Quantity;
        }

        // 3) Podsumowania nagłówka (demo: VAT 23%) — UPDATE bez OUTPUT (triggery OK)
        var tax = Math.Round(subTotal * 0.23m, 2);
        var freight = 0m;

        await db.Database.ExecuteSqlRawAsync(@"
UPDATE [Sales].[SalesOrderHeader]
SET [SubTotal] = @p0, [TaxAmt] = @p1, [Freight] = @p2
WHERE [SalesOrderID] = @p3;",
            new SqlParameter("@p0", subTotal),
            new SqlParameter("@p1", tax),
            new SqlParameter("@p2", freight),
            new SqlParameter("@p3", header.SalesOrderID)
        );

        // upewniamy EF, że header jest „czysty”
        db.Entry(header).State = EntityState.Unchanged;

        await tx.CommitAsync();

        return Results.Created($"/aw/invoices/{header.SalesOrderID}",
            new { header.SalesOrderID, header.OrderDate, SubTotal = subTotal, TaxAmt = tax, Freight = freight });
    }
    catch (Exception ex)
    {
        await tx.RollbackAsync();
        return Results.Problem(title: "Invoice create failed", detail: ex.Message, statusCode: 500);
    }
});

// ----------------------- GET LISTS -----------------------
app.MapGet("/aw/products", async (AwDb db) =>
    Results.Ok(await db.Products.OrderBy(p => p.ProductID).Take(100).ToListAsync()));

app.MapGet("/aw/customers", async (AwDb db) =>
    Results.Ok(await db.SalesCustomers.OrderBy(c => c.CustomerID).Take(100).ToListAsync()));

app.MapGet("/aw/invoices", async (AwDb db) =>
    Results.Ok(await db.SalesOrderHeaders.OrderByDescending(h => h.SalesOrderID).Take(50).ToListAsync()));

// ----------------------- GET INVOICE WITH LINES -----------------------
// ----------------------- GET INVOICE WITH LINES -----------------------
app.MapGet("/aw/invoices/{id:int}", async (AwDb db, int id) =>
{
    var header = await db.SalesOrderHeaders
        .Where(h => h.SalesOrderID == id)
        .Select(h => new
        {
            h.SalesOrderID,
            h.OrderDate,
            h.DueDate,
            h.CustomerID,
            h.SubTotal,
            h.TaxAmt,
            h.Freight
        })
        .FirstOrDefaultAsync();

    if (header is null)
        return Results.NotFound($"Invoice {id} not found.");

    var lines = await db.SalesOrderDetails
        .Where(d => d.SalesOrderID == id)
        .OrderBy(d => d.SalesOrderDetailID)
        .Select(d => new
        {
            d.SalesOrderDetailID,
            d.ProductID,
            d.OrderQty,
            d.UnitPrice,
            d.UnitPriceDiscount,
            LineTotal = d.UnitPrice * (1 - d.UnitPriceDiscount) * d.OrderQty
        })
        .ToListAsync();

    var sub = lines.Sum(l => l.LineTotal);
    var tax = Math.Round(sub * 0.23m, 2);
    var freight = header.Freight; // <-- bez ?? 0m, bo to decimal NOT NULL

    return Results.Ok(new
    {
        header.SalesOrderID,
        header.OrderDate,
        header.DueDate,
        header.CustomerID,
        Totals = new
        {
            SubTotal = sub,
            TaxAmt = tax,
            Freight = freight,
            TotalDue = sub + tax + freight
        },
        Lines = lines
    });
});


app.Run();
