export interface AdminContentReport {
  id: number;
  publicId: string;
  userId: string;
  entityType: number;
  entityId: number;
  reason: number;
  description: string | null;
  status: number;
  resolvedByUserId: string | null;
  resolvedAt: string | null;
  resolutionNote: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface AdminContentReportQuery {
  userId?: string;
  entityType?: number;
  reason?: number;
  status?: number;
  from?: string;
  to?: string;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminContentImportJob {
  id: number;
  publicId: string;
  importType: number;
  originalFileName: string;
  storagePath: string;
  status: number;
  totalRows: number;
  processedRows: number;
  successRows: number;
  failedRows: number;
  startedAt: string | null;
  completedAt: string | null;
  errorMessage: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface AdminContentImportRow {
  id: number;
  rowNumber: number;
  sourceJson: string;
  isSuccessful: boolean;
  createdEntityId: number | null;
  errorCode: string | null;
  errorMessage: string | null;
  processedAt: string;
}

export interface AdminContentImportJobQuery {
  importType?: number;
  status?: number;
  from?: string;
  to?: string;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateContentImportJobRequest {
  importType: number;
  originalFileName: string;
  storagePath: string;
}

export interface UpdateContentImportSourceRequest {
  originalFileName: string;
  storagePath: string;
}
