export interface ApiMeta {
  requestId?: string;

  page?: number;

  pageSize?: number;

  total?: number;

  totalPages?: number;

  hasNext?: boolean;
}

export interface ApiEnvelope<T> {
  data: T;
  meta?: ApiMeta;
}

export interface PagedResult<T> {
  items: T[];

  page: number;

  pageSize: number;

  total: number;

  totalPages: number;

  hasNext: boolean;
}

export interface ProblemDetailsError {
  field?: string;

  code?: string;

  message: string;
}

export interface ApiProblemDetails {
  type?: string;

  title: string;

  status: number;

  code?: string;

  detail?: string;

  traceId?: string;

  errors?: ProblemDetailsError[];
}
