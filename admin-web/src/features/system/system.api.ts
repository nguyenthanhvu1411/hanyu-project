import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AdminAuditLog,
  AdminAuditLogQuery,
  AdminProductEvent,
  AdminProductEventQuery,
} from "./system.types";

const AUDIT_ROOT = "/admin/audit-logs";
const PRODUCT_EVENT_ROOT = "/admin/product-events";

function buildQuery(query: object) {
  const params = new URLSearchParams();
  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => params.set(key, String(value)));
  return params.toString();
}

export const systemApi = {
  listAuditLogs(query: AdminAuditLogQuery = {}) {
    const queryString = buildQuery(query);
    return apiClient<PagedResult<AdminAuditLog>>(queryString ? `${AUDIT_ROOT}?${queryString}` : AUDIT_ROOT);
  },

  getAuditLog(id: number) {
    return apiClient<AdminAuditLog>(`${AUDIT_ROOT}/${id}`);
  },

  listProductEvents(query: AdminProductEventQuery = {}) {
    const queryString = buildQuery(query);
    return apiClient<PagedResult<AdminProductEvent>>(
      queryString ? `${PRODUCT_EVENT_ROOT}?${queryString}` : PRODUCT_EVENT_ROOT,
    );
  },
};
