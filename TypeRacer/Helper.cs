using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace TypeRacer
{
    public static class Helper
    {
        public static readonly TimeSpan DefaultExistsTimeout = TimeSpan.FromSeconds(int.Parse("90"));
        public static readonly TimeSpan DefaultInteractiveTimeout = TimeSpan.FromSeconds(int.Parse("60"));

        public static IWebElement WaitForElementExists(this IWebDriver driver, By locator)
        {
            // Create WebDriverWait instance 
            WebDriverWait waitForElement = new WebDriverWait(driver, DefaultExistsTimeout);

            // Wait for element to exist
            waitForElement.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(locator));

            // Return the element
            return driver.FindElement(locator);
        }

        public static IWebElement WaitForElementInteractive(this IWebDriver driver, By locator)
        {
            // Wait for the element to exist
            IWebElement element = WaitForElementExists(driver, locator);

            // Initialize the start time
            var start = DateTime.Now;

            // Loop until Timeout time has been reached
            while (DateTime.Now < start + DefaultInteractiveTimeout)
            {
                Thread.Sleep(50);
                element = WaitForElementExists(driver, locator);
                try
                {
                    // Try to send keys
                    element.SendKeys("");
                    return element;
                }
                catch (ElementNotInteractableException ex)
                {
                    Console.WriteLine(locator.ToString() + "Not Found");
                }
            }
            // Return the element
            return (element);
        }

        public static IWebElement WaitAndSendKeys(this IWebDriver driver, By locator, string text)
        {
            // Wait for the element to be interactive
            IWebElement element = WaitForElementInteractive(driver, locator);

            // Initialize the start time
            var start = DateTime.Now;

            // Calculate the wait time between each key to be send
            int waitTime = CalculateWaitTime(text.Length, 300);

            // Loop until Timeout time has been reached
            while (DateTime.Now < start + DefaultInteractiveTimeout)
            {
                Thread.Sleep(10);
                element = WaitForElementInteractive(driver, locator);
                try
                {
                    for (int i = 0; i < text.Length; i++)
                    {
                        // Enter the text, one character at a time
                        element.SendKeys(text[i].ToString());

                        // Wait for the waitTime to send the next key
                        Thread.Sleep(waitTime);
                    }
                    try
                    {
                        // Try to send the Enter key if necessary
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
            // Calculate the time, the typing should take
            double time = ((wpm * 5) * length) * 60 * 1000;

            // Return the wait time
            return Convert.ToInt32(Convert.ToDouble(length) / time);
        }

        public static string GetText(string first, string rest)
        {
            if (rest[0].ToString() != "," && rest[0].ToString() != " ")
            {
                // If the first character is not ",", insert space 
                return(first + " " + rest);
            }
            else
            {
                // If the first character is ",", do not insert a space
                return (first + rest);
            }
        }
    }
}
