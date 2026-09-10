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
            LogService.StartExecution("CLOUD BACKUP");
            
            if (!Directory.Exists(Config.Configs.CloudBackupPath))
            {
                LogService.AddLog("A NUVEM NÃO FOI ENCONTRADA!");
                
                return log.AppendLine("A NUVEM NÃO FOI ENCONTRADA!");
            }
            
            foreach (var folderName in Config.Configs.CloudBackupFolders)
            {
                var directory = Path.Combine(PathsService.BackupDriveLetter, folderName);
                
                if (!Directory.Exists(directory))
                {
                    LogService.AddLog($"A PASTA \"{folderName}\" NÃO FOI ENCONTRADA NO CAMINHO: {directory}");
                    
                    log.AppendLine($"A PASTA \"{folderName}\" NÃO FOI ENCONTRADA NO CAMINHO: {directory}");
                    
                    continue;
                }
                
                var cloudStatus = await RobocopyService.CopyAsync($"\"{directory}\" \"{Config.Configs.CloudBackupPath}\\{folderName}\" /E /COPY:DAT /R:3 /W:5");
                
                LogService.AddLog($"{folderName} STATUS: {cloudStatus.Item1}");
                
                log.AppendLine($"{folderName} STATUS: {cloudStatus.Item1}");

                if (string.IsNullOrEmpty(cloudStatus.Item2))
                {
                    continue;
                }
                
                LogService.AddLog($"{folderName} ERRO: {cloudStatus.Item2}");
                
                log.AppendLine($"{folderName} ERRO: {cloudStatus.Item2}");
            }
            
            LogService.AddLog("BACKUP NA NUVEM CONCLUIDO");
            
            LogService.EndExecution();
            
            return log.AppendLine("BACKUP NA NUVEM CONCLUIDO");
        }
        catch (Exception ex)
        {
            return log.AppendLine($"ERRO NA FUNÇÃO \"MakeCloudBackupAsync()\": {ex.Message}");
        }
    }
}