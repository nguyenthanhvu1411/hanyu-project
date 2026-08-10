import {
  StatusBadge,
} from "@/components/common/status-badge";

import type {
  UserStatus,
} from "../identity.constants";

interface UserStatusBadgeProps {
  status: UserStatus;
  deleted?: boolean;
}

export function UserStatusBadge({
  status,
  deleted = false,
}: UserStatusBadgeProps) {
  if (deleted) {
    return (
      <StatusBadge variant="danger">
        Đã xóa
      </StatusBadge>
    );
  }

  switch (status) {
    case "active":
      return (
        <StatusBadge variant="success">
          Hoạt động
        </StatusBadge>
      );

    case "locked":
      return (
        <StatusBadge variant="danger">
          Bị khóa
        </StatusBadge>
      );

    case "disabled":
      return (
        <StatusBadge variant="neutral">
          Vô hiệu hóa
        </StatusBadge>
      );

    case "pending":
      return (
        <StatusBadge variant="warning">
          Chờ duyệt
        </StatusBadge>
      );

    default:
      return (
        <StatusBadge variant="neutral">
          Không xác định
        </StatusBadge>
      );
  }
}
