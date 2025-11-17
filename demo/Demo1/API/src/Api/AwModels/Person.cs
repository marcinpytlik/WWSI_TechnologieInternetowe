using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AwModels;

[Table("Person", Schema="Person")]
public class Person
{
    [Key] public int BusinessEntityID { get; set; }
    public string PersonType { get; set; } = "IN";
    public bool NameStyle { get; set; } = false;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
