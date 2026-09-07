using System.Diagnostics;
using System.Text;
using Backup.Models;

namespace Backup.Services;

public static class CloudBackupService
{
    public static async Task<StringBuilder> MakeCloudBackupAsync()
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

                    await RobocopyService.CopyAsync($"\"{directory}\" \"{destination}\" /E /COPY:DAT /R:3 /W:5");
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