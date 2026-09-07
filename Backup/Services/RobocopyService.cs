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
            //RedirectStandardOutput = true,
            RedirectStandardError = true
        });

        //var error = await backup?.StandardError.ReadToEndAsync()!;
        
        //await backup.WaitForExitAsync();
        
        //var outputTask = backup?.StandardOutput.ReadToEndAsync();
        var errorTask = backup?.StandardError.ReadToEndAsync();

        await backup?.WaitForExitAsync()!;

        //var output = await outputTask!;
        var error = await errorTask!;
        
        return(error, backup.ExitCode);
    }
}