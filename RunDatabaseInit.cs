using System;
using System.Diagnostics;

namespace CarRentalSystem
{
    public class RunDatabaseInit
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting database initialization...");
            
            try
            {
                // Create a new process to run the InitializeDatabase class
                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run --no-build --project c:\\Users\\james\\source\\repos\\CarRentalSytem\\CarRentalSystem.csproj /p:StartupObject=CarRentalSystem.InitializeDatabase",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = false
                };

                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    
                    // Read and display output in real-time
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string line = process.StandardOutput.ReadLine();
                        Console.WriteLine(line);
                    }
                    
                    // Read any error output
                    string errors = process.StandardError.ReadToEnd();
                    if (!string.IsNullOrEmpty(errors))
                    {
                        Console.WriteLine("\nErrors:");
                        Console.WriteLine(errors);
                    }
                    
                    process.WaitForExit();
                    
                    Console.WriteLine($"\nProcess exited with code: {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}