export function formatDate(
  value:
    | string
    | Date
    | null
    | undefined,
) {
  if (!value) {
    return "-";
  }

  const date =
    value instanceof Date
      ? value
      : new Date(
          value,
        );

  if (
    Number.isNaN(
      date.getTime(),
    )
  ) {
    return "-";
  }

  return new Intl.DateTimeFormat(
    "vi-VN",
    {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
    },
  ).format(date);
}

export function formatDateTime(
  value:
    | string
    | Date
    | null
    | undefined,
) {
  if (!value) {
    return "-";
  }

  const date =
    value instanceof Date
      ? value
      : new Date(
          value,
        );

  if (
    Number.isNaN(
      date.getTime(),
    )
  ) {
    return "-";
  }

  return new Intl.DateTimeFormat(
    "vi-VN",
    {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    },
  ).format(date);
}

export function toDateInputValue(
  value:
    | string
    | Date
    | null
    | undefined,
) {
  if (!value) {
    return "";
  }

  const date =
    value instanceof Date
      ? value
      : new Date(
          value,
        );

  if (
    Number.isNaN(
      date.getTime(),
    )
  ) {
    return "";
  }

  const year =
    date.getFullYear();

  const month =
    String(
      date.getMonth() +
        1,
    ).padStart(
      2,
      "0",
    );

  const day =
    String(
      date.getDate(),
    ).padStart(
      2,
      "0",
    );

  return `${year}-${month}-${day}`;
}

export function isPastDate(
  value:
    | string
    | Date,
) {
  return (
    new Date(
      value,
    ).getTime() <
    Date.now()
  );
}
