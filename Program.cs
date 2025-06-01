using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Threading;

class Program
{
    static void Main()
    {
        try
        {
            // 1. Configure Firefox options
            var options = new FirefoxOptions();
            
            // Essential arguments for headless stability
            options.AddArguments(new[] {
                "--headless",
                "--disable-gpu",
                "--no-sandbox",
                "--disable-dev-shm-usage",
                "--disable-software-rasterizer"
            });
            
            // Correct way to set binary location (non-obsolete)
            options.BinaryLocation = "/usr/bin/firefox";
            
            // 2. Configure driver service
            var service = FirefoxDriverService.CreateDefaultService();
            service.Host = "127.0.0.1";  // Use IPv4 explicitly
            service.HideCommandPromptWindow = true;
            
            // 3. Initialize driver with extended timeout
            using (var driver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(120)))
            {
                Console.WriteLine("Driver initialized successfully");
                
                driver.Navigate().GoToUrl("https://www.google.com");
                Console.WriteLine("Page loaded");
                
                var searchBox = driver.FindElement(By.Name("q"));
                searchBox.SendKeys("Selenium C# Ubuntu");
                searchBox.Submit();
                Console.WriteLine("Search executed");
                
                Thread.Sleep(3000);
                Console.WriteLine("Test completed");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FULL ERROR:\n{ex.ToString()}");
            
            // Additional diagnostic info
            Console.WriteLine("\nDIAGNOSTICS:");
            Console.WriteLine($"Firefox version: {GetFirefoxVersion()}");
            Console.WriteLine($"Geckodriver version: {GetGeckodriverVersion()}");
        }
    }

    static string GetFirefoxVersion()
    {
        try {
            return System.Diagnostics.Process.Start("/usr/bin/firefox", "--version").StandardOutput.ReadToEnd().Trim();
        } catch {
            return "Not found";
        }
    }

    static string GetGeckodriverVersion()
    {
        try {
            return System.Diagnostics.Process.Start("geckodriver", "--version").StandardOutput.ReadToEnd().Trim();
        } catch {
            return "Not found";
        }
    }
}