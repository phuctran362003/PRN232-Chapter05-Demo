using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Presentaion.Filters;
using Presentaion.ModelBinders;
using Repository.Entities;
using Repository.Repositories;
using Service.Interfaces;
using Service.Services;
using Service.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add controllers with logging for validation
builder.Services.AddControllers(options =>
{
    // Add validation logging filter
    options.Filters.Add<ValidationLoggingFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Log validation errors in the ModelState
    var builtInFactory = options.InvalidModelStateResponseFactory;
    
    options.InvalidModelStateResponseFactory = context =>
    {
        // Log all validation errors
        Console.WriteLine($"❌ MODEL VALIDATION: Failed with {context.ModelState.ErrorCount} errors:");
        foreach (var state in context.ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($"   - Property '{state.Key}': {error.ErrorMessage}");
            }
        }
        
        // Call the default factory
        return builtInFactory(context);
    };
});

// Register the custom model binder provider
builder.Services.Configure<MvcOptions>(options =>
{
    // Add at the beginning of the provider list to ensure it wraps all other providers
    options.ModelBinderProviders.Insert(0, new LoggingModelBinderProvider());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

static IEdmModel GetEdmModel()
{
    var odataBuilder = new ODataConventionModelBuilder();
    odataBuilder.EntitySet<Style>("Style"); // ENTITY
    odataBuilder.EntitySet<WatercolorsPainting>("WatercolorsPainting"); // ENTITY
    return odataBuilder.GetEdmModel();
}

builder.Services.AddControllers().AddOData(options =>
{
    options.Select().Filter().OrderBy().Expand().SetMaxTop(null).Count();
    options.AddRouteComponents("odata", GetEdmModel());
});

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<WatercolorsPainting2024DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<UserAccountRepo>();
builder.Services.AddScoped<WatercolorsPaintingRepo>();
builder.Services.AddScoped<IWatercolorsPaintingService, WatercolorsPaintingService>();

//validator
builder.Services.AddScoped<IValidator<WatercolorsPainting>, WatercolorsPaintingValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<WatercolorsPaintingValidator>();

builder.Services.AddControllers().AddJsonOptions(option =>
{
    option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default-key-if-missing"))
        };
    });

builder.Services.AddSwaggerGen(option =>
{
    ////JWT Config
    option.DescribeAllParametersInCamelCase();
    option.ResolveConflictingActions(conf => conf.First());
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
var app = builder.Build();

// Test database connection
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<WatercolorsPainting2024DbContext>();
        await dbContext.Database.CanConnectAsync();
        Console.WriteLine("✅ Database connection successful!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database connection failed: {ex.Message}");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Log startup information about the logging implementation
Console.WriteLine("======================================================================");
Console.WriteLine("💡 LOGGING IMPLEMENTATION FOR ASP.NET CORE FLOW VISUALIZATION");
Console.WriteLine("======================================================================");
Console.WriteLine("✅ Routing logs: Shows how URL paths are matched to controller actions");
Console.WriteLine("✅ Model Binding logs: Shows how data from HTTP request is bound to C# objects");
Console.WriteLine("✅ Validation logs: Shows both Data Annotation and FluentValidation processes");
Console.WriteLine("✅ Repository logs: Shows database operations and query processing");
Console.WriteLine("======================================================================");
Console.WriteLine("🔍 Look for these log prefixes to understand the flow:");
Console.WriteLine("   🧭 ROUTING: URL pattern matching and route determination");
Console.WriteLine("   📦 MODEL BINDING: Parameter binding from route/query/body");
Console.WriteLine("   ✅ VALIDATION: Data validation using annotations and FluentValidation");
Console.WriteLine("   🔍 CONTROLLER/SERVICE: Business logic flow");
Console.WriteLine("   💾 REPOSITORY: Database interactions");
Console.WriteLine("   ⌛ PERFORMANCE: Timing information for operations");
Console.WriteLine("======================================================================");

app.UseHttpsRedirection();

// Add logging middleware for routing
app.Use(async (context, next) => {
    Console.WriteLine($"🧭 ROUTING: Request received for path {context.Request.Path} with method {context.Request.Method}");
    await next();
    Console.WriteLine($"🧭 ROUTING: Response completed for path {context.Request.Path} with status code {context.Response.StatusCode}");
});

app.UseAuthorization();

app.MapControllers();

app.Run();