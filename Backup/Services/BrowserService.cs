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
                    await Ugreen();
                }

                /*Process.Start(new ProcessStartInfo
                {
                    FileName = $"{link}",
                    UseShellExecute = true
                });*/
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }


    private static async Task Ugreen()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Channel = "msedge",
            Headless = false // true = não mostra o navegador
        });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://us.ugreen.com/pages/download");

        var input = page.GetByRole(AriaRole.Textbox, new() { Name = "Search by Product SKU/Model" });

        await input.ClickAsync();

        await input.PressSequentiallyAsync("80889");

        await input.PressAsync("Enter");
    }
}