describe("catalog over a stubbed network", () => {
    it("renders exactly the objects in our inventory fixture", () => {
        cy.intercept("GET", "**/api/Inventory", {fixture: "inventory.json"}).as("getInventory");

        cy.visit("/");
        cy.wait("@getInventory");

        cy.get("article.card").should("have.length", 3);
        
        cy.contains("article.card", "Stubbed Book Two")
            .find("dd")
            .should("have.text", "STUB-002");
    });

    it("shows the failure if the API is dead", () => {
        cy.intercept("GET", "**/api/Inventory", { statusCode: 500, body: {}}).as("getInventory500");

        cy.visit("/");
        cy.wait("@getInventory500");

        cy.contains("Could not reach the API");
    });
})