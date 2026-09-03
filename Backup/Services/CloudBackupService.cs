using System.Diagnostics;
using System.Text;
using Backup.Models;

namespace Backup.Services;

public static class CloudBackupService
{
    public static StringBuilder MakeCloudBackup()
    {
        var log = new StringBuilder();
        
        try
        {
            if (!Directory.Exists(PathsService.CloudBackup))
            {
                return log.AppendLine("A NUVEM NÃO FOI ENCONTRADA!");
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

            return log.AppendLine("BACKUP NA NUVEM CONCLUIDO");

        }

        catch (Exception ex)
        {
            return log.AppendLine($"ERRO: {ex.Message}");
        }
    }
}