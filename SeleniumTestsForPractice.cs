using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;


namespace Seleniumtests_Mironova;

public class SeleniumTestsForPractice
{
    public ChromeDriver driver;

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        //Авторизация
        Autorization();
    }

    [Test]
    public void Authorization()
    {
        var news = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        // проверяем. что мы на нужной странице
        var currentUrl = driver.Url;
        currentUrl.Should().Be("https://staff-testing.testkontur.ru/news");
        
    }

    [Test]
    public void NavigationTest()
    {
        //клик на боковое меню
        //var sidemenu = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        //sidemenu.Click();
        //клик на сообщества
        var community = driver.FindElement(By.CssSelector("[data-tid='Community']"));
        community.Click();
        Thread.Sleep(5000);
        //проверяем, что сообщества есть на странице + урл правильный
        var communityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        Assert.That(driver.Url == "https://staff-testing.testkontur.ru/communities", "На странице 'Сообщества' урл не верный");
    }

    public void Autorization()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
        
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("89612167666@mail.ru");
        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("Livik1987!");

        var enter = driver.FindElement(By.Name("button"));
        enter.Click();  
    }

    [Test]
    public void qwerty()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");
        Thread.Sleep(5000);
    }
    
    [TearDown]
    
    public void TearDown()
    {
        // хотим закрыть браузер и убиваем процесс драйвера
    driver.Quit();
    }
}