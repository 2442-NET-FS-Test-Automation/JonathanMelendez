using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Library.Tests.E2E.Selenium;

public class WindowTests : E2ETestBase
{
    public WindowTests()
    {
        Driver.Navigate().GoToUrl("http://localhost:5173");

        new WebDriverWait(Driver, TimeSpan.FromSeconds(4))
            .Until(d => d.FindElements(By.CssSelector("article.card")).Count > 0);

        var originalTab = Driver.CurrentWindowHandle;

        Driver.SwitchTo().NewWindow(WindowType.Tab);
        Driver.WindowHandles.Should().HaveCount(2);
        
        Driver.Navigate().GoToUrl("http://localhost:5173/about");
        Driver.FindElement(By.TagName("h2")).Text.Should().Be("About");

        Driver.Close();
        Driver.SwitchTo().Window(originalTab);
        Driver.FindElement(By.TagName("h2")).Text.Should().Be("Catalog");
        Driver.WindowHandles.Should().HaveCount(1);
    }

    [Fact]
    public void targetBlankLink_LoadsInANewHandle()
    {
        Driver.Navigate().GoToUrl(WidgetUrl);
        var originalTab = Driver.CurrentWindowHandle;
        
        Driver.FindElement(By.Id("open-about")).Click();
        new WebDriverWait(Driver, TimeSpan.FromSeconds(4))
            .Until(d => d.WindowHandles.Count == 2);

        var newTab = Driver.WindowHandles.First(w => w != originalTab);
        Driver.SwitchTo().Window(newTab);

        Driver.FindElement(By.TagName("h2")).Text.Should().Be("About");

        Driver.Url.Should().Contain("/about");

        Driver.Close();
        Driver.SwitchTo().Window(originalTab);
    }

    [Fact]
    public void WindowManagement_ReadsAndSetsSize()
    {
        Driver.Navigate().GoToUrl(WidgetUrl);
        Driver.Manage().Window.Size.Width.Should().Be(1280);

        Driver.Manage().Window.Size = new System.Drawing.Size(1024, 768);
        Driver.Manage().Window.Size.Width.Should().Be(1024);
    }
}