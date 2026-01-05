public class Permission
{
    public int Id { get; set; }

    /// <summary>
    /// Unique key/code to identify this permission
    /// e.g., "USER_CREATE", "POSTS_VIEW"
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Module this permission belongs to (User, Posts, etc.)
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// When the permission was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Navigation property to RolePermission (many-to-many)
    /// </summary>
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}



public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}
