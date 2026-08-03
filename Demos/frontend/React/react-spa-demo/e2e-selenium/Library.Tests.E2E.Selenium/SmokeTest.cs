using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public class SmokeTest : IDisposable
{
    private readonly ChromeDriver _driver;

    public SmokeTest()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1280,720");

        _driver = new ChromeDriver(options);

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
    }

    public void Dispose()
    {
        _driver.Quit();
    }

    [Fact]
    public void OpeningTheSpa_ShowsTitleAndHeading()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173");

        _driver.Title.Should().Be("react-spa-demo");
        _driver.FindElement(By.TagName("h1")).Text.Should().Be("Library");
    }

    [Fact]
    public void Catalog_RendersCards_FromTheLiveApi()
    {
        // Given
        _driver.Navigate().GoToUrl("http://localhost:5173");
    
        // When
        var cards = _driver.FindElements(By.CssSelector("article.card"));
    
        // Then
        cards.Should().NotBeEmpty();
    }
}
