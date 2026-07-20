import { useState } from "react"
import BookCard from "../components/BookCard"
import { InventoryItems } from "../data/data";


export default function CatalogPage(){
    const [items] = useState(InventoryItems);

    const [compact, setCompact] = useState(true);
    return (
        <>
            <div className="toolbar">
                <h2>Catalog</h2>
                <button type="button" onClick={() => setCompact(!compact)}>
                    {compact ? "Show detail" : "Compact view"}
                </button>
            </div>
            <div className="cards">
                {items.map(i => <BookCard key={i.sku} item={i} compact={compact} />)}
            </div>
        </>
    )
}