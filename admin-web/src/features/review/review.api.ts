import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AdminFlashcardSession,
  AdminFlashcardSessionDetail,
  AdminFlashcardSessionQuery,
  AdminReviewDashboard,
  AdminReviewEvent,
  AdminReviewEventQuery,
  AdminUserReviewSummary,
  AdminVocabularyState,
  AdminVocabularyStateQuery,
} from "./review.types";

function buildQuery(query: object) {
  const params = new URLSearchParams();
  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => params.set(key, String(value)));
  return params.toString();
}

export const reviewApi = {
  dashboard() {
    return apiClient<AdminReviewDashboard>("/admin/review-dashboard");
  },

  states: {
    list(query: AdminVocabularyStateQuery = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminVocabularyState>>(qs ? `/admin/review-states?${qs}` : "/admin/review-states");
    },
    get(userId: string, vocabularyId: number) {
      return apiClient<AdminVocabularyState>(`/admin/review-states/${userId}/${vocabularyId}`);
    },
    reset(userId: string, vocabularyId: number, reason: string) {
      return apiClient<void>(`/admin/review-states/${userId}/${vocabularyId}/reset`, {
        method: "POST",
        body: { reason },
      });
    },
  },

  sessions: {
    list(query: AdminFlashcardSessionQuery = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminFlashcardSession>>(qs ? `/admin/flashcard-sessions?${qs}` : "/admin/flashcard-sessions");
    },
    get(id: number) {
      return apiClient<AdminFlashcardSessionDetail>(`/admin/flashcard-sessions/${id}`);
    },
    abandon(id: number) {
      return apiClient<void>(`/admin/flashcard-sessions/${id}/abandon`, { method: "POST" });
    },
  },

  events: {
    list(query: AdminReviewEventQuery = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminReviewEvent>>(qs ? `/admin/review-events?${qs}` : "/admin/review-events");
    },
    get(id: number) {
      return apiClient<AdminReviewEvent>(`/admin/review-events/${id}`);
    },
  },

  userSummary(userId: string) {
    return apiClient<AdminUserReviewSummary>(`/admin/users/${userId}/review-summary`);
  },
};
