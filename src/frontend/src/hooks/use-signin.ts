"use client";
import { useMutation } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { apiClient } from "@/lib/api-client";
import { saveAuthTokens } from "@/hooks/use-auth-store";

export function useSignIn() {
  const router = useRouter();

  return useMutation({
    mutationFn: ({ email, password }: { email: string; password: string }) =>
      apiClient.signIn(email, password),
    onSuccess: (data) => {
      saveAuthTokens(data);
      router.push("/dashboard");
    },
  });
}
