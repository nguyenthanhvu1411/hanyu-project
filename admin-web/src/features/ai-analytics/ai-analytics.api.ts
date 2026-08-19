import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AdminAiCacheEntry,
  AdminAiConversation,
  AdminAiDashboard,
  AdminAiFeedback,
  AdminAiRequest,
  AdminAnalyticsDashboard,
  AdminDailyLearningStat,
  UserAnalyticsSummary,
} from "./ai-analytics.types";

function buildQuery(query: object) {
  const params = new URLSearchParams();
  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => params.set(key, String(value)));
  return params.toString();
}

export const aiAnalyticsApi = {
  ai: {
    dashboard: () => apiClient<AdminAiDashboard>("/admin/ai/dashboard"),
    requests(query: { userId?: string; featureType?: number; status?: number; provider?: string; model?: string; page?: number; pageSize?: number } = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminAiRequest>>(qs ? `/admin/ai/requests?${qs}` : "/admin/ai/requests");
    },
    request(id: number) { return apiClient<AdminAiRequest>(`/admin/ai/requests/${id}`); },
    cancelRequest(id: number) { return apiClient<AdminAiRequest>(`/admin/ai/requests/${id}/cancel`, { method: "POST" }); },
    conversations(query: { userId?: string; status?: number; page?: number; pageSize?: number } = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminAiConversation>>(qs ? `/admin/ai/conversations?${qs}` : "/admin/ai/conversations");
    },
    conversation(id: number) { return apiClient<AdminAiConversation>(`/admin/ai/conversations/${id}`); },
    feedback(query: { userId?: string; rating?: number; issueType?: string; page?: number; pageSize?: number } = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminAiFeedback>>(qs ? `/admin/ai/feedback?${qs}` : "/admin/ai/feedback");
    },
    cache(query: { featureType?: number; model?: string; expired?: boolean; page?: number; pageSize?: number } = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminAiCacheEntry>>(qs ? `/admin/ai/cache?${qs}` : "/admin/ai/cache");
    },
    deleteCache(id: number) { return apiClient<void>(`/admin/ai/cache/${id}`, { method: "DELETE" }); },
    deleteExpiredCache() { return apiClient<void>("/admin/ai/cache/expired", { method: "DELETE" }); },
  },

  analytics: {
    dashboard: () => apiClient<AdminAnalyticsDashboard>("/admin/analytics/dashboard"),
    daily(query: { userId?: string; from?: string; to?: string; page?: number; pageSize?: number } = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminDailyLearningStat>>(qs ? `/admin/analytics/daily?${qs}` : "/admin/analytics/daily");
    },
    user(userId: string) { return apiClient<UserAnalyticsSummary>(`/admin/users/${userId}/analytics`); },
  },
};
