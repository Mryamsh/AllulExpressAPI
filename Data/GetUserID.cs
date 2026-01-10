using AllulExpressApi.Data;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst("id"); // or ClaimTypes.NameIdentifier

            if (userIdClaim != null)
            {
                db.CurrentUserId = int.Parse(userIdClaim.Value);
            }
        }

        await _next(context);
    }
}
