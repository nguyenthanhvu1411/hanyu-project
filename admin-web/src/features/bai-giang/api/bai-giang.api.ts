import { apiClient } from "@/lib/api/api-client";
import { PagedResult } from "@/lib/api/api-result";

import {
  AdminLessonDetail,
  AdminLessonListItem,
  AdminLessonQuery,
  CreateLessonRequest,
  LessonValidationResult,
  LessonWorkflowRequest,
  UpdateLessonRequest,
} from "../types/bai-giang.types";

const BASE = "/api/v1/admin/lessons";

function buildQuery(query: AdminLessonQuery) {
  const params = new URLSearchParams();

  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => {
      params.set(key, String(value));
    });

  return params.toString();
}

export const baiGiangApi = {
  danhSach(query: AdminLessonQuery = {}) {
    const queryString = buildQuery(query);
    return apiClient<PagedResult<AdminLessonListItem>>(
      queryString ? `${BASE}?${queryString}` : BASE,
    );
  },

  chiTiet(id: number) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}`);
  },

  tao(request: CreateLessonRequest) {
    return apiClient<AdminLessonDetail>(BASE, {
      method: "POST",
      body: request,
    });
  },

  capNhat(id: number, request: UpdateLessonRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}`, {
      method: "PUT",
      body: request,
    });
  },

  kiemTra(id: number) {
    return apiClient<LessonValidationResult>(`${BASE}/${id}/validate`);
  },

  guiDuyet(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/submit-review`, {
      method: "POST",
      body: request,
    });
  },

  duyet(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/approve`, {
      method: "POST",
      body: request,
    });
  },

  xuatBan(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/publish`, {
      method: "POST",
      body: request,
    });
  },

  luuTru(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/archive`, {
      method: "POST",
      body: request,
    });
  },

  khoiPhucLuuTru(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/restore`, {
      method: "POST",
      body: request,
    });
  },

  xoa(id: number, request: LessonWorkflowRequest) {
    return apiClient<void>(`${BASE}/${id}`, {
      method: "DELETE",
      body: request,
    });
  },

  khoiPhucDaXoa(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/restore-deleted`, {
      method: "POST",
      body: request,
    });
  },
};
