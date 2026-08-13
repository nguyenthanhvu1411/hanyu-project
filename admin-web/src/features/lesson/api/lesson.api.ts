import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AddLessonPrerequisiteRequest,
  AdminLessonAsset,
  AdminLessonDetail,
  AdminLessonListItem,
  AdminLessonPrerequisite,
  AdminLessonQuery,
  AdminLessonSection,
  AdminLessonTopicOption,
  AdminLessonVocabulary,
  AdminVocabularyLookupOption,
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

interface SlugAvailabilityResponse {
  slug: string;
  available: boolean;
}

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
    const path = queryString ? `${API_ENDPOINTS.LESSON.ROOT}?${queryString}` : API_ENDPOINTS.LESSON.ROOT;
    return apiClient<PagedResult<AdminLessonListItem>>(path);
  },
  async isSlugAvailable(slug: string, excludeId?: number) {
    const normalized = slug.trim().toLowerCase();
    if (!normalized) return true;

    const result = await apiClient<SlugAvailabilityResponse>(
      API_ENDPOINTS.LESSON.SLUG_AVAILABILITY(normalized, excludeId),
    );

    return result.available;
  },
  listTopics() {
    return apiClient<AdminLessonTopicOption[]>(API_ENDPOINTS.VOCABULARY.TOPICS);
  },
  listVocabularyOptions(q = "") {
    const params = new URLSearchParams({ page: "1", pageSize: "100", sort: "simplified" });
    if (q.trim()) params.set("q", q.trim());
    return apiClient<PagedResult<AdminVocabularyLookupOption>>(`${API_ENDPOINTS.VOCABULARY.ROOT}?${params.toString()}`);
  },
  getById(id: number) {
    return apiClient<AdminLessonDetail>(API_ENDPOINTS.LESSON.DETAIL(id));
  },
  create(request: CreateLessonRequest) {
    return apiClient<AdminLessonDetail>(API_ENDPOINTS.LESSON.ROOT, { method: "POST", body: request });
  },
  update(id: number, request: UpdateLessonRequest) {
    return apiClient<AdminLessonDetail>(API_ENDPOINTS.LESSON.DETAIL(id), { method: "PUT", body: request });
  },
  validate(id: number) {
    return apiClient<LessonValidationResult>(API_ENDPOINTS.LESSON.VALIDATE(id));
  },
  submitReview(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(API_ENDPOINTS.LESSON.SUBMIT_REVIEW(id), { method: "POST", body: request });
  },
  approve(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(API_ENDPOINTS.LESSON.APPROVE(id), { method: "POST", body: request });
  },
  publish(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(API_ENDPOINTS.LESSON.PUBLISH(id), { method: "POST", body: request });
  },
  archive(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(API_ENDPOINTS.LESSON.ARCHIVE(id), { method: "POST", body: request });
  },
  restore(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(API_ENDPOINTS.LESSON.RESTORE(id), { method: "POST", body: request });
  },
  delete(id: number, request: LessonWorkflowRequest) {
    return apiClient<void>(API_ENDPOINTS.LESSON.DETAIL(id), { method: "DELETE", body: request });
  },
  restoreDeleted(id: number, request: LessonWorkflowRequest) {
    return apiClient<AdminLessonDetail>(API_ENDPOINTS.LESSON.RESTORE_DELETED(id), { method: "POST", body: request });
  },

  listSections(id: number) {
    return apiClient<AdminLessonSection[]>(API_ENDPOINTS.LESSON.SECTIONS(id));
  },
  createSection(id: number, request: CreateLessonSectionRequest) {
    return apiClient<AdminLessonSection>(API_ENDPOINTS.LESSON.SECTIONS(id), { method: "POST", body: request });
  },
  updateSection(id: number, sectionId: number, request: UpdateLessonSectionRequest) {
    return apiClient<AdminLessonSection>(API_ENDPOINTS.LESSON.SECTION(id, sectionId), { method: "PUT", body: request });
  },
  deleteSection(id: number, sectionId: number) {
    return apiClient<void>(API_ENDPOINTS.LESSON.SECTION(id, sectionId), { method: "DELETE" });
  },

  listVocabulary(id: number) {
    return apiClient<AdminLessonVocabulary[]>(API_ENDPOINTS.LESSON.VOCABULARY(id));
  },
  attachVocabulary(id: number, request: AttachLessonVocabularyRequest) {
    return apiClient<AdminLessonVocabulary>(API_ENDPOINTS.LESSON.VOCABULARY(id), { method: "POST", body: request });
  },
  updateVocabulary(id: number, vocabularyId: number, request: UpdateLessonVocabularyRequest) {
    return apiClient<AdminLessonVocabulary>(API_ENDPOINTS.LESSON.VOCABULARY_ITEM(id, vocabularyId), { method: "PUT", body: request });
  },
  detachVocabulary(id: number, vocabularyId: number) {
    return apiClient<void>(API_ENDPOINTS.LESSON.VOCABULARY_ITEM(id, vocabularyId), { method: "DELETE" });
  },

  listAssets(id: number) {
    return apiClient<AdminLessonAsset[]>(API_ENDPOINTS.LESSON.ASSETS(id));
  },
  createAsset(id: number, request: CreateLessonAssetRequest) {
    return apiClient<AdminLessonAsset>(API_ENDPOINTS.LESSON.ASSETS(id), { method: "POST", body: request });
  },
  updateAsset(id: number, assetId: number, request: UpdateLessonAssetRequest) {
    return apiClient<AdminLessonAsset>(API_ENDPOINTS.LESSON.ASSET(id, assetId), { method: "PUT", body: request });
  },
  deleteAsset(id: number, assetId: number) {
    return apiClient<void>(API_ENDPOINTS.LESSON.ASSET(id, assetId), { method: "DELETE" });
  },

  listPrerequisites(id: number) {
    return apiClient<AdminLessonPrerequisite[]>(API_ENDPOINTS.LESSON.PREREQUISITES(id));
  },
  addPrerequisite(id: number, request: AddLessonPrerequisiteRequest) {
    return apiClient<AdminLessonPrerequisite>(API_ENDPOINTS.LESSON.PREREQUISITES(id), { method: "POST", body: request });
  },
  removePrerequisite(id: number, requiredLessonId: number) {
    return apiClient<void>(API_ENDPOINTS.LESSON.PREREQUISITE(id, requiredLessonId), { method: "DELETE" });
  },
};
