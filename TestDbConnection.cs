using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CarRentalSystem.Infrastructure;
using System;
using System.IO;

namespace CarRentalSystem
{
    public class TestDbConnection
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing database connection...");
            
            // Test direct SQL connection
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CarRentalSystemDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            
            try
            {
                // First check if LocalDB is running
                Console.WriteLine("Checking if LocalDB is running...");
                try {
                    using (var masterConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;"))
                    {
                        masterConnection.Open();
                        Console.WriteLine("LocalDB is running and accessible.");
                        masterConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LocalDB connection error: {ex.Message}");
                    Console.WriteLine("LocalDB may not be installed or running properly.");
                }
                
                // Now try to connect to the application database
                using (var connection = new SqlConnection(connectionString))
                {
                    Console.WriteLine("\nAttempting to open connection to CarRentalSystemDb...");
                    connection.Open();
                    Console.WriteLine("SQL connection successful!");
                    
                    // Check if database exists
                    using (var command = new SqlCommand("SELECT DB_ID('CarRentalSystemDb')", connection))
                    {
                        var result = command.ExecuteScalar();
                        Console.WriteLine($"Database exists check: {result != DBNull.Value}");
                    }
                    
                    // Check if AspNetUsers table exists and has records
                    try {
                        using (var command = new SqlCommand("SELECT COUNT(*) FROM AspNetUsers", connection))
                        {
                            var userCount = (int)command.ExecuteScalar();
                            Console.WriteLine($"AspNetUsers table exists and has {userCount} records.");
                        }
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Error checking AspNetUsers table: {ex.Message}");
                    }
                    
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SQL connection error: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}