import { useState, useEffect } from "react";
import { Link, useParams } from "react-router-dom";
import { getInventoryItem } from "../lib/api";
import type { InventoryItem, FetchState } from "../lib/types";

export default function BookDetailPage() {
    const { sku } = useParams<{ sku: string }>(); 
    const  [item, setItem] = useState<InventoryItem | null>(null);
    const [fState, setFState] = useState<FetchState>("idle");

    useEffect(() => {
        if (!sku) return;
        let active = true;
        setFState("loading");

        getInventoryItem(sku)
            .then((data) => {
                if (!active) return;
                setItem(data);
                setFState("loaded");
            })
            .catch(() => {
                if (active) setFState("failed");
            })
        return () => {
            active = false;
        }
    }, [sku])
    
    if (fState === "idle" || fState === "loading") return <p>Loading catalog...</p>

    if (fState === "failed" || !item) return (
        <p>Could not reach the API <Link to="/">Back To Catalog</Link></p>
    )



    return (
        <article>
            <p>
                <Link to="/">&larr; Back To Catalog</Link>
            </p>
            <h2>{item.name}</h2>
            <p>SKU: {item.sku}</p>
            <p>In stock: {item.currentStock}</p>
        </article>
    );
}