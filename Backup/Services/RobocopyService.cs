using System.Diagnostics;

namespace Backup.Services;

public static class RobocopyService
{
    public static async Task<(string, string, int)> CopyAsync(string arguments)
    {
        Console.WriteLine("ANTES");
        
        var backup = Process.Start(new ProcessStartInfo
        {
            FileName = "robocopy",
            Arguments = arguments
        });

        Console.WriteLine("DEPOIS");
        
        var output = await backup?.StandardOutput.ReadToEndAsync()!;

        var error = await backup.StandardError.ReadToEndAsync();
        
        await backup.WaitForExitAsync();

        var exitCode = backup.ExitCode;
        
        return  (output, error, exitCode);
    }
    
    
}