import Link from "next/link";

export function NavBar() {
  return (
    <header className="sticky top-0 z-50 border-b border-border bg-background/80 backdrop-blur-md">
      <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4">
        <span className="font-display text-xl tracking-tight text-foreground">
          CoreLedger
        </span>
        <nav className="hidden gap-8 text-sm text-muted md:flex">
          <a
            href="#features"
            className="transition-colors hover:text-foreground"
          >
            Features
          </a>
          <a
            href="#how-it-works"
            className="transition-colors hover:text-foreground"
          >
            How It Works
          </a>
        </nav>
        <div className="flex items-center gap-3">
          <Link
            href="/signin"
            className="text-sm text-muted transition-colors hover:text-foreground"
          >
            Sign in
          </Link>
          <Link
            href="/signup"
            className="rounded-md bg-accent px-4 py-2 text-sm font-medium text-white transition-opacity hover:opacity-90"
          >
            Get started
          </Link>
        </div>
      </div>
    </header>
  );
}
