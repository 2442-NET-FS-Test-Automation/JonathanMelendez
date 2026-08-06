using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Library.Tests.E2E.Selenium.PageObjects;

public class CatalogPage(IWebDriver driver)
{
    private readonly IWebDriver _driver = driver;
    private static readonly By Cards = By.CssSelector("article.card");
    private static readonly By SearchBox = By.CssSelector("input[type='search']");
    private static readonly By SortButton = By.CssSelector(".toolbar btn");
    private static readonly By FirstTitleLink = By.CssSelector("article.card h3 a");
    private static readonly By SignedInLabel = By.CssSelector(".auth-box span");

    public CatalogPage Visit()
    {
        _driver.Navigate().GoToUrl("http://localhost:5173");

        new WebDriverWait(_driver, TimeSpan.FromSeconds(4))
            .Until(d => d.FindElements(Cards).Count > 0);
        
        return this;
    }

    public CatalogPage Search(string text) 
    {
        _driver.FindElement(SearchBox).SendKeys(text);
        return this;
    }

    public CatalogPage ToggleSort()
    {
        _driver.FindElement(SortButton).Click();
        return this;
    }

    public int CardCount => _driver.FindElements(Cards).Count;
    public string FirstTitle => _driver.FindElement(FirstTitleLink).Text;
    public string SignedInUser => _driver.FindElement(SignedInLabel).Text;
}