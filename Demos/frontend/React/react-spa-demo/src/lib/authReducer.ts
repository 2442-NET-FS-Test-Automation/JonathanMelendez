import type { Identity } from "./jwt";

export interface AuthState {
    status: "anonymous" | "authenticating" | "authenticated" | "error",
    user: Identity | null,
    error: string | null
}

export type AuthAction =
    | { type: "login_start" }
    | { type: "login_success"; user: Identity }
    | { type: "login_failure"; error: string }
    | { type: "logout" }

export const initialAuthState: AuthState = {
    status: "anonymous",
    user: null,
    error: null
}

export function authReducer(state: AuthState, action: AuthAction): AuthState {
    switch (action.type) {
        case "login_start": return { ...state, status: "authenticating", error: null }
        case "login_success": return { status: "authenticated", user: action.user, error: null }
        case "login_failure": return { status: "error", user: null, error: action.error }
        case "logout": return { ...initialAuthState }
    }
}
