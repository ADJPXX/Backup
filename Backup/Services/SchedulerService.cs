using Backup.Models;
using Microsoft.Win32.TaskScheduler; // dotnet add package TaskScheduler

namespace Backup.Services;

public static class SchedulerService
{
    public static void CheckTasks()
    {
        List<TaskConfig> nonExistentTasks = [];
        
        foreach (var task in Config.Configs.Tasks)
        {
            if (!TaskExists(task.Name))
            {
                nonExistentTasks.Add(task);
            }
        }
        
        if (nonExistentTasks.Count == 0)
        {
            return;
        }

        foreach (var task in nonExistentTasks)
        {
            try
            {
                CreateTask(task);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Não foi possivel criar a tarefa {task.Name}. Erro: {ex.Message}");
            }
        }
    }


    private static bool TaskExists(string name)
    {
        using TaskService taskService = new();

        return taskService.GetTask(name) != null;
    }


    private static void CreateTask(TaskConfig task)
    {
        using TaskService taskService = new();

        var td = taskService.NewTask();

        // Informações
        td.RegistrationInfo.Author = Environment.UserName;
        td.RegistrationInfo.Description = task.Name;

        // Executar com privilégios mais altos
        td.Principal.RunLevel = TaskRunLevel.Highest;

        // Configurações
        td.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;
        td.Settings.AllowDemandStart = true;
        td.Settings.Enabled = true;
        td.Settings.Hidden = false;
        td.Settings.StartWhenAvailable = false;
        td.Settings.RunOnlyIfIdle = false;
        td.Settings.RunOnlyIfNetworkAvailable = false;
        td.Settings.DisallowStartIfOnBatteries = false;
        td.Settings.StopIfGoingOnBatteries = false;
        td.Settings.ExecutionTimeLimit = TimeSpan.Zero;

        if (task.Delay < 5)
        {
            task.Delay = 5;
        }

        // Trigger
        var trigger = new LogonTrigger
        {
            Delay = TimeSpan.FromSeconds(task.Delay)
        };

        td.Triggers.Add(trigger);

        // Executável
        td.Actions.Add(new ExecAction(task.ExecutablePath));

        // Registrar tarefa
        taskService.RootFolder.RegisterTaskDefinition(
            task.Name,
            td,
            TaskCreation.CreateOrUpdate,
            null,
            null,
            TaskLogonType.InteractiveToken);
    }
}