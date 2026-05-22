"use client";
import { useMutation } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { apiClient } from "@/lib/api-client";
import { saveAuthTokens } from "@/hooks/use-auth-store";

export function useSignUp() {
  const router = useRouter();

  return useMutation({
    mutationFn: ({ email, password }: { email: string; password: string }) =>
      apiClient.signUp(email, password),
    onSuccess: (data) => {
      saveAuthTokens(data);
      router.push("/dashboard");
    },
  });
}
