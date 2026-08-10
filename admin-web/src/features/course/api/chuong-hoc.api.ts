import { apiClient } from "@/lib/api/api-client";
import type { CourseChapter } from "@/features/course/types/curriculum.types";

export interface CreateCourseChapterRequest {
  titleVi: string;
  descriptionVi?: string | null;
  sortOrder: number;
  isActive: boolean;
}

export interface UpdateCourseChapterRequest extends CreateCourseChapterRequest {
  concurrencyToken: string;
}

export interface EntityWorkflowRequest {
  concurrencyToken: string;
}

function base(courseId: number) {
  return `/api/v1/admin/courses/${courseId}/chapters`;
}

export const chuongHocApi = {
  danhSach(courseId: number, includeDeleted = false) {
    return apiClient<CourseChapter[]>(`${base(courseId)}?includeDeleted=${includeDeleted}`);
  },

  chiTiet(courseId: number, chapterId: number) {
    return apiClient<CourseChapter>(`${base(courseId)}/${chapterId}`);
  },

  tao(courseId: number, request: CreateCourseChapterRequest) {
    return apiClient<CourseChapter>(base(courseId), {
      method: "POST",
      body: request,
    });
  },

  capNhat(courseId: number, chapterId: number, request: UpdateCourseChapterRequest) {
    return apiClient<CourseChapter>(`${base(courseId)}/${chapterId}`, {
      method: "PUT",
      body: request,
    });
  },

  xoa(courseId: number, chapterId: number, request: EntityWorkflowRequest) {
    return apiClient<void>(`${base(courseId)}/${chapterId}`, {
      method: "DELETE",
      body: request,
    });
  },

  khoiPhuc(courseId: number, chapterId: number, request: EntityWorkflowRequest) {
    return apiClient<CourseChapter>(`${base(courseId)}/${chapterId}/restore`, {
      method: "POST",
      body: request,
    });
  },
};
