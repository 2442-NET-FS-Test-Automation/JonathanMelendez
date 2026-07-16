import { InventoryItem, HttpStatus, SortDirection, SupplierPrice, FetchState } from "./types.js";
import { ApiClient } from "./ts-client.js";

const catalog: InventoryItem[] = [
    {sku: "BK-001", name: "Clean Code", currentStock: 5 },
    {sku: "BK-002", name: "DUne", currentStock: 3 },
    {sku: "BK-003", name: "Refactoring", currentStock: 8 }
];

function printCatalog(items: InventoryItem[]) : void {
    for (const item of items) {
        console.log(`${item.sku} ${item.name} ${item.currentStock}`)
    }
}
console.log("catalog:")
printCatalog(catalog)

console.log(HttpStatus.Unauthorized)
console.log(HttpStatus[401])

const api = new ApiClient();
const liveCatalog = await api.getJson("/Inventory");
console.log(liveCatalog);

