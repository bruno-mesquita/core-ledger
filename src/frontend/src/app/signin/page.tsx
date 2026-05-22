import { SignInForm } from "@/components/auth/signin-form";

export const metadata = {
  title: "Sign In — CoreLedger",
};

export default function SignInPage() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-background px-4">
      <SignInForm />
    </main>
  );
}
