import { ContentStatus } from "@/lib/constants/content-status";

export interface CourseChapterLesson {
  id: number;
  publicId: string;

  courseChapterId?: number | null;

  slug: string;
  titleVi: string;

  sortOrder: number;

  estimatedMinutes: number;
  difficulty: number;

  status: ContentStatus;
  version: number;

  publishedAt?: string | null;

  createdAt: string;
  updatedAt: string;
}

export interface CourseChapter {
  id: number;
  publicId: string;

  courseId: number;

  titleVi: string;
  descriptionVi?: string | null;

  sortOrder: number;
  isActive: boolean;

  lessonCount: number;

  concurrencyToken: string;

  createdAt: string;
  updatedAt: string;

  deletedAt?: string | null;
}

export interface CreateChapterRequest {
  titleVi: string;
  descriptionVi?: string | null;

  sortOrder: number;

  isActive: boolean;
}

export interface UpdateChapterRequest
  extends CreateChapterRequest {
  concurrencyToken: string;
}

export interface AssignLessonRequest {
  lessonId: number;
  sortOrder: number;
}

export interface MoveLessonRequest {
  targetChapterId: number;
  sortOrder: number;
}

export interface ReorderLessonItem {
  lessonId: number;
  sortOrder: number;
}

export interface ReorderLessonsRequest {
  items: ReorderLessonItem[];
}

export interface ReorderChapterItem {
  chapterId: number;
  sortOrder: number;
}

export interface ReorderChaptersRequest {
  items: ReorderChapterItem[];
}

export interface CoursePrerequisite {
  id: number;
  publicId: string;

  courseId: number;

  requiredCourseId: number;
  requiredCoursePublicId: string;

  requiredCourseCode: string;
  requiredCourseSlug?: string;

  requiredCourseTitleVi: string;

  isRequired: boolean;

  sortOrder: number;

  concurrencyToken: string;

  deletedAt?: string | null;
}

export interface CreatePrerequisiteRequest {
  requiredCourseId: number;

  isRequired: boolean;

  sortOrder: number;
}

export interface UpdatePrerequisiteRequest
  extends CreatePrerequisiteRequest {
  concurrencyToken: string;
}

export interface EntityWorkflowRequest {
  concurrencyToken: string;
}

export interface CourseValidationIssue {
  code: string;
  message: string;
  field?: string | null;
}

export interface CourseValidationResult {
  isValid: boolean;
  issues: CourseValidationIssue[];
}
