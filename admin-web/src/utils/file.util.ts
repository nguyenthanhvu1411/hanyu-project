export function formatFileSize(
  bytes?: number,
) {
  if (
    bytes ===
      undefined ||
    bytes === null
  ) {
    return "";
  }

  if (
    bytes === 0
  ) {
    return "0 B";
  }

  const units = [
    "B",
    "KB",
    "MB",
    "GB",
    "TB",
  ];

  const index =
    Math.min(
      Math.floor(
        Math.log(
          bytes,
        ) /
          Math.log(
            1024,
          ),
      ),
      units.length -
        1,
    );

  const value =
    bytes /
    Math.pow(
      1024,
      index,
    );

  return `${value.toFixed(
    index === 0
      ? 0
      : 1,
  )} ${units[index]}`;
}

export function createFileId() {
  if (
    typeof crypto !==
      "undefined" &&
    "randomUUID" in
      crypto
  ) {
    return crypto.randomUUID();
  }

  return `${Date.now()}-${Math.random()
    .toString(36)
    .slice(2)}`;
}

export function getFileExtension(
  fileName: string,
) {
  return (
    fileName
      .split(".")
      .pop()
      ?.toLowerCase() ??
    ""
  );
}

export function removeFileExtension(
  fileName: string,
) {
  const index =
    fileName.lastIndexOf(
      ".",
    );

  if (
    index <= 0
  ) {
    return fileName;
  }

  return fileName.slice(
    0,
    index,
  );
}

export function isImageFile(
  file: File,
) {
  return file.type.startsWith(
    "image/",
  );
}

export function isAudioFile(
  file: File,
) {
  return file.type.startsWith(
    "audio/",
  );
}

export function isVideoFile(
  file: File,
) {
  return file.type.startsWith(
    "video/",
  );
}

export function downloadBlob(
  blob: Blob,
  fileName: string,
) {
  const url =
    URL.createObjectURL(
      blob,
    );

  const anchor =
    document.createElement(
      "a",
    );

  anchor.href =
    url;

  anchor.download =
    fileName;

  anchor.click();

  URL.revokeObjectURL(
    url,
  );
}
