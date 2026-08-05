using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public class ScreenShotTests : IDisposable
{
    private readonly ChromeDriver _driver;

    public ScreenShotTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1280,720");

        _driver = new ChromeDriver(options);

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

        _driver.Navigate().GoToUrl("http://localhost:5173/");

        _driver.FindElements(By.CssSelector("article.card")).Should().NotBeEmpty();
    }

    public void Dispose()
    {
        _driver.Quit();
    }

    [Fact]
    public void FullPage_Shot()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "shots", "full-page.png");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        _driver.GetScreenshot().SaveAsFile(path);

        File.Exists(path).Should().BeTrue();
    }

    [Fact]
    public void SingleElement_Shot()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "shots", "first-card.png");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        
        var card = _driver.FindElement(By.CssSelector("article.card"));

        ((ITakesScreenshot) card).GetScreenshot().SaveAsFile(path);

        File.Exists(path).Should().BeTrue();
    }
}