using Backup.Models;

namespace Backup.Services;

public static class DirectoryService
{
    public static bool VideosExists()
    {
        try
        {
            double totalSize = 0;

            var size = "MB";

            var directories = Directory.GetDirectories(PathsService.VideosPath);

            if (directories.Length <= 0)
            {
                return false;
            }

            var files = Directory.GetFiles(PathsService.VideosPath, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var infoFile = new FileInfo(file);

                var mb = infoFile.Length / 1024d / 1024d;

                totalSize += mb;
            }

            switch (totalSize)
            {
                case 0d:
                {
                    return false;
                }
                case >= 1024d:
                {
                    totalSize /= 1024d;
                    size = "GB";
                    break;
                }
            }

            while (true)
            {
                Console.WriteLine($"Foram encontrados vídeos e o tamanho total deles é: {totalSize:F2} {size}\nVocê gostaria de fazer backup deles? Digite \"S\" para SIM e \"N\" para NÃO");
                var option = ConsoleService.ReadString("Sua escolha: ").ToUpper();

                switch (option)
                {
                    case "S":
                    {
                        return true;
                    }

                    case "N":
                    {
                        return false;
                    }

                    default:
                    {
                        Console.Clear();
                        Console.WriteLine("OPÇÃO INVÁLIDA!");
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
            return false;
        }
    }


    public static string CreateDirectories()
    {
        try
        {
            var basePath = Path.Combine(PathsService.DevDrive, "Repositories");

            var recordedVideosPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Videos", "Vídeos gravados");

            Directory.CreateDirectory(recordedVideosPath);

            Directory.CreateDirectory(basePath);

            foreach (var directory in Config.Configs.FoldersToCreate)
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
}