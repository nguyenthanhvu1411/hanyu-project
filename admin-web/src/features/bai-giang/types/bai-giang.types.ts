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
