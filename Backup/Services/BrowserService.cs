using System.Diagnostics;
using Backup.Models;

namespace Backup.Services;

public static class BrowserService
{
    public static void OpenLinks()
    {
        try
        {
            foreach (var link in Config.Configs.Links)
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
}