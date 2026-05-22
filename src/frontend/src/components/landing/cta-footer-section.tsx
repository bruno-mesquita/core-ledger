import Link from "next/link";

export function CtaFooterSection() {
  return (
    <footer className="border-t border-border bg-surface">
      <div className="mx-auto max-w-6xl px-6 py-20 text-center">
        <h2 className="font-display mb-4 text-3xl tracking-tight text-foreground sm:text-4xl">
          Ready to build on solid ground?
        </h2>
        <p className="mx-auto mb-8 max-w-lg text-muted">
          Join CoreLedger and get institutional-grade financial infrastructure
          for your platform today.
        </p>
        <Link
          href="/signup"
          className="inline-block rounded-md bg-accent px-8 py-3 text-sm font-medium text-white transition-opacity hover:opacity-90"
        >
          Get started free
        </Link>
      </div>
      <div className="border-t border-border px-6 py-6 text-center text-xs text-muted">
        © {new Date().getFullYear()} CoreLedger. All rights reserved.
      </div>
    </footer>
  );
}
