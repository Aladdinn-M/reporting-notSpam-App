using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string proxiesFilePath = Path.Combine(currentDirectory, "proxies.txt");
        string emailFilePath = Path.Combine(currentDirectory, "emails.txt");

        EnsureFileExists(proxiesFilePath);
        EnsureFileExists(emailFilePath);

        Console.WriteLine("Enter Number of Users:");
        if (!int.TryParse(Console.ReadLine(), out int numberOfUsers))
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
            return;
        }

        List<Task> browserTasks = new List<Task>();

        for (int i = 0; i < numberOfUsers; i++)
        {
            browserTasks.Add(OpenBrowserAndProcessEmails(i, proxiesFilePath, emailFilePath));
        }

        await Task.WhenAll(browserTasks);
    }

    private static async Task OpenBrowserAndProcessEmails(int userIndex, string proxiesFilePath, string emailFilePath)
    {
        ChromeOptions options = new ChromeOptions();
        ConfigureProxy(options, proxiesFilePath);

        using (var driver = new ChromeDriver(options))
        {
            try
            {
                await SignInToEmail(driver, emailFilePath, userIndex);
                await ProcessSpamFolder(driver);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing user {userIndex}: {ex.Message}");
            }
        }
    }

    private static void ConfigureProxy(ChromeOptions options, string filePath)
    {
        try
        {
            var proxyDetails = File.ReadAllLines(filePath)[0].Split(':');
            if (proxyDetails.Length >= 2)
            {
                var proxy = new Proxy
                {
                    HttpProxy = $"{proxyDetails[0]}:{proxyDetails[1]}",
                    SslProxy = $"{proxyDetails[0]}:{proxyDetails[1]}"
                };
                options.Proxy = proxy;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error configuring proxy: " + ex.Message);
        }
    }

    private static async Task SignInToEmail(IWebDriver driver, string filePath, int userIndex)
    {
        string[] credentials = File.ReadAllLines(filePath);
        if (userIndex >= credentials.Length)
        {
            throw new IndexOutOfRangeException("User index is out of bounds of the credentials file.");
        }

        var userCredentials = credentials[userIndex].Split(':');
        if (userCredentials.Length < 2)
        {
            throw new FormatException("Credentials file is not in the correct format.");
        }

        string email = userCredentials[0];
        string password = userCredentials[1];

        driver.Manage().Window.Maximize();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        driver.Navigate().GoToUrl("https://login.live.com/login.srf?wa=wsignin1.0&rpsnv=19&ct=1705352608&rver=7.0.6738.0&wp=MBI_SSL&wreply=https%3a%2f%2foutlook.live.com%2fowa%2f%3fcobrandid%3dab0455a0-8d03-46b9-b18b-df2f57b9e44c%26nlp%3d1%26deeplink%3dowa%252f%26RpsCsrfState%3d451c2c6d-8d13-204d-9273-6fe70ef13cf9&id=292841&aadredir=1&CBCXT=out&lw=1&fl=dob%2cflname%2cwld&cobrandid=ab0455a0-8d03-46b9-b18b-df2f57b9e44c");
        string autoITScriptPath = @"ProxyAuth.exe";
        Process.Start(autoITScriptPath);
        IWebElement emailField = driver.FindElement(By.Name("loginfmt"));
        emailField.SendKeys(email);
        emailField.SendKeys(Keys.Enter);

        // Wait for the password field to be present
        await Task.Delay(2000); // Simulating asynchronous behavior with a delay

        IWebElement passwordField = driver.FindElement(By.Name("passwd"));
        passwordField.SendKeys(password);
        passwordField.SendKeys(Keys.Enter);

        await Task.Delay(2000); // Wait for login to process

        // Handle additional prompts if present (e.g., stay signed in? prompt)
        try
        {
            IWebElement staySignedInButton = driver.FindElement(By.Id("acceptButton"));
            staySignedInButton.Click();
        }
        catch (NoSuchElementException)
        {
            // No action needed if the element does not exist
        }
    }

    private static async Task ProcessSpamFolder(IWebDriver driver)
    {
        // Navigate to the spam/junk email folder
        driver.Navigate().GoToUrl("https://outlook.live.com/mail/junkemail");

        await Task.Delay(5000); // Wait for the page to load

        try
        {
            while (true) // Adjust this condition based on how you want to break out of processing spam
            {
                // Here, you would add logic to select and move emails as needed
                // This is an example based on the initial structure you provided
                // The specific elements and actions might need to be updated based on the actual page structure

                // Example: Find and click the first spam email
                // This is highly dependent on the page structure and might need adjustment
                IWebElement firstSpamEmail = driver.FindElement(By.CssSelector("div[class*='someClassNameForEmails']"));
                firstSpamEmail.Click();

                await Task.Delay(5000); // Wait for selection to complete

                // Find and click the "Not junk" or similar button to move the email to the inbox
                IWebElement notJunkButton = driver.FindElement(By.CssSelector("button[id*='notJunkButtonId']"));
                notJunkButton.Click();

                await Task.Delay(5000); // Wait for the email to move

                // Implement logic to break the loop when all desired emails are processed
                // For example, you might check if there are no more emails to process
            }
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine("Could not find the specified element: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while processing the spam folder: " + ex.Message);
        }
    }


    private static void EnsureFileExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Dispose();
            Console.WriteLine($"Created file: {filePath}");
        }
    }
}
