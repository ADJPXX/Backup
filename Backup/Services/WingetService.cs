using System.Diagnostics;
using Backup.Models;

namespace Backup.Services;

public static class WingetService
{
    public static bool WingetExists()
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


    public static void InstallWinget()
    {
        try
        {
            var option = ConsoleService.ReadString("DIGITE \"S\" PARA SIM E \"N\" PARA NÃO\nO SISTEMA NÃO POSSUI O INSTALADOR, DESEJA INSTALAR PARA QUE SEJA POSSÍVEL INSTALAR OS PACOTES?\nAO ESCOLHER SIM, O DOWNLOAD IRÁ INICIAR PELO SEU NAVEGADOR\nSua escolha: ").ToUpper();
            while (true)
            {
                switch (option)
                {
                    case "S" or "SIM":
                    {
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

                        break;
                    }

                    case "N" or "NAO":
                    {
                        break;
                    }

                    default:
                    {
                        Console.Clear();
                        Console.WriteLine("Opção inválida, tente novamente.\n");
                        continue;
                    }
                }
            }
        }

        catch
        {
            // ignored
        }
    }


    public static string InstallPackages()
    {
        try
        {
            var uninstall = Process.Start(new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "uninstall Microsoft.OneDrive"
            });

            uninstall?.WaitForExit();

            foreach (var app in Config.Configs.Apps)
            {
                var startInfo = Process.Start(new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = $"install {app} --silent --accept-package-agreements --accept-source-agreements"
                });

                startInfo?.WaitForExit();
            }

            return "APLICATIVOS INSTALADOS COM SUCESSO.";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }


    public static string UpgradePackages()
    {
        try
        {
            var startInfo = Process.Start(new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = "upgrade --include-unknown --all"
            });

            startInfo?.WaitForExit();

            return "APLICATIVOS ATUALIZADOS.";
        }
        catch (Exception ex)
        {
            return $"ERRO: {ex.Message}";
        }
    }
}