using System.Diagnostics;

namespace Backup.Services;

public static class DriveService
{
    public static bool DevDriveExists()
    {
        try
        {
            var driveExiste = DriveInfo.GetDrives().Any(drive =>
                drive.Name.Equals(PathsService.DevDrive, StringComparison.OrdinalIgnoreCase));

            if (driveExiste)
            {
                return true;
            }

            Console.WriteLine(
                $"Drive \"{PathsService.DevDrive}\" não encontrado, vou abrir a página de criação de drive para você fazer o Dev Drive\nAperte QUALQUER TECLA para abrir a página de criação de drive.");

            Console.ReadKey();

            Process.Start(new ProcessStartInfo
            {
                FileName = "ms-settings:disksandvolumes",
                UseShellExecute = true
            });

            Console.Clear();

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"DevDriveExists()\": {ex.Message}");
            return false;
        }
    }
}