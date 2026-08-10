import { apiClient } from "@/lib/api/api-client";
import { PagedResult } from "@/lib/api/api-result";

import {
  AdminLessonListItem,
  AdminLessonQuery,
  CreateLessonRequest,
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
    return apiClient<PagedResult<AdminLessonListItem>>(`${BASE}?${buildQuery(query)}`);
  },

  chiTiet(id: number) {
    return apiClient(`${BASE}/${id}`);
  },

  tao(request: CreateLessonRequest) {
    return apiClient(BASE, {
      method: "POST",
      body: request,
    });
  },

  capNhat(id: number, request: UpdateLessonRequest) {
    return apiClient(`${BASE}/${id}`, {
      method: "PUT",
      body: request,
    });
  },

  kiemTra(id: number) {
    return apiClient(`${BASE}/${id}/validate`);
  },

  guiDuyet(id: number, request: LessonWorkflowRequest) {
    return apiClient(`${BASE}/${id}/submit-review`, {
      method: "POST",
      body: request,
    });
  },

  duyet(id: number, request: LessonWorkflowRequest) {
    return apiClient(`${BASE}/${id}/approve`, {
      method: "POST",
      body: request,
    });
  },

  xuatBan(id: number, request: LessonWorkflowRequest) {
    return apiClient(`${BASE}/${id}/publish`, {
      method: "POST",
      body: request,
    });
  },

  luuTru(id: number, request: LessonWorkflowRequest) {
    return apiClient(`${BASE}/${id}/archive`, {
      method: "POST",
      body: request,
    });
  },
};
