using System.Diagnostics;
using System.Text;
using Backup.Models;

namespace Backup.Services;

public static class RestoreBackupService
{
    public static async Task<StringBuilder> RestoreBackupAsync()
    {
        var log = new StringBuilder();
        
        try
        {
            foreach (var directory in Directory.GetDirectories(PathsService.BackupDrive))
            {
                foreach (var dir in Config.Configs.BackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var folderName = Path.GetFileName(directory);

                    await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.Documents}\\{folderName}\" /E /COPY:DAT /XD logs log replay replays cache caches /R:3 /W:5");
                }
            }

            await RobocopyService.CopyAsync($"\"{PathsService.PublishDestination}\" \"{PathsService.PublishSource}\" publish.txt /COPY:DAT /R:3 /W:5");
            
            await RobocopyService.CopyAsync($"\"{PathsService.GitDestination}\" \"{PathsService.GitSource}\" .gitignore /COPY:DAT /R:3 /W:5");

            await RobocopyService.CopyAsync($"\"{PathsService.DotGithubDestination}\" \"{PathsService.DotGithubSource}\" /E /COPY:DAT /R:3 /W:5");

            if (Directory.Exists(PathsService.DavinciDestination))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.DavinciDestination}\" \"{PathsService.DavinciSource}\" /E /COPY:DAT /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {PathsService.DavinciDestination}");
            }

            if (Directory.Exists(PathsService.ObsDestination))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.ObsDestination}\" \"{PathsService.ObsSource}\" /E /COPY:DAT /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {PathsService.ObsDestination}");
            }

            if (Directory.Exists(PathsService.DuckStationDestination))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.DuckStationDestination}\" \"{PathsService.DuckStationSource}\" /E /COPY:DAT /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {PathsService.DuckStationDestination}");
            }

            if (Directory.Exists(PathsService.TudoInDrive))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.TudoInDrive}\" \"{PathsService.TudoInDownloads}\" /E /MOVE /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"NÃO CONTEM PASTA \"TUDO\" NO SEGUINTE CAMINHO: {PathsService.TudoInDrive}");
            }

            if (Directory.Exists(PathsService.VideosGravadosInDrive))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.VideosGravadosInDrive}\" \"{PathsService.VideosGravadosInVideos}\" /E /MOVE /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"NÃO CONTEM PASTA \"Vídeos gravados\" NO SEGUINTE CAMINHO: {PathsService.VideosGravadosInDrive}");
            }

            return log.AppendLine("TODOS ARQUIVOS RESTAURADOS");
        }
        catch (Exception ex)
        {
            return log.AppendLine($"ERRO: {ex.Message}");
        }
    }
}