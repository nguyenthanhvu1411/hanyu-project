import { ContentStatus } from "@/lib/constants/content-status";

export interface AdminLessonListItem {
  id: number;
  publicId: string;
  courseId?: number | null;
  courseTitleVi?: string | null;
  chapterId?: number | null;
  chapterTitleVi?: string | null;
  hskLevelId: number;
  hskCode?: string | null;
  topicId?: number | null;
  slug: string;
  titleVi: string;
  shortDescriptionVi?: string | null;
  sortOrder: number;
  estimatedMinutes: number;
  difficulty: number;
  isFeatured: boolean;
  status: ContentStatus;
  version: number;
  publishedAt?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface AdminLessonDetail {
  id: number;
  publicId: string;
  courseId?: number | null;
  coursePublicId?: string | null;
  courseTitleVi?: string | null;
  courseChapterId?: number | null;
  courseChapterPublicId?: string | null;
  courseChapterTitleVi?: string | null;
  hskLevelId: number;
  hskCode?: string | null;
  hskNameVi?: string | null;
  topicId?: number | null;
  topicNameVi?: string | null;
  slug: string;
  titleVi: string;
  shortDescriptionVi?: string | null;
  descriptionVi?: string | null;
  objectiveVi?: string | null;
  coverImageUrl?: string | null;
  sortOrder: number;
  estimatedMinutes: number;
  difficulty: number;
  isFeatured: boolean;
  status: ContentStatus;
  version: number;
  publishedAt?: string | null;
  sectionCount: number;
  vocabularyCount: number;
  assetCount: number;
  prerequisiteCount: number;
  createdAt: string;
  createdById?: string | null;
  updatedAt: string;
  updatedById?: string | null;
  deletedAt?: string | null;
  deletedById?: string | null;
}

export interface CreateLessonRequest {
  courseChapterId?: number | null;
  hskLevelId: number;
  topicId?: number | null;
  slug: string;
  titleVi: string;
  shortDescriptionVi?: string | null;
  descriptionVi?: string | null;
  objectiveVi?: string | null;
  coverImageUrl?: string | null;
  sortOrder: number;
  estimatedMinutes: number;
  difficulty: number;
  isFeatured: boolean;
}

export interface UpdateLessonRequest extends CreateLessonRequest {
  version: number;
}

export interface LessonWorkflowRequest {
  version: number;
}

export interface LessonValidationResult {
  isValid: boolean;
  errors: string[];
  warnings: string[];
}

export interface AdminLessonQuery {
  search?: string;
  courseId?: number;
  chapterId?: number;
  hskLevelId?: number;
  topicId?: number;
  status?: ContentStatus;
  isFeatured?: boolean;
  includeDeleted?: boolean;
  sortBy?: string;
  sortDescending?: boolean;
  page?: number;
  pageSize?: number;
}

export enum LessonSectionType {
  Introduction = 0,
  Vocabulary = 1,
  Explanation = 2,
  Example = 3,
  Grammar = 4,
  Note = 5,
  Practice = 6,
  Summary = 7,
}

export const lessonSectionTypeLabels: Record<LessonSectionType, string> = {
  [LessonSectionType.Introduction]: "Giới thiệu",
  [LessonSectionType.Vocabulary]: "Từ vựng",
  [LessonSectionType.Explanation]: "Giải thích",
  [LessonSectionType.Example]: "Ví dụ",
  [LessonSectionType.Grammar]: "Ngữ pháp",
  [LessonSectionType.Note]: "Ghi chú",
  [LessonSectionType.Practice]: "Luyện tập",
  [LessonSectionType.Summary]: "Tổng kết",
};

export enum LessonAssetType {
  Image = 0,
  Audio = 1,
  Document = 2,
}

export const lessonAssetTypeLabels: Record<LessonAssetType, string> = {
  [LessonAssetType.Image]: "Hình ảnh",
  [LessonAssetType.Audio]: "Âm thanh",
  [LessonAssetType.Document]: "Tài liệu",
};

export interface AdminLessonSection {
  id: number;
  publicId: string;
  lessonId: number;
  sectionType: LessonSectionType;
  titleVi?: string | null;
  contentVi?: string | null;
  sortOrder: number;
  isRequired: boolean;
  estimatedSeconds?: number | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateLessonSectionRequest {
  sectionType: LessonSectionType;
  sortOrder: number;
  titleVi?: string | null;
  contentVi?: string | null;
  isRequired: boolean;
  estimatedSeconds?: number | null;
}

export type UpdateLessonSectionRequest = CreateLessonSectionRequest;

export interface AdminLessonVocabulary {
  vocabularyId: number;
  vocabularyPublicId: string;
  simplified: string;
  traditional?: string | null;
  pinyin: string;
  primaryMeaningVi: string;
  sortOrder: number;
  isRequired: boolean;
}

export interface AttachLessonVocabularyRequest {
  vocabularyId: number;
  sortOrder: number;
  isRequired: boolean;
}

export interface UpdateLessonVocabularyRequest {
  sortOrder: number;
  isRequired: boolean;
}

export interface AdminLessonAsset {
  id: number;
  publicId: string;
  lessonId: number;
  audioAssetId?: number | null;
  assetType: LessonAssetType;
  url?: string | null;
  captionVi?: string | null;
  sortOrder: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateLessonAssetRequest {
  assetType: LessonAssetType;
  url?: string | null;
  captionVi?: string | null;
  audioAssetId?: number | null;
  sortOrder: number;
}

export interface UpdateLessonAssetRequest {
  url?: string | null;
  captionVi?: string | null;
  audioAssetId?: number | null;
  sortOrder: number;
}

export interface AdminLessonPrerequisite {
  requiredLessonId: number;
  requiredLessonPublicId: string;
  slug: string;
  titleVi: string;
}

export interface AddLessonPrerequisiteRequest {
  requiredLessonId: number;
}
