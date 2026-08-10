import type { FieldValues, Path, UseFormSetError } from "react-hook-form";
import { ApiError } from "./api-error";

export function applyApiFormErrors<T extends FieldValues>(
  error: unknown,
  setError: UseFormSetError<T>,
) {
  if (!(error instanceof ApiError)) {
    return false;
  }

  let applied = false;

  Object.entries(error.errors || {}).forEach(([field, messages]) => {
    if (field === "_global") {
      return;
    }

    const message = messages[0];

    if (!message) {
      return;
    }

    setError(field as Path<T>, {
      type: "server",
      message,
    });

    applied = true;
  });

  return applied;
}
