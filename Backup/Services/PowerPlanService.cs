using System.Diagnostics;

namespace Backup.Services;

public static class PowerPlanService
{
    public static void SetSleepTimeout()
    {
        RunPowerCfg("/change standby-timeout-ac 0");
    }


    public static void SetMonitorTimeout()
    {
        RunPowerCfg("/change monitor-timeout-ac 0");
    }


    public static void SetPlan()
    {
        RunPowerCfg("/setactive SCHEME_MIN");
    }


    private static void RunPowerCfg(string arguments)
    {

        Process.Start(new ProcessStartInfo
        {
            FileName = "powercfg",
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true
        })?.WaitForExit();
    }
}