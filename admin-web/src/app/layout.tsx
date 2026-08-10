import type { Metadata } from "next";
import "./globals.css";
import { AppProvider } from "@/providers/app-provider";

export const metadata: Metadata = {
  title: "HanYu Admin",
  description: "Hệ thống quản trị HanYu",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="vi">
      <body>
        <AppProvider>{children}</AppProvider>
      </body>
    </html>
  );
}
