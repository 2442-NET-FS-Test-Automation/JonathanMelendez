using FluentAssertions;
using OpenQA.Selenium;

namespace Library.Tests.E2E.Selenium;

public class AlertTests : E2ETestBase
{
    public AlertTests()
    {
        Driver.Navigate().GoToUrl(WidgetUrl);
    }

    [Fact]
    public void PlainAlert_IsReadAndAccepted()
    {
        Driver.FindElement(By.Id("alert-btn")).Click();

        var alert = Driver.SwitchTo().Alert();
        alert.Text.Should().Be("Book saved.");

        alert.Accept();
    }
    [Fact]
    public void Confirm_AcceptAndDismiss()
    {
        Driver.FindElement(By.Id("confirm-btn")).Click();
        Driver.SwitchTo().Alert().Dismiss();
        Driver.FindElement(By.Id("confirm-result")).Text.Should().Be("kept");

        Driver.FindElement(By.Id("confirm-btn")).Click();
        Driver.SwitchTo().Alert().Accept();
        Driver.FindElement(By.Id("confirm-result")).Text.Should().Be("deleted");
    }

    [Fact]
    public void Prompt_TakesTypedInput()
    {
        Driver.FindElement(By.Id("prompt-btn")).Click();
        var prompt = Driver.SwitchTo().Alert();   
        prompt.SendKeys("sometext");
        prompt.Accept();

        Driver.FindElement(By.Id("prompt-result")).Text.Should().Be("sometext");
    }
}