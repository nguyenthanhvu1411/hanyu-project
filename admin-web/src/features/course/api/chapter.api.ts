import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import type { AdminCourseChapter } from "../types/course.types";

export interface CreateCourseChapterRequest {
  titleVi: string;
  descriptionVi?: string | null;
  sortOrder: number;
  isActive: boolean;
}

export interface UpdateCourseChapterRequest extends CreateCourseChapterRequest {
  concurrencyToken: string;
}

export interface ChapterWorkflowRequest {
  concurrencyToken: string;
}

export const chapterApi = {
  list(courseId: number, includeDeleted = false) {
    return apiClient<AdminCourseChapter[]>(
      `${API_ENDPOINTS.COURSE.CHAPTERS(courseId)}?includeDeleted=${includeDeleted}`,
    );
  },
  getById(courseId: number, chapterId: number) {
    return apiClient<AdminCourseChapter>(
      API_ENDPOINTS.COURSE.CHAPTER(courseId, chapterId),
    );
  },
  create(courseId: number, request: CreateCourseChapterRequest) {
    return apiClient<AdminCourseChapter>(API_ENDPOINTS.COURSE.CHAPTERS(courseId), {
      method: "POST",
      body: request,
    });
  },
  update(courseId: number, chapterId: number, request: UpdateCourseChapterRequest) {
    return apiClient<AdminCourseChapter>(
      API_ENDPOINTS.COURSE.CHAPTER(courseId, chapterId),
      { method: "PUT", body: request },
    );
  },
  delete(courseId: number, chapterId: number, request: ChapterWorkflowRequest) {
    return apiClient<void>(API_ENDPOINTS.COURSE.CHAPTER(courseId, chapterId), {
      method: "DELETE",
      body: request,
    });
  },
  restore(courseId: number, chapterId: number, request: ChapterWorkflowRequest) {
    return apiClient<AdminCourseChapter>(
      API_ENDPOINTS.COURSE.CHAPTER_RESTORE(courseId, chapterId),
      { method: "POST", body: request },
    );
  },
};
