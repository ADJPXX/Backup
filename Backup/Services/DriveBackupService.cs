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
            foreach (var directory in Directory.GetDirectories(PathsService.Documents))
            {
                foreach (var dir in Config.Configs.BackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var folderName = Path.GetFileName(directory);

                    if (folderName.Contains("My Games"))
                    {
                        await RobocopyService.CopyAsync($"\"{PathsService.RocketLeagueSource}\" \"{PathsService.RocketLeagueDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
                        
                        continue;
                    }

                    await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.BackupDrive}{folderName}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
                }
            }

            if (!Directory.Exists(PathsService.RepositoriesPath))
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {PathsService.RepositoriesPath}");
            }
            else
            {
                foreach (var directory in Directory.GetDirectories(PathsService.RepositoriesPath))
                {
                    var folderName = Path.GetFileName(directory);

                    await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.BackupCodes}{folderName}\" /E /COPY:DAT /R:3 /W:5");
                }

                await RobocopyService.CopyAsync($"\"{PathsService.PublishSource}\" \"{PathsService.PublishDestination}\" publish.txt /COPY:DAT /R:3 /W:5");
                
                await RobocopyService.CopyAsync($"\"{PathsService.DotGithubSource}\" \"{PathsService.DotGithubDestination}\" /E /COPY:DAT /R:3 /W:5");
            }

            if (Directory.Exists(PathsService.DavinciSource))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.DavinciSource}\" \"{PathsService.DavinciDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {PathsService.DavinciSource}");
            }

            if (Directory.Exists(PathsService.ObsSource))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.ObsSource}\" \"{PathsService.ObsDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {PathsService.ObsSource}");
            }

            if (Directory.Exists(PathsService.DuckStationSource))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.DuckStationSource}\" \"{PathsService.DuckStationDestination}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {PathsService.DuckStationSource}");
            }
            
            if (Directory.Exists(PathsService.TudoExists))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.TudoExists}\" \"{PathsService.BackupDriveLetter}TUDO\" /E /MOVE /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"NÃO CONTEM A PASTA \"TUDO\" NO SEGUINTE CAMINHO: {PathsService.TudoExists}");
            }

            var videosExists = DirectoryService.VideosExists();

            if (!videosExists)
            {
                return log.AppendLine("BACKUP CONCLUIDO.");
            }

            await RobocopyService.CopyAsync($"\"{PathsService.VideosPath}\" \"{PathsService.BackupDriveLetter}Vídeos gravados\" /E /COPY:DAT /R:3 /W:5");

            return log.AppendLine("BACKUP CONCLUIDO.");
        }
        catch (Exception ex)
        {
            return log.AppendLine($"ERRO: {ex.Message}");
        }
    }
}