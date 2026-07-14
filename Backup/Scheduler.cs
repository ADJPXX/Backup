using Microsoft.Win32.TaskScheduler;

namespace Backup;

public static class Scheduler
{
    public static void VerificarTarefas(List<TaskConfig> tasks)
    {
        List<TaskConfig> tarefasInexistentes = [];
        
        foreach (var task in tasks)
        {
            if (!Existe(task.Name))
            {
                tarefasInexistentes.Add(task);
            }
        }
        
        if (tarefasInexistentes.Count == 0)
        {
            return;
        }

        foreach (var task in tarefasInexistentes)
        {
            try
            {
                Criar(task);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Não foi possivel criar a tarefa {task.Name}. Erro: {ex.Message}");
            }
        }
    }

    
    public static bool Existe(string nome)
    {
        using TaskService taskService = new();

        return taskService.GetTask(nome) != null;
    }


    public static void Criar(TaskConfig task)
    {
        using TaskService taskService = new();

        TaskDefinition td = taskService.NewTask();

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

        // Trigger
        var trigger = new LogonTrigger
        {
            Delay = TimeSpan.FromSeconds(30)
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