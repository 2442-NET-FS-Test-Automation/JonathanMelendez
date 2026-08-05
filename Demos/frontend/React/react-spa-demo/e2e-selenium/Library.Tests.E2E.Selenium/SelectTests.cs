using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Library.Tests.E2E.Selenium;

public class SelectTests : IDisposable
{
    private readonly ChromeDriver _driver;
    private static string WidgetUrl => 
        new Uri(Path.Combine(AppContext.BaseDirectory, "TestPages", "widgets.html")).AbsoluteUri;

    public SelectTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1280,720");

        _driver = new ChromeDriver(options);

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);

        _driver.Navigate().GoToUrl(WidgetUrl);
    }

    public void Dispose()
    {
        _driver.Quit();
    }

    [Fact]
    public void SingleSelect_ByTextValueAndIndex()
    {
        var format = new SelectElement(_driver.FindElement(By.Id("format")));
        format.SelectByText("Paperback");
        format.SelectedOption.GetAttribute("value").Should().Be("soft");

        format.SelectByValue("ebook");
        format.SelectedOption.Text.Should().Be("E-book");

        format.SelectByIndex(0);
        format.SelectedOption.Text.Should().Be("Hardcover");
    }

    [Fact]
    public void MultipleSelect_AccumulatesAndDeselects()
    {
        var genres = new SelectElement(_driver.FindElement(By.Id("genres")));

        genres.IsMultiple.Should().BeTrue();

        genres.SelectByText("Databases");
        genres.SelectByText("Web");

        genres.AllSelectedOptions.Should().HaveCount(2);
        genres.Options.First(o => o.Text == "Web").Selected.Should().BeTrue();

        genres.DeselectAll();
        genres.AllSelectedOptions.Should().BeNullOrEmpty();
    }
}