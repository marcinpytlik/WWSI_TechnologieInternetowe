using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AwModels;

[Table("Address", Schema="Person")]
public class Address
{
    [Key] public int AddressID { get; set; }
    public string AddressLine1 { get; set; } = null!;
    public string City { get; set; } = null!;
    public int StateProvinceID { get; set; }
    public string PostalCode { get; set; } = null!;
}
