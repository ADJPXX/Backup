namespace Backup.Models;

public static class Config
{
    public static ConfigDto Configs { get; set; } = new();

    public static bool Initialized { get; set; } = false;

    public static void Initialize(ConfigDto configDto)
    {
        Configs = configDto;
        Initialized = true;
    }

}


public class ConfigDto
{
    public List<string> Apps { get; set; } = [];

    public List<TaskConfig> Tasks { get; set; } = [];

    public List<string> BackupFolders { get; set; } = [];

    public List<string> CloudBackupFolders { get; set; } = [];

    public List<string> ExcludedFolders { get; set; } = [];

    public List<string> FoldersToCreate { get; set; } = [];

    public List<string> Links { get; set; } = [];
}