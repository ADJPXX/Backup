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
            
            foreach (var folderName in Config.Configs.CloudBackupFolders)
            {
                var directory = Path.Combine(PathsService.BackupDrive, folderName);

                if (!Directory.Exists(directory))
                {
                    log.AppendLine($"A PASTA {folderName} NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.BackupDrive}");
                    
                    continue;
                }

                await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.CloudBackup}\\{folderName}\" /E /COPY:DAT /R:3 /W:5");
            }
            
            return log.AppendLine("BACKUP NA NUVEM CONCLUIDO");
        }
        catch (Exception ex)
        {
            return log.AppendLine($"ERRO NA FUNÇÃO \"MakeCloudBackupAsync()\": {ex.Message}");
        }
    }
}