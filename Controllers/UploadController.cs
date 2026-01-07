using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadController> _logger;
    // allowed mime types
    private static readonly HashSet<string> permittedExtensions = new() { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
    {
        _env = env;
        _logger = logger;
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromServices] IPermissionService permissionService)
    {

        int userId = User.GetUserId();

        var hasPermission = await permissionService.HasPermissionAsync(
            userId, "USER_CREATE"
        );

        if (!hasPermission)
            return StatusCode(403, new { message = "Permission denied" });

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        if (file.Length > MaxFileSize)
            return BadRequest(new { message = "File too large" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            return BadRequest(new { message = "Invalid file type" });

        // Create uploads folder under wwwroot
        var uploadsRoot = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);

        // Use GUID filename to avoid collisions
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsRoot, fileName);

        // Save file
        await using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        // Build accessible URL
        var requestScheme = Request.Scheme;
        var host = Request.Host.Value;
        var fileUrl = $"{requestScheme}://{host}/uploads/{fileName}";

        return Ok(new { imageUrl = fileUrl });
    }


    [HttpGet("getimage/{filename}")]
    public IActionResult GetImage(string filename, [FromServices] IPermissionService permissionService)
    {



        var uploadsPath = Path.Combine(_env.ContentRootPath, "ProtectedUploads", filename);
        if (!System.IO.File.Exists(uploadsPath))
            return NotFound();

        var fileBytes = System.IO.File.ReadAllBytes(uploadsPath);
        return File(fileBytes, "image/jpeg");
    }
}
