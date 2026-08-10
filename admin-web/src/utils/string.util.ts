export function normalizeText(
  value: string,
) {
  return value
    .normalize(
      "NFD",
    )
    .replace(
      /[\u0300-\u036f]/g,
      "",
    )
    .replace(
      /đ/g,
      "d",
    )
    .replace(
      /Đ/g,
      "D",
    );
}

export function slugify(
  value: string,
) {
  return normalizeText(
    value,
  )
    .toLowerCase()
    .trim()
    .replace(
      /[^a-z0-9]+/g,
      "-",
    )
    .replace(
      /^-+|-+$/g,
      "",
    );
}

export function truncate(
  value: string,
  maxLength = 80,
) {
  if (
    value.length <=
    maxLength
  ) {
    return value;
  }

  return `${value.slice(
    0,
    maxLength - 1,
  )}…`;
}

export function capitalize(
  value: string,
) {
  if (!value) {
    return "";
  }

  return (
    value
      .charAt(0)
      .toUpperCase() +
    value.slice(1)
  );
}
