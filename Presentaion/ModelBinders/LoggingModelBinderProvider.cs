using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Presentaion.ModelBinders;

// Model binder provider that wraps existing model binders with logging
public class LoggingModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        // Get the original binder from other providers
        var originalBinder = GetOriginalBinder(context);
        if (originalBinder != null)
            // Wrap it with our logging binder
            return new LoggingModelBinder(originalBinder);

        return null;
    }

    private IModelBinder? GetOriginalBinder(ModelBinderProviderContext context)
    {
        // Try to get binder from other providers (skip ourselves)
        var providers = context.Services.GetRequiredService<IOptions<MvcOptions>>().Value.ModelBinderProviders;

        foreach (var provider in providers)
            if (provider != this)
            {
                var binder = provider.GetBinder(context);
                if (binder != null) return binder;
            }

        return null;
    }
}

// We'll use a different approach - implement a model binder directly without a provider
public class LoggingModelBinder : IModelBinder
{
    private readonly IModelBinder _innerBinder;

    public LoggingModelBinder(IModelBinder innerBinder)
    {
        _innerBinder = innerBinder ?? throw new ArgumentNullException(nameof(innerBinder));
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

        Console.WriteLine(
            $"📦 MODEL BINDING: Starting binding for model '{bindingContext.ModelName}' of type '{bindingContext.ModelType.Name}'");

        // Determine the source of the binding
        var source = DetermineBindingSource(bindingContext);
        Console.WriteLine($"📦 MODEL BINDING: Binding source for '{bindingContext.ModelName}' is {source}");

        // Call the inner binder to do the actual binding
        await _innerBinder.BindModelAsync(bindingContext);

        // Log the result
        if (bindingContext.Result.IsModelSet)
            Console.WriteLine(
                $"📦 MODEL BINDING: Successfully bound model '{bindingContext.ModelName}' to value: {bindingContext.Result.Model}");
        else
            Console.WriteLine(
                $"📦 MODEL BINDING: Failed to bind model '{bindingContext.ModelName}'. Errors: {string.Join(", ", bindingContext.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
    }

    private string DetermineBindingSource(ModelBindingContext bindingContext)
    {
        try
        {
            // Check if it's from route
            if (bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName) &&
                bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString()
                    .Contains("Microsoft.AspNetCore.Routing"))
                return "Route";

            // Check if it's from query string
            if (bindingContext.ActionContext.HttpContext.Request.Query.ContainsKey(bindingContext.ModelName))
                return "Query String";

            // Check if it's from form
            if (bindingContext.ActionContext.HttpContext.Request.HasFormContentType &&
                bindingContext.ActionContext.HttpContext.Request.Form.ContainsKey(bindingContext.ModelName))
                return "Form";

            // Check if it's potentially from body
            if (bindingContext.ActionContext.HttpContext.Request.ContentType?.Contains("application/json") == true)
                return "Request Body (JSON)";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error determining binding source: {ex.Message}");
        }

        return "Unknown";
    }
}