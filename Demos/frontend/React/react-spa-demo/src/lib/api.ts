import axios from "axios";
import type { InventoryItem } from "./types";

export const api = axios.create({baseURL: "http://localhost:5224/api"});


export async function getInventory(): Promise<InventoryItem[]> {
    const res = await api.get<InventoryItem[]>("/Inventory");
    return res.data;
}

export async function getInventoryItem(sku: string): Promise<InventoryItem> {
    const res = await api.get<InventoryItem>(`/Inventory/${sku}`);
    return res.data;
}