using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    static void Main()
    {
        try
        {
            // 1. Verify Firefox installation
            string firefoxPath = FindFirefoxPath();
            if (string.IsNullOrEmpty(firefoxPath))
            {
                throw new Exception("Firefox not found. Install with: sudo apt install firefox");
            }

            // 2. Configure Firefox options
            var options = new FirefoxOptions();
            options.BinaryLocation = firefoxPath;
            
            // Essential stability arguments
            options.AddArguments(new[] {
                "--headless",
                "--disable-gpu",
                "--no-sandbox",
                "--disable-dev-shm-usage",
                "--width=1920",
                "--height=1080"
            });

            // 3. Configure driver service
            var service = FirefoxDriverService.CreateDefaultService();
            service.Host = "127.0.0.1";
            service.HideCommandPromptWindow = true;
            
            // 4. Environment cleanup
            CleanTempFiles();
            KillExistingProcesses();

            // 5. Initialize driver with extended timeout
            using (var driver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(30)))
            {
                driver.Navigate().GoToUrl("https://www.google.com");
                var searchBox = driver.FindElement(By.Name("q"));
                searchBox.SendKeys("Selenium C# Ubuntu");
                searchBox.Submit();
                Thread.Sleep(3000);
                Console.WriteLine("Test completed successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine("TROUBLESHOOTING:");
            Console.WriteLine($"1. Verify Firefox is installed: sudo apt install firefox");
            Console.WriteLine($"2. Check geckodriver: chmod +x $(which geckodriver)");
            Console.WriteLine($"3. Clean temp files: rm -rf /tmp/rust_mozprofile*");
        }
    }

    static string FindFirefoxPath()
    {
        string[] possiblePaths = {
            "/usr/bin/firefox",
            "/usr/local/bin/firefox",
            "/snap/bin/firefox"
        };

        foreach (var path in possiblePaths)
        {
            if (File.Exists(path)) return path;
        }
        return null;
    }

    static void CleanTempFiles()
    {
        try {
            foreach (var file in Directory.GetFiles("/tmp", "rust_mozprofile*"))
                File.Delete(file);
        } catch {}
    }

    static void KillExistingProcesses()
    {
        try {
            Process.Start("pkill", "-f firefox").WaitForExit();
            Process.Start("pkill", "-f geckodriver").WaitForExit();
        } catch {}
    }
}