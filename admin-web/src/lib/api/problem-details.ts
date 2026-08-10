export interface ProblemDetailsFieldError {
  field?: string;
  code?: string;
  message: string;
}

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  code?: string;
  detail?: string;
  traceId?: string;
  errors?: ProblemDetailsFieldError[];
}

export function isProblemDetails(value: unknown): value is ProblemDetails {
  if (!value || typeof value !== "object") {
    return false;
  }

  const object = value as Record<string, unknown>;

  return (
    typeof object.status === "number" ||
    typeof object.title === "string" ||
    typeof object.code === "string"
  );
}
