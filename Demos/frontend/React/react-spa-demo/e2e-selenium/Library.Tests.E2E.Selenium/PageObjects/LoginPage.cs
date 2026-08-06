using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Library.Tests.E2E.Selenium.PageObjects;

public class LoginPage(IWebDriver driver)
{
    private readonly IWebDriver _driver = driver;

    private static readonly By Username = By.CssSelector("form.login input:not([type='password'])");
    private static readonly By Password = By.CssSelector("form.login input[type='password']");
    private static readonly By SubmitBtn = By.CssSelector("form.login button[type='submit']");

    public LoginPage Visit()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173/login");
        return this;
    }

    public CatalogPage LoginAs(string username, string password)
    {
        _driver.FindElement(Username).SendKeys(username);
        _driver.FindElement(Password).SendKeys(password);
        _driver.FindElement(SubmitBtn).Click();

        new WebDriverWait(_driver, TimeSpan.FromSeconds(5))
            .Until(d => d.FindElements(By.CssSelector(".auth-box span")).Count > 0);

        return new CatalogPage(_driver);
    }
}