"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";
import { signInSchema, type SignInValues } from "@/lib/auth-schemas";
import { useSignIn } from "@/hooks/use-signin";
import Link from "next/link";

export function SignInForm() {
  const form = useForm<SignInValues>({
    resolver: zodResolver(signInSchema),
    defaultValues: { email: "", password: "" },
  });
  const { mutate, isPending, error } = useSignIn();

  function onSubmit(values: SignInValues) {
    mutate(values);
  }

  return (
    <Card className="w-full max-w-md border-border bg-surface">
      <CardHeader className="space-y-1">
        <CardTitle className="font-display text-2xl text-foreground">
          Welcome back
        </CardTitle>
        <CardDescription className="text-muted">
          Sign in to your CoreLedger account
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-foreground">Email</FormLabel>
                  <FormControl>
                    <Input
                      type="email"
                      placeholder="you@example.com"
                      className="border-border bg-background text-foreground placeholder:text-muted"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="password"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-foreground">Password</FormLabel>
                  <FormControl>
                    <Input
                      type="password"
                      placeholder="••••••••"
                      className="border-border bg-background text-foreground placeholder:text-muted"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            {error && (
              <p className="text-sm text-destructive">{error.message}</p>
            )}
            <Button
              type="submit"
              className="w-full bg-accent text-white hover:opacity-90"
              disabled={isPending}
            >
              {isPending ? "Signing in…" : "Sign in"}
            </Button>
          </form>
        </Form>
        <p className="mt-4 text-center text-sm text-muted">
          No account?{" "}
          <Link
            href="/signup"
            className="text-accent hover:underline"
          >
            Create one
          </Link>
        </p>
      </CardContent>
    </Card>
  );
}
