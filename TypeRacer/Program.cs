using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace TypeRacer
{
    public static class Program
    {
        static void Main()
        {
            // Create instnace of ChromeDriver
            IWebDriver driver = new ChromeDriver();

            // Navigate to the TypeRacer Url
            driver.Navigate().GoToUrl("https://play.typeracer.com");

            try
            {
                // Find and click button to enter race
                Helper.WaitForElementExists(driver, By.LinkText("Enter a typing race"));
                IWebElement enter = driver.FindElement(By.LinkText("Enter a typing race"));
                enter.Click();

                // Play the game
                PlayGame(driver);
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine("WebDriver Failed");
                driver.Close();
            }
        }

        public static void PlayGame(IWebDriver driver)
        {
            string text = "";

            try
            {
                // Find the text elements
                Helper.WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(1)"));
                Helper.WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(2)"));
                Helper.WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(3)"));
                IWebElement firstLetter = driver.FindElement(By.CssSelector("[unselectable='on']:nth-of-type(1)"));
                IWebElement firstWord = driver.FindElement(By.CssSelector("[unselectable='on']:nth-of-type(2)"));
                IWebElement rest = driver.FindElement(By.CssSelector("[unselectable='on']:nth-of-type(3)"));

                // Get the text to be sent
                text = Helper.GetText(firstLetter.Text + firstWord.Text, rest.Text);
            }
            catch (NoSuchElementException ex)
            {
                // Find the text elements, if there is no separation between first letter and word
                Helper.WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(1)"));
                Helper.WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(2)"));
                IWebElement firstLetter = driver.FindElement(By.CssSelector("[unselectable='on']:nth-of-type(1)"));
                IWebElement rest = driver.FindElement(By.CssSelector("[unselectable='on']:nth-of-type(2)"));

                // Get the text to be sent
                text = Helper.GetText(firstLetter.Text, rest.Text);
                
            }
            finally
            {
                // Write the text to the Console
                Console.WriteLine(text);
            }
            // Send the keys to the text input
            Helper.WaitAndSendKeys(driver, By.ClassName("txtInput"), text);

        }
    }
}

