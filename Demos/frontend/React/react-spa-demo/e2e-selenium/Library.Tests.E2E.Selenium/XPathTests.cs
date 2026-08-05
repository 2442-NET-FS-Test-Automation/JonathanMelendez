using OpenQA.Selenium.Chrome;

namespace Library.Tests.E2E.Selenium;

public class XPathTests : IDisposable
{
    private readonly ChromeDriver _driver;

    public XPathTests()
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
}