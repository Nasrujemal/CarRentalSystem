using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CarRentalSystem.Infrastructure;
using CarRentalSystem.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CarRentalSystem
{
    public class DbDiagnostic
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("===== Database Diagnostic Tool =====\n");
            
            // Test direct SQL connection
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CarRentalSystemDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            
            try
            {
                // 1. Check if LocalDB is running
                Console.WriteLine("1. Checking if LocalDB is running...");
                try {
                    using (var masterConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;"))
                    {
                        masterConnection.Open();
                        Console.WriteLine("   ✓ LocalDB is running and accessible.");
                        masterConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ✗ LocalDB connection error: {ex.Message}");
                    Console.WriteLine("   LocalDB may not be installed or running properly.");
                    return;
                }
                
                // 2. Check if CarRentalSystemDb exists
                Console.WriteLine("\n2. Checking if CarRentalSystemDb exists...");
                bool dbExists = false;
                try
                {
                    using (var masterConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;"))
                    {
                        masterConnection.Open();
                        using (var command = new SqlCommand("SELECT DB_ID('CarRentalSystemDb')", masterConnection))
                        {
                            var result = command.ExecuteScalar();
                            dbExists = (result != DBNull.Value && result != null);
                            Console.WriteLine(dbExists 
                                ? "   ✓ CarRentalSystemDb exists." 
                                : "   ✗ CarRentalSystemDb does not exist.");
                        }
                        masterConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ✗ Error checking database existence: {ex.Message}");
                }
                
                // 3. Try to connect to the application database
                Console.WriteLine("\n3. Attempting to connect to CarRentalSystemDb...");
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        Console.WriteLine("   ✓ Connection to CarRentalSystemDb successful!");
                        
                        // 4. Check if AspNetUsers table exists
                        Console.WriteLine("\n4. Checking if AspNetUsers table exists...");
                        try {
                            using (var command = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers'", connection))
                            {
                                var tableExists = (int)command.ExecuteScalar() > 0;
                                Console.WriteLine(tableExists 
                                    ? "   ✓ AspNetUsers table exists." 
                                    : "   ✗ AspNetUsers table does not exist.");
                                
                                if (tableExists)
                                {
                                    // 5. Check if AspNetUsers has records
                                    Console.WriteLine("\n5. Checking if AspNetUsers has records...");
                                    using (var userCommand = new SqlCommand("SELECT COUNT(*) FROM AspNetUsers", connection))
                                    {
                                        var userCount = (int)userCommand.ExecuteScalar();
                                        Console.WriteLine(userCount > 0 
                                            ? $"   ✓ AspNetUsers table has {userCount} records." 
                                            : "   ✗ AspNetUsers table has no records.");
                                    }
                                }
                            }
                        }
                        catch (Exception ex) {
                            Console.WriteLine($"   ✗ Error checking AspNetUsers table: {ex.Message}");
                        }
                        
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ✗ Connection error: {ex.Message}");
                    Console.WriteLine($"   Inner exception: {ex.InnerException?.Message}");
                }
                
                // 6. Suggest potential solutions
                Console.WriteLine("\n===== Diagnostic Results =====");
                if (!dbExists)
                {
                    Console.WriteLine("\nProblem: The database does not exist.");
                    Console.WriteLine("Possible solutions:");
                    Console.WriteLine("1. Ensure LocalDB is properly installed and running.");
                    Console.WriteLine("2. Check if the connection string in appsettings.json is correct.");
                    Console.WriteLine("3. Modify Program.cs to ensure EnsureCreated() is called before Migrate().");
                }
                else
                {
                    Console.WriteLine("\nPossible issues with user registration/login:");
                    Console.WriteLine("1. Check if Identity is properly configured in Program.cs.");
                    Console.WriteLine("2. Verify that AccountController is correctly saving user data.");
                    Console.WriteLine("3. Ensure that the database connection is maintained throughout the request.");
                    Console.WriteLine("4. Check for any validation errors during registration/login.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}