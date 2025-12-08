using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AllulExpressApi.Data;



[Route("api/[controller]")]
[Authorize]
public class changelanguageController : ControllerBase
{

    private readonly AppDbContext _db;
    public changelanguageController(AppDbContext db)
    {
        _db = db;

    }
    [HttpPatch("employee/{id}/changelanguage")]
    public async Task<IActionResult> UpdateLanguage(int id, [FromBody] LanguageUpdateRequest request)
    {
        var user = await _db.Employees.FindAsync(id);

        if (user == null)
            return NotFound(new { message = "Employee not found" });

        user.Language = request.Language; // assuming "Language" column exists
        _db.Entry(user).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
            return Ok(new { message = "Language updated successfully" });
        }
        catch (DbUpdateException e)
        {
            return StatusCode(500, new { message = "Failed to update language", error = e.Message });
        }
    }
    public class LanguageUpdateRequest
    {
        public string Language { get; set; }
    }
}
