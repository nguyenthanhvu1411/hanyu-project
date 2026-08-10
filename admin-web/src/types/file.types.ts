export interface FileUploadItem {
  id: string;

  file?: File;

  name: string;

  url?: string;

  size?: number;

  mimeType?: string;

  existing?: boolean;
}
