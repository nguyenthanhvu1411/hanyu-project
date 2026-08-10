import { apiClient } from "@/lib/api/api-client";
import type {
  CreateHskLevelRequest,
  UpdateHskLevelRequest,
  AdminHskLevelDto,
} from "@/dto/learning/hsk-level.dto";

const BASE_URL = "/api/v1/admin/hsk-levels";

export const learningApi = {
  hskLevels: {
    list: () => apiClient<AdminHskLevelDto[]>(BASE_URL),

    create: (request: CreateHskLevelRequest) =>
      apiClient<AdminHskLevelDto>(BASE_URL, {
        method: "POST",
        body: request,
      }),

    update: (id: number, request: UpdateHskLevelRequest) =>
      apiClient<AdminHskLevelDto>(`${BASE_URL}/${id}`, {
        method: "PUT",
        body: request,
      }),

    remove: (id: number) =>
      apiClient<void>(`${BASE_URL}/${id}`, {
        method: "DELETE",
      }),

    activate: (id: number) =>
      apiClient<AdminHskLevelDto>(`${BASE_URL}/${id}/activate`, {
        method: "POST",
      }),

    deactivate: (id: number) =>
      apiClient<AdminHskLevelDto>(`${BASE_URL}/${id}/deactivate`, {
        method: "POST",
      }),
  },
};
