export const HSK_LEVEL_DEFAULT_PAGE_SIZE = 20;

export const HSK_LEVEL_STATUS_OPTIONS = [
  {
    label: "Hoạt động",
    value: "true",
  },
  {
    label: "Ngừng hoạt động",
    value: "false",
  },
] as const;

export const HSK_LEVEL_SORT_OPTIONS = [
  {
    label: "Thứ tự tăng dần",
    value: "sortOrder-asc",
  },
  {
    label: "Thứ tự giảm dần",
    value: "sortOrder-desc",
  },
  {
    label: "Mã A → Z",
    value: "code-asc",
  },
  {
    label: "Mã Z → A",
    value: "code-desc",
  },
] as const;
