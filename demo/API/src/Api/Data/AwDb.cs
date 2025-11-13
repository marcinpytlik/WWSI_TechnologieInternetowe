using Api.AwModels;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AwDb : DbContext
{
    public AwDb(DbContextOptions<AwDb> options) : base(options) { }
    public DbSet<BusinessEntity> BusinessEntities => Set<BusinessEntity>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<SalesCustomer> SalesCustomers => Set<SalesCustomer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SalesOrderHeader> SalesOrderHeaders => Set<SalesOrderHeader>();
    public DbSet<SalesOrderDetail> SalesOrderDetails => Set<SalesOrderDetail>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<StateProvince> StateProvinces => Set<StateProvince>();
    public DbSet<ShipMethod> ShipMethods => Set<ShipMethod>();
    public DbSet<SalesTerritory> SalesTerritories => Set<SalesTerritory>();

    protected override void OnModelCreating(ModelBuilder b)
{
    // --- Core mapowania tabel ---
    b.Entity<BusinessEntity>().ToTable("BusinessEntity", "Person")
        .HasKey(x => x.BusinessEntityID);

    b.Entity<Person>().ToTable("Person", "Person")
        .HasKey(x => x.BusinessEntityID);

    b.Entity<SalesCustomer>().ToTable("Customer", "Sales")
        .HasKey(x => x.CustomerID);

    b.Entity<Product>().ToTable("Product", "Production")
        .HasKey(x => x.ProductID);

    b.Entity<SalesOrderHeader>().ToTable("SalesOrderHeader", "Sales")
        .HasKey(x => x.SalesOrderID);

    b.Entity<SalesOrderDetail>(e =>
    {
        e.ToTable("SalesOrderDetail", "Sales");
        // PK złożony jak w AW
        e.HasKey(x => new { x.SalesOrderID, x.SalesOrderDetailID });
        // IDENTITY(1,1) — baza generuje wartość
        e.Property(x => x.SalesOrderDetailID).UseIdentityColumn();

        // typy pieniężne
        e.Property(x => x.UnitPrice).HasColumnType("money");
        e.Property(x => x.UnitPriceDiscount).HasColumnType("money");
    });

    b.Entity<SalesOrderHeader>()
        .HasMany(x => x.Details)
        .WithOne(d => d.Header)
        .HasForeignKey(d => d.SalesOrderID);

    b.Entity<Address>().ToTable("Address", "Person").HasKey(x => x.AddressID);
    b.Entity<StateProvince>().ToTable("StateProvince", "Person").HasKey(x => x.StateProvinceID);
    b.Entity<ShipMethod>().ToTable("ShipMethod", "Purchasing").HasKey(x => x.ShipMethodID);
    b.Entity<SalesTerritory>().ToTable("SalesTerritory", "Sales").HasKey(x => x.TerritoryID);

    // --- Kolumny wyliczane / computed ---
    b.Entity<SalesCustomer>(e =>
    {
        e.Property(p => p.AccountNumber)
         .HasComputedColumnSql("[dbo].[ufnGetCustomerAccountNumber]([PersonID])", stored: false)
         .ValueGeneratedOnAddOrUpdate();
    });

    // --- Typy pieniężne = money ---
    b.Entity<Product>(e =>
    {
        e.Property(p => p.ListPrice).HasColumnType("money");
        e.Property(p => p.StandardCost).HasColumnType("money");
    });

    b.Entity<SalesOrderHeader>(e =>
    {
        e.Property(h => h.SubTotal).HasColumnType("money");
        e.Property(h => h.TaxAmt).HasColumnType("money");
        e.Property(h => h.Freight).HasColumnType("money");
    });
}




    }

