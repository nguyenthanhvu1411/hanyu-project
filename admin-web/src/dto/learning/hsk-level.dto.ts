export interface AdminHskLevelDto {
  id: number;
  code: string;
  nameVi: string;
  sortOrder: number;
  isActive: boolean;
  concurrencyToken?: string | null;
  version?: number | null;
}

export interface CreateHskLevelRequest {
  id: number;
  code: string;
  nameVi: string;
  sortOrder: number;
  isActive: boolean;
}

export interface UpdateHskLevelRequest {
  code: string;
  nameVi: string;
  sortOrder: number;
  isActive: boolean;
  concurrencyToken?: string | null;
  version?: number | null;
}
