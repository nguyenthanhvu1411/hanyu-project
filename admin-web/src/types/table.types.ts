import type {
  ReactNode,
} from "react";

export type SortDirection =
  | "asc"
  | "desc";

export interface TableSort {
  field?: string;
  direction?: SortDirection;
}

export interface TablePagination {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface DataTableColumn<T> {
  id: string;

  header: string;

  width?: string;

  align?:
    | "left"
    | "center"
    | "right";

  sortable?: boolean;

  accessor?:
    | keyof T
    | ((
        item: T,
      ) => ReactNode);

  cell?: (
    item: T,
    index: number,
  ) => ReactNode;
}
