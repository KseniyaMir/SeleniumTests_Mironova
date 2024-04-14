using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Seleniumtests_Mironova;

public class SeleniumTestsForPractice
{
    [Test]
    public void Authorization()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        
        // зайти в хром
        var driver = new ChromeDriver(options);
        
        // перейти на урлу
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        Thread.Sleep(5000);
        
        // ввести логин и пароль
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("89612167666@mail.ru");

        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("Livik1987!");
        
        Thread.Sleep(5000);
        
        // нажать войти
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();
        
        Thread.Sleep(3000);
        
        // проверяем. что мы на нужной странице
        var currentUrl = driver.Url;
        Assert.That(currentUrl == "https://staff-testing.testkontur.ru/news");
        
        
        // хотим закрыть браузер и убиваем процесс драйвера
        driver.Quit();
    }
}