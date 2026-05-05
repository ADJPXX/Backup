using System.Diagnostics;
using System.Security.Principal;

namespace Backup;

public static class Program
{
    private static readonly ProcessStartInfo StartInfo = new();
    private static bool IsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
    
    
    public static void Main(string[] args)
    {
        /* CRIAR UM PROGRAMA QUE COPIE TODAS PASTAS DE BACKUP PARA O HD, ASSIM POSSO EXECUTAR ELE ANTES DE FORMATAR O PC */

        if (!IsAdmin())
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule!.FileName,
                UseShellExecute = true,
                Verb = "runas"
            };

            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                Console.WriteLine("Permissão de administrador negada.");
            }

            return;
        }

        Menu();
    }
    

    private static void Menu()
    {
        Console.Clear();
        while (true)
        {
            var opcao = LerInt("\nDIGITE O QUE QUER FAZER\n[ 0 ]SAIR\n[ 1 ]FAZER BACKUP\n[ 2 ]RESTAURAR BACKUP\n[ 3 ]CRIAR DIRETÓRIO\n[ 4 ]INSTALAR PACOTES\n[ 5 ]ATUALIZAR PACOTES\nSua opção: ");
            
            if (opcao == 0)
                break;

            switch (opcao)
            {
                case 1:
                {
                    var resultado = FazerBackup();
                    
                    Console.Clear();
                    
                    Console.WriteLine(resultado);
                    break;
                }

                case 2:
                {
                    var resultado = RestaurarBackup();
                    
                    Console.Clear();

                    Console.WriteLine(resultado);
                    break;
                }

                case 3:
                {
                    var resultado = CriarDiretório();

                    Console.Clear();
                    
                    Console.WriteLine(resultado);
                    break;
                }

                case 4:
                {
                    var resultado = InstalarPacotes();

                    Console.Clear();

                    Console.WriteLine(resultado);
                    break;
                }

                case 5:
                {
                    var resultado = AtualizarPacotes();

                    Console.Clear();

                    Console.WriteLine(resultado);
                    break;
                }
            }
        }
    }
    
    
    private static string FazerBackup()
    {
        try
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            const string destino = @"D:\Backups\";

            foreach (var directory in Directory.GetDirectories(documents))
            {
                if (!directory.Contains("Assetto") && !directory.Contains("iRacing") && !directory.Contains("Automobilista") && !directory.Contains("Race") && !directory.Contains("My Games"))
                    continue;
                
                var nomePasta = Path.GetFileName(directory);
                
                StartInfo.FileName = "robocopy";
                StartInfo.Arguments = $"\"{directory}\" \"{destino}{nomePasta}\" /E /COPY:DAT /XD logs replay cache /R:3 /W:5";

                var process = Process.Start(StartInfo);
                process?.WaitForExit();
                
            }

            return $"TODOS ARQUIVOS COPIADOS PARA: {destino}";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }


    private static string RestaurarBackup()
    {
        try
        {
            const string pastaBackup = @"D:\Backups\";

            var destino = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            foreach (var directory in Directory.GetDirectories(pastaBackup))
            {
                if (!directory.Contains("Assetto") && !directory.Contains("iRacing") && !directory.Contains("Automobilista") && !directory.Contains("Race") && !directory.Contains("My Games"))
                    continue;
                
                var nomePasta = Path.GetFileName(directory);
                
                StartInfo.FileName = "robocopy";
                StartInfo.Arguments = $"\"{directory}\" \"{destino}\\{nomePasta}\" /E /COPY:DAT /XD logs replay cache /R:3 /W:5";

                var process = Process.Start(StartInfo);
                process?.WaitForExit();
                
            }

            return $"TODOS ARQUIVOS COPIADOS PARA: {destino}";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }


    private static string CriarDiretório()
    {
        try
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            
            
            StartInfo.FileName = "cmd.exe";
            StartInfo.Arguments = $@"/C mkdir {documents}\DEVELOPER\repositories";

            var process = Process.Start(StartInfo);
            process?.WaitForExit();
            
            return $"PASTA \"{documents}\\DEVELOPER\\repositories\" CRIADA";
        }
        catch (Exception ex)
        {
            return  $"ERRO: {ex.Message}";
        }
    }


    private static string InstalarPacotes()
    {
        try
        {
            List<string> apps = 
            [
                "Microsoft.AppInstaller",
                "Microsoft.WindowsTerminal",
                "Microsoft.DotNet.SDK.10",
                "Python.Python.3.14",
                "Oracle.JavaRuntimeEnvironment",
                "Git.Git",
                "Axosoft.GitKraken",
                "AgileBits.1Password",
                "Logitech.GHUB",
                "Google.Chrome",
                "Parsec.Parsec",
                "Valve.Steam",
                "Discord.Discord",
                "OBSProject.OBSStudio",
                "JetBrains.Toolbox"
            ];

            var uninstall = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "uninstall Microsoft.OneDrive"
            };

            var processUninstall = Process.Start(uninstall);
            processUninstall?.WaitForExit();
            
            foreach (var app in apps)
            {
                StartInfo.FileName = "winget";
                StartInfo.Arguments = $"install {app}";

                var process = Process.Start(StartInfo);
                process?.WaitForExit();
            }
            
            return "APLICATIVOS INSTALADOS COM SUCESSO.";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }


    private static string AtualizarPacotes()
    {
        try
        {
            StartInfo.FileName = "winget";
            StartInfo.Arguments = "upgrade --include-unknown --all";

            var process = Process.Start(StartInfo);
            process?.WaitForExit();

            return "APLICATIVOS ATUALIZADOS.";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }
    
    
    private static int LerInt(string msg)
    {
        try
        {
            while (true)
            {
                Console.Write(msg);
                if (int.TryParse(Console.ReadLine(), out var inteiro))
                {
                    return inteiro;
                }
            }
        }
        catch (Exception)
        {
            return -1;
        }
    }
}