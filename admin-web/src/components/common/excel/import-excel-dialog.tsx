"use client";

import {
  AlertCircle,
  FileSpreadsheet,
  Loader2,
  Upload,
  X,
} from "lucide-react";

import {
  useEffect,
  useState,
} from "react";

import {
  Button,
} from "@/components/ui/button";

import {
  ExcelFileInfo,
} from "./excel-file-info";

import {
  ExcelImportSummary,
} from "./excel-import-summary";

import {
  ExcelPreviewTable,
} from "./excel-preview-table";

import {
  ExcelTemplateCard,
} from "./excel-template-card";

import {
  ExcelUploadZone,
} from "./excel-upload-zone";

import {
  parseExcelFile,
} from "@/utils/excel.util";

import type {
  ExcelPreviewData,
  ExcelTemplateColumn,
} from "@/types/excel.types";

interface ImportExcelDialogProps {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  moduleName: string;

  sampleFileUrl: string;

  sampleFileName: string;

  templateColumns?: ExcelTemplateColumn[];

  onImport?: (
    file: File,
  ) => void | Promise<void>;
}

export function ImportExcelDialog({
  open,
  onOpenChange,
  moduleName,
  sampleFileUrl,
  sampleFileName,
  templateColumns = [],
  onImport,
}: ImportExcelDialogProps) {
  const [
    file,
    setFile,
  ] =
    useState<File | null>(
      null,
    );

  const [
    preview,
    setPreview,
  ] =
    useState<ExcelPreviewData | null>(
      null,
    );

  const [
    parsing,
    setParsing,
  ] = useState(false);

  const [
    importing,
    setImporting,
  ] = useState(false);

  const [
    error,
    setError,
  ] =
    useState<string | null>(
      null,
    );

  useEffect(() => {
    if (!open) {
      setFile(
        null,
      );

      setPreview(
        null,
      );

      setError(
        null,
      );

      setParsing(
        false,
      );

      setImporting(
        false,
      );
    }
  }, [open]);

  if (!open) {
    return null;
  }

  async function handleFile(
    selectedFile: File,
  ) {
    if (
      selectedFile.size >
      10 *
        1024 *
        1024
    ) {
      setError(
        "Dung lượng file tối đa là 10 MB.",
      );

      return;
    }

    setFile(
      selectedFile,
    );

    setPreview(
      null,
    );

    setError(
      null,
    );

    setParsing(
      true,
    );

    try {
      const result =
        await parseExcelFile(
          selectedFile,
        );

      setPreview(
        result,
      );
    } catch (
      exception
    ) {
      setError(
        exception instanceof
          Error
          ? exception.message
          : "Không thể đọc file Excel.",
      );
    } finally {
      setParsing(
        false,
      );
    }
  }

  function removeFile() {
    setFile(
      null,
    );

    setPreview(
      null,
    );

    setError(
      null,
    );
  }

  async function submitImport() {
    if (!file) {
      return;
    }

    setImporting(
      true,
    );

    try {
      await onImport?.(
        file,
      );

      // Khi nối backend thành công
      // có thể đóng dialog tại đây.
    } finally {
      setImporting(
        false,
      );
    }
  }

  const validRows =
    preview?.rows.filter(
      (
        row,
      ) =>
        row.valid !==
        false,
    ).length ?? 0;

  const invalidRows =
    preview?.rows.filter(
      (
        row,
      ) =>
        row.valid ===
        false,
    ).length ?? 0;

  return (
    <div
      className="
        fixed
        inset-0
        z-[100]
        flex
        items-center
        justify-center
        p-4
        sm:p-6
      "
    >
      <button
        type="button"
        aria-label="Đóng"
        onClick={() =>
          onOpenChange(
            false,
          )
        }
        className="
          absolute
          inset-0
          bg-black/40
          backdrop-blur-[1px]
        "
      />

      <div
        role="dialog"
        aria-modal="true"
        className="
          relative
          z-10
          flex
          max-h-[92vh]
          w-full
          max-w-[1080px]
          flex-col
          overflow-hidden
          rounded-[16px]
          border
          border-[#e6e0d7]
          bg-white
          shadow-[0_28px_80px_rgba(0,0,0,0.18)]
        "
      >
        {/* Header */}
        <div
          className="
            flex
            shrink-0
            items-start
            justify-between
            gap-4
            border-b
            border-[#ebe6df]
            px-5
            py-4
            sm:px-6
          "
        >
          <div className="flex items-start gap-3">
            <div
              className="
                flex
                h-10
                w-10
                shrink-0
                items-center
                justify-center
                rounded-[9px]
                bg-[#fff0ee]
                text-[#ef241c]
              "
            >
              <FileSpreadsheet
                size={20}
              />
            </div>

            <div>
              <h2
                className="
                  text-[16px]
                  font-semibold
                  text-[#292929]
                "
              >
                Nhập dữ liệu Excel
              </h2>

              <p
                className="
                  mt-[3px]
                  text-[11px]
                  text-[#858585]
                "
              >
                Nhập danh sách{" "}
                {moduleName} từ
                file Excel.
              </p>
            </div>
          </div>

          <button
            type="button"
            onClick={() =>
              onOpenChange(
                false,
              )
            }
            className="
              flex
              h-8
              w-8
              shrink-0
              items-center
              justify-center
              rounded-[7px]
              text-[#888]
              hover:bg-[#f3f3f3]
            "
          >
            <X
              size={18}
            />
          </button>
        </div>

        {/* Body */}
        <div
          className="
            scrollbar-thin
            flex-1
            overflow-y-auto
            px-5
            py-5
            sm:px-6
          "
        >
          <ExcelTemplateCard
            sampleFileUrl={
              sampleFileUrl
            }
            sampleFileName={
              sampleFileName
            }
            columns={
              templateColumns
            }
          />

          <div className="my-5">
            {!file ? (
              <ExcelUploadZone
                onFileSelect={
                  handleFile
                }
              />
            ) : (
              <ExcelFileInfo
                file={
                  file
                }
                sheetName={
                  preview?.sheetName
                }
                totalRows={
                  preview?.totalRows
                }
                onRemove={
                  removeFile
                }
              />
            )}
          </div>

          {error && (
            <div
              className="
                mb-5
                flex
                items-start
                gap-2
                rounded-[9px]
                border
                border-[#f3cbc7]
                bg-[#fff3f1]
                px-3
                py-3
                text-[11px]
                text-[#cc342d]
              "
            >
              <AlertCircle
                size={16}
                className="
                  mt-[1px]
                  shrink-0
                "
              />

              {
                error
              }
            </div>
          )}

          {parsing && (
            <div
              className="
                flex
                min-h-[180px]
                flex-col
                items-center
                justify-center
                rounded-[10px]
                border
                border-[#e9e4dd]
                bg-[#faf9f7]
              "
            >
              <Loader2
                size={26}
                className="
                  animate-spin
                  text-[#ef241c]
                "
              />

              <div
                className="
                  mt-3
                  text-[12px]
                  font-medium
                  text-[#555]
                "
              >
                Đang đọc file Excel...
              </div>

              <div
                className="
                  mt-1
                  text-[10px]
                  text-[#999]
                "
              >
                Vui lòng chờ trong
                giây lát.
              </div>
            </div>
          )}

          {!parsing &&
            preview && (
              <div className="space-y-5">
                <ExcelImportSummary
                  totalRows={
                    preview.totalRows
                  }
                  validRows={
                    validRows
                  }
                  invalidRows={
                    invalidRows
                  }
                  columns={
                    preview.headers
                      .length
                  }
                />

                <ExcelPreviewTable
                  data={
                    preview
                  }
                  maxPreviewRows={
                    10
                  }
                />
              </div>
            )}
        </div>

        {/* Footer */}
        <div
          className="
            flex
            shrink-0
            flex-col-reverse
            gap-2
            border-t
            border-[#ebe6df]
            bg-[#faf9f7]
            px-5
            py-3
            sm:flex-row
            sm:items-center
            sm:justify-between
            sm:px-6
          "
        >
          <div
            className="
              text-[10px]
              leading-[16px]
              text-[#8c8c8c]
            "
          >
            Dữ liệu chỉ đang được
            xem trước. Chưa có dữ
            liệu nào được gửi lên
            hệ thống.
          </div>

          <div
            className="
              flex
              items-center
              justify-end
              gap-2
            "
          >
            <Button
              type="button"
              variant="outline"
              className="
                h-[38px]
                text-[12px]
              "
              onClick={() =>
                onOpenChange(
                  false,
                )
              }
            >
              Hủy
            </Button>

            <Button
              type="button"
              disabled={
                !file ||
                !preview ||
                parsing ||
                invalidRows >
                  0
              }
              loading={
                importing
              }
              onClick={
                submitImport
              }
              className="
                h-[38px]
                gap-2
                text-[12px]
              "
            >
              <Upload
                size={15}
              />

              Nhập dữ liệu
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
