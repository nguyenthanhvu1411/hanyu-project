export function isValidEmail(
  value: string,
) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(
    value,
  );
}

export function isValidUrl(
  value: string,
) {
  try {
    new URL(value);

    return true;
  } catch {
    return false;
  }
}

export function isEmpty(
  value: unknown,
) {
  if (
    value === null ||
    value ===
      undefined
  ) {
    return true;
  }

  if (
    typeof value ===
    "string"
  ) {
    return (
      value.trim()
        .length === 0
    );
  }

  if (
    Array.isArray(
      value,
    )
  ) {
    return (
      value.length ===
      0
    );
  }

  return false;
}

export function validateFileSize(
  file: File,
  maxMb: number,
) {
  return (
    file.size <=
    maxMb *
      1024 *
      1024
  );
}

export function validateFileType(
  file: File,
  allowedTypes: string[],
) {
  return allowedTypes.includes(
    file.type,
  );
}
