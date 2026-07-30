using System.Diagnostics;
using Backup.Models;
using Microsoft.Playwright; // dotnet add package Microsoft.Playwright

namespace Backup.Services;

public static class BrowserService
{
    private static IBrowser? _browser;

    private static IPlaywright? _playwright;

    private static IPage? _page;

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
        if (_playwright == null)
        {
            _playwright = await Playwright.CreateAsync();

            _browser = await _playwright.Chromium.LaunchAsync(new()
            {
                Channel = "msedge",
                Headless = false // true = não mostra o navegador
            });
        }

        if (_page == null)
        {
            _page = await _browser.NewPageAsync();

            await _page.GotoAsync("https://us.ugreen.com/pages/download");
        }

        await _page.BringToFrontAsync();

        var input = _page.GetByRole(AriaRole.Textbox, new() { Name = "Search by Product SKU/Model" });

        await input.FillAsync("");

        await input.PressSequentiallyAsync(model);

        await input.PressAsync("Enter");
    }
}