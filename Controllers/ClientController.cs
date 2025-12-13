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

    // GET: api/client
    [HttpGet("allclients")]
    public async Task<ActionResult<IEnumerable<object>>> GetClients()
    {
        var clients = await _context.Clients
            .Include(c => c.Posts) // optional, only if you need some info from posts
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
                // add only the columns you want
                //  PostCount = c.Posts.Count // example: number of posts
            })
            .ToListAsync();

        return Ok(clients);
    }


    // GET: api/client/5
    [HttpGet("getclient/{id}")]
    public async Task<ActionResult<Clients>> GetClient(int id)
    {
        var client = await _context.Clients
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return NotFound(new { message = "Client not found" });

        return Ok(client);
    }

    // POST: api/client
    [HttpPost("addclients")]
    public async Task<ActionResult<Clients>> AddClient([FromBody] Clients client)
    {
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
    public async Task<IActionResult> UpdateClient(int id, [FromBody] Clients updated)
    {
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
    [HttpPut("toggle-status/{id}")]
    public async Task<IActionResult> ToggleClientStatus(int id, [FromBody] bool enabled)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            return NotFound(new { message = "Client not found" });

        client.Enabled = enabled;
        await _context.SaveChangesAsync();

        string status = enabled ? "enabled" : "disabled";
        return Ok(new { message = $"Client {status} successfully", client.Id, client.Enabled });
    }
}
