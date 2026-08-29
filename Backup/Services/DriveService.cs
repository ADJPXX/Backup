using System.Diagnostics;

namespace Backup.Services;

public static class DriveService
{

    public static bool DevDriveExists()
    {
        var driveExiste = DriveInfo.GetDrives().Any(drive => drive.Name.Equals(PathsService.DevDrive, StringComparison.OrdinalIgnoreCase));

        if (driveExiste)
        {
            return true;
        }

        Console.WriteLine($"Drive \"{PathsService.DevDrive}\" não encontrado, vou abrir a página de criação de drive para você fazer o Dev Drive\nAperte qualquer tecla para abrir a página de criação de drive.");

        Console.ReadKey();

        Process.Start(new ProcessStartInfo
        {
            FileName = "ms-settings:disksandvolumes",
            UseShellExecute = true
        });

        for (var i = 0; i < 30; i++)
        {
            Console.WriteLine($"\nO programa irá esperar 30 SEGUNDOS para que você crie o Dev Drive ({PathsService.DevDrive}).\nCaso um Dev Drive não seja detectado, a verificação irá acontecer novamente.");

            var time = TimeSpan.FromSeconds(i);

            Console.WriteLine($"\nContador: {time:mm\\:ss}");

            Thread.Sleep(1000);

            Console.Clear();
        }

        Console.Clear();

        return false;
    }
}