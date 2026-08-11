"use client";

import { useMutation, useQuery, useQueryClient, type UseQueryResult } from "@tanstack/react-query";
import { learningKeys } from "../learning.keys";
import { learningService } from "../learning.service";
import { learningApi } from "../learning.api";
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

export function useHskLevelDetail(id?: number) {
  return useQuery({
    queryKey: ["learning", "hsk-levels", "detail", id],
    queryFn: () => learningApi.hskLevels.getById(Number(id)),
    enabled: Number.isSafeInteger(id) && Number(id) > 0,
  });
}

export function useCreateHskLevel() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateHskLevelRequest) => learningService.hskLevels.create(request),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: learningKeys.hskLevels() });
    },
  });
}

export function useUpdateHskLevel(id: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: UpdateHskLevelRequest) => learningService.hskLevels.update(id, request),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: learningKeys.hskLevels() }),
        queryClient.invalidateQueries({ queryKey: ["learning", "hsk-levels", "detail", id] }),
      ]);
    },
  });
}

export function useDeleteHskLevel() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => learningService.hskLevels.remove(id),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: learningKeys.hskLevels() });
    },
  });
}

export function useRestoreHskLevel() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => learningApi.hskLevels.restore(id),
    onSuccess: async (_item, id) => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: learningKeys.hskLevels() }),
        queryClient.invalidateQueries({ queryKey: ["learning", "hsk-levels", "detail", id] }),
      ]);
    },
  });
}

export function useActivateHskLevel() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => learningService.hskLevels.activate(id),
    onSuccess: async (_item, id) => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: learningKeys.hskLevels() }),
        queryClient.invalidateQueries({ queryKey: ["learning", "hsk-levels", "detail", id] }),
      ]);
    },
  });
}

export function useDeactivateHskLevel() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: number) => learningService.hskLevels.deactivate(id),
    onSuccess: async (_item, id) => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: learningKeys.hskLevels() }),
        queryClient.invalidateQueries({ queryKey: ["learning", "hsk-levels", "detail", id] }),
      ]);
    },
  });
}
