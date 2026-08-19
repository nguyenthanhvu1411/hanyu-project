export interface PagedResult<T> {
  items: T[];

  page: number;
  pageSize: number;

  /** Canonical backend field from HanYu.Application.Common.Models.PagedResult. */
  total: number;

  /** Legacy client compatibility for older endpoints. */
  totalCount?: number;

  totalPages?: number;

  hasPrevious?: boolean;
  hasNext?: boolean;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}
