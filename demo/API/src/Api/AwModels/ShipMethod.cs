using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AwModels;

[Table("ShipMethod", Schema="Purchasing")]
public class ShipMethod
{
    [Key] public int ShipMethodID { get; set; }
}
