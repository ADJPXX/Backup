namespace Backup.Models;

public class Config
{
    public List<string> Apps { get; set; } = [];
    
    public List<TaskConfig> Tasks { get; set; } = [];
    
    public List<string> BackupFolders { get; set; } = [];
    
    public List<string> CloudBackupFolders { get; set; } = [];
    
    public List<string> ExcludedFolders { get; set; } = [];
    
    public List<string> FoldersToCreate { get; set; } = [];
    
    public List<string> Links { get; set; } = [];
}