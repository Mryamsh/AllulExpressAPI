using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public UploadController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("uploadimage")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        try
        {
            // Use WebRootPath if available, otherwise fallback
            var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var fileUrl = $"{baseUrl}/uploads/{fileName}";

            return Ok(new { url = fileUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

}
