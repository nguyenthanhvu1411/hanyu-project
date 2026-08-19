import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { ProfileOverview } from "@/features/identity/components/profile-overview";

export default function ProfilePage() {
  return (
    <PageContainer>
      <PageHeader title="Hồ sơ của tôi" description="Thông tin tài khoản, roles, permissions và các lối tắt bảo mật của Admin hiện tại." />
      <ProfileOverview />
    </PageContainer>
  );
}
