using Business.DI;
using Infra.DI;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Templates.Themes;
using SerilogTracing;
using SerilogTracing.Expressions;


// Global logger configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(Formatters.CreateConsoleTextFormatter(theme: TemplateTheme.Code))
    .CreateLogger();

// Enable tracing of ASP.NET requests
using var listener = new ActivityListenerConfiguration()
    .Instrument.AspNetCoreRequests()
    .TraceToSharedLogger();

Log.Information("Starting application...");

var builder = WebApplication.CreateBuilder(args);
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

// Get connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext with Npgsql provider
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddBusinessServices();
builder.Services.AddInfraServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();

    if (app.Environment.IsDevelopment())
    {
        await DAL.Seeds.BogusSeeder.SeedAsync(db);
    }
}


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
