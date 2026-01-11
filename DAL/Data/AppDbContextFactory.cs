using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Find appsettings.json by searching from current directory upward
        var basePath = FindAppSettingsPath();
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Could not find a connection string named 'DefaultConnection'. " +
                "Please ensure appsettings.json exists in the DAL project directory.");
        }

        // Create options builder
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // Create and return the context
        return new AppDbContext(optionsBuilder.Options);
    }

    private static string FindAppSettingsPath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDir);

        // Search upward for appsettings.json
        while (directory != null)
        {
            var appSettingsPath = Path.Combine(directory.FullName, "appsettings.json");
            if (File.Exists(appSettingsPath))
            {
                return directory.FullName;
            }

            // Check if we're in a DAL subdirectory
            var dalPath = Path.Combine(directory.FullName, "DAL", "appsettings.json");
            if (File.Exists(dalPath))
            {
                return Path.Combine(directory.FullName, "DAL");
            }

            directory = directory.Parent;
        }

        // Fallback to current directory
        return currentDir;
    }
}

