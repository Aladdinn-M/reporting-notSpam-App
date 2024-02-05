using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;
using System.Text;
using System.Diagnostics;
using OpenQA.Selenium.Interactions;



internal class Program
{
    private static async Task Main(string[] args)
    {
        // Create a list to store tasks 
        List<Task> tasks = new List<Task>();

       
          
        

        // Get the current directory
        string currentDirectory = Directory.GetCurrentDirectory();


        // Combine the current directory with the proxy file name
        string proxiesFilePath = Path.Combine(currentDirectory, "proxies.txt");

        try
        {
            // Check if the file already exists
            if (File.Exists(proxiesFilePath))
            {
                Console.WriteLine($"File 'proxies.txt' already exists in the following path:");
                Console.WriteLine(proxiesFilePath);
            }
            else
            {
                // Create the file if it doesn't exist
                File.Create(proxiesFilePath);

                Console.WriteLine($"File 'proxies.txt' created successfully in the following path:");
                Console.WriteLine(proxiesFilePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }




       



        // Combine the current directory with the emails file name
        string emailFilePath = Path.Combine(currentDirectory, "emails.txt");

        try
        {
            // Check if the file already exists
            if (File.Exists(emailFilePath))
            {
                Console.WriteLine($"File 'emails.txt' already exists in the following path:");
                Console.WriteLine(emailFilePath);
            }
            else
            {
                // Create the file if it doesn't exist
                File.Create(emailFilePath);

                Console.WriteLine($"File 'emails.txt' created successfully in the following path:");
                Console.WriteLine(emailFilePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }


        Console.WriteLine("Enter Number of Users :");
        int numberOfUsers = int.Parse(Console.ReadLine());


           


        for (int i = 0; i < numberOfUsers; i++)
        {
            Proxy proxy = new Proxy();

            string proxyIp = extractfile(0, 0, proxiesFilePath);
            int proxyPort = int.Parse(extractfile(1, 0, proxiesFilePath));
            string ipAndPort = $"{proxyIp}:{proxyPort}";
            proxy.HttpProxy = ipAndPort;
            proxy.SslProxy = ipAndPort;

            ChromeOptions options = new ChromeOptions();

            options.Proxy = proxy;

            IWebDriver driver = new ChromeDriver(options);
      
            




            // Set implicit wait
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            

            

            try
            {
                string url;
                url = "https://username:password@login.live.com/login.srf?wa=wsignin1.0&rpsnv=19&ct=1705352608&rver=7.0.6738.0&wp=MBI_SSL&wreply=https%3a%2f%2foutlook.live.com%2fowa%2f%3fcobrandid%3dab0455a0-8d03-46b9-b18b-df2f57b9e44c%26nlp%3d1%26deeplink%3dowa%252f%26RpsCsrfState%3d451c2c6d-8d13-204d-9273-6fe70ef13cf9&id=292841&aadredir=1&CBCXT=out&lw=1&fl=dob%2cflname%2cwld&cobrandid=ab0455a0-8d03-46b9-b18b-df2f57b9e44c";
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(url);
                string autoITScriptPath = @"ProxyAuth.exe";
                Process.Start(autoITScriptPath);

            }
            catch (Exception) { Console.WriteLine("--------------error in openning browser!!"); }



            try
            {

                IWebElement loginInput = driver.FindElement(By.CssSelector("[name=\"loginfmt\"]"));
                loginInput.SendKeys(extractfile(0, i, emailFilePath, driver));          // zero means email parts[0] in the file;
                loginInput.SendKeys(Keys.Enter);
                Thread.Sleep(2000);

                IWebElement passInput = driver.FindElement(By.CssSelector("[name=\"passwd\"]"));
                passInput.SendKeys(extractfile(1, i, emailFilePath, driver));          //one means paasword parts[1] in the file ;
                passInput.SendKeys(Keys.Enter);
                Thread.Sleep(2000);
            }
            catch (Exception) { Console.WriteLine("--------------error in login into email"); }

            try
            {
                try
                {
                    IWebElement loginForm1 = driver.FindElement(By.Name("kmsiForm"));
                    loginForm1.Submit();
                }
                catch (NoSuchElementException)
                {
                    // Handle the case where the first element is not found
                    IWebElement loginForm2 = driver.FindElement(By.Id("idSIButton9"));
                    loginForm2.Submit();
                }

            }
            catch (Exception) { Console.WriteLine("--------------error in access to inbox "); //driver.Quit(); 
            }



            try
            {
                 string urlspam = "https://outlook.live.com/mail/0/junkemail";
                 driver.Navigate().GoToUrl(urlspam);

            }
            catch (Exception)
            {
                Console.WriteLine("--------------error in access to spam "); //driver.Quit();

            }

            int index = i;


            // Start a new task for each browser
            Task task = Task.Run(() => ExecuteInfiniteLoop(driver, index));
            tasks.Add(task);

        }


        // Wait for all tasks to complete or until cancellation
        await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(TimeSpan.FromMinutes(10))); // Adjust the timeout as needed



        // Wait for all tasks to complete
        await Task.WhenAll(tasks);
        Task.WaitAll(tasks.ToArray());








        static void ExecuteInfiniteLoop(IWebDriver driver, int index)
        {

            try
            {
                do
                {
                    string urlspam = "https://outlook.live.com/mail/0/junkemail";
                    driver.Navigate().GoToUrl(urlspam);

                    Thread.Sleep(5000);


                    IWebElement firstSpam = driver.FindElement(By.Id("MainModule"));
                    firstSpam = firstSpam.FindElement(By.ClassName("S2NDX"));
                    firstSpam.Click();
                    Thread.Sleep(5000);


                    IWebElement moveToButton = driver.FindElement(By.Id("540"));
                    moveToButton.Click();
                    Thread.Sleep(5000);

                    IWebElement inboxButton = driver.FindElement(By.Name("Inbox"));
                    inboxButton.Click();
                    Thread.Sleep(5000);
                    IWebElement confirmButton = driver.FindElement(By.Id("ok-1"));
                    confirmButton.Click();



                    Thread.Sleep(5000);

                    Console.WriteLine($"=======================Task {index}: Loop iteration");


                } while (true);
            }
            catch (Exception)
            {
                // Task was canceled, clean up if needed
                Console.WriteLine($"-------------Task {index}: Task was canceled."); driver.Quit();
            }

        }





        string extractfile(int zeroOrOne, int index , string filePath,IWebDriver driver=null )
        {



            string result = null;
            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                // Check if there's at least one line in the file
                if (lines.Length > 0)
                {
                    // Split the line using the colon (":") separator
                    string[] parts = lines[index].Split(':');

                    // Check if there are two parts
                    if (parts.Length >1 )
                    {

                        // Extract the first part (text1)
                        result = parts[zeroOrOne];

                    }
                    else
                    {
                        Console.WriteLine("--------------Invalid format in the file. Each line should be in the format 'text1:text2'."); driver.Quit();
                    }
                }
                else
                {
                    Console.WriteLine("--------------The file is empty."); driver.Quit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("--------------An error occurred: " + ex.Message); driver.Quit();
            }
            return result;
        }
    }
}
