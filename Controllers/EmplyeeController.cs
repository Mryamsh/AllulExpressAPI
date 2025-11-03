using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // for password hashing
using AllulExpressApi.Data;
using AllulExpressApi.Models;
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;


    public EmployeesController(AppDbContext context)
    {
        _context = context;

    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<Employees>> AddEmployee([FromBody] Employees employee)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);



        if (await _context.Employees.AnyAsync(e => e.Phonenum1 == employee.Phonenum1))
            return Conflict(new { message = "Phone number already in use" });

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(employee.Password);

        var emplyee = new Employees
        {

            Name = employee.Name,
            Phonenum1 = employee.Phonenum1,
            Phonenum2 = employee.Phonenum2,
            Email = employee.Email,
            Password = hashedPassword,

            Role = employee.Role,
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

    [HttpGet("{id}")]
    public async Task<ActionResult> GetEmployee(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking() // Optional: improves performance for read-only
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return NotFound(new { message = "Employee not found" });

        // Return DTO without password
        var employeeDto = new
        {
            employee.Id,
            employee.Name,
            employee.Email,
            employee.Role,
            employee.IDimagefront,
            employee.IDimageback,
            employee.Phonenum1,
            employee.Phonenum2,
            employee.Language,
            employee.Note,
            employee.Enabled
        };

        return Ok(employeeDto);
    }

    [HttpGet]
    public async Task<ActionResult> GetAllEmployees()
    {
        var employees = await _context.Employees
            .AsNoTracking()
            .Select(e => new
            {
                e.Id,
                e.Name,
                e.Email,
                e.Role,
                e.IDimagefront,
                e.IDimageback,
                e.Phonenum1,
                e.Phonenum2,
                e.Language,
                e.Note,
                e.Enabled
            })
            .ToListAsync();

        return Ok(employees);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employees updatedEmployee)
    {
        if (id != updatedEmployee.Id)
            return BadRequest(new { message = "ID mismatch" });

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return NotFound(new { message = "Employee not found" });

        employee.Name = updatedEmployee.Name;
        employee.Email = updatedEmployee.Email;
        employee.Role = updatedEmployee.Role;
        employee.Phonenum1 = updatedEmployee.Phonenum1;
        employee.Phonenum2 = updatedEmployee.Phonenum2;
        employee.Language = updatedEmployee.Language;
        employee.Note = updatedEmployee.Note;
        employee.Enabled = updatedEmployee.Enabled;
        employee.IDimagefront = updatedEmployee.IDimagefront;
        employee.IDimageback = updatedEmployee.IDimageback;
        employee.Savedate = updatedEmployee.Savedate;

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
    [HttpPut("{id}/enable")]
    public async Task<IActionResult> ToggleClientStatus(int id, [FromBody] bool enabled)
    {
        var emplyee = await _context.Employees.FindAsync(id);
        if (emplyee == null)
            return NotFound(new { message = "Employee not found" });

        emplyee.Enabled = enabled;
        await _context.SaveChangesAsync();

        string status = enabled ? "enabled" : "disabled";
        return Ok(new { message = $"Employee {status} successfully", emplyee.Id, emplyee.Enabled });
    }



}
