import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import type {
  CreateHskLevelRequest,
  UpdateHskLevelRequest,
  AdminHskLevelDto,
} from "@/dto/learning/hsk-level.dto";

export const learningApi = {
  hskLevels: {
    list: () => apiClient<AdminHskLevelDto[]>(API_ENDPOINTS.LEARNING.HSK_LEVELS),

    getById: (id: number) =>
      apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVEL(id)),

    create: (request: CreateHskLevelRequest) =>
      apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVELS, {
        method: "POST",
        body: request,
      }),

    update: (id: number, request: UpdateHskLevelRequest) =>
      apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVEL(id), {
        method: "PUT",
        body: request,
      }),

    remove: (id: number) =>
      apiClient<void>(API_ENDPOINTS.LEARNING.HSK_LEVEL(id), {
        method: "DELETE",
      }),

    restore: (id: number) =>
      apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVEL_RESTORE(id), {
        method: "POST",
      }),

    activate: (id: number) =>
      apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVEL_ACTIVATE(id), {
        method: "POST",
      }),

    deactivate: (id: number) =>
      apiClient<AdminHskLevelDto>(API_ENDPOINTS.LEARNING.HSK_LEVEL_DEACTIVATE(id), {
        method: "POST",
      }),
  },
};
