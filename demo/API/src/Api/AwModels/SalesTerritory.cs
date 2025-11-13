using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AwModels;

[Table("SalesTerritory", Schema="Sales")]
public class SalesTerritory
{
    [Key] public int TerritoryID { get; set; }
}
