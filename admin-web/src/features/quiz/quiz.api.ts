import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AdminQuiz,
  AdminQuizQuery,
  CreateQuizRequest,
  UpdateQuizRequest,
} from "./quiz.types";

const ROOT = "/admin/quizzes";

function buildQuery(query: AdminQuizQuery) {
  const params = new URLSearchParams();
  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => params.set(key, String(value)));
  return params.toString();
}

export const quizApi = {
  list(query: AdminQuizQuery = {}) {
    const queryString = buildQuery(query);
    return apiClient<PagedResult<AdminQuiz>>(queryString ? `${ROOT}?${queryString}` : ROOT);
  },

  getById(id: number) {
    return apiClient<AdminQuiz>(`${ROOT}/${id}`);
  },

  create(request: CreateQuizRequest) {
    return apiClient<AdminQuiz>(ROOT, { method: "POST", body: request });
  },

  update(id: number, request: UpdateQuizRequest) {
    return apiClient<AdminQuiz>(`${ROOT}/${id}`, { method: "PUT", body: request });
  },

  submitReview(id: number) {
    return apiClient<void>(`${ROOT}/${id}/submit-review`, { method: "POST" });
  },

  approve(id: number) {
    return apiClient<void>(`${ROOT}/${id}/approve`, { method: "POST" });
  },

  publish(id: number) {
    return apiClient<void>(`${ROOT}/${id}/publish`, { method: "POST" });
  },

  archive(id: number) {
    return apiClient<void>(`${ROOT}/${id}/archive`, { method: "POST" });
  },

  restore(id: number) {
    return apiClient<void>(`${ROOT}/${id}/restore`, { method: "POST" });
  },

  delete(id: number) {
    return apiClient<void>(`${ROOT}/${id}`, { method: "DELETE" });
  },
};
