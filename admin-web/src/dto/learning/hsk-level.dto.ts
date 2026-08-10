export interface AdminHskLevelDto {
  /** Backend long. JavaScript number is safe for the current database identity range. */
  id: number;
  code: string;
  nameVi: string;
  sortOrder: number;
  isActive: boolean;
}

export interface CreateHskLevelRequest {
  code: string;
  nameVi: string;
  sortOrder: number;
}

export interface UpdateHskLevelRequest {
  code: string;
  nameVi: string;
  sortOrder: number;
}
