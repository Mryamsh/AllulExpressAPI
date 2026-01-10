using AllulExpressApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AllulExpressApi.Models;
using AllulExpressApi.Data;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public LoginController(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {


        // 1 fetch user from DB
        var user = await _db.Employees
     .FirstOrDefaultAsync(u => u.Phonenum1 == request.Phonenum1);

        if (user == null)
        {
            // user not found
            return Unauthorized(new { message = "Invalid phone or password" });
        }

        if (!user.Enabled) // or user.IsActive == false
        {
            return StatusCode(403, new
            {
                message = "Your account has been disabled. Please contact support."
            });
        }

        //  Verify the password (hashed)
        bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!isValidPassword)
        {
            // password incorrect
            return Unauthorized(new { message = "Invalid phone or password" });
        }


        //  generate JWT
        var token = _tokenService.GenerateToken(user);

        // 3️⃣ save in ValidTokens
        _db.ValidTokens.Add(new ValidToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });


        await _db.SaveChangesAsync();

        // 4️⃣ return token & role
        return Ok(new { Token = token, Role = user.Role, roleID = user.RoleId, user.Email, user.Language, user.Id });
    }




    [HttpGet("validate-token")]
    public async Task<IActionResult> ValidateToken([FromQuery] string token)
    {
        var tokenRecord = await _db.ValidTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (tokenRecord == null)
            return Unauthorized(new { valid = false, reason = "Token not found" });

        if (tokenRecord.ExpiresAt < DateTime.UtcNow)
            return Unauthorized(new { valid = false, reason = "Token expired" });

        return Ok(new { valid = true });
    }

}
