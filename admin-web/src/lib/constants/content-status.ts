export enum ContentStatus {
  Draft = 0,
  Review = 1,
  Approved = 2,
  Published = 3,
  Archived = 4,
}

export const contentStatusLabels: Record<ContentStatus, string> = {
  [ContentStatus.Draft]: "Bản nháp",
  [ContentStatus.Review]: "Chờ duyệt",
  [ContentStatus.Approved]: "Đã duyệt",
  [ContentStatus.Published]: "Đã xuất bản",
  [ContentStatus.Archived]: "Đã lưu trữ",
};

export function getContentStatusLabel(status: ContentStatus): string {
  return contentStatusLabels[status] ?? "Không xác định";
}
