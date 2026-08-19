import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { ProfileEditForm } from "@/features/identity/components/profile-edit-form";

export default function EditProfilePage() {
  return (
    <PageContainer>
      <PageHeader
        title="Chỉnh sửa hồ sơ"
        description="Cập nhật tên hiển thị, trình độ HSK, mục tiêu học tập, múi giờ và ngôn ngữ giao diện."
      />
      <ProfileEditForm />
    </PageContainer>
  );
}