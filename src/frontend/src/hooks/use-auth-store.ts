export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
  email: string;
  role: string;
}

const STORAGE_KEY = "cl_auth";

export function saveAuthTokens(tokens: AuthTokens): void {
  if (typeof window === "undefined") return;
  localStorage.setItem(STORAGE_KEY, JSON.stringify(tokens));
}

export function getAuthTokens(): AuthTokens | null {
  if (typeof window === "undefined") return null;
  const raw = localStorage.getItem(STORAGE_KEY);
  if (!raw) return null;
  try {
    const parsed = JSON.parse(raw) as AuthTokens;
    if (!parsed.accessToken || !parsed.refreshToken) {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
    return parsed;
  } catch {
    localStorage.removeItem(STORAGE_KEY);
    return null;
  }
}

export function clearAuthTokens(): void {
  if (typeof window === "undefined") return;
  localStorage.removeItem(STORAGE_KEY);
}
