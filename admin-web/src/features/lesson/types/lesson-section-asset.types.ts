export interface AdminLessonSectionAsset {
  id: number;
  publicId: string;
  lessonSectionId: number;
  lessonAssetId: number;
  sortOrder: number;
  captionVi?: string | null;
  isRequired: boolean;
  assetType: string;
  url?: string | null;
  audioAssetId?: number | null;
  assetCaptionVi?: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface AttachLessonSectionAssetRequest {
  lessonAssetId: number;
  sortOrder: number;
  captionVi?: string | null;
  isRequired: boolean;
}

export interface UpdateLessonSectionAssetRequest {
  sortOrder: number;
  captionVi?: string | null;
  isRequired: boolean;
}
