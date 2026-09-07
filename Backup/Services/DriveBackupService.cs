using System.Text;
using Backup.Models;

namespace Backup.Services;

public static class DriveBackupService
{
    public static async Task<StringBuilder> MakeDriveBackupAsync()
    {
        var log = new StringBuilder();
        
        try
        {
            foreach (var folderName in Config.Configs.BackupFolders)
            {
                var directory = Path.Combine(PathsService.Documents, folderName);
                
                if (!Directory.Exists(directory))
                {
                    log.AppendLine($"A PASTA \"{folderName}\" NÃO FOI ENCONTRADA NO CAMINHO \"{PathsService.Documents}\"");

                    continue;
                }

                if (folderName.Equals("My Games", StringComparison.OrdinalIgnoreCase))
                {
                    await RobocopyService.CopyAsync($"\"{PathsService.RocketLeagueSource}\" \"{PathsService.RocketLeagueDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
                    
                    continue;
                }
                
                await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.BackupDrive}{folderName}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
            }
            
            if (!Directory.Exists(PathsService.RepositoriesPath))
            {
                log.AppendLine($"A PASTA \"Repositories\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.RepositoriesPath}");
            }
            else
            {
                foreach (var directory in Directory.GetDirectories(PathsService.RepositoriesPath))
                {
                    var folderName = Path.GetFileName(directory);

                    await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.BackupCodes}{folderName}\" /E /COPY:DAT /R:3 /W:5");
                }

                if (Directory.Exists(PathsService.DotGithubSource))
                {
                    await RobocopyService.CopyAsync($"\"{PathsService.DotGithubSource}\" \"{PathsService.DotGithubDestination}\" /E /COPY:DAT /R:3 /W:5");
                }
                else
                {
                    log.AppendLine($"A PASTA \".github\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.DotGithubSource}");
                }

                if (File.Exists(Path.Combine(PathsService.CSharpDevDrive, ".gitignore")))
                {
                    await RobocopyService.CopyAsync($"\"{PathsService.CSharpDevDrive}\" \"{PathsService.CSharpBackup}\" .gitignore /COPY:DAT /R:3 /W:5");
                }
                else
                {
                    log.AppendLine($"O ARQUIVO \".gitignore\" NÃO FOI ENCONTRADO NO CAMINHO: {PathsService.CSharpDevDrive}");
                }
            }

            if (Directory.Exists(PathsService.DavinciSource))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.DavinciSource}\" \"{PathsService.DavinciDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A PASTA \"Blackmagic Design\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.DavinciSource}");
            }

            if (Directory.Exists(PathsService.ObsSource))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.ObsSource}\" \"{PathsService.ObsDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A PASTA \"obs-studio\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.ObsSource}");
            }

            if (Directory.Exists(PathsService.DuckStationSource))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.DuckStationSource}\" \"{PathsService.DuckStationDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A PASTA \"DuckStation\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.DuckStationSource}");
            }
            
            if (Directory.Exists(PathsService.TudoInDownloads))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.TudoInDownloads}\" \"{PathsService.TudoInDrive}\" /E /MOVE /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A PASTA \"TUDO\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.TudoInDownloads}");
            }

            var videosExists = DirectoryService.VideosExists();

            if (!videosExists)
            {
                return log.AppendLine("BACKUP CONCLUIDO.");
            }

            await RobocopyService.CopyAsync($"\"{PathsService.VideosGravadosInVideos}\" \"{PathsService.VideosGravadosInDrive}\" /E /COPY:DAT /R:3 /W:5");

            return log.AppendLine("BACKUP CONCLUIDO.");
        }
        catch (Exception ex)
        {
            return log.AppendLine($"ERRO: {ex.Message}");
        }
    }
}