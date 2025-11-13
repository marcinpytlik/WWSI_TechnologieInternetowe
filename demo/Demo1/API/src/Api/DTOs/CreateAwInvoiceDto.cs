namespace Api.DTOs;
public record CreateAwInvoiceLineDto(int ProductId, int Quantity, decimal UnitPrice, decimal UnitPriceDiscount);
public record CreateAwInvoiceDto(int CustomerId, DateOnly IssueDate, List<CreateAwInvoiceLineDto> Lines);
