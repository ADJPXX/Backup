using System.Diagnostics;

namespace Backup.Services;

public static class DriveService
{
    private const string DevDrive = @"E:\";

    public static (bool, string) DevDriveExists()
    {
        var driveExiste = DriveInfo.GetDrives().Any(drive => drive.Name.Equals(DevDrive, StringComparison.OrdinalIgnoreCase));

        if (driveExiste)
        {
            return (true, DevDrive);
        }

        Console.WriteLine($"Drive \"{DevDrive}\" não encontrado, vou abrir a página de criação de drive para você fazer o Dev Drive\nAperte qualquer tecla para abrir a página de criação de drive.");

        Console.ReadKey();

        Process.Start(new ProcessStartInfo
        {
            FileName = "ms-settings:disksandvolumes",
            UseShellExecute = true
        });

        Console.Clear();

        return (false, DevDrive);

    }
}