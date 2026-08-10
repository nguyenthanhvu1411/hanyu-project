import {
  PageContainer,
} from "@/components/layout/page-container";

import {
  PageHeader,
} from "@/components/layout/page-header";

import {
  UserForm,
} from "@/features/identity/components/user-form";

export default function CreateUserPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Thêm người dùng"
        description="Tạo mới tài khoản người dùng trong hệ thống."
      />

      <UserForm />
    </PageContainer>
  );
}
