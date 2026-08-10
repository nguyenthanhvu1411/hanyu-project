import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";

export interface UploadedImage {
  url: string;
  relativeUrl: string;
  fileName: string;
  contentType: string;
  size: number;
}

export const mediaApi = {
  uploadImage(file: File) {
    const formData = new FormData();
    formData.append("file", file);

    return apiClient<UploadedImage>(API_ENDPOINTS.ADMIN.UPLOAD_IMAGE, {
      method: "POST",
      body: formData,
    });
  },
};
