import { useEffect, useState } from "react"
import BookCard from "../components/BookCard"
import SearchBar from "../components/SearchBar";

import type { FetchState, InventoryItem, SortDirection } from "../lib/types";
import { getInventory } from "../lib/api";


export default function CatalogPage(){
    const [items, setItems] = useState<InventoryItem[]>([]);
    const [fState, setFState] = useState<FetchState>("idle")
    const [compact, setCompact] = useState(true);

    const [userQuery, setUserQuery] = useState("")
    const [sortDir, setSortDir] = useState<SortDirection>("asc");

    useEffect(() => {
        let active = true;
        setFState("loading")

        getInventory()
            .then((res: InventoryItem[]) => {
                if(!active) return;
                setItems(res);
                setFState("loaded")
            })
            .catch(() => {
                if (active) setFState("failed");
            })
        
        return () => {
            active = false;
        };    
    }, [])

    const visibleBooks = [...items]
        .filter((i) => i.name.toLowerCase().includes(userQuery.toLowerCase()))
        .sort((a, b) => sortDir === "asc" ? 
            a.name.localeCompare(a.name) : 
            b.name.localeCompare(b.name)
        );

    if (fState === "idle" || fState === "loading") return <p>Loading catalog...</p>

    if (fState === "failed") return <p>Could not reach the API</p>

    return (
        <section>
            <div className="toolbar">
                <h2>Catalog</h2>
                <SearchBar value={userQuery} onChange={setUserQuery} />
                <button type="button" onClick={() => setCompact(!compact)}>
                    {compact ? "Show detail" : "Compact view"}
                </button>
                <button type="button"
                    onClick={() => {
                        setSortDir((d) => d === "asc" ? "desc" : "asc")
                    }}
                >
                    Sort {sortDir === "asc" ? "Z-A" : "A-Z"}
                </button>
                <div className="cards">
                    {visibleBooks.map(i => <BookCard key={i.sku} item={i} compact={compact}/>)}
                </div>
            </div>
        </section>
    )
}