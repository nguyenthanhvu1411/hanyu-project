export type SortDirection = "asc" | "desc";

export interface HskLevelListQuery {
  page?: number;
  pageSize?: number;
  q?: string;
  isActive?: boolean;
  sortBy?: string;
  sortDirection?: SortDirection;
}
