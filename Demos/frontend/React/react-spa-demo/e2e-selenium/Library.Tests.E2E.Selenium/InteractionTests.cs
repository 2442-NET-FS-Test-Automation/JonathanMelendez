using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public class InteractionTests : IDisposable
{
    private readonly ChromeDriver _driver;

    public InteractionTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1280,720");

        _driver = new ChromeDriver(options);

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

        _driver.Navigate().GoToUrl("http://localhost:5173/");
    }

    public void Dispose()
    {
        _driver.Quit();
    }

    [Fact]
    public void LoginForm_SignsIn_ThroughUI()
    {
        // Given
        _driver.Navigate().GoToUrl("http://localhost:5173/login");
        var username = _driver.FindElement(By.CssSelector("form.login input:not([type='password'])"));
        var password = _driver.FindElement(By.CssSelector("form.login input[type='password']"));
        var submit = _driver.FindElement(By.CssSelector("form.login button[type='submit']"));
    
        // When
        username.SendKeys("jon");
        password.SendKeys("qwerty123");
        submit.Click();
    
        // Then
        var who = _driver.FindElement(By.CssSelector(".auth-box span"));
        who.Text.Should().Be("jon (admin)");
    }

    [Fact]
    public void SendKeysAndClear_DriveAControlledInput()
    {
        // Given
        var search = _driver.FindElement(By.CssSelector("input[type='search']"));
    
        // When
        search.SendKeys("clean");
        search.GetAttribute("placeholder").Should().Be("Filter by name");

        // Then
        _driver.FindElements(By.CssSelector("article.card")).Should().HaveCount(1);

        search.Clear();
        search.GetAttribute("value").Should().Be("");
    }

    [Fact]
    public void DisplayedAndEnabled_ReadElementState()
    {
        // Given
        var heading = _driver.FindElement(By.TagName("h2"));
        heading.Displayed.Should().BeTrue();
        heading.Text.Should().Be("Catalog");

        _driver.FindElement(By.CssSelector(".toolbar button")).Enabled.Should().BeTrue();
    }
}