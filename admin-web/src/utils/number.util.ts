export function formatNumber(
  value:
    | number
    | null
    | undefined,
) {
  if (
    value === null ||
    value ===
      undefined
  ) {
    return "-";
  }

  return new Intl.NumberFormat(
    "vi-VN",
  ).format(value);
}

export function clamp(
  value: number,
  min: number,
  max: number,
) {
  return Math.min(
    Math.max(
      value,
      min,
    ),
    max,
  );
}

export function toNumber(
  value: unknown,
  fallback = 0,
) {
  const number =
    Number(value);

  return Number.isFinite(
    number,
  )
    ? number
    : fallback;
}
