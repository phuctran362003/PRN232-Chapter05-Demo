using Microsoft.AspNetCore.Mvc.Filters;

namespace Presentaion.Filters;

public class ValidationLoggingFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine($"✅ VALIDATION: Starting validation for action '{context.ActionDescriptor.DisplayName}'");
        
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors != null && e.Value.Errors.Count > 0)
                .SelectMany(kvp => kvp.Value!.Errors.Select(e => new { Property = kvp.Key, Error = e.ErrorMessage }))
                .ToList();
            
            Console.WriteLine($"❌ VALIDATION: Model validation failed with {errors.Count} errors:");
            foreach (var error in errors)
            {
                Console.WriteLine($"   - Property '{error.Property}': {error.Error}");
            }
        }
        else
        {
            Console.WriteLine($"✅ VALIDATION: Model state is valid for action '{context.ActionDescriptor.DisplayName}'");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine($"✅ VALIDATION: Action '{context.ActionDescriptor.DisplayName}' execution completed with status code: {context.HttpContext.Response.StatusCode}");
    }
}