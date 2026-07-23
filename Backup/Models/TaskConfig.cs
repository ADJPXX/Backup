namespace Backup.Models;

public class TaskConfig
{
    public string Name { get; set; } = "";
    
    public string ExecutablePath { get; set; } = "";

    public int Delay { get; set; }
}