using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AwModels;

[Table("Product", Schema="Production")]
public class Product
{
    [Key] public int ProductID { get; set; }
    public string Name { get; set; } = null!;
    public string ProductNumber { get; set; } = null!;
    public bool MakeFlag { get; set; }
    public bool FinishedGoodsFlag { get; set; }
    public short SafetyStockLevel { get; set; }
    public short ReorderPoint { get; set; }
    public decimal StandardCost { get; set; }
    public decimal ListPrice { get; set; }
    public int DaysToManufacture { get; set; }
    public DateTime SellStartDate { get; set; }
}
