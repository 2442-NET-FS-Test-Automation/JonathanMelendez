using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public class LocatorTests : IDisposable
{
    private readonly ChromeDriver _driver;

    public LocatorTests()
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
    public void ByTagName_FindsTheHeader()
    {
        _driver.FindElement(By.TagName("h1")).Text.Should().Be("Library");
    }

    [Fact]
    public void ByClassName_FindsEveryCard()
    {
        var cards = _driver.FindElements(By.ClassName("card"));
        cards.Should().NotBeEmpty();
    }

    [Fact]
    public void ByCssSelector_ComposesStructureAndClass()
    {
        var firstTitleLink = _driver.FindElement(By.CssSelector("article.card h3 a"));
        firstTitleLink.Text.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ByLinkText_FindsAnchorsByWhatUserReads()
    {
        _driver.FindElement(By.LinkText("About")).TagName.Should().Be("a");
        _driver.FindElement(By.PartialLinkText("Cata")).Text.Should().Be("Catalog");
    }
}