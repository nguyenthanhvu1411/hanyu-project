export enum ContentEntityType {
  Vocabulary = 0,
  VocabularyExample = 1,
  Lesson = 2,
  LessonSection = 3,
  QuizQuestion = 4,
  AudioAsset = 5,
  Course = 6,
  CourseChapter = 7,
}

export enum ContentReportReason {
  IncorrectContent = 0,
  IncorrectTranslation = 1,
  IncorrectPinyin = 2,
  AudioProblem = 3,
  Typo = 4,
  OutdatedContent = 5,
  Other = 99,
}

export enum ContentReportStatus {
  Open = 0,
  InReview = 1,
  Resolved = 2,
  Rejected = 3,
}

export enum ContentImportType {
  Vocabulary = 0,
  VocabularyExample = 1,
  Lesson = 2,
  Quiz = 3,
}

export enum ContentImportStatus {
  Pending = 0,
  Processing = 1,
  Completed = 2,
  CompletedWithErrors = 3,
  Failed = 4,
}

export interface AdminContentReport {
  id: number;
  publicId: string;
  userId: string;
  entityType: ContentEntityType;
  entityId: number;
  reason: ContentReportReason;
  description: string | null;
  status: ContentReportStatus;
  resolvedByUserId: string | null;
  resolvedAt: string | null;
  resolutionNote: string | null;
  createdAt: string;
  updatedAt: string;
  userDisplayName?: string | null;
  userEmail?: string | null;
  entityDisplayName?: string | null;
  resolvedByDisplayName?: string | null;
}

export interface AdminContentReportQuery {
  userId?: string;
  entityType?: ContentEntityType;
  reason?: ContentReportReason;
  status?: ContentReportStatus;
  from?: string;
  to?: string;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminContentImportJob {
  id: number;
  publicId: string;
  importType: ContentImportType;
  originalFileName: string;
  storagePath: string;
  status: ContentImportStatus;
  totalRows: number;
  processedRows: number;
  successRows: number;
  failedRows: number;
  startedAt: string | null;
  completedAt: string | null;
  errorMessage: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface AdminContentImportRow {
  id: number;
  rowNumber: number;
  sourceJson: string;
  isSuccessful: boolean;
  createdEntityId: number | null;
  errorCode: string | null;
  errorMessage: string | null;
  processedAt: string;
}

export interface AdminContentImportJobQuery {
  importType?: ContentImportType;
  status?: ContentImportStatus;
  from?: string;
  to?: string;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateContentImportJobRequest {
  importType: ContentImportType;
  originalFileName: string;
  storagePath: string;
}

export interface UpdateContentImportSourceRequest {
  originalFileName: string;
  storagePath: string;
}

export const CONTENT_ENTITY_TYPE_LABELS: Record<ContentEntityType, string> = {
  [ContentEntityType.Vocabulary]: "Từ vựng",
  [ContentEntityType.VocabularyExample]: "Ví dụ từ vựng",
  [ContentEntityType.Lesson]: "Bài học",
  [ContentEntityType.LessonSection]: "Phần bài học",
  [ContentEntityType.QuizQuestion]: "Câu hỏi Quiz",
  [ContentEntityType.AudioAsset]: "Audio",
  [ContentEntityType.Course]: "Khóa học",
  [ContentEntityType.CourseChapter]: "Chương khóa học",
};

export const CONTENT_REPORT_REASON_LABELS: Record<ContentReportReason, string> = {
  [ContentReportReason.IncorrectContent]: "Nội dung không chính xác",
  [ContentReportReason.IncorrectTranslation]: "Bản dịch không chính xác",
  [ContentReportReason.IncorrectPinyin]: "Pinyin không chính xác",
  [ContentReportReason.AudioProblem]: "Vấn đề audio",
  [ContentReportReason.Typo]: "Lỗi chính tả",
  [ContentReportReason.OutdatedContent]: "Nội dung lỗi thời",
  [ContentReportReason.Other]: "Khác",
};

export const CONTENT_REPORT_STATUS_LABELS: Record<ContentReportStatus, string> = {
  [ContentReportStatus.Open]: "Mới",
  [ContentReportStatus.InReview]: "Đang xử lý",
  [ContentReportStatus.Resolved]: "Đã giải quyết",
  [ContentReportStatus.Rejected]: "Đã từ chối",
};

export const CONTENT_IMPORT_TYPE_LABELS: Record<ContentImportType, string> = {
  [ContentImportType.Vocabulary]: "Từ vựng",
  [ContentImportType.VocabularyExample]: "Ví dụ từ vựng",
  [ContentImportType.Lesson]: "Bài học",
  [ContentImportType.Quiz]: "Quiz",
};

export const CONTENT_IMPORT_STATUS_LABELS: Record<ContentImportStatus, string> = {
  [ContentImportStatus.Pending]: "Chờ xử lý",
  [ContentImportStatus.Processing]: "Đang xử lý",
  [ContentImportStatus.Completed]: "Hoàn tất",
  [ContentImportStatus.CompletedWithErrors]: "Hoàn tất có lỗi",
  [ContentImportStatus.Failed]: "Thất bại",
};
