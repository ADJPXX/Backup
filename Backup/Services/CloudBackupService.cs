using System.Diagnostics;
using Backup.Models;

namespace Backup.Services;

public static class CloudBackupService
{
    private const string CloudBackup = @"G:\Meu Drive\BackupCloud\";

    private const string BackupDriveLetter = @"D:\";

    public static string MakeCloudBackup()
    {
        try
        {
            if (!Directory.Exists(CloudBackup))
            {
                return "A NUVEM NÃO FOI ENCONTRADA!";
            }

            foreach (var directory in Directory.GetDirectories(BackupDriveLetter))
            {
                foreach (var dir in Config.Configs.CloudBackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var nomePasta = Path.GetFileName(directory);

                    var destination = Path.Combine(CloudBackup, nomePasta);

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