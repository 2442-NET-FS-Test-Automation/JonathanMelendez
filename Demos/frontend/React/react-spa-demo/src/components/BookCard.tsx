import { Link } from "react-router-dom"
import type { InventoryItem } from "../lib/types"

interface BookCardProps {
    item: InventoryItem,
    compact?: boolean
}

export default function BookCard({ item, compact=false }: BookCardProps){

    return(
        <article className="card">
            <h3>
                <Link to={`/inventory/${item.sku}`} >{item.name}</Link>
            </h3>
            <dl>
                <dd>{item.sku}</dd>
                {!compact && (
                    <>
                        <dt>In Stock</dt>
                        <dd className={item.currentStock === 0 ? "out" : ""}>
                            {item.currentStock}
                        </dd>
                    </>
                )}
            </dl>
        </article>
    )
}