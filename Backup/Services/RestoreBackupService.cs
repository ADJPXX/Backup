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
            foreach (var folderName in Config.Configs.BackupFolders)
            {
                var directory = Path.Combine(PathsService.BackupDrive, folderName);

                if (!Directory.Exists(directory))
                {
                    log.AppendLine($"A PASTA \"{folderName}\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.BackupDrive}\"");

                    continue;
                }
                
                await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.Documents}\\{folderName}\" /E /COPY:DAT /R:3 /W:5");
            }
            
            if (File.Exists(Path.Combine(PathsService.CSharpBackup, "publish.txt")))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.CSharpBackup}\" \"{PathsService.CSharpDevDrive}\" publish.txt /COPY:DAT /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"O ARQUIVO \"publish.txt\" NÃO FOI ENCONTRADO NO CAMINHO: \"{PathsService.CSharpBackup}\"");
            }

            if (File.Exists(Path.Combine(PathsService.CSharpBackup, ".gitignore")))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.CSharpBackup}\" \"{PathsService.CSharpDevDrive}\" .gitignore /COPY:DAT /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"O ARQUIVO \".gitignore\" NÃO FOI ENCONTRADO NO CAMINHO: \"{PathsService.CSharpBackup}\"");
            }

            if (Directory.Exists(PathsService.DotGithubDestination))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.DotGithubDestination}\" \"{PathsService.DotGithubSource}\" /E /COPY:DAT /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A PASTA \".github\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.DotGithubDestination}\"");
            }

            if (Directory.Exists(PathsService.DavinciDestination))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.DavinciDestination}\" \"{PathsService.DavinciSource}\" /E /COPY:DAT /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A PASTA \"Blackmagic Design\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.DavinciDestination}\"");
            }

            if (Directory.Exists(PathsService.ObsDestination))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.ObsDestination}\" \"{PathsService.ObsSource}\" /E /COPY:DAT /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A PASTA \"obs-studio\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.ObsDestination}\"");
            }

            if (Directory.Exists(PathsService.DuckStationDestination))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.DuckStationDestination}\" \"{PathsService.DuckStationSource}\" /E /COPY:DAT /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A PASTA \"DuckStation\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.DuckStationDestination}\"");
            }

            if (Directory.Exists(PathsService.TudoInDrive))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.TudoInDrive}\" \"{PathsService.TudoInDownloads}\" /E /MOVE /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A PASTA \"TUDO\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.TudoInDrive}");
            }

            if (Directory.Exists(PathsService.VideosGravadosInDrive))
            {
                await RobocopyService.CopyAsync($"\"{PathsService.VideosGravadosInDrive}\" \"{PathsService.VideosGravadosInVideos}\" /E /MOVE /R:3 /W:5");
            }
            else
            {
                log.AppendLine($"A PASTA \"Vídeos gravados\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.VideosGravadosInDrive}");
            }

            return log.AppendLine("TODOS ARQUIVOS RESTAURADOS");
        }
        catch (Exception ex)
        {
            return log.AppendLine($"ERRO: {ex.Message}");
        }
    }
}