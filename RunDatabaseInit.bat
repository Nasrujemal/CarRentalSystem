@echo off
echo Running Database Initialization...

dotnet run --project "%~dp0CarRentalSystem.csproj" /p:StartupObject=CarRentalSystem.InitializeDatabase

echo.
echo Press any key to exit...
pause > nul