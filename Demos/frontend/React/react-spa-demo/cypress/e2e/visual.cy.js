// Visual regression

describe("catalog visual regression", {browser: "electron"}, () => {
    it("catalog page matches the baseline", () => {
        cy.intercept("GET", "**/api/Inventory", { fixture: "../fixtures/inventory.json" }).as("getInventory");

        cy.visit("/");
        cy.wait("@getInventory");
        cy.get("article.card").should("have.length", 3);

        cy.task("Log", "visual: comparing catalog-stubbed against baseline");

        cy.compareSnapshot("catalog-stubbed");
    })
})