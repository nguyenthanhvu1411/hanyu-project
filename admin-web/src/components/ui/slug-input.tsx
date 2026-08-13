"use client";

import { RotateCcw } from "lucide-react";
import { useEffect, useRef, useState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { slugify } from "@/lib/utils/slug";

interface SlugInputProps {
  value: string;
  sourceValue: string;
  onChange: (value: string) => void;
  mode?: "create" | "edit";
  label?: string;
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  previewPrefix?: string;
  className?: string;
}

export function SlugInput({
  value,
  sourceValue,
  onChange,
  mode = "create",
  label = "Slug",
  placeholder = "tu-dong-theo-ten",
  required = false,
  disabled = false,
  previewPrefix = "/",
  className,
}: SlugInputProps) {
  const [manual, setManual] = useState(() => mode === "edit" && Boolean(value));
  const initializedRef = useRef(false);

  useEffect(() => {
    if (initializedRef.current) return;
    initializedRef.current = true;

    if (mode === "create" && !value) {
      onChange(slugify(sourceValue));
    }
  }, [mode, onChange, sourceValue, value]);

  useEffect(() => {
    if (manual || mode !== "create") return;

    const generated = slugify(sourceValue);
    if (generated !== value) {
      onChange(generated);
    }
  }, [manual, mode, onChange, sourceValue, value]);

  const preview = value ? `${previewPrefix}${value}` : "Slug sẽ tự sinh theo tên";

  return (
    <label className={`block space-y-2 ${className ?? ""}`}>
      <span className="text-[13px] font-medium text-[#555]">
        {label}{required ? " *" : ""}
      </span>

      <div className="flex gap-2">
        <Input
          value={value}
          disabled={disabled}
          placeholder={placeholder}
          onChange={(event) => {
            const rawValue = event.target.value;

            if (!rawValue.trim()) {
              setManual(false);
              onChange(slugify(sourceValue));
              return;
            }

            setManual(true);
            onChange(slugify(rawValue));
          }}
          className="h-10 px-3 text-[14px]"
        />

        {manual && !disabled ? (
          <Button
            type="button"
            variant="outline"
            size="icon"
            title="Tạo lại slug theo tên"
            aria-label="Tạo lại slug theo tên"
            onClick={() => {
              setManual(false);
              onChange(slugify(sourceValue));
            }}
          >
            <RotateCcw size={15} />
          </Button>
        ) : null}
      </div>

      <div className="flex flex-wrap items-center justify-between gap-2 text-[12px] leading-5 text-[#8a8a8a]">
        <span>{preview}</span>
        <span>
          {mode === "edit"
            ? "Đổi tên không tự đổi URL. Chỉ đổi slug khi bạn chỉnh trực tiếp."
            : manual
              ? "Slug đang được chỉnh thủ công."
              : "Đang tự động theo tên."}
        </span>
      </div>
    </label>
  );
}
