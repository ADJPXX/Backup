namespace Backup;

public class Config
{
    public List<string> Apps { get; set; } = [];
    public List<string> BackupFolders { get; set; } = [];
    public List<string> ExcludedFolders { get; set; } = [];
}