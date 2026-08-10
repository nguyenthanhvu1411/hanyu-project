import { ApiError } from "@/lib/api/api-error";

export function isConcurrencyConflict(error: unknown) {
  if (!(error instanceof ApiError)) {
    return false;
  }

  if (error.status !== 409) {
    return false;
  }

  const code = error.code?.toLowerCase() ?? "";

  return (
    code.includes("concurrency") ||
    code.includes("version") ||
    code.includes("conflict")
  );
}

export function isConflictError(error: unknown) {
  return error instanceof ApiError && error.status === 409;
}
