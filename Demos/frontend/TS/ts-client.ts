export interface ApiError {
    status: number,
    message: string
}

export class ApiClient {
    constructor(private readonly baseUrl: string = "http://localhost:5224/api") {}
    async getJson<T>(path: string): Promise<T | ApiError> {
        try {
            const res = await fetch(this.baseUrl+path);
            if (!res.ok) return { status: res.status, message: `API said: ${res.status}`}
            return await res.json() as T;
        } catch {
            return { status: 0, message: `Cannot reach the API`}
        }
    }
}