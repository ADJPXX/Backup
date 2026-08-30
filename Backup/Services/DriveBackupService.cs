using System.Diagnostics;
using Backup.Models;

namespace Backup.Services;

public static class DriveBackupService
{
    public static string MakeDriveBackup()
    {
        try
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var repositoriesPath = Path.Combine(PathsService.DevDrive, "Repositories");

            var excludedFolders = string.Join(' ', Config.Configs.ExcludedFolders.Select(folder => $"\"{folder}\""));

            foreach (var directory in Directory.GetDirectories(documents))
            {
                foreach (var dir in Config.Configs.BackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var folderName = Path.GetFileName(directory);

                    var documentsBackup = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments =
                            $"\"{directory}\" \"{PathsService.BackupDrive}{folderName}\" /E /COPY:DAT /XD {excludedFolders} /R:3 /W:5"
                    });

                    documentsBackup?.WaitForExit();

                    if (documentsBackup is { ExitCode: > 3 })
                    {
                        Console.WriteLine($"Erro ao copiar: {directory}");
                    }
                }
            }

            if (!Directory.Exists(repositoriesPath))
            {
                Console.WriteLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {repositoriesPath}");
            }

            else
            {
                foreach (var directory in Directory.GetDirectories(repositoriesPath))
                {
                    var folderName = Path.GetFileName(directory);

                    var repositories = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments = $"\"{directory}\" \"{PathsService.BackupCodes}{folderName}\" /E /COPY:DAT /R:3 /W:5"
                    });

                    repositories?.WaitForExit();

                    if (repositories is { ExitCode: > 3 })
                    {
                        Console.WriteLine($"Erro ao copiar repo: {directory}");
                    }
                }

                var publishSource = Path.Combine(PathsService.DevDrive, "Repositories", "C#");

                var publishDestination = Path.Combine(PathsService.BackupCodes, "C#");

                var publishBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{publishSource}\" \"{publishDestination}\" publish.txt /COPY:DAT /R:3 /W:5"
                });

                publishBackup?.WaitForExit();

                var dotGithubSource = Path.Combine(PathsService.DevDrive, "Repositories", "C#", ".github");

                var dotGithubDestination = Path.Combine(PathsService.BackupCodes, "C#", ".github");

                var dotGithubBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{dotGithubSource}\" \"{dotGithubDestination}\" /E /COPY:DAT /R:3 /W:5"
                });

                dotGithubBackup?.WaitForExit();
            }

            var davinciSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Blackmagic Design");

            var davinciDestination = Path.Combine(PathsService.BackupDrive, "DaVinci Resolve", "Blackmagic Design");

            if (Directory.Exists(davinciSource))
            {
                var davinciBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{davinciSource}\" \"{davinciDestination}\" /E /COPY:DAT /R:3 /W:5"
                });

                davinciBackup?.WaitForExit();
            }
            else
            {
                Console.WriteLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {davinciSource}");
            }

            var obsSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "obs-studio");

            var obsDestination = Path.Combine(PathsService.BackupDrive, "obs-studio");

            if (Directory.Exists(obsSource))
            {
                var obsBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{obsSource}\" \"{obsDestination}\" /E /COPY:DAT /R:3 /W:5"
                });

                obsBackup?.WaitForExit();
            }
            else
            {
                Console.WriteLine($"A SEGUINTE PASTA NÃO FOI ENCONTRADA: {obsSource}");
            }

            if (Directory.Exists(PathsService.TudoExists))
            {
                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");

                var downloadsBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{downloadsPath}\" \"{PathsService.BackupDriveLetter}TUDO\" /E /MOVE /R:3 /W:5"
                });

                downloadsBackup?.WaitForExit();
            }
            else
            {
                Console.WriteLine($"NÃO CONTEM A PASTA \"TUDO\" NO SEGUINTE CAMINHO: {PathsService.TudoExists}");
            }

            var videosExists = DirectoryService.VideosExists();

            if (!videosExists)
            {
                return "BACKUP FEITO DE TODOS OS ARQUIVOS";
            }

            var videosBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{PathsService.VideosPath}\" \"{PathsService.BackupDriveLetter}Vídeos gravados\" /E /COPY:DAT /R:3 /W:5"
            });

            videosBackup?.WaitForExit();

            return "BACKUP FEITO DE TODOS OS ARQUIVOS";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }
}