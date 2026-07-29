using System.Diagnostics;
using Backup.Models;

namespace Backup.Services;

public class DriveBackupService
{
    private const string BackupDrive = @"D:\Backups\";

    private const string BackupCodes = @"D:\Codigos\";

    private const string BackupDriveLetter = @"D:\";

    private const string DevDrive = @"E:\";

    private static readonly string VideosPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Videos", "Vídeos gravados");

    private static readonly string TudoExists = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");

    public static string MakeDriveBackup()
    {
        try
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var repositoriesPath = Path.Combine(DevDrive, "Repositories");

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
                            $"\"{directory}\" \"{BackupDrive}{folderName}\" /E /COPY:DAT /XD {excludedFolders} /R:3 /W:5"
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
                        Arguments = $"\"{directory}\" \"{BackupCodes}{folderName}\" /E /COPY:DAT /R:3 /W:5"
                    });

                    repositories?.WaitForExit();

                    if (repositories is { ExitCode: > 3 })
                    {
                        Console.WriteLine($"Erro ao copiar repo: {directory}");
                    }
                }

                var publishSource = Path.Combine(DevDrive, "Repositories", "C#");

                var publishDestination = Path.Combine(BackupCodes, "C#");

                var publishBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{publishSource}\" \"{publishDestination}\" publish.txt /COPY:DAT /R:3 /W:5"
                });

                publishBackup?.WaitForExit();
            }

            if (Directory.Exists(TudoExists))
            {
                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");

                var downloadsBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{downloadsPath}\" \"{BackupDriveLetter}TUDO\" /E /MOVE /R:3 /W:5"
                });

                downloadsBackup?.WaitForExit();
            }
            else
            {
                Console.WriteLine($"NÃO CONTEM A PASTA \"TUDO\" NO SEGUINTE CAMINHO: {TudoExists}");
            }

            var videosExists = DirectoryService.VideosExists();

            if (!videosExists)
            {
                return "BACKUP FEITO DE TODOS OS ARQUIVOS";
            }

            var videosBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{VideosPath}\" \"{BackupDriveLetter}Vídeos gravados\" /E /COPY:DAT /R:3 /W:5"
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