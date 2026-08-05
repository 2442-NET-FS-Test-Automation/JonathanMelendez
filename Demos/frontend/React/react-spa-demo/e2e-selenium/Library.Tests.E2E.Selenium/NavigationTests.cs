using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public class NavigationTests : IDisposable
{
    private readonly ChromeDriver _driver;

    public NavigationTests()
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
    public void DirectUrl_LoadsADeepRoute()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/inventory/BK-001");
        _driver.FindElement(By.TagName("h2")).Text.Should().Be("Clean Code");
    }

    [Fact]
    public void BackForwardRefresh_WalkTheHistory()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/");
        _driver.Navigate().GoToUrl("http://localhost:5173/about");

        _driver.FindElement(By.TagName("h2")).Text.Should().Be("About");

        _driver.Navigate().Back();
        _driver.FindElement(By.TagName("h2")).Text.Should().Be("Catalog");

        _driver.Navigate().Refresh();
        _driver.FindElement(By.TagName("h2")).Text.Should().Be("About");
        _driver.Url.Should().EndWith("/about");
    }
}