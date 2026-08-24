using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CarRentalSystem.Infrastructure;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CarRentalSystem
{
    public class InitializeDatabase
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("===== Database Initialization Tool =====\n");
            
            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
                
            string connectionString = configuration.GetConnectionString("ApplicationDbContext");
            
            try
            {
                // 1. Check if LocalDB is running
                Console.WriteLine("1. Checking if LocalDB is running...");
                using (var masterConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;"))
                {
                    masterConnection.Open();
                    Console.WriteLine("   ✓ LocalDB is running and accessible.");
                    
                    // 2. Check if database exists and drop it if it does
                    Console.WriteLine("\n2. Checking if CarRentalSystemDb exists...");
                    using (var command = new SqlCommand("SELECT DB_ID('CarRentalSystemDb')", masterConnection))
                    {
                        var result = command.ExecuteScalar();
                        bool dbExists = (result != DBNull.Value && result != null);
                        
                        if (dbExists)
                        {
                            Console.WriteLine("   ✓ CarRentalSystemDb exists. Dropping the database to create a fresh one.");
                            
                            // Drop the existing database
                            using (var dropCommand = new SqlCommand("ALTER DATABASE [CarRentalSystemDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [CarRentalSystemDb];", masterConnection))
                            {
                                dropCommand.ExecuteNonQuery();
                                Console.WriteLine("   ✓ Existing database dropped successfully.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("   ✓ CarRentalSystemDb does not exist. Will create a new one.");
                        }
                    }
                    
                    // 3. Create a new database
                    Console.WriteLine("\n3. Creating a new CarRentalSystemDb...");
                    using (var createCommand = new SqlCommand("CREATE DATABASE [CarRentalSystemDb]", masterConnection))
                    {
                        createCommand.ExecuteNonQuery();
                        Console.WriteLine("   ✓ New database created successfully.");
                    }
                    
                    masterConnection.Close();
                }
                
                // 4. Apply migrations using EF Core
                Console.WriteLine("\n4. Applying Entity Framework migrations...");
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(connectionString);
                
                using (var context = new ApplicationDbContext(optionsBuilder.Options))
                {
                    context.Database.Migrate();
                    Console.WriteLine("   ✓ Migrations applied successfully.");
                    
                    // 5. Verify tables were created
                    Console.WriteLine("\n5. Verifying database tables...");
                    var tables = context.Model.GetEntityTypes().Select(t => t.GetTableName()).Where(t => t != null).Distinct().ToList();
                    foreach (var table in tables)
                    {
                        Console.WriteLine($"   - {table}");
                    }
                    
                    // 6. Verify specific tables
                    bool usersTableExists = tables.Any(t => t != null && t.Equals("AspNetUsers", StringComparison.OrdinalIgnoreCase));
                    bool vehiclesTableExists = tables.Any(t => t != null && t.Equals("Vehicles", StringComparison.OrdinalIgnoreCase));
                    bool bookingsTableExists = tables.Any(t => t != null && t.Equals("Bookings", StringComparison.OrdinalIgnoreCase));
                    
                    Console.WriteLine($"\n   AspNetUsers table exists: {usersTableExists}");
                    Console.WriteLine($"   Vehicles table exists: {vehiclesTableExists}");
                    Console.WriteLine($"   Bookings table exists: {bookingsTableExists}");
                }
                
                Console.WriteLine("\n✅ Database initialization completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"\nInner exception: {ex.InnerException.Message}");
                }
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}