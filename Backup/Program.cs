using Backup.Services;

namespace Backup;

public static class Program
{
    public static async Task Main(string[] args)
    {
        if (!InitializerService.IsAdmin())
        {
            InitializerService.ElevateToAdmin();
            return;
        }

        InitializerService.ReadJson();

        SchedulerService.CheckTasks();

        LanguageLayoutService.DisableLanguageShortcut();

        PowerPlanService.SetPlan();

        PowerPlanService.SetMonitorTimeout();

        PowerPlanService.SetSleepTimeout();
        
        DriveService.DevDriveExists();

        await MenuService.MenuAsync();
    }
}