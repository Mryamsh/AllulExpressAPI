using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllulExpressApi.Data;
using AllulExpressApi.Models;

[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly AppDbContext _context;

    public DriversController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ GET all drivers
    [HttpGet("getdrivers")]
    public async Task<ActionResult<IEnumerable<Drivers>>> GetDrivers()
    {
        var drivers = await _context.Drivers
            .Include(d => d.Cities)
             .Select(d => new
             {
                 d.Id,
                 d.Name,
                 d.Email,
                 d.Phonenum1,
                 d.Phonenum2,
                 d.Arrivedpost,
                 d.Remainedpost,
                 d.IsActive,
                 d.Cities

             })
            .ToListAsync();
        return Ok(drivers);
    }

    // ✅ GET driver by id
    [HttpGet("getdriver/{id}")]
    public async Task<ActionResult<Drivers>> GetDriver(int id)
    {
        var driver = await _context.Drivers
            .Include(d => d.Cities)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (driver == null)
            return NotFound(new { message = "Driver not found" });

        return Ok(driver);
    }

    //  POST: add new driver

    [HttpPost("adddriver")]
    public async Task<ActionResult<Drivers>> AddDriver([FromBody] Drivers driver)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Drivers.AnyAsync(d => d.Phonenum1 == driver.Phonenum1))
            return Conflict(new { message = "Phone number already in use" });

        // Handle existing cities
        var attachedCities = new List<Cities>();

        foreach (var city in driver.Cities)
        {
            // Try to find the city in DB by its Id
            var existingCity = await _context.Cities.FindAsync(city.Id);

            if (existingCity != null)
            {
                attachedCities.Add(existingCity); // Attach existing
            }
            else
            {
                attachedCities.Add(city); // Add new
            }
        }

        driver.Cities = attachedCities;
        driver.Password = BCrypt.Net.BCrypt.HashPassword(driver.Password);

        _context.Drivers.Add(driver);
        await _context.SaveChangesAsync();

        return Ok(driver);
    }


    //  PUT: update driver

    [HttpPut("updateDriver/{id}")]
    public async Task<IActionResult> UpdateDriver(int id, [FromBody] Drivers updated)
    {
        if (id != updated.Id)
            return BadRequest(new { message = "ID mismatch" });

        var driver = await _context.Drivers
            .Include(d => d.Cities)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (driver == null)
            return NotFound(new { message = "Driver not found" });

        //  Update simple fields
        driver.Name = updated.Name;
        driver.Email = updated.Email;
        driver.Phonenum1 = updated.Phonenum1;
        driver.Phonenum2 = updated.Phonenum2;
        driver.Paymentpayed = updated.Paymentpayed;
        driver.Paymentremained = updated.Paymentremained;
        driver.Arrivedpost = updated.Arrivedpost;
        driver.Remainedpost = updated.Remainedpost;
        driver.Vehicledetail = updated.Vehicledetail;
        driver.IsActive = updated.IsActive;
        driver.IDimagefront = updated.IDimagefront;
        driver.IDimageback = updated.IDimageback;
        driver.Savedate = updated.Savedate;
        driver.Note = updated.Note;
        driver.Enabled = updated.Enabled;
        driver.Language = updated.Language;


        // ✅ Update related cities safely
        if (updated.Cities != null && updated.Cities.Count > 0)
        {
            // Get existing city IDs
            var updatedCityIds = updated.Cities.Select(c => c.Id).ToList();

            // Fetch these cities from the database
            var citiesFromDb = await _context.Cities
                .Where(c => updatedCityIds.Contains(c.Id))
                .ToListAsync();

            // Replace the existing cities list
            driver.Cities = citiesFromDb;
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Driver updated successfully", driver });
    }



    [HttpGet("drivers-by-city/{cityName}")]
    public async Task<IActionResult> GetDriversByCity(string cityName)
    {
        var drivers = await _context.Drivers
            .Where(d => d.Cities.Any(c => c.City == cityName))
            .Select(d => new
            {
                d.Id,
                d.Name,
                d.Phonenum1,
                d.Phonenum2,
                d.Email
            })
            .ToListAsync();

        return Ok(drivers);
    }


    [HttpPut("toggle-status/{id}/toggle")]
    public async Task<IActionResult> ToggleDriver(int id)
    {
        var driver = await _context.Drivers.FindAsync(id);
        if (driver == null)
            return NotFound(new { message = "Driver not found" });

        driver.Enabled = !driver.Enabled;
        await _context.SaveChangesAsync();

        string status = driver.Enabled ? "enabled" : "disabled";
        return Ok(new { message = $"Driver {status}", driver.Id, driver.Enabled });
    }


    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteDriver(int id)
    // {
    //     var driver = await _context.Drivers.FindAsync(id);
    //     if (driver == null)
    //         return NotFound(new { message = "Driver not found" });

    //     _context.Drivers.Remove(driver);
    //     await _context.SaveChangesAsync();
    //     return Ok(new { message = "Driver deleted successfully" });
    // }
}
