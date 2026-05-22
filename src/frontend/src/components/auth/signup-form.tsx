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
  FormDescription,
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
import { signUpSchema, type SignUpValues } from "@/lib/auth-schemas";
import { useSignUp } from "@/hooks/use-signup";
import Link from "next/link";

export function SignUpForm() {
  const form = useForm<SignUpValues>({
    resolver: zodResolver(signUpSchema),
    defaultValues: { email: "", password: "" },
  });
  const { mutate, isPending, error } = useSignUp();

  function onSubmit(values: SignUpValues) {
    mutate(values);
  }

  return (
    <Card className="w-full max-w-md border-border bg-surface">
      <CardHeader className="space-y-1">
        <CardTitle className="font-display text-2xl text-foreground">
          Create your account
        </CardTitle>
        <CardDescription className="text-muted">
          Get started with CoreLedger today
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
                  <FormDescription className="text-muted text-xs">
                    Min 8 chars · 1 uppercase · 1 number · 1 special character
                  </FormDescription>
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
              {isPending ? "Creating account…" : "Create account"}
            </Button>
          </form>
        </Form>
        <p className="mt-4 text-center text-sm text-muted">
          Already have an account?{" "}
          <Link
            href="/signin"
            className="text-accent hover:underline"
          >
            Sign in
          </Link>
        </p>
      </CardContent>
    </Card>
  );
}
