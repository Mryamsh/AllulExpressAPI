using System.Security.Claims;
using AllulExpressApi.Data;
using Microsoft.EntityFrameworkCore;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(int userId, string permissionCode);
}

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _db;

    public PermissionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> HasPermissionAsync(int userId, string permissionCode)
    {
        return await _db.Employees
            .Where(e => e.Id == userId && e.Enabled)
            .SelectMany(e => e.RoleNavigation.RolePermissions)
            .AnyAsync(rp => rp.Permission.Code == permissionCode);
    }
}


public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
}
