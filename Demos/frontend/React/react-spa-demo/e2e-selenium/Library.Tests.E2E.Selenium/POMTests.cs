using FluentAssertions;
using Library.Tests.E2E.Selenium.PageObjects;

namespace Library.Tests.E2E.Selenium;

public class POMTests : E2ETestBase
{
    [Fact]
    public void Filter_ThroughThePOM()
    {
        var catalog = new CatalogPage(Driver).Visit().Search("clean");
        catalog.CardCount.Should().Be(1);
        catalog.FirstTitle.Should().Be("Clean Code");
    }

    [Fact]
    public void ToggleSort_POM()
    {
        var catalog = new CatalogPage(Driver).Visit().ToggleSort();
        catalog.FirstTitle.Should().Be("The Pragmatic Programmer");
    }

    [Fact]
    public void SignsIn_ThroughUIPOM()
    {
        Guarded(() =>
        {
            var catalog = new LoginPage(Driver).Visit().LoginAs("jon", "qwerty123");
            catalog.SignedInUser.Should().Be("jon (Admin)");
        });
    }
}