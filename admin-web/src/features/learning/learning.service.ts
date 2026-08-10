import { learningApi } from "./learning.api";
import { mapPagedResult } from "./learning.mapper";
import type {
  AdminHskLevelDto,
  CreateHskLevelRequest,
  UpdateHskLevelRequest,
} from "@/dto/learning/hsk-level.dto";
import type { PagedResult } from "@/types/api.types";
import type { HskLevelListQuery } from "./learning.types";

export const learningService = {
  hskLevels: {
    async list(query?: HskLevelListQuery): Promise<PagedResult<AdminHskLevelDto>> {
      const response = await learningApi.hskLevels.list(query);
      return mapPagedResult<AdminHskLevelDto>(response);
    },

    async create(request: CreateHskLevelRequest) {
      const response = await learningApi.hskLevels.create(request);
      return response;
    },

    async update(id: number, request: UpdateHskLevelRequest) {
      const response = await learningApi.hskLevels.update(id, request);
      return response;
    },

    async remove(id: number) {
      await learningApi.hskLevels.remove(id);
    },

    async activate(id: number) {
      const response = await learningApi.hskLevels.activate(id);
      return response;
    },

    async deactivate(id: number) {
      const response = await learningApi.hskLevels.deactivate(id);
      return response;
    },
  },
};
