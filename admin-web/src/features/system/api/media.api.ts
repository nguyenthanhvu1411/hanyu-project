import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";

export type MediaKind = "image" | "audio" | "video" | "document";

export interface UploadedMedia {
  url: string;
  objectKey: string;
  fileName: string;
  contentType: string;
  size: number;
  kind: MediaKind;
}

export interface MediaReadUrl {
  objectKey: string;
  url: string;
}

export const STORAGE_REFERENCE_PREFIX = "storage://";

export function toStorageReference(objectKey: string) {
  return `${STORAGE_REFERENCE_PREFIX}${objectKey.replace(/^\/+/, "")}`;
}

export function getStorageObjectKey(value?: string | null): string | null {
  const normalized = value?.trim();
  if (!normalized) return null;

  if (normalized.startsWith(STORAGE_REFERENCE_PREFIX)) {
    return normalized.slice(STORAGE_REFERENCE_PREFIX.length).replace(/^\/+/, "");
  }

  try {
    const parsed = new URL(normalized);
    if (parsed.hostname.endsWith("backblazeb2.com")) {
      return decodeURIComponent(parsed.pathname.replace(/^\/+/, ""));
    }
  } catch {
    return null;
  }

  return null;
}

function upload(file: File, endpoint: string) {
  const formData = new FormData();
  formData.append("file", file);

  return apiClient<UploadedMedia>(endpoint, {
    method: "POST",
    body: formData,
  });
}

export const mediaApi = {
  uploadImage: (file: File) => upload(file, API_ENDPOINTS.ADMIN.UPLOAD_IMAGE),
  uploadAudio: (file: File) => upload(file, API_ENDPOINTS.ADMIN.UPLOAD_AUDIO),
  uploadVideo: (file: File) => upload(file, API_ENDPOINTS.ADMIN.UPLOAD_VIDEO),
  uploadDocument: (file: File) => upload(file, API_ENDPOINTS.ADMIN.UPLOAD_DOCUMENT),
  getReadUrl: (objectKey: string) =>
    apiClient<MediaReadUrl>(API_ENDPOINTS.MEDIA.READ_URL(objectKey)),
};
