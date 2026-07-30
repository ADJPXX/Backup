using System.Diagnostics;
using Backup.Models;
using OpenQA.Selenium; // dotnet add package Selenium.WebDriver
using OpenQA.Selenium.Edge; // dotnet add package Selenium.WebDriver
using OpenQA.Selenium.Support.UI; // dotnet add package Selenium.WebDriver

namespace Backup.Services;

public static class BrowserService
{
    private static EdgeDriver? _driver;

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


    private static async Task Ugreen()
    {
        try
        {
            Console.WriteLine("Abrindo o site...");

            if (_driver == null)
            {
                var options = new EdgeOptions();

                _driver = new EdgeDriver(options);
            }

            var driver = _driver;

            await driver.Navigate().GoToUrlAsync("https://us.ugreen.com/pages/download");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var input = wait.Until(d => d.FindElement(By.CssSelector("input[placeholder*='SKU']")));

            input.Clear();
            input.SendKeys("80889");
            input.SendKeys(Keys.Enter);

            Console.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao abrir o Ugreen: {ex.Message}");
        }
    }
}