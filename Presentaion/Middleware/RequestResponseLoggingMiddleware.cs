using System.Text;

namespace Presentaion.Middleware;

/// <summary>
///     Middleware for logging HTTP requests and responses to understand the routing and model binding process
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the incoming request
        await LogRequest(context);

        // Enable response body buffering so we can read it multiple times
        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            // Call the next middleware
            await _next(context);

            // Log the response
            await LogResponse(context, responseBodyStream);
        }
        finally
        {
            // Copy the response back to the original stream
            if (responseBodyStream.Position > 0)
                responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequest(HttpContext context)
    {
        // Log request details
        var requestMethod = context.Request.Method;
        var requestPath = context.Request.Path;
        var queryString = context.Request.QueryString;
        var routeValues = context.Request.RouteValues;

        Console.WriteLine($"🧭 ROUTING: Request received - {requestMethod} {requestPath}{queryString}");

        // Log route values if any
        if (routeValues != null && routeValues.Count > 0)
        {
            Console.WriteLine("🧭 ROUTING: Route values:");
            foreach (var routeValue in routeValues) Console.WriteLine($"   - {routeValue.Key}: {routeValue.Value}");
        }

        // Log query parameters
        if (queryString.HasValue && queryString.Value.Length > 1)
        {
            Console.WriteLine("📦 MODEL BINDING: Query parameters:");
            foreach (var query in context.Request.Query) Console.WriteLine($"   - {query.Key}: {query.Value}");
        }

        // Check for request body (for POST/PUT/PATCH)
        if (context.Request.ContentLength > 0 &&
            (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "PATCH"))
        {
            context.Request.EnableBuffering();
            var buffer = new byte[context.Request.ContentLength.Value];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);
            context.Request.Body.Position = 0;

            Console.WriteLine($"📦 MODEL BINDING: Request body (from {context.Request.ContentType}):");
            Console.WriteLine($"   {requestBody}");
        }
    }

    private async Task LogResponse(HttpContext context, MemoryStream responseBodyStream)
    {
        // Read the response body
        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

        // Log response details
        Console.WriteLine($"🧭 ROUTING: Response - Status Code {context.Response.StatusCode}");

        // Check if it's a validation error (400)
        if (context.Response.StatusCode == 400 && !string.IsNullOrEmpty(responseBody))
        {
            Console.WriteLine("✅ VALIDATION: Validation failed with the following errors:");
            Console.WriteLine($"   {responseBody}");
        }

        // Reset stream position for the next middleware
        responseBodyStream.Seek(0, SeekOrigin.Begin);
    }
}

// Extension method to add the middleware to the pipeline
public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}