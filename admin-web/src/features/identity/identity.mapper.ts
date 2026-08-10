import type {
    PagedResult,
} from "@/types/api.types";

export function mapPagedResult<T>(
  response: PagedResult<T>,
): PagedResult<T> {
  return response;
}
