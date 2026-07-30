using Microsoft.Playwright;

namespace Backup.Services;

public static class PathsService
{
    public const string CloudBackup = @"G:\Meu Drive\BackupCloud\";

    public const string BackupDriveLetter = @"D:\";

    public static readonly string VideosPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Videos", "Vídeos gravados");

    public const string DevDrive = @"E:\";

    public const string BackupDrive = @"D:\Backups\";

    public const string BackupCodes = @"D:\Codigos\";

    public static readonly string TudoExists = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");

    public static readonly string VideosGravadosExiste = Path.Combine(BackupDriveLetter, "Vídeos gravados");
}