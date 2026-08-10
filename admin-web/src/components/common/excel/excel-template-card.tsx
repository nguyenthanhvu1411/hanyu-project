import {
  Download,
  FileSpreadsheet,
  Info,
} from "lucide-react";

import type {
  ExcelTemplateColumn,
} from "@/types/excel.types";

interface ExcelTemplateCardProps {
  sampleFileUrl: string;

  sampleFileName: string;

  columns?: ExcelTemplateColumn[];
}

export function ExcelTemplateCard({
  sampleFileUrl,
  sampleFileName,
  columns = [],
}: ExcelTemplateCardProps) {
  return (
    <div
      className="
        rounded-[10px]
        border
        border-[#e8e2d9]
        bg-[#fffdf9]
        p-4
      "
    >
      <div
        className="
          flex
          items-start
          gap-3
        "
      >
        <div
          className="
            flex
            h-9
            w-9
            shrink-0
            items-center
            justify-center
            rounded-[8px]
            bg-[#edf8f2]
            text-[#16975b]
          "
        >
          <FileSpreadsheet
            size={18}
          />
        </div>

        <div className="min-w-0 flex-1">
          <div
            className="
              text-[12px]
              font-semibold
              text-[#333]
            "
          >
            Sử dụng đúng định dạng
            file mẫu
          </div>

          <div
            className="
              mt-1
              text-[10px]
              leading-[16px]
              text-[#888]
            "
          >
            Không thay đổi tên cột
            hoặc thứ tự tiêu đề nếu
            hệ thống chưa hỗ trợ
            ánh xạ cột.
          </div>
        </div>

        <a
          href={
            sampleFileUrl
          }
          download={
            sampleFileName
          }
          className="
            flex
            h-8
            shrink-0
            items-center
            gap-1
            rounded-[7px]
            border
            border-[#dcd7cf]
            bg-white
            px-3
            text-[10px]
            font-medium
            text-[#16975b]
            transition
            hover:bg-[#f5fbf7]
          "
        >
          <Download
            size={13}
          />

          Tải file mẫu
        </a>
      </div>

      {columns.length >
        0 && (
        <div
          className="
            mt-4
            overflow-hidden
            rounded-[8px]
            border
            border-[#ebe6de]
          "
        >
          <div
            className="
              flex
              items-center
              gap-2
              border-b
              border-[#ebe6de]
              bg-[#faf9f7]
              px-3
              py-2
              text-[10px]
              font-semibold
              text-[#666]
            "
          >
            <Info
              size={13}
            />

            Cấu trúc file
          </div>

          <div className="divide-y divide-[#eeeae4]">
            {columns.map(
              (
                column,
              ) => (
                <div
                  key={
                    column.key
                  }
                  className="
                    grid
                    grid-cols-[135px_1fr]
                    gap-3
                    px-3
                    py-2
                    text-[10px]
                  "
                >
                  <div
                    className="
                      font-medium
                      text-[#444]
                    "
                  >
                    {
                      column.label
                    }

                    {column.required && (
                      <span
                        className="
                          ml-1
                          text-[#ef241c]
                        "
                      >
                        *
                      </span>
                    )}
                  </div>

                  <div
                    className="
                      text-[#888]
                    "
                  >
                    {
                      column.description
                    }

                    {column.example && (
                      <span>
                        {" "}
                        Ví dụ:{" "}
                        <code
                          className="
                            rounded
                            bg-[#f3f1ed]
                            px-1
                            py-[1px]
                            text-[#666]
                          "
                        >
                          {
                            column.example
                          }
                        </code>
                      </span>
                    )}
                  </div>
                </div>
              ),
            )}
          </div>
        </div>
      )}
    </div>
  );
}
