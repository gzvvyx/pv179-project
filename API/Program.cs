using Common.DI;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Templates.Themes;
using SerilogTracing;
using SerilogTracing.Expressions;

var builder = WebApplication.CreateBuilder(args);

// Global logger configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Enable tracing of ASP.NET requests
using var listener = new ActivityListenerConfiguration()
    .Instrument.AspNetCoreRequests()
    .TraceToSharedLogger();

Log.Information("Starting application...");

builder.Logging.ClearProviders();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services
    .AddControllers()
    .AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        Description = "API Key",
        Scheme = "ApiKeyScheme"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddPv179Core(builder.Configuration);

var app = builder.Build();

await app.MigrateAndSeedAsync();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseMiddleware<API.Middleware.AuthMiddleware>();
app.UseMiddleware<API.Middleware.ResponseFormatMiddleware>();

// Log all incoming HTTP requests automatically
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
