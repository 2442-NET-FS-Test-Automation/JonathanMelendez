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

export enum HttpStatus {
    Ok = 200,
    Accepted = 201,
    NoContent = 204,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404
}

export enum SortDirection{
    Ascending = "asc",
    Descending = "desc"
}