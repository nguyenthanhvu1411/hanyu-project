"use client";

import {
  Download,
  FileDown,
  FileSpreadsheet,
  Upload,
} from "lucide-react";

import {
  useState,
} from "react";

import {
  Button,
} from "@/components/ui/button";

import {
  ImportExcelDialog,
} from "./import-excel-dialog";

import type {
  ExcelTemplateColumn,
} from "@/types/excel.types";

interface ExcelToolbarProps {
  moduleName: string;

  sampleFileUrl: string;

  sampleFileName?: string;

  templateColumns?: ExcelTemplateColumn[];

  onExport?: () => void;

  onImport?: (
    file: File,
  ) => void | Promise<void>;
}

export function ExcelToolbar({
  moduleName,
  sampleFileUrl,
  sampleFileName = "file-mau.xlsx",
  templateColumns = [],
  onExport,
  onImport,
}: ExcelToolbarProps) {
  const [
    importOpen,
    setImportOpen,
  ] = useState(false);

  return (
    <>
      <div
        className="
          flex
          flex-wrap
          items-center
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
          onClick={
            onExport
          }
        >
          <FileDown
            size={15}
            className="mr-2"
          />

          Xuất Excel
        </Button>

        <Button
          type="button"
          variant="outline"
          className="
            h-[38px]
            text-[12px]
          "
          onClick={() =>
            setImportOpen(
              true,
            )
          }
        >
          <Upload
            size={15}
            className="mr-2"
          />

          Nhập Excel
        </Button>

        <a
          href={
            sampleFileUrl
          }
          download={
            sampleFileName
          }
        >
          <Button
            type="button"
            variant="ghost"
            className="
              h-[38px]
              text-[12px]
              text-[#16975b]
            "
          >
            <FileSpreadsheet
              size={15}
              className="mr-2"
            />

            File mẫu
          </Button>
        </a>
      </div>

      <ImportExcelDialog
        open={
          importOpen
        }
        onOpenChange={
          setImportOpen
        }
        moduleName={
          moduleName
        }
        sampleFileUrl={
          sampleFileUrl
        }
        sampleFileName={
          sampleFileName
        }
        templateColumns={
          templateColumns
        }
        onImport={
          onImport
        }
      />
    </>
  );
}
