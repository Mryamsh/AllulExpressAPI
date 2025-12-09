using AllulExpressApi.Data;
using AllulExpressApi.Models;
using AllulExpressApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly QrCodeService _qr;

    public PostsController(AppDbContext context, QrCodeService qr)
    {
        _context = context;
        _qr = qr;
    }

    // ✅ Get all posts (with client info)
    [HttpGet("getposts")]
    public async Task<ActionResult<IEnumerable<Posts>>> GetAllPosts()
    {
        var posts = await _context.Posts
            // .Include(p => p.Client)
            // .Include(p => p.driver)
            .Select(p => new
            {
                p.Id,
                p.Businessname,
                p.Poststatus,
                p.Phonenum1,
                p.Phonenum2,
                p.Exactaddress


            })

            .ToListAsync();

        return Ok(posts);
    }

    // ✅ Get post by ID
    [HttpGet("getpost/{id}")]
    public async Task<ActionResult<Posts>> GetPostById(int id)
    {
        var post = await _context.Posts
            .Include(p => p.Client)
            .Include(p => p.driver)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            return NotFound(new { message = "Post not found" });

        return Ok(post);
    }


    [HttpPost("addposts")]
    public async Task<ActionResult<Posts>> CreatePost([FromBody] Posts post)
    {
        //  1. Validate Client
        var client = await _context.Clients.FindAsync(post.ClientId);
        if (client == null)
            return BadRequest(new { message = "Invalid ClientId" });

        //  2. Set save date
        post.Savedate = DateTime.UtcNow;

        //  3. Add Post
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        //  4. Activate the assigned driver if exists
        if (post.DriverID.HasValue)
        {
            var driver = await _context.Drivers.FindAsync(post.DriverID.Value);
            if (driver != null && !driver.IsActive)
            {
                driver.IsActive = true;
                _context.Drivers.Update(driver);
                await _context.SaveChangesAsync();
            }
        }
        QrResult qr = _qr.CreatePostQr(post.Businessname, post.Savedate, post.Id);
        post.Qrcodetext = qr.Qrcodetext;
        post.Qrcode = qr.Qrcode;

        await _context.SaveChangesAsync();
        //  5. Return created post
        return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
    }




    //  Update post
    [HttpPut("updatepost/{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] Posts updatedPost)
    {
        if (id != updatedPost.Id)
            return BadRequest(new { message = "ID mismatch" });

        var post = await _context.Posts.FindAsync(id);
        if (post == null)
            return NotFound(new { message = "Post not found" });

        // Update fields
        post.Businessname = updatedPost.Businessname;
        post.City = updatedPost.City;
        post.Phonenum1 = updatedPost.Phonenum1;
        post.Phonenum2 = updatedPost.Phonenum2;
        post.Price = updatedPost.Price;
        post.Shipmentfee = updatedPost.Shipmentfee;
        post.Postnum = updatedPost.Postnum;
        post.Exactaddress = updatedPost.Exactaddress;
        post.Poststatus = updatedPost.Poststatus;
        post.ChangeOrReturn = updatedPost.ChangeOrReturn;
        post.Numberofpieces = updatedPost.Numberofpieces;
        post.Note = updatedPost.Note;

        await _context.SaveChangesAsync();

        return Ok(post);
    }

    // ✅ Delete post
    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeletePost(int id)
    // {
    //     var post = await _context.Posts.FindAsync(id);
    //     if (post == null)
    //         return NotFound(new { message = "Post not found" });

    //     _context.Posts.Remove(post);
    //     await _context.SaveChangesAsync();

    //     return Ok(new { message = "Post deleted successfully" });
    // }

    // ✅ Get posts by client ID
    [HttpGet("client/{clientId}")]
    public async Task<ActionResult<IEnumerable<Posts>>> GetPostsByClient(int clientId)
    {
        var clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
        if (!clientExists)
            return NotFound(new { message = "Client not found" });

        var posts = await _context.Posts
            .Where(p => p.ClientId == clientId)
            .ToListAsync();

        return Ok(posts);
    }
    [HttpGet("posts/counts")]
    public async Task<IActionResult> GetPostCounts()
    {
        var allCount = await _context.Posts.CountAsync();
        var todaysPostsCount = await _context.Posts
            .Where(p => p.Savedate.Date == DateTime.UtcNow.Date)
            .CountAsync();
        var returnedPostsCount = await _context.Posts
            .Where(p => p.Poststatus == "Returned")
            .CountAsync();
        var receivedPostsCount = await _context.Posts
            .Where(p => p.Poststatus == "Received")
            .CountAsync();
        var todaysReceivedPostsCount = await _context.Posts
            .Where(p => p.Poststatus == "Received" && p.Savedate.Date == DateTime.UtcNow.Date)
            .CountAsync();

        var result = new
        {
            All = allCount,
            TodaysPosts = todaysPostsCount,
            ReturnedPosts = returnedPostsCount,
            RecievedPosts = receivedPostsCount,
            TodaysRecievedPosts = todaysReceivedPostsCount
        };

        return Ok(result);
    }
    [HttpGet("posts/filter/{filterName}")]
    public async Task<IActionResult> GetPostsByFilter(string filterName)
    {
        IQueryable<Posts> query = _context.Posts;

        switch (filterName.ToLower())
        {
            case "all":
                break;
            case "todays_posts":
                query = query.Where(p => p.Savedate.Date == DateTime.UtcNow.Date);
                break;
            case "returned_posts":
                query = query.Where(p => p.Poststatus.ToLower() == "returned");
                break;
            case "recieved_posts":
                query = query.Where(p => p.Poststatus.ToLower() == "received");
                break;
            case "todays_recieved_posts":
                query = query.Where(p => p.Poststatus.ToLower() == "received"
                                         && p.Savedate.Date == DateTime.UtcNow.Date);
                break;
            default:
                return BadRequest(new { message = "Invalid filter" });
        }

        var posts = await query
            .Select(p => new
            {
                p.Id,
                p.Businessname,
                p.Poststatus,
                p.Phonenum1,
                p.Phonenum2,
                p.Exactaddress
            })
            .ToListAsync();

        return Ok(posts);
    }

}
