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
};
