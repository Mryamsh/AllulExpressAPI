using System.ComponentModel.DataAnnotations.Schema;
using AllulExpressApi.Models;
[Table("roles")]
public class Role
{

    public int Id { get; set; }
    [Column("name")]
    public string? Name { get; set; }
    public string? Normalized_Name { get; set; }
    [Column("description")]
    public string? Description { get; set; }
    /// <summary>
    /// Link to permissions
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<Employees>? Employees { get; set; }
}
