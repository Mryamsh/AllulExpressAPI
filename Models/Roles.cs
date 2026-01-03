using System.ComponentModel.DataAnnotations.Schema;
using AllulExpressApi.Models;
[Table("roles")]
public class Role
{
    [Column("role_id")]
    public int Id { get; set; }
    [Column("name")]
    public string? Name { get; set; }
    public string? Normalized_Name { get; set; }
    [Column("description")]
    public string? Description { get; set; }

    public ICollection<Employees>? Employees { get; set; }
}
