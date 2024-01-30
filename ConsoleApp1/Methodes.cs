using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ConsoleApp1
{
    internal class Methodes
    {

        public void goHotmail(int numberOfUsers) {
           
        }


        public void loginEmail(IWebDriver driver, int numberOfUsers)
        {
       
            try
            {

                IWebElement loginInput = driver.FindElement(By.CssSelector("[name=\"loginfmt\"]"));
                loginInput.SendKeys(ReadEmail(numberOfUsers));
                loginInput.SendKeys(Keys.Enter);
                Thread.Sleep(2000);

                IWebElement passInput = driver.FindElement(By.CssSelector("[name=\"passwd\"]"));
                passInput.SendKeys(ReadPassword());
                passInput.SendKeys(Keys.Enter);
                    Thread.Sleep(2000);
            }
            catch (Exception) { Console.WriteLine("--------------error in login into email"); }
            try
            {
                IWebElement loginForm = driver.FindElement(By.Id("idSIButton9"));
                loginForm.Submit();
                string urlspam = "https://outlook.live.com/mail/0/junkemail";
                driver.Navigate().GoToUrl(urlspam);
                Thread.Sleep(3000);
            }
            catch (Exception)
            {
                Console.WriteLine("--------------error befor access to inbox");
            }
        }

         string ReadEmail(int numberOfUsers)
        {
            string text1 = null;
            string filePath = "emails.txt";

            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                // Check if there's at least one line in the file
                if (lines.Length > 0)
                {
                    // Split the line using the colon (":") separator
                    string[] parts = lines[0].Split(':');

                    // Check if there are two parts
                    if (parts.Length == 2)
                    {
                        for (int i = 0;i< numberOfUsers;i++)
                        // Extract the first part (text1)
                        text1 = parts[i];
                    }
                    else
                    {
                        Console.WriteLine("Invalid format in the file. Each line should be in the format 'text1:text2'.");
                    }
                }
                else
                {
                    Console.WriteLine("The file is empty.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return text1;
        }

         string ReadPassword()
        {
            string text2 = null;
            string filePath = "emails.txt";

            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                // Check if there's at least one line in the file
                if (lines.Length > 0)
                {
                    // Split the line using the colon (":") separator
                    string[] parts = lines[0].Split(':');

                    // Check if there are two parts
                    if (parts.Length == 2)
                    {
                        // Extract the first part (text1)
                        text2 = parts[1];
                    }
                    else
                    {
                        Console.WriteLine("Invalid format in the file. Each line should be in the format 'text1:text2'.");
                    }
                }
                else
                {
                    Console.WriteLine("The file is empty.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return text2;
        }










    }
}