import SearchBar from "../../src/components/SearchBar";

import { mount } from "cypress/react";

describe("SearchBar [component]", () => {
    it("renders the value passed in by parent", () => {
        mount(<SearchBar value="clean" onChange={() => {}} />);

        cy.get("input[type=search]").should("have.value", "clean");
    });

    it("reports every key stroke", () => {
        const onChange = cy.spy().as("onChange");

        mount(<SearchBar value="" onChange={onChange} />);

        cy.get("input[type=search]").type("dune");
        cy.get("@onChange").should("have.callCount", 4);
        cy.get("@onChange").should("have.been.calledWith", "d");
        cy.get("@onChange").should("have.been.calledWith", "e");
    });
});