"use client";

import {
  FileDown,
} from "lucide-react";

import {
  Button,
} from "@/components/ui/button";

interface ExcelExportButtonProps {
  onClick?: () => void;

  label?: string;
}

export function ExcelExportButton({
  onClick,
  label = "Xuất Excel",
}: ExcelExportButtonProps) {
  return (
    <Button
      type="button"
      variant="outline"
      onClick={
        onClick
      }
      className="
        h-[38px]
        gap-2
        text-[12px]
      "
    >
      <FileDown
        size={15}
      />

      {label}
    </Button>
  );
}
