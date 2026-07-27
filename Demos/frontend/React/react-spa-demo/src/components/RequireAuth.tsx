import type { ReactNode } from "react";
import { useAuth } from "../ctx/AuthCtx";
import { Navigate, useLocation } from "react-router-dom";

interface RequireAuthProps {
    children: ReactNode
    role?: string;
}

export function RequireAuth({children, role}: RequireAuthProps) {
    const { status, user } = useAuth();
    const location = useLocation()

    if (status !== "authenticated") return <Navigate to="/login" state={{from:location}} replace />

    if (role && user?.role !== role) return <Navigate to="/" replace />

    return <>{children}</>
}