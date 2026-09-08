using Backup.Models;

namespace Backup.Services;

public static class DirectoryService
{
    public static (bool, bool) VideosExists()
    {
        try
        {
            double totalSize = 0;

            var size = "MB";

            var directories = Directory.GetDirectories(PathsService.VideosGravadosInVideos);

            if (directories.Length <= 0)
            {
                return (false, false);
            }

            var files = Directory.GetFiles(PathsService.VideosGravadosInVideos, "*", SearchOption.AllDirectories);

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
                    return (false, false);
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
                        return (true, false);
                    }

                    case "N":
                    {
                        const bool yesNo = true;
                        
                        return (false, yesNo);
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
            Console.WriteLine($"ERRO NA FUNÇÃO \"VideosExists()\": {ex.Message}");
            return (false, false);
        }
    }


    public static string CreateDirectories()
    {
        try
        {
            Directory.CreateDirectory(PathsService.VideosGravadosInVideos);

            Directory.CreateDirectory(PathsService.RepositoriesPath);

            foreach (var directory in Config.Configs.FoldersToCreate)
            {
                Directory.CreateDirectory(Path.Combine(PathsService.RepositoriesPath, directory));
            }

            return "TODAS AS PASTAS FORAM CRIADAS";
        }
        catch (Exception ex)
        {
            return $"ERRO NA FUNÇÃO \"CreateDirectories()\": {ex.Message}";
        }
    }
}