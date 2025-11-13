using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AwModels;

[Table("StateProvince", Schema="Person")]
public class StateProvince
{
    [Key] public int StateProvinceID { get; set; }
}
