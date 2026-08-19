import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { ChangePasswordForm } from "@/features/identity/components/change-password-form";

export default function ChangePasswordPage() {
  return (
    <PageContainer>
      <PageHeader title="Đổi mật khẩu" description="Cập nhật mật khẩu cho tài khoản Admin đang đăng nhập." />
      <ChangePasswordForm />
    </PageContainer>
  );
}
