import { CatalogPage } from "../pages/catalogPage";

describe("catalog via POM", () => {
    const catalog = new CatalogPage();

    beforeEach(() => {
        catalog.visit();
    })

    it("filter through the page object", () => {
        catalog.search("Clean");
        catalog.getCards().should("have.length", 1);
        catalog.firstTitle().should("contain.text", "Clean Code");
    })

    it("sorts through the page object", () => {
        catalog.toggleSort();
        catalog.firstTitle().should("contain.text", "The Pragmatic Programmer");
    })
})