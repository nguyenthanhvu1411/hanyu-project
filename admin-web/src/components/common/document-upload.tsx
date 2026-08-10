"use client";

import {
  FileText,
} from "lucide-react";

import {
  FileUpload,
} from "./file-upload";

interface DocumentUploadProps {
  value?: File[];

  onChange: (
    files: File[],
  ) => void;

  maxFiles?: number;

  maxSizeMb?: number;

  disabled?: boolean;
}

export function DocumentUpload({
  value = [],
  onChange,
  maxFiles = 10,
  maxSizeMb = 25,
  disabled = false,
}: DocumentUploadProps) {
  return (
    <div>
      <div
        className="
          mb-2
          flex
          items-center
          gap-2
          text-[10px]
          text-[#868686]
        "
      >
        <FileText
          size={13}
        />

        PDF, Word, Excel,
        PowerPoint và văn bản.
      </div>

      <FileUpload
        value={value}
        onChange={
          onChange
        }
        accept=".pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.txt,.csv"
        maxSizeMb={
          maxSizeMb
        }
        maxFiles={
          maxFiles
        }
        multiple
        disabled={
          disabled
        }
        title="Tải tài liệu"
        description="Kéo tài liệu vào đây hoặc nhấn để chọn file."
      />
    </div>
  );
}
