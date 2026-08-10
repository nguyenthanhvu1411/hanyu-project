export function isActiveRoute(
  pathname: string,
  href: string,
) {
  if (
    href === "/"
  ) {
    return (
      pathname ===
      href
    );
  }

  return (
    pathname ===
      href ||
    pathname.startsWith(
      `${href}/`,
    )
  );
}

export function buildRoute(
  base: string,
  ...segments: Array<
    string | number
  >
) {
  return [
    base.replace(
      /\/+$/,
      "",
    ),

    ...segments.map(
      (segment) =>
        encodeURIComponent(
          String(
            segment,
          ),
        ),
    ),
  ].join("/");
}

export function getRouteSegments(
  pathname: string,
) {
  return pathname
    .split("/")
    .filter(Boolean);
}
