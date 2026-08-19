import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import type { PagedResult } from "@/lib/api/api-result";
import type {
  CreateHskLevelRequest,
  UpdateHskLevelRequest,
  AdminHskLevelDto,
} from "@/dto/learning/hsk-level.dto";
import type {
  AdminLearningActivity,
  AdminLearningActivityQuery,
  AdminLearningGoal,
  AdminLearningGoalQuery,
  AdminLearningSummary,
  AdminLearningSummaryQuery,
  CreateLearningActivityRequest,
  CreateLearningGoalRequest,
  UpdateLearningActivityRequest,
  UpdateLearningGoalRequest,
} from "./learning.types";

const GOALS_ROOT = "/admin/learning/goals";
const ACTIVITIES_ROOT = "/admin/learning/activities";
const SUMMARIES_ROOT = "/admin/learning/summaries";

function buildQuery(query: object) {
  const params = new URLSearchParams();
  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => params.set(key, String(value)));
  return params.toString();
}

export const learningApi = {
  hskLevels: {
    list: () => apiClient<AdminHskLevelDto[]>(API_ENDPOINTS.LEARNING.HSK_LEVELS),
    getById: (id: number) => apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVEL(id)),
    create: (request: CreateHskLevelRequest) => apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVELS, { method: "POST", body: request }),
    update: (id: number, request: UpdateHskLevelRequest) => apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVEL(id), { method: "PUT", body: request }),
    remove: (id: number) => apiClient<void>(API_ENDPOINTS.LEARNING.HSK_LEVEL(id), { method: "DELETE" }),
    restore: (id: number) => apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVEL_RESTORE(id), { method: "POST" }),
    activate: (id: number) => apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVEL_ACTIVATE(id), { method: "POST" }),
    deactivate: (id: number) => apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVEL_DEACTIVATE(id), { method: "POST" }),
  },

  goals: {
    list(query: AdminLearningGoalQuery = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminLearningGoal>>(qs ? `${GOALS_ROOT}?${qs}` : GOALS_ROOT);
    },
    get(id: number) { return apiClient<AdminLearningGoal>(`${GOALS_ROOT}/${id}`); },
    create(request: CreateLearningGoalRequest) { return apiClient<AdminLearningGoal>(GOALS_ROOT, { method: "POST", body: request }); },
    update(id: number, request: UpdateLearningGoalRequest) { return apiClient<AdminLearningGoal>(`${GOALS_ROOT}/${id}`, { method: "PUT", body: request }); },
    remove(id: number) { return apiClient<void>(`${GOALS_ROOT}/${id}`, { method: "DELETE" }); },
  },

  activities: {
    list(query: AdminLearningActivityQuery = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminLearningActivity>>(qs ? `${ACTIVITIES_ROOT}?${qs}` : ACTIVITIES_ROOT);
    },
    get(id: number) { return apiClient<AdminLearningActivity>(`${ACTIVITIES_ROOT}/${id}`); },
    create(request: CreateLearningActivityRequest) { return apiClient<AdminLearningActivity>(ACTIVITIES_ROOT, { method: "POST", body: request }); },
    update(id: number, request: UpdateLearningActivityRequest) { return apiClient<AdminLearningActivity>(`${ACTIVITIES_ROOT}/${id}`, { method: "PUT", body: request }); },
    remove(id: number) { return apiClient<void>(`${ACTIVITIES_ROOT}/${id}`, { method: "DELETE" }); },
  },

  summaries: {
    list(query: AdminLearningSummaryQuery = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminLearningSummary>>(qs ? `${SUMMARIES_ROOT}?${qs}` : SUMMARIES_ROOT);
    },
    get(userId: string) { return apiClient<AdminLearningSummary>(`${SUMMARIES_ROOT}/${userId}`); },
    recompute(userId: string) { return apiClient<AdminLearningSummary>(`${SUMMARIES_ROOT}/${userId}/recompute`, { method: "POST" }); },
  },
};
