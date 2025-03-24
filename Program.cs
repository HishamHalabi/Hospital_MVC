using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Hospital
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Ensure database is created and migrated (Optional - Use only if necessary)
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();

                try
                {
                    Console.WriteLine("Testing database connection...");

                    // Test the database connection
                    if (context.Database.CanConnect())
                    {
                        Console.WriteLine("Database connection successful.");
                    }
                    else
                    {
                        Console.WriteLine("Database connection failed.");
                    }

                    // Remove automatic migration if database is stable
                    // context.Database.Migrate();  
                    // Console.WriteLine("Database migrated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database connection/migration failed: {ex.Message}");
                }
            }

            host.Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Load configuration from appsettings.json
                    var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    // Read connection string from appsettings.json
                    var connectionString = config.GetConnectionString("DefaultConnection");

                    // Register AppDbContext with SQL Server
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(connectionString,
                            sqlOptions => sqlOptions.CommandTimeout(10))); // 10 sec timeout

                    // Add any other necessary services here
                });
    }
}
