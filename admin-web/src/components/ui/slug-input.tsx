"use client";

import { CheckCircle2, Loader2, RotateCcw } from "lucide-react";
import { useEffect, useRef, useState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { slugify } from "@/lib/utils/slug";

export interface SlugValidationState {
  checking: boolean;
  error: string | null;
}

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
  validateSlug?: (slug: string) => Promise<string | null>;
  validationDelayMs?: number;
  onValidationChange?: (state: SlugValidationState) => void;
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
  validateSlug,
  validationDelayMs = 450,
  onValidationChange,
}: SlugInputProps) {
  const [manual, setManual] = useState(() => mode === "edit" && Boolean(value));
  const [validation, setValidation] = useState<SlugValidationState>({
    checking: false,
    error: null,
  });
  const validationSequence = useRef(0);

  useEffect(() => {
    if (mode === "edit" && value) {
      setManual(true);
    }
  }, [mode, value]);

  useEffect(() => {
    if (manual) return;

    const generated = slugify(sourceValue);
    if (generated !== value) {
      onChange(generated);
    }
  }, [manual, onChange, sourceValue, value]);

  useEffect(() => {
    const slug = slugify(value);
    const sequence = ++validationSequence.current;

    if (!validateSlug || !slug || disabled) {
      const next = { checking: false, error: null };
      setValidation(next);
      onValidationChange?.(next);
      return;
    }

    const checkingState = { checking: true, error: null };
    setValidation(checkingState);
    onValidationChange?.(checkingState);

    const timer = window.setTimeout(() => {
      void validateSlug(slug)
        .then((error) => {
          if (validationSequence.current !== sequence) return;
          const next = { checking: false, error };
          setValidation(next);
          onValidationChange?.(next);
        })
        .catch(() => {
          if (validationSequence.current !== sequence) return;
          const next = {
            checking: false,
            error: "Không thể kiểm tra slug lúc này.",
          };
          setValidation(next);
          onValidationChange?.(next);
        });
    }, Math.max(0, validationDelayMs));

    return () => window.clearTimeout(timer);
  }, [disabled, onValidationChange, validateSlug, validationDelayMs, value]);

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
          aria-invalid={Boolean(validation.error)}
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

      <div className="flex flex-wrap items-center justify-between gap-2 text-[12px] leading-5">
        <span className={validation.error ? "text-[#b42318]" : "text-[#8a8a8a]"}>
          {validation.error ?? preview}
        </span>
        <span className="inline-flex items-center gap-1.5 text-[#8a8a8a]">
          {validation.checking ? (
            <>
              <Loader2 size={13} className="animate-spin" /> Đang kiểm tra slug...
            </>
          ) : validateSlug && value && !validation.error ? (
            <>
              <CheckCircle2 size={13} /> Slug có thể sử dụng
            </>
          ) : manual ? (
            mode === "edit"
              ? "URL hiện tại được giữ nguyên cho đến khi bạn sửa slug."
              : "Slug đang được chỉnh thủ công."
          ) : (
            "Đang tự động theo tên."
          )}
        </span>
      </div>
    </label>
  );
}
