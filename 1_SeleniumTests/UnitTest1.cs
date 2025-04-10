using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace _1_SeleniumTests;

public class Tests
{
    public IWebDriver driver;
    public ChromeOptions options;
    public WebDriverWait wait;
    public string createdCommunityUrl;
    
    [SetUp]
    public void Setup()
    {
        options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");
        driver = new ChromeDriver(options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(7));
        createdCommunityUrl = "";
        Authorization();
    }

    private void Authorization()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru");
      
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("");
        var password = driver.FindElement(By.Name("Password"));
        password.SendKeys("");
        
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();

        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[data-tid='Title']"))); 
    }

    private void CreatedCommunity()
    {       
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/communities");
  
        var create = driver.FindElement(By.CssSelector("[data-tid='PageHeader']")).FindElement(By.XPath(".//button[contains(text(), 'СОЗДАТЬ')]"));
        create.Click();
      
        var nameCommunity = driver.FindElement(By.CssSelector("[placeholder='Название сообщества']"));

        nameCommunity.SendKeys("Название");

        var createCommunity = driver.FindElement(By.CssSelector("[data-tid='CreateButton']"));
        createCommunity.Click(); 

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='SettingsTabWrapper']")));

        createdCommunityUrl = driver.Url.Substring(0, driver.Url.Length - 9);
    }

    private void DeleteCommunity()
    {
        driver.Navigate().GoToUrl($"{createdCommunityUrl}/settings");
        
        var deleteButton = driver.FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        deleteButton.Click();

        var delete = driver.FindElement(By.CssSelector("[data-tid='ModalPageFooter']"))
            .FindElement(By.CssSelector("[data-tid='DeleteButton']"));
        delete.Click();
    }

    [Test]
    public void Authorization_test()
    {
        Assert.That(driver.Title, Does.Contain("Новости"),
        "После авторизации ожидается заголовок содержащий 'Новости'"); 
        Console.WriteLine("Authorization_test - Авторизация прошла успешно");
    }

    [Test]
    public void CommSettings_LinkHasCorrectName_Test()
    {
        CreatedCommunity();
         
        var communityLink = driver.FindElement(By.CssSelector("a.sc-eCApnc.fDWJqR[href^='/communities/']"));
        Assert.That(communityLink.Text, Is.EqualTo("Название"), 
        "Ожидается, что ссылка на сообщество в настройках соответствует названию при создании");
        Console.WriteLine("CommSettings_LinkHasCorrectName_Test - Ссылка на сообщество имеет правильное название");
    }

    [Test]
    public void CommSettings_LinkLeadsToCorrectUrl_Test()
    {
        CreatedCommunity();

        var communityLink = driver.FindElement(By.CssSelector("a.sc-eCApnc.fDWJqR[href^='/communities/']"));
        communityLink.Click();

        Assert.That(driver.Url, Is.EqualTo(createdCommunityUrl),
        "Ожидается, что ссылка на сообщество в настройках ведет на страницу этого сообщества");
       Console.WriteLine("CommSettings_LinkLeadsToCorrectUrl_Test - Ссылка на сообщество ведет на правильный URL");
    }


    [Test]
    public void DeleteCommunity_Test()
    {
        CreatedCommunity();
        DeleteCommunity();

        driver.Navigate().GoToUrl(createdCommunityUrl);
        var message = driver.FindElement(By.CssSelector("[data-tid='ValidationMessage']"));
        Assert.That(message.Text, Does.Contain("Объект не найден"),
        "Ожидается сообщение 'Объект не найден'");
        Console.WriteLine("DeleteCommunity_Test - Собщество не найдено");
    }

    [Test]
    public void Logout_Test()
    {
        var profile = driver.FindElement(By.CssSelector("[data-tid='Avatar']"));
        profile.Click();
        
        var logout = driver.FindElement(By.CssSelector("[data-tid='Logout']"));
        logout.Click();
        
        var logoutConfirmationElement = driver.FindElement(By.ClassName("PostLogoutRedirectUri"));
        Assert.That(logoutConfirmationElement.Displayed, Is.True, "После выхода из системы ожидается наличие класса 'PostLogoutRedirectUri'"); 
        Console.WriteLine("Logout_Test - Выход из системы выполнен успешно");
    }

    [TearDown]
    public void TearDown()
    {
            driver.Close();
            driver.Quit();
}
}
