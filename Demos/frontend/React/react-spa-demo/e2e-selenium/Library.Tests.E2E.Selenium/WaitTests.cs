using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Library.Tests.E2E.Selenium;

public class WaitTests : IDisposable
{
    private readonly ChromeDriver _driver;

    public WaitTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1280,720");

        _driver = new ChromeDriver(options);

        _driver.Navigate().GoToUrl("http://localhost:5173/");
    }

    public void Dispose()
    {
        _driver.Quit();
    }

    [Fact]
    public void WithoutAnyWait_ApiRenderRace()
    {
        var cards = _driver.FindElements(By.CssSelector("article.card"));

        cards.Should().NotBeEmpty();
    }

    [Fact]
    public void ExplicitWait_TargetsOneCondition()
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
        var cards = wait.Until(d =>
        {
            var found = d.FindElements(By.CssSelector("article.card"));
            return found.Count > 0 ? found : null;
        });

        cards.Should().NotBeEmpty();
    }

    [Fact]
    public void ExplicitWait_PinsATransientUIState()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/login");

        _driver.FindElement(By.CssSelector("form.login input:not([type='password'])"))
            .SendKeys("ada");
        
        _driver.FindElement(By.CssSelector("form.login input[type='password']"))
            .SendKeys("wrong-password");
        
        _driver.FindElement(By.CssSelector("form.login button[type='submit']")).Click();

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(4));

        var errorP = wait.Until(d =>
        {
            var found = d.FindElements(By.CssSelector("p.error"));
            return found.Count > 0 ? found[0] : null;
        });

        errorP.Text.Should().Be("Invalid username or password");
    }

    [Fact]
    public void FluentWait_SetPollingIgnoreNoise()
    {
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(4))
        {
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };

        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

        var firstCard = wait.Until(d =>
        {
            return d.FindElement(By.CssSelector("article.card h3 a"));
        });

        firstCard.Text.Should().NotBeNullOrEmpty();
    }
}