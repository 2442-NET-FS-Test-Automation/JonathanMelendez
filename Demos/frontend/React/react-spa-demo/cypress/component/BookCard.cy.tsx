import { MemoryRouter } from "react-router-dom";
import BookCard from "../../src/components/BookCard";
import type { InventoryItem } from "../../src/lib/types";

import { mount } from "cypress/react";

describe("Bookcard (component)", () => {
    const item: InventoryItem = { sku: "BK-001", name: "Clean Code", currentStock: 4 }

    it("renders info no compact", () => {
        mount(
            <MemoryRouter>
                <BookCard item={item} compact={false} />
            </MemoryRouter>
        );

        cy.contains("h3", "Clean Code");
        cy.contains("dd", "BK-001");
        cy.contains("dd", "4");
    });

    it("renders info compact", () => {
        mount(
            <MemoryRouter>
                <BookCard item={item} compact={true} />
            </MemoryRouter>
        );

        cy.contains("h3", "Clean Code");
        cy.contains("dd", "BK-001");
        cy.should("not.have.text", "4");
    });

    it("marks an out-of-stock book with 'out' class", () => {
        mount(
            <MemoryRouter>
                <BookCard item={{ sku: "BK-001", name: "Clean Code", currentStock: 0 }} compact={false} />
            </MemoryRouter>
        );

        cy.get("dd.out").should("have.text", "0");
    })
})