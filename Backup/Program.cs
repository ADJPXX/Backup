using System.Diagnostics;
using System.Security.Principal;
using System.Text.Json;

namespace Backup;

public static class Program
{
    private static bool IsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }


    private static Config? _config;
    
    public static void Main(string[] args)
    {
        /* CRIAR UM PROGRAMA QUE COPIE TODAS AS PASTAS DE "BACKUP" PARA O HD, ASSIM POSSO EXECUTAR ELE ANTES DE FORMATAR O PC */

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

        try
        {
            const string jsonPath = @"D:\Instaladores\BackupConfig.json";

            var json = File.ReadAllText(jsonPath);

            _config = JsonSerializer.Deserialize<Config>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        Menu();
    }
    

    private static void Menu()
    {
        while (true)
        {
            var opcao = LerInt("\nDIGITE O QUE QUER FAZER\n[ 0 ]SAIR\n[ 1 ]FAZER BACKUP\n[ 2 ]RESTAURAR BACKUP\n[ 3 ]CRIAR DIRETÓRIO\n[ 4 ]INSTALAR PACOTES\n[ 5 ]ATUALIZAR PACOTES\nSua opção: ");

            Console.Clear();
            
            if (opcao == 0)
                break;

            switch (opcao)
            {
                case 1:
                {
                    var resultado = FazerBackup();
                    
                    Console.WriteLine(resultado);
                    break;
                }

                case 2:
                {
                    var resultado = RestaurarBackup();

                    Console.WriteLine(resultado);
                    break;
                }

                case 3:
                {
                    var resultado = CriarDiretório();
                    
                    
                    Console.WriteLine(resultado);
                    break;
                }

                case 4:
                {
                    var wingetExiste = WingetExiste();

                    if (wingetExiste)
                    {
                        var resultado = InstalarPacotes();
                        

                        Console.WriteLine(resultado);
                    }

                    else
                    {
                        InstalarWinget();
                    }

                    break;
                }

                case 5:
                {
                    var wingetExiste =  WingetExiste();
                    
                    if (wingetExiste)
                    {
                        var resultado = AtualizarPacotes();

                        Console.WriteLine(resultado);
                    }

                    else
                    {
                        InstalarWinget();
                    }
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
            
            var repositoriesPath = Path.Combine(documents, "DEVELOPER", "repositories");

            const string destinoRepo = @"D:\Codigos\";

            const string destino = @"D:\Backups\";

            const string drive = @"D:\";
            
            var excludedFolders = string.Join(' ', _config!.ExcludedFolders);
            
            foreach (var directory in Directory.GetDirectories(documents))
            {
                foreach (var dir in _config.BackupFolders)
                {
                    if (!directory.Contains(dir))
                        continue;

                    var nomePasta = Path.GetFileName(directory);

                    var documentsBackup = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments =
                            $"\"{directory}\" \"{destino}{nomePasta}\" /E /COPY:DAT /XD {excludedFolders} /R:3 /W:5"
                    });

                    documentsBackup?.WaitForExit();

                    if (documentsBackup is { ExitCode: > 3 })
                    {
                        Console.WriteLine($"Erro ao copiar: {directory}");
                    }
                }
            }

            if (!Directory.Exists(repositoriesPath)) 
                return "PASTA NÃO ENCONTRADA";
            
            foreach (var directory in Directory.GetDirectories(repositoriesPath))
            {
                var nomePasta = Path.GetFileName(directory);

                var repositories = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{directory}\" \"{destinoRepo}{nomePasta}\" /E /COPY:DAT /XD /R:3 /W:5"
                });

                repositories?.WaitForExit();
                
                if (repositories is { ExitCode: > 3 })
                {
                    Console.WriteLine($"Erro ao copiar repo: {directory}");
                }
            }

            var publishOrigem = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DEVELOPER", "repositories", "C#");

            const string publishDestino = @"D:\Codigos\C#\";

            var publishBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{publishOrigem}\" \"{publishDestino}\" publish.txt /E /COPY:DAT /R:3 /W:5"
            });
            
            publishBackup?.WaitForExit();
            
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "downloads", "TUDO");
            
            var downloadsBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{downloadsPath}\" \"{drive}TUDO\" /E /MOVE /R:3 /W:5"
            });
            
            downloadsBackup?.WaitForExit();
            
            return "TODOS ARQUIVOS COPIADOS";
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

            const string drive = @"D:\";

            var destino = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            foreach (var directory in Directory.GetDirectories(pastaBackup))
            {
                foreach (var dir in _config!.BackupFolders)
                {
                    if (!directory.Contains(dir))
                        continue;

                    var nomePasta = Path.GetFileName(directory);

                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments =
                            $"\"{directory}\" \"{destino}\\{nomePasta}\" /E /COPY:DAT /XD logs log replay replays cache caches /R:3 /W:5"
                    };

                    var process = Process.Start(startInfo);
                    process?.WaitForExit();
                }
            }

            const string publishOrigem = @"D:\Codigos\C#";
            
            var publishDestino = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DEVELOPER", "repositories", "C#");

            var publishBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{publishOrigem}\" \"{publishDestino}\" publish.txt /E /COPY:DAT /R:3 /W:5"
            });
            
            publishBackup?.WaitForExit();
            
            var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "downloads", "TUDO");
            
            var downloadsBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{drive}TUDO\" \"{downloadsPath}\" /E /MOVE /R:3 /W:5"
            });
            
            downloadsBackup?.WaitForExit();

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

            var basePath = Path.Combine(documents, "DEVELOPER", "repositories");
            
            Directory.CreateDirectory(basePath);
            Directory.CreateDirectory(Path.Combine(basePath, "C#"));
            Directory.CreateDirectory(Path.Combine(basePath, "Python"));
            
            return $"TODAS AS PASTAS FORAM CRIADAS EM {basePath}";
            
        }
        catch (Exception ex)
        {
            return  $"ERRO: {ex.Message}";
        }
    }


    private static bool WingetExiste()
    {
        try
        {
            var startInfo = Process.Start(new ProcessStartInfo
            {
                FileName = "where",
                Arguments = "winget",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            startInfo?.WaitForExit();
            return startInfo?.ExitCode == 0;
        }

        catch
        {
            return false;
        }
    }


    private static void InstalarWinget()
    {
        try
        {
            var opcao = LerString("DIGITE \"S\" PARA SIM E \"N\" PARA NÃO\nO SISTEMA NÃO POSSUI O INSTALADOR, DESEJA INSTALAR PARA QUE SEJA POSSÍVEL INSTALAR OS PACOTES?\nAO ESCOLHER SIM, O DOWNLOAD IRÁ INICIAR PELO SEU NAVEGADOR\nSua escolha: ").ToUpper();
            if (opcao is not ("S" or "SIM")) return;
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://aka.ms/getwinget",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO BAIXAR O INSTALADOR. ERRO {ex.Message}");
            }
        }

        catch
        {
            // ignored
        }
    }
    

    private static string InstalarPacotes()
    {
        try
        {
            var uninstall = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "uninstall Microsoft.OneDrive"
            };

            var processUninstall = Process.Start(uninstall);
            processUninstall?.WaitForExit();
            
            foreach (var app in _config!.Apps)
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = $"install {app} --silent --accept-package-agreements --accept-source-agreements"
                };

                var process = Process.Start(startInfo);
                process?.WaitForExit();
            }

            Console.ReadKey();
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
            var startInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "upgrade --include-unknown --all"
            };

            var process = Process.Start(startInfo);
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
                if (int.TryParse(Console.ReadLine()?.Trim(), out var inteiro))
                {
                    return inteiro;
                }
            }
        }
        catch
        {
            return -1;
        }
    }

    
    private static string LerString(string msg)
    {
        try
        {
            while (true)
            {
                Console.Write(msg);
                var str = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(str))
                {
                    return str;
                }
            }
        }
        catch (Exception)
        {
            return "";
        }
    }
}