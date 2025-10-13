using DAL.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Get connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

bool seedData = builder.Environment.IsDevelopment();

// Register DbContext with Npgsql provider
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped(provider =>
{
    var options = provider.GetRequiredService<DbContextOptions<AppDbContext>>();
    var context = new AppDbContext(options, seedData);
    context.Database.EnsureCreated();
    return context;
});

var app = builder.Build();