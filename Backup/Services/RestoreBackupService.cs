using System.Diagnostics;
using Backup.Models;

namespace Backup.Services;

public static class RestoreBackupService
{
    private const string BackupDrive = @"D:\Backups\";

    private const string BackupCodigos = @"D:\Codigos\";

    private const string BackupDriveLetter = @"D:\";

    private const string DevDrive = @"E:\";

    private static readonly string VideosPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Videos", "Vídeos gravados");

    private static readonly string VideosGravadosExiste = Path.Combine(BackupDriveLetter, "Vídeos gravados");

    public static string RestoreBackup()
    {
        try
        {
            var destination = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            foreach (var directory in Directory.GetDirectories(BackupDrive))
            {
                foreach (var dir in Config.Configs.BackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var folderName = Path.GetFileName(directory);

                    var startInfo = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments =
                            $"\"{directory}\" \"{destination}\\{folderName}\" /E /COPY:DAT /XD logs log replay replays cache caches /R:3 /W:5"
                    });

                    startInfo?.WaitForExit();
                }
            }

            var publishSource = Path.Combine(BackupCodigos, "C#");

            var publishDestination = Path.Combine(DevDrive, "Repositories", "C#");

            var publishBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{publishSource}\" \"{publishDestination}\" publish.txt /COPY:DAT /R:3 /W:5"
            });

            publishBackup?.WaitForExit();

            var gitSource = Path.Combine(BackupCodigos, "C#");

            var gitDestination = Path.Combine(DevDrive, "Repositories", "C#");

            var gitBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{gitSource}\" \"{gitDestination}\" .gitignore /COPY:DAT /R:3 /W:5"
            });

            gitBackup?.WaitForExit();

            var tudoExists = Path.Combine(BackupDriveLetter, "TUDO");

            if (Directory.Exists(tudoExists))
            {
                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");

                var downloadsBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{BackupDriveLetter}TUDO\" \"{downloadsPath}\" /E /MOVE /R:3 /W:5"
                });

                downloadsBackup?.WaitForExit();
            }
            else
            {
                Console.WriteLine($"NÃO CONTEM PASTA \"TUDO\" NO SEGUINTE CAMINHO: {tudoExists}");
            }

            if (Directory.Exists(VideosGravadosExiste))
            {
                var videosBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{BackupDriveLetter}Vídeos gravados\" \"{VideosPath}\" /E /MOVE /R:3 /W:5"
                });

                videosBackup?.WaitForExit();
            }

            else
            {
                Console.WriteLine($"NÃO CONTEM PASTA \"Vídeos gravados\" NO SEGUINTE CAMINHO: {VideosGravadosExiste}");
            }

            return "TODOS ARQUIVOS RESTAURADOS";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }
}