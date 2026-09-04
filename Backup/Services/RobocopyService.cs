using System.Diagnostics;

namespace Backup.Services;

public static class RobocopyService
{
    public static async Task<(string, int)> CopyAsync(string arguments)
    {
        var backup = Process.Start(new ProcessStartInfo
        {
            FileName = "robocopy",
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardError = true
        });

        var error = await backup?.StandardError.ReadToEndAsync()!;
        
        await backup.WaitForExitAsync();
        
        return(error, backup.ExitCode);
    }
}