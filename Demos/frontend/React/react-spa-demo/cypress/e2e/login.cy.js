describe("login", () => {
    beforeEach(() => {
        cy.visit("/login");
    })

    it("signs in the seeded admin and updates the header", () => {
        cy.contains("label", "Username").find("input").type("jon")
        cy.contains("label", "Password").find("input").type("qwerty123")

        cy.contains("button", "Sign in").click();
    });

    it("admin seeded credentials", () => {
        cy.contains("label", "Username").find("input").type("jon");
        cy.contains("label", "Password").find("input").type("qwerty123");

        cy.contains("button", "Sign in").click();

    });

    it("shows the error message for bad credentials", () => {
        cy.contains("label", "Username").find("input").type("ada")
        cy.contains("label", "Password").find("input").type("not jon")

        cy.contains("button", "Sign in").click();

        cy.get("p.error").should("have.text", "Invalid username or password");

        cy.url().should("include", "/login");
        cy.contains("button", "Sign in");
    });
});