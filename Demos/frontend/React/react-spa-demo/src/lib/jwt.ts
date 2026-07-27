const NAME_CLAIM = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
const ROLE_CLAIM = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

interface JwtPayload {
    [claim: string]: string | number | undefined;
}

export interface Identity {
    name: string,
    role: string
}
//  just for demo
export function decodeToken(token:string): Identity | null {
    try {
        const segment = token.split(".")[1];
        if(!segment) return null;
        
        console.log("token", token)
        console.log("segment", segment)
        
        const base64 = segment.replace(/-/g, "+").replace(/_/g, "/")
        console.log("base64", base64)
        
        const payload = JSON.parse(atob(base64)) as JwtPayload
        console.log("payload", payload)

        const name = payload[NAME_CLAIM]
        const role = payload[ROLE_CLAIM]

        if(typeof name !== "string" || typeof role !== "string") return null;

        return {name, role}
    } catch {
        return null;
    }
}