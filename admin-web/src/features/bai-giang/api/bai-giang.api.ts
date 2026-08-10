import { apiClient } from "@/lib/api/api-client";
import { PagedResult } from "@/lib/api/api-result";

import {
  AddLessonPrerequisiteRequest,
  AdminLessonAsset,
  AdminLessonDetail,
  AdminLessonListItem,
  AdminLessonPrerequisite,
  AdminLessonQuery,
  AdminLessonSection,
  AdminLessonVocabulary,
  AttachLessonVocabularyRequest,
  CreateLessonAssetRequest,
  CreateLessonRequest,
  CreateLessonSectionRequest,
  LessonValidationResult,
  LessonWorkflowRequest,
  UpdateLessonAssetRequest,
  UpdateLessonRequest,
  UpdateLessonSectionRequest,
  UpdateLessonVocabularyRequest,
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
    return apiClient<AdminLessonDetail>(BASE, { method: "POST", body: request });
  },

  capNhat(id: number, request: UpdateLessonRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}`, { method: "PUT", body: request });
  },

  kiemTra(id: number) {
    return apiClient<LessonValidationResult>(`${BASE}/${id}/validate`);
  },

  guiDuyet(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/submit-review`, { method: "POST", body: request });
  },

  duyet(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/approve`, { method: "POST", body: request });
  },

  xuatBan(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/publish`, { method: "POST", body: request });
  },

  luuTru(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/archive`, { method: "POST", body: request });
  },

  khoiPhucLuuTru(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/restore`, { method: "POST", body: request });
  },

  xoa(id: number, request: LessonWorkflowRequest) {
    return apiClient<void>(`${BASE}/${id}`, { method: "DELETE", body: request });
  },

  khoiPhucDaXoa(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/restore-deleted`, { method: "POST", body: request });
  },

  danhSachPhan(id: number) {
    return apiClient<AdminLessonSection[]>(`${BASE}/${id}/sections`);
  },

  taoPhan(id: number, request: CreateLessonSectionRequest) {
    return apiClient<AdminLessonSection>(`${BASE}/${id}/sections`, { method: "POST", body: request });
  },

  capNhatPhan(id: number, sectionId: number, request: UpdateLessonSectionRequest) {
    return apiClient<AdminLessonSection>(`${BASE}/${id}/sections/${sectionId}`, { method: "PUT", body: request });
  },

  xoaPhan(id: number, sectionId: number) {
    return apiClient<void>(`${BASE}/${id}/sections/${sectionId}`, { method: "DELETE" });
  },

  danhSachTuVung(id: number) {
    return apiClient<AdminLessonVocabulary[]>(`${BASE}/${id}/vocabulary`);
  },

  ganTuVung(id: number, request: AttachLessonVocabularyRequest) {
    return apiClient<AdminLessonVocabulary>(`${BASE}/${id}/vocabulary`, { method: "POST", body: request });
  },

  capNhatTuVung(id: number, vocabularyId: number, request: UpdateLessonVocabularyRequest) {
    return apiClient<AdminLessonVocabulary>(`${BASE}/${id}/vocabulary/${vocabularyId}`, { method: "PUT", body: request });
  },

  goTuVung(id: number, vocabularyId: number) {
    return apiClient<void>(`${BASE}/${id}/vocabulary/${vocabularyId}`, { method: "DELETE" });
  },

  danhSachTaiNguyen(id: number) {
    return apiClient<AdminLessonAsset[]>(`${BASE}/${id}/assets`);
  },

  taoTaiNguyen(id: number, request: CreateLessonAssetRequest) {
    return apiClient<AdminLessonAsset>(`${BASE}/${id}/assets`, { method: "POST", body: request });
  },

  capNhatTaiNguyen(id: number, assetId: number, request: UpdateLessonAssetRequest) {
    return apiClient<AdminLessonAsset>(`${BASE}/${id}/assets/${assetId}`, { method: "PUT", body: request });
  },

  xoaTaiNguyen(id: number, assetId: number) {
    return apiClient<void>(`${BASE}/${id}/assets/${assetId}`, { method: "DELETE" });
  },

  danhSachTienQuyet(id: number) {
    return apiClient<AdminLessonPrerequisite[]>(`${BASE}/${id}/prerequisites`);
  },

  themTienQuyet(id: number, request: AddLessonPrerequisiteRequest) {
    return apiClient<AdminLessonPrerequisite>(`${BASE}/${id}/prerequisites`, { method: "POST", body: request });
  },

  xoaTienQuyet(id: number, requiredLessonId: number) {
    return apiClient<void>(`${BASE}/${id}/prerequisites/${requiredLessonId}`, { method: "DELETE" });
  },
};
