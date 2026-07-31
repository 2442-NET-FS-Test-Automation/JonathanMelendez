Cypress.Commands.add("resetInventory", () => {
    cy.request("POST", "http://localhost:5196/inventory/reset")
});

Cypress.Commands.add("login", (username, password) => {
    cy.request("POST", "http://localhost:5224/auth/login", { username, password })
        .then(({ body }) => {
            window.localStorage.setItem("library.token", body.token)
        });
});