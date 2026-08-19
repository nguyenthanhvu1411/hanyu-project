import { learningApi } from "./learning.api";
import type {
  AdminHskLevelDto,
  CreateHskLevelRequest,
  UpdateHskLevelRequest,
} from "@/dto/learning/hsk-level.dto";
import type { PagedResult } from "@/types/api.types";
import type { HskLevelListQuery } from "./learning.types";

export const learningService = {
  hskLevels: {
    async list(query: HskLevelListQuery = {}): Promise<PagedResult<AdminHskLevelDto>> {
      const items = await learningApi.hskLevels.list();
      const keyword = query.q?.trim().toLowerCase();

      const filtered = items
        .filter((item) => {
          if (query.isActive !== undefined && item.isActive !== query.isActive) {
            return false;
          }

          if (!keyword) {
            return true;
          }

          return (
            item.code.toLowerCase().includes(keyword) ||
            item.nameVi.toLowerCase().includes(keyword)
          );
        })
        .sort((a, b) => {
          const direction = query.sortDirection === "desc" ? -1 : 1;

          switch (query.sortBy) {
            case "code":
              return a.code.localeCompare(b.code) * direction;
            case "nameVi":
              return a.nameVi.localeCompare(b.nameVi) * direction;
            case "sortOrder":
            default:
              return (a.sortOrder - b.sortOrder) * direction;
          }
        });

      const page = Math.max(1, query.page ?? 1);
      const pageSize = Math.max(1, query.pageSize ?? 20);
      const total = filtered.length;
      const totalPages = Math.max(1, Math.ceil(total / pageSize));
      const start = (page - 1) * pageSize;

      return {
        items: filtered.slice(start, start + pageSize),
        page,
        pageSize,
        total,
        totalPages,
        hasNext: page < totalPages,
      };
    },

    create(request: CreateHskLevelRequest) {
      return learningApi.hskLevels.create(request);
    },

    update(id: number, request: UpdateHskLevelRequest) {
      return learningApi.hskLevels.update(id, request);
    },

    async remove(id: number) {
      await learningApi.hskLevels.remove(id);
    },

    activate(id: number) {
      return learningApi.hskLevels.activate(id);
    },

    deactivate(id: number) {
      return learningApi.hskLevels.deactivate(id);
    },
  },
};
