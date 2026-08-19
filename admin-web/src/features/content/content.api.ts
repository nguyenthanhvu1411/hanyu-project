import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AdminContentImportJob,
  AdminContentImportJobQuery,
  AdminContentImportRow,
  AdminContentReport,
  AdminContentReportQuery,
  CreateContentImportJobRequest,
  UpdateContentImportSourceRequest,
} from "./content.types";

function buildQuery(query: object) {
  const params = new URLSearchParams();
  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => params.set(key, String(value)));
  return params.toString();
}

export const contentApi = {
  reports: {
    list(query: AdminContentReportQuery = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminContentReport>>(qs ? `/admin/content-reports?${qs}` : "/admin/content-reports");
    },
    get(id: number) { return apiClient<AdminContentReport>(`/admin/content-reports/${id}`); },
    startReview(id: number) { return apiClient<AdminContentReport>(`/admin/content-reports/${id}/start-review`, { method: "POST" }); },
    resolve(id: number, resolutionNote?: string) { return apiClient<AdminContentReport>(`/admin/content-reports/${id}/resolve`, { method: "POST", body: { resolutionNote: resolutionNote || null } }); },
    reject(id: number, resolutionNote?: string) { return apiClient<AdminContentReport>(`/admin/content-reports/${id}/reject`, { method: "POST", body: { resolutionNote: resolutionNote || null } }); },
    reopen(id: number) { return apiClient<AdminContentReport>(`/admin/content-reports/${id}/reopen`, { method: "POST" }); },
  },

  imports: {
    list(query: AdminContentImportJobQuery = {}) {
      const qs = buildQuery(query);
      return apiClient<PagedResult<AdminContentImportJob>>(qs ? `/admin/content-imports?${qs}` : "/admin/content-imports");
    },
    get(id: number) { return apiClient<AdminContentImportJob>(`/admin/content-imports/${id}`); },
    create(request: CreateContentImportJobRequest) { return apiClient<AdminContentImportJob>("/admin/content-imports", { method: "POST", body: request }); },
    updateSource(id: number, request: UpdateContentImportSourceRequest) { return apiClient<AdminContentImportJob>(`/admin/content-imports/${id}/source`, { method: "PUT", body: request }); },
    remove(id: number) { return apiClient<void>(`/admin/content-imports/${id}`, { method: "DELETE" }); },
    rows(id: number) { return apiClient<AdminContentImportRow[]>(`/admin/content-imports/${id}/rows`); },
  },
};
