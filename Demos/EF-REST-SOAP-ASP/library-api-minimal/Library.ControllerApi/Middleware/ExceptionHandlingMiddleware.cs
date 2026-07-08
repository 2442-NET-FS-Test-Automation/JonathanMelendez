using System.Text.Json;

namespace Library.ControllerApi.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> log)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _log = log;

    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await _next(ctx); }
        catch (Exception e) 
        { 
            _log.LogError(e, "Unhandled exception on {Path}", ctx.Request.Path); 
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize( new
            {
                error = "An unexpected error ocurred.",
                traceId = ctx.TraceIdentifier
            }));
        }
    }
}