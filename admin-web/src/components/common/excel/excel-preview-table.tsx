"use client";

import {
  AlertCircle,
  CheckCircle2,
} from "lucide-react";

import type {
  ExcelPreviewData,
} from "@/types/excel.types";

interface ExcelPreviewTableProps {
  data: ExcelPreviewData;

  maxPreviewRows?: number;
}

export function ExcelPreviewTable({
  data,
  maxPreviewRows = 10,
}: ExcelPreviewTableProps) {
  const previewRows =
    data.rows.slice(
      0,
      maxPreviewRows,
    );

  return (
    <div>
      <div
        className="
          mb-2
          flex
          items-center
          justify-between
          gap-3
        "
      >
        <div>
          <div
            className="
              text-[13px]
              font-semibold
              text-[#333]
            "
          >
            Xem trước nội dung
          </div>

          <div
            className="
              mt-[2px]
              text-[10px]
              text-[#8b8b8b]
            "
          >
            Hiển thị{" "}
            {
              previewRows.length
            }{" "}
            /{" "}
            {
              data.totalRows
            }{" "}
            dòng dữ liệu
          </div>
        </div>

        <div
          className="
            rounded-full
            bg-[#f4f3f0]
            px-3
            py-1
            text-[10px]
            text-[#777]
          "
        >
          Sheet:{" "}
          {
            data.sheetName
          }
        </div>
      </div>

      <div
        className="
          overflow-hidden
          rounded-[10px]
          border
          border-[#e7e2db]
        "
      >
        <div
          className="
            max-h-[350px]
            overflow-auto
          "
        >
          <table
            className="
              min-w-full
              border-collapse
              text-[11px]
            "
          >
            <thead
              className="
                sticky
                top-0
                z-10
                bg-[#faf9f7]
              "
            >
              <tr>
                <th
                  className="
                    w-[52px]
                    whitespace-nowrap
                    border-b
                    border-r
                    border-[#e7e2db]
                    px-3
                    py-[9px]
                    text-center
                    font-semibold
                    text-[#666]
                  "
                >
                  Dòng
                </th>

                <th
                  className="
                    w-[68px]
                    whitespace-nowrap
                    border-b
                    border-r
                    border-[#e7e2db]
                    px-3
                    py-[9px]
                    text-center
                    font-semibold
                    text-[#666]
                  "
                >
                  Kiểm tra
                </th>

                {data.headers.map(
                  (
                    header,
                  ) => (
                    <th
                      key={
                        header
                      }
                      className="
                        min-w-[135px]
                        whitespace-nowrap
                        border-b
                        border-r
                        border-[#e7e2db]
                        px-3
                        py-[9px]
                        text-left
                        font-semibold
                        text-[#666]
                        last:border-r-0
                      "
                    >
                      {
                        header
                      }
                    </th>
                  ),
                )}
              </tr>
            </thead>

            <tbody>
              {previewRows.map(
                (
                  row,
                ) => (
                  <tr
                    key={
                      row.rowNumber
                    }
                    className="
                      border-b
                      border-[#eeeae4]
                      bg-white
                      last:border-b-0
                      hover:bg-[#fffaf8]
                    "
                  >
                    <td
                      className="
                        border-r
                        border-[#eeeae4]
                        px-3
                        py-[9px]
                        text-center
                        text-[#999]
                      "
                    >
                      {
                        row.rowNumber
                      }
                    </td>

                    <td
                      className="
                        border-r
                        border-[#eeeae4]
                        px-3
                        py-[9px]
                        text-center
                      "
                    >
                      {row.valid !==
                      false ? (
                        <CheckCircle2
                          size={15}
                          className="
                            mx-auto
                            text-[#16975b]
                          "
                        />
                      ) : (
                        <AlertCircle
                          size={15}
                          className="
                            mx-auto
                            text-[#ef241c]
                          "
                        />
                      )}
                    </td>

                    {data.headers.map(
                      (
                        header,
                      ) => (
                        <td
                          key={
                            header
                          }
                          title={String(
                            row.values[
                              header
                            ] ??
                              "",
                          )}
                          className="
                            max-w-[240px]
                            truncate
                            border-r
                            border-[#eeeae4]
                            px-3
                            py-[9px]
                            text-[#444]
                            last:border-r-0
                          "
                        >
                          {String(
                            row.values[
                              header
                            ] ??
                              "",
                          )}
                        </td>
                      ),
                    )}
                  </tr>
                ),
              )}
            </tbody>
          </table>
        </div>

        {data.totalRows >
          maxPreviewRows && (
          <div
            className="
              border-t
              border-[#e7e2db]
              bg-[#faf9f7]
              px-4
              py-2
              text-center
              text-[10px]
              text-[#888]
            "
          >
            Còn{" "}
            {data.totalRows -
              maxPreviewRows}{" "}
            dòng khác không hiển thị
            trong bản xem trước.
          </div>
        )}
      </div>
    </div>
  );
}
