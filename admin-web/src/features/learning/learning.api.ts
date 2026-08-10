import { apiClient } from "@/lib/api/api-client";
import type {
  CreateHskLevelRequest,
  UpdateHskLevelRequest,
  AdminHskLevelDto,
} from "@/dto/learning/hsk-level.dto";
import type { HskLevelListQuery } from "./learning.types";
import type { ApiEnvelope } from "@/types/api.types";

const BASE_URL = "/api/v1/admin/hsk-levels";

export const learningApi = {
  hskLevels: {
    list: (query?: HskLevelListQuery) => {
      const searchParams = new URLSearchParams();
      if (query?.page) searchParams.append("page", query.page.toString());
      if (query?.pageSize) searchParams.append("pageSize", query.pageSize.toString());
      if (query?.q) searchParams.append("keyword", query.q);
      if (query?.isActive !== undefined) searchParams.append("isActive", query.isActive.toString());
      if (query?.sortBy) searchParams.append("sortBy", query.sortBy);

      const qs = searchParams.toString();
      return apiClient<ApiEnvelope<AdminHskLevelDto[]>>(`${BASE_URL}${qs ? `?${qs}` : ""}`);
    },
    
    create: (request: CreateHskLevelRequest) => {
      return apiClient<AdminHskLevelDto>(BASE_URL, {
        method: "POST",
        body: request,
      });
    },

    update: (id: number, request: UpdateHskLevelRequest) => {
      return apiClient<AdminHskLevelDto>(`${BASE_URL}/${id}`, {
        method: "PUT",
        body: request,
      });
    },

    remove: (id: number) => {
      return apiClient<void>(`${BASE_URL}/${id}`, {
        method: "DELETE",
      });
    },

    activate: (id: number) => {
      return apiClient<AdminHskLevelDto>(`${BASE_URL}/${id}/activate`, {
        method: "PUT",
      });
    },

    deactivate: (id: number) => {
      return apiClient<AdminHskLevelDto>(`${BASE_URL}/${id}/deactivate`, {
        method: "PUT",
      });
    },
  }
};
