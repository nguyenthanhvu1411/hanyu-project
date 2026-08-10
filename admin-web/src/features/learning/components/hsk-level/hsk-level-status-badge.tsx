import { StatusBadge } from "@/components/common/status-badge";

export function HskLevelStatusBadge({ isActive }: { isActive: boolean }) {
  return isActive ? (
    <StatusBadge variant="success">Hoạt động</StatusBadge>
  ) : (
    <StatusBadge variant="neutral">Ngừng hoạt động</StatusBadge>
  );
}
