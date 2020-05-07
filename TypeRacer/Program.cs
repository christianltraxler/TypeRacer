using System;
using System.Linq;
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

            // Find and click button to enter race
            Helper.WaitForElementExists(driver, By.LinkText("Enter a typing race"));
            IWebElement enter = driver.FindElement(By.LinkText("Enter a typing race"));
            enter.Click();

            // Play the game
            PlayGame(driver);
        }

        public static void PlayGame(IWebDriver driver)
        {
            string text = "";
            // Find the text elements
            IWebElement first = Helper.WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(1)"));
            IWebElement second  = Helper.WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(2)"));
            // Find the third element if possible
            IWebElement rest = driver.FindElements(By.CssSelector("[unselectable='on']:nth-of-type(3)")).FirstOrDefault();

            if (rest != null)
            {
                // If the third text element exists
                text = Helper.GetText(first.Text + second.Text, rest.Text);
            }
            else
            {
                // If the third text element does not exist
                rest = second;
                text = Helper.GetText(first.Text, rest.Text);
            }
            
            // Print the text to be entered
            Console.WriteLine(text);

            // Send the keys to the text input
            Helper.WaitAndSendKeys(driver, By.ClassName("txtInput"), text);

        }
    }
}

