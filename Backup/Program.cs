using Backup.Services;

namespace Backup;

public static class Program
{
    public static void Main(string[] args)
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

        while (true)
        {
            var devDriveExists = DriveService.DevDriveExists();

            if (devDriveExists)
            {
                break;
            }
        }

        MenuService.Menu();
    }
}