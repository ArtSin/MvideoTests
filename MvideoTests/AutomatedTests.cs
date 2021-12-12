using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace MvideoTests
{
    public class AutomatedTests
    {
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Navigate().GoToUrl("https://www.mvideo.ru/");
        }

        [Test]
        public void TestPriceFilter()
        {
            driver.FindElement(By.CssSelector(".popular-categories__item")).Click();

            driver.FindElement(By.XPath("//input[contains(@class, 'range__price') and @name='minPrice']")).Clear();
            driver.FindElement(By.XPath("//input[contains(@class, 'range__price') and @name='minPrice']")).SendKeys("15000");
            driver.FindElement(By.XPath("//input[contains(@class, 'range__price') and @name='minPrice']")).SendKeys(Keys.Enter);
            new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                .Until(x => !driver.FindElements(By.CssSelector(".animated-background")).Any());

            driver.FindElement(By.XPath("//input[contains(@class, 'range__price') and @name='maxPrice']")).Clear();
            driver.FindElement(By.XPath("//input[contains(@class, 'range__price') and @name='maxPrice']")).SendKeys("30000");
            driver.FindElement(By.XPath("//input[contains(@class, 'range__price') and @name='maxPrice']")).SendKeys(Keys.Enter);
            new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                .Until(x => !driver.FindElements(By.CssSelector(".animated-background")).Any());

            List<int> actualValues = driver.FindElements(By.CssSelector(".price__main-value"))
                .Select(elem => int.Parse(elem.Text.Replace(" ", "").Replace("₽", "")))
                .ToList();
            actualValues.ForEach(actualPrice => Assert.True(actualPrice >= 15000 && actualPrice <= 30000,
                $"Price filter works wrong. Actual price is {actualPrice}. But should be more or equal than 15000 and less or equal than 30000"));
        }

        [Test]
        public void TestTooltipText()
        {
            driver.FindElement(By.CssSelector(".popular-categories__item")).Click();
            new Actions(driver).MoveToElement(driver.FindElement(By.XPath("//div[contains(@class, 'product-checkout__button')]//button"))).Build().Perform();
            Assert.AreEqual("Добавить в корзину", driver.FindElement(By.XPath("//div[contains(@class, 'product-checkout__button')]//button"))
                .GetAttribute("title").Trim(), "Tooltip has not appeared.");
        }

        [Test]
        public void NegativeSignUpTest()
        {
            driver.FindElement(By.XPath("//a[contains(@class, 'link') and contains(@href, 'login')]")).Click();
            driver.FindElement(By.XPath("//label[@data-holder='#useEmailToEnter']")).Click();
            driver.FindElement(By.CssSelector("input#login_password")).SendKeys("TestTestTest123");
            driver.FindElement(By.CssSelector("button#submit-button")).Click();
            Assert.AreEqual("Укажите телефон или email",
                driver.FindElement(By.XPath("//label[@class='u-error-text' and @for='login-original']")).Text,
                "Login/registration is possible when phone number input has no value.");
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}
