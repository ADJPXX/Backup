using System.Diagnostics;

namespace Backup.Services;

public static class PowerPlanService
{
    public static void SetSleepTimeout()
    {
        try
        {
            RunPowerCfg("/change standby-timeout-ac 0");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"SetSleepTimeout()\": {ex.Message}");
        }
    }


    public static void SetMonitorTimeout()
    {
        try
        {
            RunPowerCfg("/change monitor-timeout-ac 0");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"SetMonitorTimeout()\": {ex.Message}");
        }
    }


    public static void SetPlan()
    {
        try
        {
            RunPowerCfg("/setactive SCHEME_MIN");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"SetPlan()\": {ex.Message}");
        }
    }


    private static void RunPowerCfg(string arguments)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "powercfg",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            })?.WaitForExit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"RunPowerCfg()\": {ex.Message}");
        }
    }
}