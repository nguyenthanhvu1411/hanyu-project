export type UploadMediaType =
  | "image"
  | "audio"
  | "video";

export interface UploadFileValue {
  id: string;

  file?: File;

  url: string;

  name: string;

  size?: number;

  type?: string;

  existing?: boolean;
}
