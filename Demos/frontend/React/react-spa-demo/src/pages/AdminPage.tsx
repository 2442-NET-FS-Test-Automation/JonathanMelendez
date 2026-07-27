import { useState } from "react";
import type { SubmitEvent } from "react";
import { createBook, deleteBook } from "../lib/api";
import { useAuth } from "../ctx/AuthCtx";

export default function AdminPage(){
    const { user } = useAuth();
    const [sku, setSku] = useState("");
    const [name, setName] = useState("");
    const [price, setPrice] = useState(0);
    const [stock, setStock] = useState(0);
    const [message, setMessage] = useState<string | null>(null)

    async function onCreate(e:SubmitEvent<HTMLFormElement>) {
        e.preventDefault();
        setMessage(null);
        try {
            const created = await createBook({sku, name, price, currentStock: stock});
            setMessage("Created "+created.sku+" - "+created.name);
            setSku("")
            setName("")
            setPrice(0)
            setStock(0)
        } catch {
            setMessage("Create Failed. Check fields and permissions")
        }
    }

    async function onDelete() {
        if (!sku) return;
        setMessage(null)
        try {
            await deleteBook(sku);
            setSku("");
        } catch {
            setMessage("Delete failed for sku: "+sku)
        }
    }

    return (
        <section>
            <h2>Admin - {user?.name}</h2>
            <form className="admin-form" onSubmit={onCreate}>
                <input type="text" placeholder="SKU" value={sku} onChange={(e) => setSku(e.target.value)} />
                <input type="text" placeholder="Name" value={name} onChange={(e) => setName(e.target.value)} />
                <input type="number" placeholder="Price" value={price} onChange={(e) => setPrice(e.target.valueAsNumber)} />
                <input type="number" placeholder="Stock" value={stock} onChange={(e) => setStock(e.target.valueAsNumber)} />
                <button type="submit">Create</button>
                <button type="button" onClick={onDelete}>Delete by SKU</button>
            </form>
            {message && <p>{message}</p>}
        </section>
    )
}