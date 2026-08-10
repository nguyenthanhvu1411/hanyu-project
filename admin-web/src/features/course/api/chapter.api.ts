import { apiClient } from "@/lib/api/api-client";
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

function base(courseId: number) {
  return `/api/v1/admin/courses/${courseId}/chapters`;
}

export const chapterApi = {
  list(courseId: number, includeDeleted = false) {
    return apiClient<AdminCourseChapter[]>(
      `${base(courseId)}?includeDeleted=${includeDeleted}`,
    );
  },

  getById(courseId: number, chapterId: number) {
    return apiClient<AdminCourseChapter>(`${base(courseId)}/${chapterId}`);
  },

  create(courseId: number, request: CreateCourseChapterRequest) {
    return apiClient<AdminCourseChapter>(base(courseId), {
      method: "POST",
      body: request,
    });
  },

  update(
    courseId: number,
    chapterId: number,
    request: UpdateCourseChapterRequest,
  ) {
    return apiClient<AdminCourseChapter>(`${base(courseId)}/${chapterId}`, {
      method: "PUT",
      body: request,
    });
  },

  delete(courseId: number, chapterId: number, request: ChapterWorkflowRequest) {
    return apiClient<void>(`${base(courseId)}/${chapterId}`, {
      method: "DELETE",
      body: request,
    });
  },

  restore(courseId: number, chapterId: number, request: ChapterWorkflowRequest) {
    return apiClient<AdminCourseChapter>(`${base(courseId)}/${chapterId}/restore`, {
      method: "POST",
      body: request,
    });
  },
};
