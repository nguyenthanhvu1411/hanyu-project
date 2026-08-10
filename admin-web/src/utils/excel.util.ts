import * as XLSX from "xlsx";

import type {
  ExcelPreviewData,
  ExcelPreviewRow,
} from "@/types/excel.types";

export async function parseExcelFile(
  file: File,
): Promise<ExcelPreviewData> {
  const buffer =
    await file.arrayBuffer();

  const workbook =
    XLSX.read(
      buffer,
      {
        type: "array",
      },
    );

  const sheetName =
    workbook
      .SheetNames[0];

  if (!sheetName) {
    throw new Error(
      "File Excel không có sheet dữ liệu.",
    );
  }

  const worksheet =
    workbook.Sheets[
      sheetName
    ];

  const matrix =
    XLSX.utils.sheet_to_json<
      Array<
        string |
          number |
          boolean |
          null
      >
    >(
      worksheet,
      {
        header: 1,
        defval: "",
        raw: false,
      },
    );

  if (
    matrix.length ===
    0
  ) {
    throw new Error(
      "File Excel không có dữ liệu.",
    );
  }

  const headers =
    (
      matrix[0] ??
      []
    ).map(
      (
        value,
        index,
      ) =>
        String(
          value ??
            "",
        ).trim() ||
        `Cot_${
          index + 1
        }`,
    );

  const rows: ExcelPreviewRow[] =
    matrix
      .slice(1)
      .filter(
        (row) =>
          row.some(
            (value) =>
              String(
                value ??
                  "",
              ).trim() !==
              "",
          ),
      )
      .map(
        (
          row,
          index,
        ) => {
          const values =
            headers.reduce<
              Record<
                string,
                string |
                  number |
                  boolean |
                  null
              >
            >(
              (
                result,
                header,
                columnIndex,
              ) => {
                result[
                  header
                ] =
                  row[
                    columnIndex
                  ] ??
                  "";

                return result;
              },
              {},
            );

          return {
            rowNumber:
              index + 2,

            values,

            valid: true,

            errors: [],
          };
        },
      );

  return {
    fileName:
      file.name,

    fileSize:
      file.size,

    sheetName,

    headers,

    rows,

    totalRows:
      rows.length,
  };
}

export function exportExcel<
  T extends Record<
    string,
    unknown
  >,
>(
  data: T[],
  fileName: string,
  sheetName =
    "DuLieu",
) {
  const worksheet =
    XLSX.utils.json_to_sheet(
      data,
    );

  const workbook =
    XLSX.utils.book_new();

  XLSX.utils.book_append_sheet(
    workbook,
    worksheet,
    sheetName,
  );

  XLSX.writeFile(
    workbook,
    fileName.endsWith(
      ".xlsx",
    )
      ? fileName
      : `${fileName}.xlsx`,
  );
}

export function exportExcelWithHeaders<
  T,
>(
  data: T[],
  columns: Array<{
    key: keyof T;
    label: string;
  }>,
  fileName: string,
  sheetName =
    "DuLieu",
) {
  const mapped =
    data.map(
      (item) => {
        const row: Record<
          string,
          unknown
        > = {};

        columns.forEach(
          (column) => {
            row[
              column.label
            ] =
              item[
                column.key
              ];
          },
        );

        return row;
      },
    );

  exportExcel(
    mapped,
    fileName,
    sheetName,
  );
}
