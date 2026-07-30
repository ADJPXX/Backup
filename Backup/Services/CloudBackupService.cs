using System.Diagnostics;
using Backup.Models;

namespace Backup.Services;

public static class CloudBackupService
{
    public static string MakeCloudBackup()
    {
        try
        {
            if (!Directory.Exists(PathsService.CloudBackup))
            {
                return "A NUVEM NÃO FOI ENCONTRADA!";
            }

            foreach (var directory in Directory.GetDirectories(PathsService.BackupDriveLetter))
            {
                foreach (var dir in Config.Configs.CloudBackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var nomePasta = Path.GetFileName(directory);

                    var destination = Path.Combine(PathsService.CloudBackup, nomePasta);

                    var cloudBackup = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments = $"\"{directory}\" \"{destination}\" /E /COPY:DAT /R:3 /W:5"
                    });

                    cloudBackup?.WaitForExit();
                }
            }

            return "BACKUP NA NUVEM CONCLUIDO";

        }

        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }
}