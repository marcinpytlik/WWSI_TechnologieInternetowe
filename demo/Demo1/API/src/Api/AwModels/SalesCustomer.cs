using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AwModels;

[Table("Customer", Schema="Sales")]
public class SalesCustomer
{
    [Key] public int CustomerID { get; set; }
    public int? PersonID { get; set; }
    public int? StoreID { get; set; }
    public int TerritoryID { get; set; }
[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string? AccountNumber { get; set; }
}
