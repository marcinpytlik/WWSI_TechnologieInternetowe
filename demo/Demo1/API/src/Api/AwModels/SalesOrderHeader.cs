using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AwModels;

[Table("SalesOrderHeader", Schema="Sales")]
public class SalesOrderHeader
{
    [Key] public int SalesOrderID { get; set; }
    public int CustomerID { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ShipDate { get; set; }
    public byte Status { get; set; }
    public bool OnlineOrderFlag { get; set; }
    public int BillToAddressID { get; set; }
    public int ShipToAddressID { get; set; }
    public int ShipMethodID { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmt { get; set; }
    public decimal Freight { get; set; }

    public List<SalesOrderDetail> Details { get; set; } = new();
}
