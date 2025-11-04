using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllulExpressApi.Data;
using AllulExpressApi.Models;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CityController : ControllerBase
{
    private readonly AppDbContext _context;

    public CityController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet("allcities")]
    public async Task<ActionResult<IEnumerable<Cities>>> GetAllCityFees()
    {
        var cityFees = await _context.Cities
            .Include(c => c.Drivers)
            .ToListAsync();
        return Ok(cityFees);
    }

    [HttpGet("getcity/{id}")]
    public async Task<ActionResult<Cities>> GetCityFee(int id)
    {
        var cityFee = await _context.Cities
            .Include(c => c.Drivers)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cityFee == null)
            return NotFound(new { message = "CityFee not found" });

        return Ok(cityFee);
    }


    [HttpPost("addcity")]
    public async Task<ActionResult<Cities>> AddCityFee([FromBody] Cities newCityFee)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Cities.Add(newCityFee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCityFee), new { id = newCityFee.Id }, newCityFee);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCityFee(int id, [FromBody] Cities updated)
    {
        if (id != updated.Id)
            return BadRequest(new { message = "ID mismatch" });

        var cityFee = await _context.Cities.FindAsync(id);
        if (cityFee == null)
            return NotFound(new { message = "CityFee not found" });

        cityFee.City = updated.City;
        cityFee.totalfee = updated.totalfee;
        cityFee.Driverfee = updated.Driverfee;
        cityFee.benifit = updated.benifit;

        await _context.SaveChangesAsync();

        return Ok(new { message = "CityFee updated successfully", cityFee });
    }


    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteCityFee(int id)
    // {
    //     var cityFee = await _context.CityFees.FindAsync(id);
    //     if (cityFee == null)
    //         return NotFound(new { message = "CityFee not found" });

    //     _context.CityFees.Remove(cityFee);
    //     await _context.SaveChangesAsync();

    //     return Ok(new { message = "CityFee deleted successfully" });
    // }
}
