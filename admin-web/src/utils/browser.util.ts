export function isBrowser() {
  return (
    typeof window !==
      "undefined" &&
    typeof document !==
      "undefined"
  );
}

export function getOrigin() {
  if (!isBrowser()) {
    return "";
  }

  return window.location.origin;
}

export function reloadPage() {
  if (!isBrowser()) {
    return;
  }

  window.location.reload();
}

export function scrollToTop(
  smooth = true,
) {
  if (!isBrowser()) {
    return;
  }

  window.scrollTo({
    top: 0,
    behavior:
      smooth
        ? "smooth"
        : "auto",
  });
}

export function openInNewTab(
  url: string,
) {
  if (!isBrowser()) {
    return;
  }

  window.open(
    url,
    "_blank",
    "noopener,noreferrer",
  );
}
