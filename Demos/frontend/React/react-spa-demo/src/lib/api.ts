import axios from "axios";
import type { InventoryItem } from "./types";
import { getToken } from "./storage";

export const api = axios.create({ baseURL: "http://localhost:5224/api" });

api.interceptors.request.use((config) => {
    const token = getToken();
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
})


export async function getInventory(): Promise<InventoryItem[]> {
    const res = await api.get<InventoryItem[]>("/Inventory");
    return res.data;
}

export async function getInventoryItem(sku: string): Promise<InventoryItem> {
    const res = await api.get<InventoryItem>(`/Inventory/${sku}`);
    return res.data;
}

export async function loginRequest(username: string, password: string): Promise<string> {
    const response = api.post<{ token: string }>("auth/login", { username, password });
    return (await response).data.token;
}