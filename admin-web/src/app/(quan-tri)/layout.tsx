import { AdminShell } from "@/components/layout/admin-shell";
import { AuthGuard } from "@/security/auth-guard";

export default function AdminLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <AuthGuard>
      <AdminShell>{children}</AdminShell>
    </AuthGuard>
  );
}
