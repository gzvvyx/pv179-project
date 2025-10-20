namespace API.Middleware;

public class ResponseFormatMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseFormatMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Query.TryGetValue("format", out var format))
        {
            var value = format.ToString().ToLowerInvariant();

            if (value == "json")
            {
                context.Response.ContentType = "application/json";
            }
            else if (value == "xml")
            {
                context.Response.ContentType = "application/xml";
            }
        }
        
        await _next(context);
    }
}
