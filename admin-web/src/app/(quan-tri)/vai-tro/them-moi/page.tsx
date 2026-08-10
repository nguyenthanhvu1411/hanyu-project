import {
  PageContainer,
} from "@/components/layout/page-container";

import {
  PageHeader,
} from "@/components/layout/page-header";

import {
  RoleForm,
} from "@/features/identity/components/role-form";

export default function CreateRolePage() {
  return (
    <PageContainer>
      <PageHeader
        title="Thêm vai trò"
        description="Tạo vai trò và gán quyền truy cập."
      />

      <RoleForm />
    </PageContainer>
  );
}
