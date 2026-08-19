import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { SecurityCenter } from "@/features/identity/components/security-center";

export default function SecurityPage() {
  return (
    <PageContainer>
      <PageHeader title="Bảo mật tài khoản" description="Quản lý sessions và theo dõi các sự kiện bảo mật của tài khoản Admin hiện tại." />
      <SecurityCenter />
    </PageContainer>
  );
}
