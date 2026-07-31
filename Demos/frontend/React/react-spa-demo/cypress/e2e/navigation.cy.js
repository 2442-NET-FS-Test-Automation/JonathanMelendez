describe("navigation", () => {
    it("goes from a card to the detail page and back", () => {
        cy.visit("/");

        cy.get("article.card").should("have.length.at.least", 1);

        cy.get("article.card h3 a").first().click();

        cy.url().should("include", "/inventory/BK-001");
        cy.contains("SKU: BK-001")
        cy.contains("In stock: ");

        cy.contains("Sign in to see supplier prices");

        cy.contains("a", "Back to catalog").click();
        cy.url().should("not.include", "/inventory");
        cy.contains("h2", "Catalog");
    });

    it("serves the static About page route", () => {
        cy.visit("/about");
        cy.contains("h2", "About");
        cy.contains("A react single-page app over our demo Library")
    });

    it("shows teh not found page for a bad route", () => {
        cy.visit("/no-page");
        cy.contains("Page non fount");
    });
});