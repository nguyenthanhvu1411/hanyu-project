import { AccessDenied } from "@/components/common/access-denied";

export default function AccessDeniedPage() {
  return (
    <AccessDenied
      title="Không có quyền truy cập"
      description="Tài khoản của bạn không được cấp quyền truy cập chức năng này."
    />
  );
}
