export const USER_STATUS = {
  ACTIVE: "active",
  LOCKED: "locked",
  DISABLED: "disabled",
  PENDING: "pending",
} as const;

export type UserStatus =
  (typeof USER_STATUS)[keyof typeof USER_STATUS];

export const USER_STATUS_OPTIONS = [
  {
    value: USER_STATUS.ACTIVE,
    label: "Hoạt động",
  },
  {
    value: USER_STATUS.LOCKED,
    label: "Đã khóa",
  },
  {
    value: USER_STATUS.DISABLED,
    label: "Đã vô hiệu hóa",
  },
  {
    value: USER_STATUS.PENDING,
    label: "Chờ kích hoạt",
  },
] as const;
