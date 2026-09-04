using Backup.Models;

namespace Backup.Services;

public static class PathsService
{
    public static readonly string Documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    
    public static readonly string RepositoriesPath = Path.Combine(DevDrive, "Repositories");
    
    public static readonly string ExcludedFolders = string.Join(' ', Config.Configs.ExcludedFolders.Select(folder => $"\"{folder}\""));
    
    public static readonly string RocketLeagueSource = Path.Combine(Documents, "My Games", "Rocket League");
    
    public static readonly string RocketLeagueDestination = Path.Combine(BackupDrive, "My Games", "Rocket League");
    
    public static readonly string PublishSource = Path.Combine(DevDrive, "Repositories", "C#");

    public static readonly string PublishDestination = Path.Combine(BackupCodes, "C#");
    
    public static readonly string DotGithubSource = Path.Combine(DevDrive, "Repositories", "C#", ".github");

    public static readonly string DotGithubDestination = Path.Combine(BackupCodes, "C#", ".github");
    
    public static readonly string DavinciSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Blackmagic Design");

    public static readonly string DavinciDestination = Path.Combine(BackupDrive, "DaVinci Resolve", "Blackmagic Design");
    
    public static readonly string ObsSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "obs-studio");

    public static readonly string ObsDestination = Path.Combine(BackupDrive, "obs-studio");
    
    public static readonly string DuckStationSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DuckStation");

    public static readonly string DuckStationDestination = Path.Combine(BackupDrive, "DuckStation");
    
    public const string CloudBackup = @"G:\Meu Drive\BackupCloud\";

    public const string BackupDriveLetter = @"D:\";

    public static readonly string VideosPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Videos", "Vídeos gravados");

    public const string DevDrive = @"E:\";

    public const string BackupDrive = @"D:\Backups\";

    public const string BackupCodes = @"D:\Codigos\";

    public static readonly string TudoExists = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");

    public static readonly string VideosGravadosExiste = Path.Combine(BackupDriveLetter, "Vídeos gravados");
}