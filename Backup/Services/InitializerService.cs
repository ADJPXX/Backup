using System.Diagnostics;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Text.Json;
using Backup.Models;

namespace Backup.Services;

public static class InitializerService
{
    public static void ReadJson()
    {
        try
        {
            if (!File.Exists(PathsService.JsonPath))
            {
                CreateDefaultSettings(PathsService.JsonPath);
            }

            var json = File.ReadAllText(PathsService.JsonPath);

            var config = JsonSerializer.Deserialize<ConfigDto>(json);

            if (config == null)
            {
                throw new Exception("ARQUIVO DE CONFIGURAÇÃO INVÁLIDO");
            }

            Config.Initialize(config);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"ReadJson()\": {ex.Message}");
        }
    }


    private static void CreateDefaultSettings(string jsonPath)
    {
        try
        {
            var configs = new ConfigDto
            {
                Apps =
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
                    "JetBrains.Toolbox",
                    "Google.GoogleDrive"
                ],


                Tasks =
                [
                    new TaskConfig
                    {
                        Name = "TempCleaner",
                        ExecutablePath = @"D:\SCRIPTS\TempCleaner.exe",
                        Delay = 10
                    },
                    new TaskConfig
                    {
                        Name = "CloudBackup",
                        ExecutablePath = @"D:\SCRIPTS\CloudBackup\CloudBackup.exe",
                        Delay = 30
                    },
                    new TaskConfig
                    {
                        Name = "MyCalendar",
                        ExecutablePath = @"D:\SCRIPTS\MyCalendar\MyCalendar.exe",
                        Delay = 5
                    }
                ],


                BackupFolders =
                [
                    "Assetto Corsa",
                    "Assetto Corsa Competizione",
                    "iRacing",
                    "Automobilista 2",
                    "RaceLabApps",
                    "My Games"
                ],


                CloudBackupFolders =
                [
                    "Backups",
                    "Book do globis",
                    "Codigos",
                    "Contratos apartamentos",
                    "Fotos Steam",
                    "Instaladores",
                    "Jogos e emuladores",
                    "Vídeos",
                    "Wallpapers"
                ],


                ExcludedFolders =
                [
                    "log",
                    "cache",
                    "replay",
                    "logs",
                    "caches",
                    "replays"
                ],

                FoldersToCreate =
                [
                    "C",
                    "C#",
                    "Python"
                ],


                Links =
                [
                    "https://www.amd.com/en/support/downloads/drivers.html/chipsets/am5/x670e.html",
                    "https://www.nvidia.com/pt-br/drivers/",
                    "https://us.ugreen.com/pages/download"
                ]
            };

            var jsonWrite = JsonSerializer.Serialize(configs, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            File.WriteAllText(jsonPath, jsonWrite);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"CreateDefaultSettings()\": {ex.Message}");
        }
    }


    public static void CheckLogFile()
    {
        if (!File.Exists(PathsService.LogPath))
        {
            File.Create(PathsService.LogPath).Dispose();
        }
    }
    
    
    public static bool IsAdmin()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"IsAdmin()\": {ex.Message}");
            return false;
        }
    }


    public static void ElevateToAdmin()
    {
        try
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NA FUNÇÃO \"ElevateToAdmin()\": {ex.Message}");
        }
    }
}