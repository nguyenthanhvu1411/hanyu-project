export interface ExcelPreviewRow {
  rowNumber: number;

  values: Record<
    string,
    string | number | boolean | null
  >;

  valid?: boolean;

  errors?: string[];
}

export interface ExcelPreviewData {
  fileName: string;

  fileSize: number;

  sheetName: string;

  headers: string[];

  rows: ExcelPreviewRow[];

  totalRows: number;
}

export interface ExcelTemplateColumn {
  key: string;

  label: string;

  required?: boolean;

  description?: string;

  example?: string;
}
