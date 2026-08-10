import { ContentStatus } from "@/lib/constants/content-status";

export interface AdminCourseListItem {
  id: number;
  publicId: string;
  code: string;
  slug: string;
  titleVi: string;
  hskLevelId?: number | null;
  hskCode?: string | null;
  hskNameVi?: string | null;
  coverImageUrl?: string | null;
  sortOrder: number;
  estimatedMinutes?: number | null;
  status: ContentStatus;
  isActive: boolean;
  isFeatured: boolean;
  chapterCount: number;
  publishedAt?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface AdminCourseChapter {
  id: number;
  publicId: string;
  titleVi: string;
  descriptionVi?: string | null;
  sortOrder: number;
  isActive: boolean;
  lessonCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface AdminCoursePrerequisite {
  id: number;
  requiredCourseId: number;
  requiredCoursePublicId: string;
  requiredCourseCode: string;
  requiredCourseTitleVi: string;
  isRequired: boolean;
  sortOrder: number;
}

export interface AdminCourseDetail {
  id: number;
  publicId: string;
  code: string;
  slug: string;
  titleVi: string;
  shortDescriptionVi?: string | null;
  descriptionVi?: string | null;
  hskLevelId?: number | null;
  hskCode?: string | null;
  hskNameVi?: string | null;
  coverImageUrl?: string | null;
  sortOrder: number;
  estimatedMinutes?: number | null;
  status: ContentStatus;
  isActive: boolean;
  isFeatured: boolean;
  publishedAt?: string | null;
  publishedById?: string | null;
  archivedAt?: string | null;
  archivedById?: string | null;
  concurrencyToken: string;
  createdAt: string;
  createdById?: string | null;
  updatedAt: string;
  updatedById?: string | null;
  deletedAt?: string | null;
  deletedById?: string | null;
  chapters: AdminCourseChapter[];
  prerequisites: AdminCoursePrerequisite[];
}

export interface CreateCourseRequest {
  code: string;
  slug: string;
  titleVi: string;
  shortDescriptionVi?: string | null;
  descriptionVi?: string | null;
  hskLevelId?: number | null;
  coverImageUrl?: string | null;
  sortOrder: number;
  estimatedMinutes?: number | null;
  isFeatured: boolean;
}

export interface UpdateCourseRequest extends CreateCourseRequest {
  concurrencyToken: string;
}

export interface CourseWorkflowRequest {
  concurrencyToken: string;
}

export interface RejectCourseRequest extends CourseWorkflowRequest {
  reason: string;
}

export interface AdminCourseQuery {
  search?: string;
  hskLevelId?: number;
  status?: ContentStatus;
  isActive?: boolean;
  isFeatured?: boolean;
  includeDeleted?: boolean;
  sortBy?: string;
  sortDescending?: boolean;
  page?: number;
  pageSize?: number;
}
