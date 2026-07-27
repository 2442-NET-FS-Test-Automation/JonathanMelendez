import axios from "axios";
import type { InventoryItem, SupplierPrice } from "./types";
import { getToken } from "./storage";

export const api = axios.create({ baseURL: "http://localhost:5224/api" });

api.interceptors.request.use((config) => {
    const token = getToken();
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
})

// Inventory
export async function getInventory(): Promise<InventoryItem[]> {
    const res = await api.get<InventoryItem[]>("/Inventory");
    return res.data;
}

export async function getInventoryItem(sku: string): Promise<InventoryItem> {
    const res = await api.get<InventoryItem>(`/Inventory/${sku}`);
    return res.data;
}

export interface CreateInventoryBody {
    sku: string,
    name: string,
    price: number,
    currentStock: number
}

export async function getSupplierPrice(sku:string): Promise<SupplierPrice> {
    const response = await api.get<SupplierPrice>("/Inventory/"+sku+"/supplier-price");
    return response.data;
}

export async function createBook(body:CreateInventoryBody): Promise<InventoryItem> {
    const response = await api.post<InventoryItem>("/Inventory", body);
    return response.data;
}

export async function deleteBook(sku:string): Promise<void> {
    await api.delete("Inventory"+sku);
} 


// Auth
export async function loginRequest(username: string, password: string): Promise<string> {
    const response = api.post<{ token: string }>("auth/login", { username, password });
    return (await response).data.token;
}