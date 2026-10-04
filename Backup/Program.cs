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

        // This function needs to be here as the first thing the code will execute since this one loads the mandatory configurations for the program to work properly.
        InitializerService.ReadJson();

        InitializerService.CheckLogFile();

        var taskPathExists = SchedulerService.CheckTasks();

        if (!taskPathExists)
        {
            await MenuService.MenuTasksAsync();
        }

        LanguageLayoutService.DisableLanguageShortcut();

        PowerPlanService.SetPlan();

        PowerPlanService.SetMonitorTimeout();

        PowerPlanService.SetSleepTimeout();
        
        DriveService.DevDriveExists();

        await MenuService.MenuAsync();
    }
}