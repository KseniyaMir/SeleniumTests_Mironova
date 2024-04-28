using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;


namespace SeleniumTests_Mironova;

public class SeleniumTestsForPractice
{
    public ChromeDriver driver;
    public WebDriverWait wait;

 public void Authorization()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");

        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("89612167666@mail.ru");
        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("Livik1987!");

        var enter = driver.FindElement(By.Name("button"));
        enter.Click();


        // ждем перехода в сервис после авторизации
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']")));
    }

    public string CreateCommunity()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");

        var createButton = driver.FindElement(By.ClassName("sc-juXuNZ"));
        createButton.Click();

        var communityName = driver.FindElement(By.CssSelector("[data-tid='Name']"));
        communityName.SendKeys("Test");
        var aboutCommunity = driver.FindElement(By.CssSelector("[data-tid='Message']"));
        aboutCommunity.SendKeys("12345");
        var createComButton = driver.FindElement(By.CssSelector("[data-tid='CreateButton']"));
        createComButton.Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='SettingsTabWrapper']")));

        var getUrl = driver.Url;
        var splittedUrl = getUrl.Split("/");
        var communityId = splittedUrl[splittedUrl.Length - 2];

        return communityId;
    }

    public void DeleteCommunity(string comId)
    {
        driver.Navigate().GoToUrl($"https://staff-testing.testkontur.ru/communities/{comId}/settings");


        var deleteButton = driver.FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        deleteButton.Click();

        var confirmDeleteButton =
            driver.FindElement(By.CssSelector("[data-tid='ModalPageFooter'] [data-tid='DeleteButton']"));
        confirmDeleteButton.Click();

        // ждем перехода после удаления
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']")));
    }


    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");

        driver = new ChromeDriver(options);

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5)); //явное ожидание
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5); // неявное ожидание

        Authorization();
    }


    [Test, Description("Проверяем, что после авторизации попадем на страницу новостей")]
    public void AuthorizationTest()
    {
        var currentUrl = driver.Url;
        currentUrl.Should().Be("https://staff-testing.testkontur.ru/news");
    }

    [Test, Description("Проверяем, что работает переход в раздел Сообщества")]
    public void NavigationTest()
    {
        var community = driver.FindElement(By.CssSelector("[data-tid='Community']"));
        community.Click();

        var communityTitle = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        Assert.That(communityTitle.Text == "Сообщества", "Неверный заголовок");
        Assert.That(driver.Url == "https://staff-testing.testkontur.ru/communities",
            "На странице 'Сообщества' урл неверный");
    }


    [Test, Description("Проверяем успешное создание сообщества")]
    public void CreateCommunityTest()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");

        var createButton = driver.FindElement(By.ClassName("sc-juXuNZ"));
        createButton.Click();
        
        const string name = "Тест Сообщество 21.04";
        var communityName = driver.FindElement(By.CssSelector("[data-tid='Name']"));
        communityName.SendKeys(name);
        var aboutCommunity = driver.FindElement(By.CssSelector("[data-tid='Message']"));
        aboutCommunity.SendKeys("12345");
        var createComButton = driver.FindElement(By.CssSelector("[data-tid='CreateButton']"));
        createComButton.Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='SettingsTabWrapper']")));
        var communityTitle = driver.FindElement(By.CssSelector("[data-tid='PageHeader'] [data-tid='Title']"));
        Assert.That(communityTitle.Text == $"Управление сообществом «{name}»",
            $"название сообщества отображается как {communityTitle.Text} вместо {name}");

        var getUrl = driver.Url;
        var splitedUrl = getUrl.Split("/");
        var communityId = splitedUrl[splitedUrl.Length - 2];

        DeleteCommunity(communityId); // удаляем созданное тестом сообщество
    }

    [Test, Description("В только что созданном сообществе нет новостей")]
    public void EmptyNewsTest()
    {
        var id = CreateCommunity();

        driver.Navigate().GoToUrl($"https://staff-testing.testkontur.ru/communities/{id}");
        var message = driver.FindElement(By.TagName("h2"));
        Assert.That(message.Text == "Пока новостей нет", "Не отображается нужный текст");
        
        DeleteCommunity(id); // удаляем созданное тестом сообщество
    }

    [Test, Description("Проверяем успешное удаление сообщества")]
    public void DeleteCommunityTest()
    {
        CreateCommunity();

        var deleteButton = driver.FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        deleteButton.Click();
        var confirmDeleteButton =
            driver.FindElement(By.CssSelector("[data-tid='ModalPageFooter'] [data-tid='DeleteButton']"));
        confirmDeleteButton.Click();

        // ждем перехода после удаления
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']")));
    }

    [Test, Description("Проверяем переключение табов в списке сообщества")]
    public void CommunityListByMembersTest()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");

        var iAmMemberTab = driver.FindElement(By.LinkText("Я участник"));
        iAmMemberTab.Click();

        const string iAmMemberUrl = "https://staff-testing.testkontur.ru/communities?activeTab=isMember";
        var currentUrl = driver.Url;
        Assert.That(currentUrl == iAmMemberUrl, $"current url = {currentUrl}, а должен быть {iAmMemberUrl}");
    }

    [TearDown]
    public void TearDown()
    {
        driver.Quit(); 
        // хотим закрыть браузер и убиваем процесс драйвера
    }
}