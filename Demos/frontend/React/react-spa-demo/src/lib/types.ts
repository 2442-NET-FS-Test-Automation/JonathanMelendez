export interface InventoryItem {
    sku: Sku;
    name: string;
    currentStock: number;
}

export interface SupplierPrice {
    sku: Sku;
    supplierPrice: number;
}

export type Sku = string;

export type FetchState = "idle" | "loading" | "loaded" | "failed"

export type HttpStatus = {
    Ok: 200,
    Accepted: 201,
    NoContent: 204,
    BadRequest: 400,
    Unauthorized: 401,
    Forbidden: 403,
    NotFound: 404
}
 
export type SortDirection = "asc" | "desc";

export type InventoryPatch = Partial<InventoryItem>

export type NewInventoryItem = Omit<InventoryItem, "sku">