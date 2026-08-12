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

        PowerPlanService.SetPlan();

        PowerPlanService.SetMonitorTimeout();

        PowerPlanService.SetSleepTimeout();

        var devDriveExists = DriveService.DevDriveExists();

        if (!devDriveExists.Item1)
        {
            Console.WriteLine($"\nO programa irá iniciar em 30 SEGUNDOS para que você crie o Dev Drive ({devDriveExists.Item2}).\nCaso contrário poderá dar problema ao fazer backup ou restaurar backup.");
            Thread.Sleep(30000);
            Console.Clear();
        }

        MenuService.Menu();
    }
}