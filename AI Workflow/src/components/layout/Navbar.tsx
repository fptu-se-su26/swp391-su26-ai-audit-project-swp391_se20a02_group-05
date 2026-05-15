import Link from "next/link";
import { Plane, Compass, User, Menu } from "lucide-react";
import { Button } from "@/components/ui/button";

export function Navbar() {
  return (
    <header className="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container flex h-14 max-w-screen-2xl items-center px-4">
        <div className="mr-4 hidden md:flex">
          <Link href="/" className="mr-6 flex items-center space-x-2">
            <Plane className="h-6 w-6 text-primary" />
            <span className="hidden font-bold sm:inline-block">
              AI Travel Planner
            </span>
          </Link>
          <nav className="flex items-center space-x-6 text-sm font-medium">
            <Link
              href="/dashboard"
              className="transition-colors hover:text-foreground/80 text-foreground/60"
            >
              Dashboard
            </Link>
            <Link
              href="/trips"
              className="transition-colors hover:text-foreground/80 text-foreground/60"
            >
              My Trips
            </Link>
          </nav>
        </div>
        <Button variant="ghost" size="icon" className="md:hidden">
          <Menu className="h-5 w-5" />
          <span className="sr-only">Toggle Menu</span>
        </Button>
        <div className="flex flex-1 items-center justify-between space-x-2 md:justify-end">
          <div className="w-full flex-1 md:w-auto md:flex-none">
            {/* Search could go here */}
          </div>
          <nav className="flex items-center space-x-2">
            <Button variant="ghost" size="icon">
              <Compass className="h-5 w-5" />
              <span className="sr-only">Discover</span>
            </Button>
            <Button variant="ghost" size="icon">
              <User className="h-5 w-5" />
              <span className="sr-only">Profile</span>
            </Button>
          </nav>
        </div>
      </div>
    </header>
  );
}
