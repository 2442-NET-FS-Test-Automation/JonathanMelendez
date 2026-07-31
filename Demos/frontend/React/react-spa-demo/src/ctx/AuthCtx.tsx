import { createContext, useContext, useEffect, useReducer, type ReactNode } from "react";
import { authReducer, initialAuthState, type AuthState } from "../lib/authReducer";
import { clearToken, getToken, setToken } from "../lib/storage";
import { loginRequest } from "../lib/api";
import { decodeToken } from "../lib/jwt";

interface AuthContextValue extends AuthState {
    login: (username: string, password: string) => Promise<boolean>,
    logout: () => void,
}

export const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider ({ children }: { children: ReactNode }) {
    const [state, dispatch] = useReducer(authReducer, initialAuthState);

    useEffect(() => {
        const token = getToken();
        if (!token) return;

        const user = decodeToken(token);
        if (user) dispatch({ type: "login_success", user });
        else clearToken();

    }, []);

    async function login(username: string, password: string): Promise<boolean> {
        dispatch({ type: "login_start" })

        try {
            const token = await loginRequest(username, password);

            const user = decodeToken(token);
            if (!user) throw new Error("token missing expected claims");

            setToken(token);
            dispatch({ type: "login_success", user });

            return true;
        } catch (e) {
            console.log(e);
            dispatch({ type: "login_failure", error: "Invalid username or password" })
            return false;
        }
    }

    function logout() {
        clearToken();
        dispatch({ type: "logout" })
    }

    return (
        <AuthContext.Provider value={{ ...state, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error("not wrapped");
    return ctx;
}