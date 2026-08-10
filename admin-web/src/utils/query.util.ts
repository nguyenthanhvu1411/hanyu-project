export function createQueryString(
  params: Record<
    string,
    any
  >,
) {
  const search =
    new URLSearchParams();

  Object.entries(
    params,
  ).forEach(
    ([
      key,
      value,
    ]) => {
      if (
        value ===
          undefined ||
        value === null ||
        value === ""
      ) {
        return;
      }

      if (
        Array.isArray(
          value,
        )
      ) {
        value.forEach(
          (item) =>
            search.append(
              key,
              String(
                item,
              ),
            ),
        );

        return;
      }

      search.set(
        key,
        String(
          value,
        ),
      );
    },
  );

  return search.toString();
}

export function parsePage(
  value:
    | string
    | null
    | undefined,
) {
  const result =
    Number(value);

  return Number.isFinite(
    result,
  ) &&
    result > 0
    ? Math.floor(
        result,
      )
    : 1;
}

export function parsePageSize(
  value:
    | string
    | null
    | undefined,
  fallback = 10,
) {
  const result =
    Number(value);

  return Number.isFinite(
    result,
  ) &&
    result > 0
    ? Math.floor(
        result,
      )
    : fallback;
}
