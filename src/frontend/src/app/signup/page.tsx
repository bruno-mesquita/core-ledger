import { SignUpForm } from "@/components/auth/signup-form";

export const metadata = {
  title: "Create Account — CoreLedger",
};

export default function SignUpPage() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-background px-4">
      <SignUpForm />
    </main>
  );
}
