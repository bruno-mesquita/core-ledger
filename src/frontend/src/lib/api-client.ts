const BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  email: string;
  role: string;
}

async function request<T>(path: string, init: RequestInit): Promise<T> {
  const res = await fetch(`${BASE}${path}`, {
    headers: { "Content-Type": "application/json", ...(init.headers ?? {}) },
    ...init,
  });
  if (!res.ok) {
    const body = await res.json().catch(() => ({}));
    const msg =
      (body as { title?: string; error?: string }).title ??
      (body as { error?: string }).error ??
      `HTTP ${res.status}`;
    throw new Error(msg);
  }
  return res.json() as Promise<T>;
}

export const apiClient = {
  signIn: (email: string, password: string) =>
    request<AuthResponse>("/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    }),
  signUp: (email: string, password: string) =>
    request<AuthResponse>("/auth/register", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    }),
};
