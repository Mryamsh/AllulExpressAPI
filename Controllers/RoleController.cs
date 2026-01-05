using AllulExpressApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RolesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _context.Roles
            .AsNoTracking()
            .Select(r => new
            {
                r.Id,
                r.Name
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleWithPermissions(int id)
    {
        var role = await _context.Roles
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.Name,
                Permissions = r.RolePermissions
                               .Select(rp => new
                               {
                                   //  rp.Permission.Id,
                                   rp.Permission.Code,
                                   // rp.Permission.Name,
                                   //rp.Permission.Description,
                                   //rp.Permission.Module
                               })
                               .ToList()
            })
            .FirstOrDefaultAsync();

        if (role == null) return NotFound();

        return Ok(role);
    }

}
