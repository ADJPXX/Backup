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

    private const string BackupDrive = @"D:\Backups\";
    
    private const string BackupCodigos = @"D:\Codigos\";
    
    private const string BackupDriveLetter = @"D:\";

    private const string CloudBackup = @"G:\Meu Drive\BackupCloud\";
    
    private const string DevDrive = @"E:\";

    private static readonly string VideosPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Videos", "Vídeos gravados");
    
    private static readonly string TudoExiste = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");
    
    private static readonly string VideosGravadosExiste = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Videos", "Vídeos gravados");
    
    public static void Main(string[] args)
    {
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
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupConfig.json");

            var json = File.ReadAllText(jsonPath);

            _config = JsonSerializer.Deserialize<Config>(json);

            if (_config == null)
            {
                Console.WriteLine("Erro ao carregar as configurações.");
                return;
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        DevDriveExiste();
        Menu();
    }
    

    private static void Menu()
    {
        while (true)
        {
            var opcao = LerInt("\nDIGITE O QUE QUER FAZER\n[ 0 ]SAIR\n[ 1 ]FAZER BACKUP PARA O DRIVE\n[ 2 ]FAZER BACKUP PARA A NUVEM\n[ 3 ]RESTAURAR BACKUP\n[ 4 ]CRIAR DIRETÓRIO\n[ 5 ]INSTALAR PACOTES\n[ 6 ]ATUALIZAR PACOTES\n[ 7 ]ABRIR LINKS PARA DOWNLOAD DE DRIVERS\nSua opção: ");

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
                    var resultado = FazerBackupNuvem();

                    Console.WriteLine(resultado);
                    break;
                }

                case 3:
                {
                    var resultado = RestaurarBackup();

                    Console.WriteLine(resultado);
                    break;
                }

                case 4:
                {
                    var resultado = CriarDiretório();
                    
                    
                    Console.WriteLine(resultado);
                    break;
                }

                case 5:
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

                case 6:
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

                case 7:
                {
                    AbrirLinks();
                    break;
                }

                default:
                {
                    Console.WriteLine("Opção inválida.");
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
            
            var repositoriesPath = Path.Combine(DevDrive, "Repositories");
            
            var excludedFolders = string.Join(' ', _config!.ExcludedFolders.Select(folder => $"\"{folder}\""));
            
            foreach (var directory in Directory.GetDirectories(documents))
            {
                foreach (var dir in _config.BackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var nomePasta = Path.GetFileName(directory);

                    var documentsBackup = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments =
                            $"\"{directory}\" \"{BackupDrive}{nomePasta}\" /E /COPY:DAT /XD {excludedFolders} /R:3 /W:5"
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
                    Arguments = $"\"{directory}\" \"{BackupCodigos}{nomePasta}\" /E /COPY:DAT /R:3 /W:5"
                });

                repositories?.WaitForExit();
                
                if (repositories is { ExitCode: > 3 })
                {
                    Console.WriteLine($"Erro ao copiar repo: {directory}");
                }
            }

            var publishOrigem = Path.Combine(DevDrive, "Repositories", "C#");

            var publishDestino = Path.Combine(BackupCodigos, "C#");

            var publishBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{publishOrigem}\" \"{publishDestino}\" publish.txt /COPY:DAT /R:3 /W:5"
            });
            
            publishBackup?.WaitForExit();
            
            if (Directory.Exists(TudoExiste))
            {
                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");
                
                var downloadsBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{downloadsPath}\" \"{BackupDriveLetter}TUDO\" /E /MOVE /R:3 /W:5"
                });
                
                downloadsBackup?.WaitForExit();
            }
            else
            {
                Console.WriteLine("NÃO CONTEM A PASTA \"TUDO\"");
            }
            
            var existeVideos = ExisteVideos();

            if (!existeVideos)
            {
                return "BACKUP FEITO DE TODOS OS ARQUIVOS";
            }
            
            var videosBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{VideosPath}\" \"{BackupDriveLetter}Vídeos gravados\" /E /COPY:DAT /R:3 /W:5"
            });
            
            videosBackup?.WaitForExit();

            return "BACKUP FEITO DE TODOS OS ARQUIVOS";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }


    private static string FazerBackupNuvem()
    {
        try
        {
            foreach (var directory in Directory.GetDirectories(BackupDriveLetter))
            {
                foreach (var dir in _config!.CloudBackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    
                    var nomePasta = Path.GetFileName(directory);

                    var destino = Path.Combine(CloudBackup, nomePasta);

                    var backupNuvem = Process.Start(new ProcessStartInfo
                    {
                        FileName = "robocopy",
                        Arguments = $"\"{directory}\" \"{destino}\" /E /COPY:DAT /R:3 /W:5"
                    });

                    backupNuvem?.WaitForExit();
                }
            }

            return "BACKUP NA NUVEM CONCLUIDO";
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
            var destino = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            foreach (var directory in Directory.GetDirectories(BackupDrive))
            {
                foreach (var dir in _config!.BackupFolders)
                {
                    if (!Path.GetFileName(directory).Equals(dir, StringComparison.OrdinalIgnoreCase))
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

            var publishOrigem = Path.Combine(BackupCodigos, "C#");
            
            var publishDestino = Path.Combine(DevDrive, "Repositories", "C#");

            var publishBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{publishOrigem}\" \"{publishDestino}\" publish.txt /COPY:DAT /R:3 /W:5"
            });
            
            publishBackup?.WaitForExit();
            
            var gitOrigem = Path.Combine(BackupCodigos, "C#");
            
            var gitDestino = Path.Combine(DevDrive, "Repositories", "C#");

            var gitBackup = Process.Start(new ProcessStartInfo
            {
                FileName = "robocopy",
                Arguments = $"\"{gitOrigem}\" \"{gitDestino}\" .gitignore /COPY:DAT /R:3 /W:5"
            });
            
            gitBackup?.WaitForExit();

            var tudoExiste = Path.Combine(BackupDriveLetter, "TUDO");
            
            if (Directory.Exists(tudoExiste))
            {
                var downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "TUDO");

                var downloadsBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{BackupDriveLetter}TUDO\" \"{downloadsPath}\" /E /MOVE /R:3 /W:5"
                });

                downloadsBackup?.WaitForExit();
            }
            else
            {
                Console.WriteLine("NÃO CONTEM PASTA \"TUDO\"");
            }

            if (Directory.Exists(VideosGravadosExiste))
            {
                var videosBackup = Process.Start(new ProcessStartInfo
                {
                    FileName = "robocopy",
                    Arguments = $"\"{BackupDriveLetter}Vídeos gravados\" \"{VideosPath}\" /E /MOVE /R:3 /W:5"
                });

                videosBackup?.WaitForExit();
            }

            else
            {
                Console.WriteLine("NÃO CONTEM PASTA \"Vídeos gravados\"");
            }
            
            return "TODOS ARQUIVOS RESTAURADOS";
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
            var basePath = Path.Combine(DevDrive, "Repositories");
            
            var recordedVideosPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Videos", "Vídeos gravados");
            
            Directory.CreateDirectory(recordedVideosPath);
            
            Directory.CreateDirectory(basePath);

            foreach (var directory in _config!.FoldersToCreate) 
            {
                Directory.CreateDirectory(Path.Combine(basePath, directory));
            }
            
            return "TODAS AS PASTAS FORAM CRIADAS";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }


    private static void AbrirLinks()
    {
        try
        {
            foreach (var link in _config!.Links)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"{link}",
                    UseShellExecute = true
                });

                if (link.Contains("us.ugreen.com"))
                {
                    Console.WriteLine("DIGITE \"80889\" NA BARRA DE PESQUISA DO SITE \"us.ugreen.com\" PARA BAIXAR O DRIVER DO MODELO CERTO!");
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }


    private static bool ExisteVideos()
    {
        try
        {
            double tamanhoTotal = 0;

            var tamanho = "MB";
            
            var diretorios = Directory.GetDirectories(VideosPath);

            if (diretorios.Length <= 0)
            {
                return false;
            }

            var files = Directory.GetFiles(VideosPath, "*", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                var infoFile = new FileInfo(file);

                var mb = infoFile.Length / 1024d / 1024d;

                tamanhoTotal += mb;
            }

            switch (tamanhoTotal)
            {
                case 0d:
                {
                    return false;
                }
                case >= 1024d:
                {
                    tamanhoTotal /= 1024d;
                    tamanho = "GB";
                    break;
                }
            }

            while (true)
            {
                Console.WriteLine($"Foram encontrados vídeos e o tamanho total deles é: {tamanhoTotal:F2} {tamanho}\nVocê gostaria de fazer backup deles? Digite \"S\" para SIM e \"N\" para NÃO");
                var opcao = LerString("Sua escolha: ").ToUpper();

                switch (opcao)
                {
                    case "S":
                        return true;
                    case "N":
                        return false;
                    default:
                        Console.Clear();
                        Console.WriteLine("OPÇÃO INVÁLIDA!");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
            return false;
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


    private static void DevDriveExiste()
    {
        var driveExiste = DriveInfo.GetDrives().Any(drive => drive.Name.Equals(DevDrive, StringComparison.OrdinalIgnoreCase));

        if (!driveExiste)
        {
            Console.WriteLine($"Drive \"{DevDrive}\" não encontrado, vou abrir a página de criação de drive para você fazer o Dev Drive\nAperte qualquer tecla para abrir a página de criação de drive.");

            Console.ReadKey();
            
            Process.Start(new ProcessStartInfo
            {
                FileName = "ms-settings:disksandvolumes",
                UseShellExecute = true
            });

            Console.Clear();
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