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
            LogService.StartExecution("RESTAURAR");
            
            await DocumentsBackupAsync(log);

            await ScriptsBackupAsync(log);

            await RepositoriesBackupAsync(log);

            await DaVinciBackupAsync(log);

            await ObsBackupAsync(log);

            await DuckStationBackupAsync(log);
            
            await CsBackupAsync(log);

            await LmuBackupAsync(log);

            await TarkovBackupAsync(log);

            await AfterburnerBackupAsync(log);

            await RivaTunerBackupAsync(log);

            await TudoBackupAsync(log);

            await VideosBackupAsync(log);

            log.AppendLine("RESTAURAÇÃO DE BACKUP CONCLUIDA");
            
            LogService.AddLog("RESTAURAÇÃO DE BACKUP CONCLUIDA");
            
            LogService.EndExecution();
            
            return log;
        }
        catch (Exception ex)
        {
            return log.AppendLine($"ERRO NA FUNÇÃO \"RestoreBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task DocumentsBackupAsync(StringBuilder log)
    {
        try
        {
            foreach (var folderName in Config.Configs.BackupFolders)
            {
                var directory = Path.Combine(PathsService.BackupDrive, folderName);

                if (!Directory.Exists(directory))
                {
                    LogService.AddLog($"A PASTA \"{folderName}\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.BackupDrive}\"");
                    
                    log.AppendLine($"A PASTA \"{folderName}\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.BackupDrive}\"");
                    
                    continue;
                }

                var backupFoldersStatus = await RobocopyService.CopyAsync($"\"{directory}\" \"{PathsService.Documents}\\{folderName}\" /E /COPY:DAT /R:3 /W:5");
                
                LogService.AddLog($"{folderName} STATUS: {backupFoldersStatus.Item1}");
                
                log.AppendLine($"{folderName} STATUS: {backupFoldersStatus.Item1}");

                if (string.IsNullOrEmpty(backupFoldersStatus.Item2))
                {
                    continue;
                }
                
                LogService.AddLog($"{folderName} ERRO: {backupFoldersStatus.Item2}");
                
                log.AppendLine($"{folderName} ERRO: {backupFoldersStatus.Item2}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"DocumentsBackupAsync()\": {ex.Message}");
        }
    }


    public static async Task ScriptsBackupAsync(StringBuilder? log=null)
    {
        try
        {
            if (Directory.Exists(PathsService.ScriptsFolderInD))
            {
                var scriptsFolderStatus = await RobocopyService.CopyAsync($"\"{PathsService.ScriptsFolderInD}\" \"{PathsService.ScriptsFolderInC}\" /E /COPY:DAT /R:3 /W:5");

                LogService.AddLog($"SCRIPTS STATUS: {scriptsFolderStatus.Item1}");

                log?.AppendLine($"SCRIPTS STATUS: {scriptsFolderStatus.Item1}");

                if (!string.IsNullOrEmpty(scriptsFolderStatus.Item2))
                {
                    LogService.AddLog($"SCRIPTS ERRO: {scriptsFolderStatus.Item2}");

                    log?.AppendLine($"SCRIPTS ERRO: {scriptsFolderStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"A PASTA \"SCRIPTS\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.ScriptsFolderInD}\"");

                log?.AppendLine($"A PASTA \"SCRIPTS\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.ScriptsFolderInD}\"");
            }

        }
        catch (Exception ex)
        {
            log?.AppendLine($"ERRO NA FUNÇÃO \"ScriptsBackupAsync()\": {ex.Message}");
        }


    }


    private static async Task RepositoriesBackupAsync(StringBuilder log)
    {
        try
        {
            if (File.Exists(Path.Combine(PathsService.CSharpBackup, "publish.txt")))
            {
                var publishStatus = await RobocopyService.CopyAsync($"\"{PathsService.CSharpBackup}\" \"{PathsService.CSharpDevDrive}\" publish.txt /COPY:DAT /R:3 /W:5");
                
                LogService.AddLog($"publish.txt STATUS: {publishStatus.Item1}");
                    
                log.AppendLine($"publish.txt STATUS: {publishStatus.Item1}");

                if (!string.IsNullOrEmpty(publishStatus.Item2))
                {
                    LogService.AddLog($"publish.txt ERRO: {publishStatus.Item2}");
                    
                    log.AppendLine($"publish.txt ERRO: {publishStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"O ARQUIVO \"publish.txt\" NÃO FOI ENCONTRADO NO CAMINHO: \"{PathsService.CSharpBackup}\"");
                
                log.AppendLine($"O ARQUIVO \"publish.txt\" NÃO FOI ENCONTRADO NO CAMINHO: \"{PathsService.CSharpBackup}\"");
            }

            if (File.Exists(Path.Combine(PathsService.CSharpBackup, ".gitignore")))
            {
                var gitignoreStatus = await RobocopyService.CopyAsync($"\"{PathsService.CSharpBackup}\" \"{PathsService.CSharpDevDrive}\" .gitignore /COPY:DAT /R:3 /W:5");
                
                LogService.AddLog($".gitignore STATUS: {gitignoreStatus.Item1}");
                    
                log.AppendLine($".gitignore STATUS: {gitignoreStatus.Item1}");
                
                if (!string.IsNullOrEmpty(gitignoreStatus.Item2))
                {
                    LogService.AddLog($".gitignore ERRO: {gitignoreStatus.Item2}");
                    
                    log.AppendLine($".gitignore ERRO: {gitignoreStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"O ARQUIVO \".gitignore\" NÃO FOI ENCONTRADO NO CAMINHO: \"{PathsService.CSharpBackup}\"");
                
                log.AppendLine($"O ARQUIVO \".gitignore\" NÃO FOI ENCONTRADO NO CAMINHO: \"{PathsService.CSharpBackup}\"");
            }

            if (Directory.Exists(PathsService.DotGithubDestination))
            {
                var dotGitStatus = await RobocopyService.CopyAsync($"\"{PathsService.DotGithubDestination}\" \"{PathsService.DotGithubSource}\" /E /COPY:DAT /R:3 /W:5");
                
                LogService.AddLog($".github STATUS: {dotGitStatus.Item1}");
                    
                log.AppendLine($".github STATUS: {dotGitStatus.Item1}");

                if (!string.IsNullOrEmpty(dotGitStatus.Item2))
                {
                    LogService.AddLog($".github ERRO: {dotGitStatus.Item2}");
                    
                    log.AppendLine($".github ERRO: {dotGitStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"A PASTA \".github\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.DotGithubDestination}\"");
                
                log.AppendLine($"A PASTA \".github\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.DotGithubDestination}\"");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"RepositoriesBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task DaVinciBackupAsync(StringBuilder log)
    {
        try
        {
            if (Directory.Exists(PathsService.DavinciDestination))
            {
                var daVinciStatus = await RobocopyService.CopyAsync($"\"{PathsService.DavinciDestination}\" \"{PathsService.DavinciSource}\" /E /COPY:DAT /R:3 /W:5");
                
                LogService.AddLog($"DaVinci Resolve STATUS: {daVinciStatus.Item1}");
                    
                log.AppendLine($"DaVinci Resolve STATUS: {daVinciStatus.Item1}");

                if (!string.IsNullOrEmpty(daVinciStatus.Item2))
                {
                    LogService.AddLog($"Blackmagic Design ERRO: {daVinciStatus.Item2}");
                    
                    log.AppendLine($"Blackmagic Design ERRO: {daVinciStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"A PASTA \"Blackmagic Design\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.DavinciDestination}\"");
                
                log.AppendLine($"A PASTA \"Blackmagic Design\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.DavinciDestination}\"");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"DaVinciBackupAsync()\": {ex.Message}");
        }
    }
    
    
    private static async Task ObsBackupAsync(StringBuilder log)
    {
        try
        {
            if (Directory.Exists(PathsService.ObsDestination))
            {
                var obsStatus = await RobocopyService.CopyAsync($"\"{PathsService.ObsDestination}\" \"{PathsService.ObsSource}\" /E /COPY:DAT /R:3 /W:5");
                
                LogService.AddLog($"obs-studio STATUS: {obsStatus.Item1}");
                    
                log.AppendLine($"obs-studio STATUS: {obsStatus.Item1}");

                if (!string.IsNullOrEmpty(obsStatus.Item2))
                {
                    LogService.AddLog($"obs-studio ERRO: {obsStatus.Item2}");
                    
                    log.AppendLine($"obs-studio ERRO: {obsStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"A PASTA \"obs-studio\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.ObsDestination}\"");
                
                log.AppendLine($"A PASTA \"obs-studio\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.ObsDestination}\"");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"ObsBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task DuckStationBackupAsync(StringBuilder log)
    {
        try
        {
            if (Directory.Exists(PathsService.DuckStationDestination))
            {
                var duckStationStatus = await RobocopyService.CopyAsync($"\"{PathsService.DuckStationDestination}\" \"{PathsService.DuckStationSource}\" /E /COPY:DAT /R:3 /W:5");
                
                LogService.AddLog($"DuckStation STATUS: {duckStationStatus.Item1}");
                    
                log.AppendLine($"DuckStation STATUS: {duckStationStatus.Item1}");

                if (!string.IsNullOrEmpty(duckStationStatus.Item2))
                {
                    LogService.AddLog($"DuckStation ERRO: {duckStationStatus.Item2}");
                    
                    log.AppendLine($"DuckStation ERRO: {duckStationStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"A PASTA \"DuckStation\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.DuckStationDestination}\"");
                
                log.AppendLine($"A PASTA \"DuckStation\" NÃO FOI ENCONTRADA NO CAMINHO: \"{PathsService.DuckStationDestination}\"");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"DuckStationBackupAsync()\": {ex.Message}");
        }
    }
    
    
    private static async Task CsBackupAsync(StringBuilder log)
    {
        try
        {
            if (!Directory.Exists(PathsService.CsInD))
            {
                LogService.AddLog($"A PASTA \"cfg\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.CsInD}");
                
                log.AppendLine($"A PASTA \"cfg\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.CsInD}");

                return;
            }
            
            var csCfgStatus = await RobocopyService.CopyAsync($"\"{PathsService.CsInD}\" \"{PathsService.CsInC}\" *.cfg /COPY:DAT /R:3 /W:5");
            
            LogService.AddLog($"Cs cfg STATUS: {csCfgStatus.Item1}");
                    
            log.AppendLine($"Cs cfg STATUS: {csCfgStatus.Item1}");
            
            if (!string.IsNullOrEmpty(csCfgStatus.Item2))
            {
                LogService.AddLog($"Cs cfg ERRO: {csCfgStatus.Item2}");
                    
                log.AppendLine($"Cs cfg ERRO: {csCfgStatus.Item2}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"CSBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task LmuBackupAsync(StringBuilder log)
    {
        try
        {
            if (!Directory.Exists(PathsService.LmuInD))
            {
                LogService.AddLog($"A PASTA \"UserData\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.LmuInD}");

                log.AppendLine($"A PASTA \"UserData\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.LmuInD}");

                return;
            }

            var lmuStatus = await RobocopyService.CopyAsync($"\"{PathsService.LmuInD}\" \"{PathsService.LmuInC}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");

            LogService.AddLog($"LMU STATUS: {lmuStatus.Item1}");

            log.AppendLine($"LMU STATUS: {lmuStatus.Item1}");

            if (!string.IsNullOrEmpty(lmuStatus.Item2))
            {
                LogService.AddLog($"LMU ERRO: {lmuStatus.Item2}");

                log.AppendLine($"LMU ERRO: {lmuStatus.Item2}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"LmuBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task TarkovBackupAsync(StringBuilder log)
    {
        try
        {
            if (!Directory.Exists(PathsService.TarkovInD))
            {
                LogService.AddLog($"A PASTA \"Settings\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.TarkovInD}");
                
                log.AppendLine($"A PASTA \"Settings\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.TarkovInD}");

                return;
            }
            
            var tarkovStatus = await RobocopyService.CopyAsync($"\"{PathsService.TarkovInD}\" \"{PathsService.TarkovInC}\" *.ini /COPY:DAT /R:3 /W:5");
            
            LogService.AddLog($"Tarkov STATUS: {tarkovStatus.Item1}");
                    
            log.AppendLine($"Tarkov STATUS: {tarkovStatus.Item1}");
            
            if (!string.IsNullOrEmpty(tarkovStatus.Item2))
            {
                LogService.AddLog($"Tarkov ERRO: {tarkovStatus.Item2}");
                    
                log.AppendLine($"Tarkov ERRO: {tarkovStatus.Item2}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"TarkovBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task AfterburnerBackupAsync(StringBuilder log)
    {
        try
        {
            if (!Directory.Exists(PathsService.AfterburnerInD))
            {
                LogService.AddLog($"A PASTA \"Profiles\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.AfterburnerInD}");

                log.AppendLine($"A PASTA \"Profiles\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.AfterburnerInD}");

                return;
            }

            var afterburnerStatus = await RobocopyService.CopyAsync($"\"{PathsService.AfterburnerInD}\" \"{PathsService.AfterburnerInC}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");

            LogService.AddLog($"Afterburner STATUS: {afterburnerStatus.Item1}");

            log.AppendLine($"Afterburner STATUS: {afterburnerStatus.Item1}");

            if (!string.IsNullOrEmpty(afterburnerStatus.Item2))
            {
                LogService.AddLog($"Afterburner ERRO: {afterburnerStatus.Item2}");

                log.AppendLine($"Afterburner ERRO: {afterburnerStatus.Item2}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"AfterburnerBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task RivaTunerBackupAsync(StringBuilder log)
    {
        try
        {
            // PROFILES FOLDER COPY
            if (!Directory.Exists(PathsService.RivaProfilesInD))
            {
                LogService.AddLog($"A PASTA \"Profiles\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.RivaProfilesInD}");

                log.AppendLine($"A PASTA \"Profiles\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.RivaProfilesInD}");
            }

            else
            {
                var rivaProfilesStatus = await RobocopyService.CopyAsync($"\"{PathsService.RivaProfilesInD}\" \"{PathsService.RivaProfilesInC}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");

                LogService.AddLog($"Riva \"Profiles\" STATUS: {rivaProfilesStatus.Item1}");

                log.AppendLine($"Riva \"Profiles\" STATUS: {rivaProfilesStatus.Item1}");

                if (!string.IsNullOrEmpty(rivaProfilesStatus.Item2))
                {
                    LogService.AddLog($"Riva \"Profiles\" ERRO: {rivaProfilesStatus.Item2}");

                    log.AppendLine($"Riva \"Profiles\" ERRO: {rivaProfilesStatus.Item2}");
                }
            }

            // PLUGINS FOLDER COPY
            if (!Directory.Exists(PathsService.RivaPluginsInD))
            {
                LogService.AddLog($"A PASTA \"Plugins\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.RivaPluginsInD}");

                log.AppendLine($"A PASTA \"Plugins\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.RivaPluginsInD}");
            }

            else
            {
                var rivaPluginsStatus = await RobocopyService.CopyAsync($"\"{PathsService.RivaPluginsInD}\" \"{PathsService.RivaPluginsInC}\" /E /COPY:DAT /XD {PathsService.ExcludedFolders} /R:3 /W:5");

                LogService.AddLog($"Riva \"Plugins\" STATUS: {rivaPluginsStatus.Item1}");

                log.AppendLine($"Riva \"Plugins\" STATUS: {rivaPluginsStatus.Item1}");

                if (!string.IsNullOrEmpty(rivaPluginsStatus.Item2))
                {
                    LogService.AddLog($"Riva \"Plugins\" ERRO: {rivaPluginsStatus.Item2}");

                    log.AppendLine($"Riva \"Plugins\" ERRO: {rivaPluginsStatus.Item2}");
                }
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"RivaTunerBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task TudoBackupAsync(StringBuilder log)
    {
        try
        {
            if (Directory.Exists(PathsService.TudoCut))
            {
                var tudoStatus = await RobocopyService.CopyAsync($"\"{PathsService.TudoCut}\" \"{PathsService.TudoInDownloads}\" /E /MOVE /R:3 /W:5");
                
                LogService.AddLog($"TUDO STATUS: {tudoStatus.Item1}");
                    
                log.AppendLine($"TUDO STATUS: {tudoStatus.Item1}");

                if (!string.IsNullOrEmpty(tudoStatus.Item2))
                {
                    LogService.AddLog($"TUDO ERRO: {tudoStatus.Item2}");
                    
                    log.AppendLine($"TUDO ERRO: {tudoStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"A PASTA \"TUDO\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.TudoCut}");
                
                log.AppendLine($"A PASTA \"TUDO\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.TudoCut}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"TudoBackupAsync()\": {ex.Message}");
        }
    }


    private static async Task VideosBackupAsync(StringBuilder log)
    {
        try
        {
            if (Directory.Exists(PathsService.VideosGravadosInDrive))
            {
                var videosStatus = await RobocopyService.CopyAsync($"\"{PathsService.VideosGravadosInDrive}\" \"{PathsService.VideosGravadosInVideos}\" /E /MOVE /R:3 /W:5");
                
                LogService.AddLog($"Vídeos Gravados STATUS: {videosStatus.Item1}");
                    
                log.AppendLine($"Vídeos Gravados STATUS: {videosStatus.Item1}");

                if (!string.IsNullOrEmpty(videosStatus.Item2))
                {
                    LogService.AddLog($"Vídeos Gravados ERRO: {videosStatus.Item2}");
                    
                    log.AppendLine($"Vídeos Gravados ERRO: {videosStatus.Item2}");
                }
            }
            else
            {
                LogService.AddLog($"A PASTA \"Vídeos gravados\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.VideosGravadosInDrive}");
                
                log.AppendLine($"A PASTA \"Vídeos gravados\" NÃO FOI ENCONTRADA NO CAMINHO: {PathsService.VideosGravadosInDrive}");
            }
        }
        catch (Exception ex)
        {
            log.AppendLine($"ERRO NA FUNÇÃO \"VideosBackupAsync()\": {ex.Message}");
        }
    }
}