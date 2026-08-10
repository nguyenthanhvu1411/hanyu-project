"use client";

import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import { learningKeys } from "../learning.keys";
import { learningService } from "../learning.service";
import type { HskLevelListQuery } from "../learning.types";
import type {
  AdminHskLevelDto,
  CreateHskLevelRequest,
  UpdateHskLevelRequest,
} from "@/dto/learning/hsk-level.dto";
import type { PagedResult } from "@/types/api.types";

export function useHskLevels(query: HskLevelListQuery = {}): UseQueryResult<PagedResult<AdminHskLevelDto>, Error> {
  return useQuery({
    queryKey: learningKeys.hskLevelList(query),
    queryFn: () => learningService.hskLevels.list(query),
  });
}

/**
 * Backend hiện không có:
 * GET /admin/hsk-levels/{id}
 *
 * Vì vậy detail/edit dùng GET list rồi find.
 */
export function useHskLevelDetail(id?: number) {
  const query = useHskLevels({
    page: 1,
    pageSize: 100,
    sortBy: "sortOrder",
    sortDirection: "asc",
  });

  const item = query.data?.items.find((x) => x.id === id);

  return {
    ...query,
    data: item,
  } as Omit<typeof query, "data"> & { data: AdminHskLevelDto | undefined };
}

export function useCreateHskLevel() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateHskLevelRequest) =>
      learningService.hskLevels.create(request),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: learningKeys.hskLevels(),
      });
    },
  });
}

export function useUpdateHskLevel(id: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: UpdateHskLevelRequest) =>
      learningService.hskLevels.update(id, request),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: learningKeys.hskLevels(),
      });
    },
  });
}

export function useDeleteHskLevel() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => learningService.hskLevels.remove(id),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: learningKeys.hskLevels(),
      });
    },
  });
}

export function useActivateHskLevel() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => learningService.hskLevels.activate(id),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: learningKeys.hskLevels(),
      });
    },
  });
}

export function useDeactivateHskLevel() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => learningService.hskLevels.deactivate(id),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: learningKeys.hskLevels(),
      });
    },
  });
}
