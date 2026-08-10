import { apiClient } from "@/lib/api/api-client";
import { AdminCourseChapter } from "@/features/khoa-hoc/types/khoa-hoc.types";

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
    return apiClient<AdminCourseChapter[]>(`${base(courseId)}?includeDeleted=${includeDeleted}`);
  },

  chiTiet(courseId: number, chapterId: number) {
    return apiClient<AdminCourseChapter>(`${base(courseId)}/${chapterId}`);
  },

  tao(courseId: number, request: CreateCourseChapterRequest) {
    return apiClient<AdminCourseChapter>(base(courseId), {
      method: "POST",
      body: request,
    });
  },

  capNhat(courseId: number, chapterId: number, request: UpdateCourseChapterRequest) {
    return apiClient<AdminCourseChapter>(`${base(courseId)}/${chapterId}`, {
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
    return apiClient<AdminCourseChapter>(`${base(courseId)}/${chapterId}/restore`, {
      method: "POST",
      body: request,
    });
  },
};
