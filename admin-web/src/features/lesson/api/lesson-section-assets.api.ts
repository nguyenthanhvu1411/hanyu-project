import { apiClient } from "@/lib/api/api-client";

import type {
  AdminLessonSectionAsset,
  AttachLessonSectionAssetRequest,
  UpdateLessonSectionAssetRequest,
} from "../types/lesson-section-asset.types";

function root(lessonId: number, sectionId: number) {
  return `/admin/lessons/${lessonId}/sections/${sectionId}/assets`;
}

export const lessonSectionAssetsApi = {
  list(lessonId: number, sectionId: number) {
    return apiClient<AdminLessonSectionAsset[]>(root(lessonId, sectionId));
  },

  attach(lessonId: number, sectionId: number, request: AttachLessonSectionAssetRequest) {
    return apiClient<AdminLessonSectionAsset>(root(lessonId, sectionId), {
      method: "POST",
      body: request,
    });
  },

  update(
    lessonId: number,
    sectionId: number,
    linkId: number,
    request: UpdateLessonSectionAssetRequest,
  ) {
    return apiClient<AdminLessonSectionAsset>(`${root(lessonId, sectionId)}/${linkId}`, {
      method: "PUT",
      body: request,
    });
  },

  remove(lessonId: number, sectionId: number, linkId: number) {
    return apiClient<void>(`${root(lessonId, sectionId)}/${linkId}`, { method: "DELETE" });
  },
};
