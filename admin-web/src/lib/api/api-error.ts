export interface ApiProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  code?: string;
  errors?: Record<string, string[]>;
}

export class ApiError extends Error {
  readonly status: number;
  readonly code?: string;
  readonly errors?: Record<string, string[]>;

  constructor(
    message: string,
    status: number,
    code?: string,
    errors?: Record<string, string[]>,
  ) {
    super(message);

    this.name = "ApiError";
    this.status = status;
    this.code = code;
    this.errors = errors;
  }
}

export function normalizeApiError(error: unknown): any {
  if (error instanceof ApiError) {
    return error;
  }
  if (error instanceof Error) {
    return error;
  }
  return new Error(typeof error === "string" ? error : "Lỗi hệ thống không xác định");
}
