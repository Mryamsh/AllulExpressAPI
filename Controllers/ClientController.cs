using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllulExpressApi.Data;
using AllulExpressApi.Models;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/client/allclients?page=1
    [HttpGet("allclients")]
    public async Task<IActionResult> GetClients(
        [FromServices] IPermissionService permissionService,
        [FromQuery] int page = 1
    )
    {
        const int pageSize = 10;

        int userId = User.GetUserId();

        var hasPermission = await permissionService.HasPermissionAsync(userId, "USER_VIEW");
        if (!hasPermission)
            return StatusCode(403, new { message = "Permission denied" });

        if (page < 1) page = 1;

        var totalCount = await _context.Clients.CountAsync();

        var clients = await _context.Clients
            .OrderBy(c => c.Id) // IMPORTANT for stable pagination
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Email,
                c.Business,
                c.Phonenum1,
                c.Phonenum2,
                c.Totalpaymentpayed,
                c.Totalposts
            })
            .ToListAsync();

        return Ok(new
        {
            page,
            pageSize,
            totalCount,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            data = clients
        });
    }



    // GET: api/client/5
    [HttpGet("getclient/{id}")]
    public async Task<ActionResult<Clients>> GetClient(int id, [FromServices] IPermissionService permissionService)
    {

        int userId = User.GetUserId();

        var hasPermission = await permissionService.HasPermissionAsync(
            userId, "USER_VIEW"
        );

        if (!hasPermission)
            return StatusCode(403, new { message = "Permission denied" });
        var client = await _context.Clients
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return NotFound(new { message = "Client not found" });

        return Ok(client);
    }

    // POST: api/client
    [HttpPost("addclients")]
    public async Task<ActionResult<Clients>> AddClient([FromBody] Clients client, [FromServices] IPermissionService permissionService)
    {


        int userId = User.GetUserId();

        var hasPermission = await permissionService.HasPermissionAsync(
            userId, "USER_CREATE"
        );

        if (!hasPermission)
            return StatusCode(403, new { message = "Permission denied" });


        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Clients.AnyAsync(c => c.Phonenum1 == client.Phonenum1))
            return Conflict(new { message = "Phone number already in use" });

        // Hash password if provided
        if (!string.IsNullOrEmpty(client.Password))
        {
            client.Password = BCrypt.Net.BCrypt.HashPassword(client.Password);
        }

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return Ok(client);
    }

    // PUT: api/client/5
    [HttpPut("updateclient/{id}")]
    public async Task<IActionResult> UpdateClient(int id, [FromBody] Clients updated, [FromServices] IPermissionService permissionService)
    {


        int userId = User.GetUserId();

        var hasPermission = await permissionService.HasPermissionAsync(
            userId, "USER_UPDATE"
        );

        if (!hasPermission)
            return StatusCode(403, new { message = "Permission denied" });
        if (id != updated.Id)
            return BadRequest(new { message = "ID mismatch" });

        var client = await _context.Clients
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return NotFound(new { message = "Client not found" });

        // Update fields
        client.Name = updated.Name;
        client.Business = updated.Business;
        client.Email = updated.Email;
        client.Phonenum1 = updated.Phonenum1;
        client.Phonenum2 = updated.Phonenum2;
        if (!string.IsNullOrEmpty(updated.Password))
            client.Password = BCrypt.Net.BCrypt.HashPassword(updated.Password);
        client.Equipment = updated.Equipment;
        client.Cashmoney = updated.Cashmoney;
        client.Balance = updated.Balance;
        client.Totalpaymentpayed = updated.Totalpaymentpayed;
        client.Totalposts = updated.Totalposts;
        client.Returnedposts = updated.Returnedposts;
        client.Savedate = updated.Savedate;
        client.Note = updated.Note;
        client.Enabled = updated.Enabled;
        client.Language = updated.Language;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Client updated successfully", client });
    }

    // PUT: api/client/5/enable
    [HttpPost("toggle-status/{id}/toggle")]
    public async Task<IActionResult> ToggleClient(int id, [FromServices] IPermissionService permissionService)
    {

        int userId = User.GetUserId();

        var hasPermission = await permissionService.HasPermissionAsync(
            userId, "USER_ENABLE"
        );

        if (!hasPermission)
            return StatusCode(403, new { message = "Permission denied" });
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            return NotFound(new { message = "Client not found" });

        client.Enabled = !client.Enabled;
        await _context.SaveChangesAsync();

        string status = client.Enabled ? "enabled" : "disabled";
        return Ok(new
        {
            message = $"Client {status}",
            client.Id,
            client.Enabled
        });
    }

}
