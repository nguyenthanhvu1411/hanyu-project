import type { ApiEnvelope, PagedResult } from "@/types/api.types";

export function mapPagedResult<T>(
  response: ApiEnvelope<T[]>
): PagedResult<T> {
  const items = response.data ?? [];

  const page = response.meta?.page ?? 1;

  const pageSize =
    response.meta?.pageSize ??
    (items.length > 0 ? items.length : 20);

  const total = response.meta?.total ?? items.length;

  const totalPages =
    response.meta?.totalPages ??
    Math.max(1, Math.ceil(total / Math.max(1, pageSize)));

  return {
    items,
    page,
    pageSize,
    total,
    totalPages,
    hasNext: response.meta?.hasNext ?? page < totalPages,
  };
}
