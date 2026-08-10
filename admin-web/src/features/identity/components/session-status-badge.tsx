import {
  StatusBadge,
} from "@/components/common/status-badge";

export function SessionStatusBadge({
  active,
}: {
  active: boolean;
}) {
  return active ? (
    <StatusBadge variant="success">
      Đang hoạt động
    </StatusBadge>
  ) : (
    <StatusBadge variant="neutral">
      Đã thu hồi
    </StatusBadge>
  );
}
