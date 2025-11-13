using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AwModels;

[Table("BusinessEntity", Schema = "Person")]
public class BusinessEntity
{
    [Key] public int BusinessEntityID { get; set; } // IDENTITY(1,1)
}
