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
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BackupConfig.json");

            if (!File.Exists(jsonPath))
            {
                CreateDefaultSettings(jsonPath);
            }

            var json = File.ReadAllText(jsonPath);

            var config = JsonSerializer.Deserialize<ConfigDto>(json);

            if (config == null)
            {
                throw new Exception("Invalid config file");
            }

            Config.Initialize(config);

        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    private static void CreateDefaultSettings(string jsonPath)
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


    public static bool IsAdmin()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }


    public static void ElevateToAdmin()
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
}