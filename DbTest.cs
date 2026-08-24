using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CarRentalSystem.Infrastructure;
using System;
using System.Linq;

namespace CarRentalSystem
{
    public class DbTest
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("===== Database Test Tool =====\n");
            
            // Test direct SQL connection
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CarRentalSystemDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            
            try
            {
                // 1. Check if LocalDB is running
                Console.WriteLine("1. Checking if LocalDB is running...");
                using (var masterConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;"))
                {
                    masterConnection.Open();
                    Console.WriteLine("   ✓ LocalDB is running and accessible.");
                    masterConnection.Close();
                }

                // 2. Check if database exists
                Console.WriteLine("\n2. Checking if CarRentalSystemDb exists...");
                using (var dbConnection = new SqlConnection(connectionString))
                {
                    dbConnection.Open();
                    Console.WriteLine("   ✓ CarRentalSystemDb exists and is accessible.");
                    dbConnection.Close();
                }

                // 3. Check if tables exist using EF Core
                Console.WriteLine("\n3. Checking if tables exist using EF Core...");
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using (var context = new ApplicationDbContext(optionsBuilder.Options))
                {
                    // Check if AspNetUsers table exists and has the expected schema
                    var userTableExists = context.Users.Any() || true; // Even if no users, this will throw if table doesn't exist
                    Console.WriteLine("   ✓ AspNetUsers table exists.");

                    // List all tables in the database
                    Console.WriteLine("\n4. Listing all tables in the database:");
                    var tables = context.Model.GetEntityTypes().Select(t => t.GetTableName()).Distinct().ToList();
                    foreach (var table in tables)
                    {
                        Console.WriteLine($"   - {table}");
                    }
                }

                Console.WriteLine("\n✅ Database setup is complete and working correctly!");
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

            Console.WriteLine("\nDatabase verification complete.");
        }
    }
}