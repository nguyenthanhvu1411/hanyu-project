import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AdminNotification,
  AdminNotificationQuery,
  BroadcastNotificationRequest,
  SendNotificationRequest,
} from "./notification.types";

const ROOT = "/admin/notifications";

function buildQuery(query: AdminNotificationQuery) {
  const params = new URLSearchParams();
  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => params.set(key, String(value)));
  return params.toString();
}

export const notificationApi = {
  list(query: AdminNotificationQuery = {}) {
    const queryString = buildQuery(query);
    return apiClient<PagedResult<AdminNotification>>(queryString ? `${ROOT}?${queryString}` : ROOT);
  },

  getById(id: number) {
    return apiClient<AdminNotification>(`${ROOT}/${id}`);
  },

  send(request: SendNotificationRequest) {
    return apiClient<AdminNotification>(ROOT, { method: "POST", body: request });
  },

  broadcast(request: BroadcastNotificationRequest) {
    return apiClient<{ sentCount: number }>(`${ROOT}/broadcast`, { method: "POST", body: request });
  },
};
