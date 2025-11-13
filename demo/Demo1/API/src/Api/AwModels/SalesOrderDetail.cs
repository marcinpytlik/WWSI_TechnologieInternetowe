using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AwModels;

[Table("SalesOrderDetail", Schema="Sales")]
public class SalesOrderDetail
{
    [Key] public int SalesOrderDetailID { get; set; }
    public int SalesOrderID { get; set; }
    public string? CarrierTrackingNumber { get; set; }
    public short OrderQty { get; set; }
    public int ProductID { get; set; }
    public int SpecialOfferID { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal UnitPriceDiscount { get; set; }

    public SalesOrderHeader? Header { get; set; }
}
