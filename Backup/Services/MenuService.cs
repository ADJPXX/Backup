namespace Backup.Services;

public static class MenuService
{
    public static async Task MenuAsync()
    {
        while (true)
        {
            var option = ConsoleService.ReadInt("\nDIGITE O QUE QUER FAZER\n[ 0 ]SAIR\n[ 1 ]FAZER BACKUP PARA O DRIVE\n[ 2 ]FAZER BACKUP PARA A NUVEM\n[ 3 ]RESTAURAR BACKUP\n[ 4 ]CRIAR DIRETÓRIO\n[ 5 ]INSTALAR PACOTES\n[ 6 ]ATUALIZAR PACOTES\n[ 7 ]ABRIR LINKS PARA DOWNLOAD DE DRIVERS\nSua opção: ");

            Console.Clear();

            if (option == 0)
                break;

            switch (option)
            {
                case 1:
                {
                    var result = await DriveBackupService.MakeDriveBackupAsync();

                    Console.WriteLine(result);
                    break;
                }

                case 2:
                {
                    var result = CloudBackupService.MakeCloudBackup();

                    Console.WriteLine(result);
                    break;
                }

                case 3:
                {
                    var result = RestoreBackupService.RestoreBackup();

                    Console.WriteLine(result);
                    break;
                }

                case 4:
                {
                    var driveExists = DriveService.DevDriveExists();

                    if (!driveExists)
                    {
                        break;
                    }

                    var result = DirectoryService.CreateDirectories();

                    Console.WriteLine(result);
                    break;
                }

                case 5:
                {
                    var wingetExists = WingetService.WingetExists();

                    if (wingetExists)
                    {
                        var result = WingetService.InstallPackages();


                        Console.WriteLine(result);
                    }

                    else
                    {
                        WingetService.InstallWinget();
                    }

                    break;
                }

                case 6:
                {
                    var wingetExists = WingetService.WingetExists();

                    if (wingetExists)
                    {
                        var result = WingetService.UpgradePackages();

                        Console.WriteLine(result);
                    }

                    else
                    {
                        WingetService.InstallWinget();
                    }
                    break;
                }

                case 7:
                {
                    BrowserService.OpenLinks();
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
}