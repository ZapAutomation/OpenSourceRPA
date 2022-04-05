using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Zappy.ZappyActions.Code
{
    class RunSeleniumCommand
    {
        public static void test()
        {
            IWebDriver driver = new ChromeDriver();

                        driver.Navigate().GoToUrl("http:www.google.com");

                        IWebElement element = driver.FindElement(By.Name("q"));

                        element.SendKeys("executeautomation");

                        driver.Close();

        }
    }
}
