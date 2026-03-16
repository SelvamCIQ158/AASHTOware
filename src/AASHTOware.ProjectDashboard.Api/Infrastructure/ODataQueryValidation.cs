using System.Text.Json;

namespace AASHTOware.ProjectDashboard.Api.Infrastructure;

/// <summary>
/// Middleware that catches unhandled exceptions and returns a structured OData-compliant
/// error response instead of leaking stack traces or returning empty 500 responses.
/// In Development environments the original exception details are included in an inner error.
/// </summary>
public sealed class ODataQueryValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ODataQueryValidationMiddleware> _logger;

    public ODataQueryValidationMiddleware(RequestDelegate next, ILogger<ODataQueryValidationMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request.");
            await WriteErrorResponseAsync(context, ex);
        }
    }

    private async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();

        object errorBody;

        if (environment.IsDevelopment())
        {
            errorBody = new
            {
                error = new
                {
                    code = "InternalError",
                    message = "An unexpected error occurred. Please try again later.",
                    innererror = new
                    {
                        message = exception.Message,
                        type = exception.GetType().FullName,
                        stacktrace = exception.StackTrace
                    }
                }
            };
        }
        else
        {
            errorBody = new
            {
                error = new
                {
                    code = "InternalError",
                    message = "An unexpected error occurred. Please try again later."
                }
            };
        }

        var json = JsonSerializer.Serialize(errorBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension methods for registering <see cref="ODataQueryValidationMiddleware"/> in the pipeline.
/// </summary>
public static class ODataQueryValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseODataQueryValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ODataQueryValidationMiddleware>();
    }
}
