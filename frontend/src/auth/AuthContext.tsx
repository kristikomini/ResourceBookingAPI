import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { clearToken, getToken, setToken } from "../api/client";
import { login as loginRequest } from "../api/endpoints";

// The .NET TokenService issues claims under the standard WS-Identity URIs.
const CLAIM_ID =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
const CLAIM_EMAIL =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
const CLAIM_NAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

export interface CurrentUser {
  userId: number;
  email: string;
  name: string;
}

interface AuthState {
  user: CurrentUser | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthState | undefined>(undefined);

function decodeToken(token: string): CurrentUser | null {
  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    const exp = payload.exp as number | undefined;
    if (exp && exp * 1000 < Date.now()) {
      return null; // expired
    }
    return {
      userId: Number(payload[CLAIM_ID]),
      email: payload[CLAIM_EMAIL] ?? "",
      name: payload[CLAIM_NAME] ?? "",
    };
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<CurrentUser | null>(() => {
    const token = getToken();
    return token ? decodeToken(token) : null;
  });

  const login = useCallback(async (email: string, password: string) => {
    const token = await loginRequest(email, password);
    setToken(token);
    const decoded = decodeToken(token);
    if (!decoded) throw new Error("Received an invalid token.");
    setUser(decoded);
  }, []);

  const logout = useCallback(() => {
    clearToken();
    setUser(null);
  }, []);

  const value = useMemo(() => ({ user, login, logout }), [user, login, logout]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth(): AuthState {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within an AuthProvider");
  return ctx;
}
