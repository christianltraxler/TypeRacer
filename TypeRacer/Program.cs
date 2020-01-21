using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace TypeRacer
{
    public static class Program
    {
        public static readonly TimeSpan DefaultExistsTimeout = TimeSpan.FromSeconds(int.Parse("90"));
        public static readonly TimeSpan DefaultInteractiveTimeout = TimeSpan.FromSeconds(int.Parse("60"));

        static void Main()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://play.typeracer.com");

            try
            {
                WaitForElementExists(driver, By.LinkText("Enter a typing race"));
                IWebElement enter = driver.FindElement(By.LinkText("Enter a typing race"));
                enter.Click();
                PlayGame(driver);
            }
            catch (WebDriverException ex)
            {
                Console.WriteLine("error1");
                driver.Close();
            }
        }

        public static void PlayGame(IWebDriver driver)
        {
            string text = "";

            try
            {
                WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(1)"));
                WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(2)"));
                WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(3)"));
                IWebElement firstLetter = driver.FindElement(By.CssSelector("[unselectable='on']:nth-of-type(1)"));
                IWebElement firstWord = driver.FindElement(By.CssSelector("[unselectable='on']:nth-of-type(2)"));
                IWebElement rest = driver.FindElement(By.CssSelector("[unselectable='on']:nth-of-type(3)"));
                text = firstLetter.Text + firstWord.Text + " " + rest.Text;

            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine("error2");
                WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(1)"));
                WaitForElementExists(driver, By.CssSelector("[unselectable='on']:nth-of-type(2)"));
                IWebElement firstLetter = driver.FindElement(By.CssSelector("[unselectable='on']:nth-of-type(1)"));
                IWebElement rest = driver.FindElement(By.CssSelector("[unselectable='on']:nth-of-type(2)"));
                text = firstLetter.Text + " " + rest.Text;
            }
            finally
            {
                Console.WriteLine(text);
            }
            WaitAndSendKeys(driver, By.ClassName("txtInput"), text);

        }

        public static IWebElement WaitForElementExists(this IWebDriver driver, By locator)
        {
            WebDriverWait waitForElement = new WebDriverWait(driver, DefaultExistsTimeout);
            waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(locator));
            return driver.FindElement(locator);
        }

        public static IWebElement WaitForElementInteractive(this IWebDriver driver, By locator)
        {
            IWebElement element = WaitForElementExists(driver, locator);

            var start = DateTime.Now;
            while (DateTime.Now < start + DefaultInteractiveTimeout)
            {
                Thread.Sleep(50);
                element = WaitForElementExists(driver, locator);
                try
                {
                    element.SendKeys("");
                    return element;
                }
                catch (ElementNotInteractableException ex)
                {
                    Console.WriteLine("error3");
                }
            }
            return (element);
        }

        public static IWebElement WaitAndSendKeys(this IWebDriver driver, By locator, string text)
        {
            IWebElement element = WaitForElementInteractive(driver, locator);

            var start = DateTime.Now;
            int waitTime = CalculateWaitTime(text.Length, 300);
            while (DateTime.Now < start + DefaultInteractiveTimeout)
            {
                Thread.Sleep(10);
                element = WaitForElementInteractive(driver, locator);
                try
                {
                    for (int i = 0; i < text.Length; i++)
                    {
                        element.SendKeys(text[i].ToString());
                        Thread.Sleep(waitTime);
                    }
                    try
                    {
                        element.SendKeys(Keys.Enter);
                    }
                    catch (ElementNotInteractableException)
                    {
                        Console.WriteLine("Enter key not required");
                    }

                    return element;
                }
                catch (ElementNotInteractableException ex)
                {
                    Console.WriteLine("Game did not start");
                }
            }
            return element;
        }

        public static int CalculateWaitTime(int length, int wpm)
        {
            double time = ((wpm * 5) * length) * 60 * 1000;

            return Convert.ToInt32(Convert.ToDouble(length) / time);
        }
    }
}

