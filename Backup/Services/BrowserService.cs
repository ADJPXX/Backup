using System.Diagnostics;
using Backup.Models;
using Microsoft.Playwright; // dotnet add package Microsoft.Playwright

namespace Backup.Services;

public static class BrowserService
{
    public static async Task OpenLinks()
    {
        try
        {
            foreach (var link in Config.Configs.Links)
            {
                if (link.Contains("us.ugreen.com"))
                {
                    await Ugreen("80889");
                }

                if (!link.Contains("us.ugreen.com"))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = $"{link}",
                        UseShellExecute = true
                    });
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }


    private static async Task Ugreen(string model)
    {
        if (PathsService._playwright == null)
        {
            PathsService._playwright = await Playwright.CreateAsync();

            PathsService._browser = await PathsService._playwright.Chromium.LaunchAsync(new()
            {
                Channel = "msedge",
                Headless = false // true = não mostra o navegador
            });
        }

        if (PathsService._page == null)
        {
            PathsService._page = await PathsService._browser.NewPageAsync();

            await PathsService._page.GotoAsync("https://us.ugreen.com/pages/download");
        }

        await PathsService._page.BringToFrontAsync();

        var input = PathsService._page.GetByRole(AriaRole.Textbox, new() { Name = "Search by Product SKU/Model" });

        await input.FillAsync("");

        await input.PressSequentiallyAsync(model);

        await input.PressAsync("Enter");
    }
}