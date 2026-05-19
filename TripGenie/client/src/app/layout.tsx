import type { Metadata } from "next";
import { Plus_Jakarta_Sans, Inter } from "next/font/google";
import "./globals.css";
import { Providers } from "./providers";
import { cookies } from "next/headers";

// Premium Typography setup
const outfit = Plus_Jakarta_Sans({
  variable: "--font-outfit",
  subsets: ["latin", "vietnamese"],
  weight: ["300", "400", "500", "600", "700", "800"],
});

const inter = Inter({
  variable: "--font-inter",
  subsets: ["latin", "vietnamese"],
  weight: ["300", "400", "500", "600", "700", "800"],
});

export const metadata: Metadata = {
  title: "TripGenie AI - Enterprise Travel Companion Platform",
  description: "Plan, secure, and experience customized itineraries designed by advanced travel intelligence.",
};

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const cookieStore = await cookies();
  const cookieVal = cookieStore.get("i18next")?.value;
  const locale = (cookieVal === "en" || cookieVal === "vi") ? cookieVal : "vi";

  return (
    <html
      lang={locale}
      className={`${outfit.variable} ${inter.variable} h-full antialiased dark`}
    >
      <body className="min-h-full flex flex-col font-sans bg-zinc-50 dark:bg-zinc-950 transition-colors duration-300">
        <Providers locale={locale}>
          {children}
        </Providers>
      </body>
    </html>
  );
}
