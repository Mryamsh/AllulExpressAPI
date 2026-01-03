using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // for password hashing
using AllulExpressApi.Data;
using AllulExpressApi.Models;
using Microsoft.AspNetCore.Authorization;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;


    public EmployeesController(AppDbContext context)
    {
        _context = context;

    }

    // POST: api/employees
    [HttpPost("addemployee")]
    public async Task<ActionResult<Employees>> AddEmployee([FromBody] Employees employee)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);



        if (await _context.Employees.AnyAsync(e => e.Phonenum1 == employee.Phonenum1))
            return Conflict(new { message = "Phone number already in use" });

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(employee.Password);
        var role = await _context.Roles.FindAsync(employee.RoleId);
        var emplyee = new Employees
        {

            Name = employee.Name,
            Phonenum1 = employee.Phonenum1,
            Phonenum2 = employee.Phonenum2,
            Email = employee.Email,
            Password = hashedPassword,

            RoleId = role.Id,
            Role = role.Name,
            IDimagefront = employee.IDimagefront,
            IDimageback = employee.IDimageback,
            Savedate = employee.Savedate,
            Note = employee.Note,
            Enabled = employee.Enabled,
            Language = employee.Language,
        };

        try
        {
            _context.Employees.Add(emplyee);
            await _context.SaveChangesAsync();
            return Ok(emplyee);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }



    }

    [HttpGet("getEmployee/{id}")]
    public async Task<ActionResult> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.RoleNavigation) // include role
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return NotFound(new { message = "Employee not found" });

        return Ok(new
        {
            employee.Id,
            employee.Name,
            employee.Email,
            RoleId = employee.RoleId,
            RoleName = employee.RoleNavigation?.Name, // get role name dynamically
            employee.Phonenum1,
            employee.Phonenum2,
            employee.Language,
            employee.Note,
            employee.Enabled
        });
    }


    [HttpGet("getemployees")]
    public async Task<ActionResult> GetAllEmployees()
    {
        var employees = await _context.Employees
            .Include(e => e.RoleNavigation) // include role from Roles table
            .AsNoTracking()
            .Select(e => new
            {
                e.Id,
                e.Name,
                e.Email,
                RoleId = e.RoleId,
                RoleName = e.RoleNavigation != null ? e.RoleNavigation.Name : null, // get role name dynamically
                e.Phonenum1,
                e.Phonenum2,
                e.Language
            })
            .ToListAsync();

        return Ok(employees);
    }



    [HttpPut("updateEmployee/{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employees updatedEmployee)
    {
        if (id != updatedEmployee.Id)
            return BadRequest(new { message = "ID mismatch" });

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return NotFound(new { message = "Employee not found" });

        employee.Name = updatedEmployee.Name;
        employee.Email = updatedEmployee.Email;
        // employee.Role = updatedEmployee.Role;
        employee.Phonenum1 = updatedEmployee.Phonenum1;
        employee.Phonenum2 = updatedEmployee.Phonenum2;
        employee.Language = updatedEmployee.Language;
        employee.Note = updatedEmployee.Note;
        employee.Enabled = updatedEmployee.Enabled;
        employee.IDimagefront = updatedEmployee.IDimagefront;
        employee.IDimageback = updatedEmployee.IDimageback;
        employee.Savedate = updatedEmployee.Savedate;
        if (updatedEmployee.RoleId != employee.RoleId)
        {
            var role = await _context.Roles.FindAsync(updatedEmployee.RoleId);
            if (role == null)
                return BadRequest(new { message = "Invalid RoleId" });

            employee.RoleId = role.Id;      // store foreign key
            employee.Role = role.Name;      // optional: store name in string for legacy reasons
        }

        try
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Employee updated successfully", employee });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Update failed", error = ex.Message });
        }
    }
    [HttpPost("toggle-status/{id}")]
    public async Task<IActionResult> ToggleClientStatus(
        int id,
        [FromBody] bool enabled
    )
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return NotFound(new { message = "Employee not found" });

        employee.Enabled = enabled;
        await _context.SaveChangesAsync();

        string status = enabled ? "enabled" : "disabled";
        return Ok(new { message = $"Employee {status} successfully", employee.Id, employee.Enabled });
    }



}
