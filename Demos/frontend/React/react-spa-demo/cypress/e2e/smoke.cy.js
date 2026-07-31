describe("catalog smoke test", () => {
    it("loads the catalog from the live API", () => {
        cy.visit("/");
        cy.get("h1").should("have.text", "Library");

        cy.get("article.card").should("have.length.at.least", 1);
    });
});