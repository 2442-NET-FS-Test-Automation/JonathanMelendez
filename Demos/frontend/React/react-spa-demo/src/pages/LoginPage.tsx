import { useState } from "react";
import { useAuth } from "../ctx/AuthCtx";
import { useNavigate } from "react-router-dom";


export default function LoginPage(){
    const { login, status } = useAuth();
    
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);

    const navigate = useNavigate()

    async function onSubmit(e: React.SubmitEvent<HTMLFormElement>) {
        e.preventDefault();

        setError(null);

        const ok = await login(username, password)

        if (ok) navigate("/");
        else setError("Invalid username or password");
    }

    return(
        <form className="login" onSubmit={onSubmit}>
            <h2>Sign in</h2>
            <label>
                Username
                <input type="text" value={username} 
                    onChange={(e) => setUsername(e.target.value)} />
            </label>
            <label>
                Password
                <input type="password" value={password} 
                    onChange={(e) => setPassword(e.target.value)} />
            </label>
            <button type="submit" disabled={status === "authenticating"}>
                {status === "authenticating" ? "Signing in..." : "Sign in"}
            </button>

            {error && <p className="error">{error}</p>}
        </form>
    );
}