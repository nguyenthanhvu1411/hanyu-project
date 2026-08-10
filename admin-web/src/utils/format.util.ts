export function formatBoolean(
  value:
    | boolean
    | null
    | undefined,
) {
  if (
    value ===
    true
  ) {
    return "Có";
  }

  if (
    value ===
    false
  ) {
    return "Không";
  }

  return "-";
}

export function formatNullable(
  value:
    | string
    | number
    | null
    | undefined,
) {
  if (
    value === null ||
    value ===
      undefined ||
    value === ""
  ) {
    return "-";
  }

  return String(
    value,
  );
}

export function formatPercent(
  value: number,
  fractionDigits = 1,
) {
  return `${value.toFixed(
    fractionDigits,
  )}%`;
}
