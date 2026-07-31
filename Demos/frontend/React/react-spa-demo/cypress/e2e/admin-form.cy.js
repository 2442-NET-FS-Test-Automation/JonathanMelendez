describe("admin form", () => {
    beforeEach(() => {
        cy.resetInventory();
        cy.fixture("users.json").then((users) => {
            cy.login(users.admin.username, users.admin.password);
        });
        cy.visit("/admin");
        cy.contains("h2", "Admin - jon");
    })

    it("creates a book then deletes it via quick find copy", () => {
        cy.get('input[placeholder="SKU"]').type("BK-E2E");
        cy.get('input[placeholder="Name"]').type("Cypress E2E Book");
        cy.get('input[placeholder="Price"]').type("19.99");
        cy.get('input[placeholder="Stock"]').type("3");
        cy.contains("button", "Create").click();

        cy.contains("Created BK-E2E - Cypress E2E Book");

        cy.get('input[placeholder="Quick SKU (uncontrolled)]').type("BK-E2E");
        cy.contains("button", "Copy into form").click();
        
        cy.get('input[placeholder="SKU"]').should("have.value", "BK-E2E");

        cy.contains("button", "Delete by SKU").click();
        cy.contains("Deleted BK-E2E");
    });

    it("surfaces failure messages when creation fails", () => {
        cy.get('input[placeholder="SKU"]').type("BK-E2E");
        cy.get('input[placeholder="Name"]').type("Cypress E2E Book");
        cy.get('input[placeholder="Price"]').type("-19.99");
        cy.get('input[placeholder="Stock"]').type("-15");
        cy.contains("button", "Create").click();

        cy.contains("Create Failed. Check fields and permissions");
    });
})