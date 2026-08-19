import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AdminQuizAttempt,
  AdminQuizAttemptDetail,
  AdminQuizAttemptQuery,
  AdminQuizAttemptStatistics,
} from "./quiz-attempts.types";

const ROOT = "/admin/quiz-attempts";

function buildQuery(query: object) {
  const params = new URLSearchParams();
  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => params.set(key, String(value)));
  return params.toString();
}

export const quizAttemptsApi = {
  list(query: AdminQuizAttemptQuery = {}) {
    const qs = buildQuery(query);
    return apiClient<PagedResult<AdminQuizAttempt>>(qs ? `${ROOT}?${qs}` : ROOT);
  },
  get(id: number) {
    return apiClient<AdminQuizAttemptDetail>(`${ROOT}/${id}`);
  },
  statistics(query: AdminQuizAttemptQuery = {}) {
    const qs = buildQuery(query);
    return apiClient<AdminQuizAttemptStatistics>(qs ? `${ROOT}/statistics?${qs}` : `${ROOT}/statistics`);
  },
};