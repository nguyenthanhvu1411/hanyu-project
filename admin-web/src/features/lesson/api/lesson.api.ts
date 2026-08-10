import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

import type {
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
} from "../types/lesson.types";

const BASE = "/api/v1/admin/lessons";

function buildQuery(query: AdminLessonQuery) {
  const params = new URLSearchParams();
  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => params.set(key, String(value)));
  return params.toString();
}

export const lessonApi = {
  list(query: AdminLessonQuery = {}) {
    const queryString = buildQuery(query);
    return apiClient<PagedResult<AdminLessonListItem>>(queryString ? `${BASE}?${queryString}` : BASE);
  },
  getById(id: number) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}`);
  },
  create(request: CreateLessonRequest) {
    return apiClient<AdminLessonDetail>(BASE, { method: "POST", body: request });
  },
  update(id: number, request: UpdateLessonRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}`, { method: "PUT", body: request });
  },
  validate(id: number) {
    return apiClient<LessonValidationResult>(`${BASE}/${id}/validate`);
  },
  submitReview(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/submit-review`, { method: "POST", body: request });
  },
  approve(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/approve`, { method: "POST", body: request });
  },
  publish(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/publish`, { method: "POST", body: request });
  },
  archive(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/archive`, { method: "POST", body: request });
  },
  restore(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/restore`, { method: "POST", body: request });
  },
  delete(id: number, request: LessonWorkflowRequest) {
    return apiClient<void>(`${BASE}/${id}`, { method: "DELETE", body: request });
  },
  restoreDeleted(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(`${BASE}/${id}/restore-deleted`, { method: "POST", body: request });
  },
  listSections(id: number) {
    return apiClient<AdminLessonSection[]>(`${BASE}/${id}/sections`);
  },
  createSection(id: number, request: CreateLessonSectionRequest) {
    return apiClient<AdminLessonSection>(`${BASE}/${id}/sections`, { method: "POST", body: request });
  },
  updateSection(id: number, sectionId: number, request: UpdateLessonSectionRequest) {
    return apiClient<AdminLessonSection>(`${BASE}/${id}/sections/${sectionId}`, { method: "PUT", body: request });
  },
  deleteSection(id: number, sectionId: number) {
    return apiClient<void>(`${BASE}/${id}/sections/${sectionId}`, { method: "DELETE" });
  },
  listVocabulary(id: number) {
    return apiClient<AdminLessonVocabulary[]>(`${BASE}/${id}/vocabulary`);
  },
  attachVocabulary(id: number, request: AttachLessonVocabularyRequest) {
    return apiClient<AdminLessonVocabulary>(`${BASE}/${id}/vocabulary`, { method: "POST", body: request });
  },
  updateVocabulary(id: number, vocabularyId: number, request: UpdateLessonVocabularyRequest) {
    return apiClient<AdminLessonVocabulary>(`${BASE}/${id}/vocabulary/${vocabularyId}`, { method: "PUT", body: request });
  },
  detachVocabulary(id: number, vocabularyId: number) {
    return apiClient<void>(`${BASE}/${id}/vocabulary/${vocabularyId}`, { method: "DELETE" });
  },
  listAssets(id: number) {
    return apiClient<AdminLessonAsset[]>(`${BASE}/${id}/assets`);
  },
  createAsset(id: number, request: CreateLessonAssetRequest) {
    return apiClient<AdminLessonAsset>(`${BASE}/${id}/assets`, { method: "POST", body: request });
  },
  updateAsset(id: number, assetId: number, request: UpdateLessonAssetRequest) {
    return apiClient<AdminLessonAsset>(`${BASE}/${id}/assets/${assetId}`, { method: "PUT", body: request });
  },
  deleteAsset(id: number, assetId: number) {
    return apiClient<void>(`${BASE}/${id}/assets/${assetId}`, { method: "DELETE" });
  },
  listPrerequisites(id: number) {
    return apiClient<AdminLessonPrerequisite[]>(`${BASE}/${id}/prerequisites`);
  },
  addPrerequisite(id: number, request: AddLessonPrerequisiteRequest) {
    return apiClient<AdminLessonPrerequisite>(`${BASE}/${id}/prerequisites`, { method: "POST", body: request });
  },
  removePrerequisite(id: number, requiredLessonId: number) {
    return apiClient<void>(`${BASE}/${id}/prerequisites/${requiredLessonId}`, { method: "DELETE" });
  },
};
