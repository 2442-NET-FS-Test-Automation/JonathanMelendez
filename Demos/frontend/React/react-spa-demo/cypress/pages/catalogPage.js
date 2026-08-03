// Page Object Model

export class CatalogPage {
    visit() {
        cy.visit("/");
        cy.get("article.card").should("have.length.at.least", 1);
        return this;
    }

    search(text) {
        cy.get('input[type="search"][placeholder="Filter by name"]').type(text);
        return this;
    }

    toggleSort() {
        cy.contains("button", "Sort Z-A").click();
        return this;
    }

    getCards() {
        return cy.get("article.card");
    }

    firstTitle() {
        return cy.get("article.card h3 a").first();
    }
}