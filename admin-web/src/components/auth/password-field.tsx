"use client";

import {
  Eye,
  EyeOff,
  LockKeyhole,
} from "lucide-react";
import {
  forwardRef,
  useState,
} from "react";

import { Input } from "@/components/ui/input";
import { cn } from "@/lib/utils/cn";

interface PasswordFieldProps
  extends React.InputHTMLAttributes<HTMLInputElement> {
  error?: boolean;
}

export const PasswordField =
  forwardRef<
    HTMLInputElement,
    PasswordFieldProps
  >(function PasswordField(
    {
      className,
      error,
      ...props
    },
    ref,
  ) {
    const [visible, setVisible] =
      useState(false);

    return (
      <div className="relative">
        <LockKeyhole
          size={20}
          strokeWidth={1.7}
          className="
            pointer-events-none
            absolute left-[17px] top-1/2
            -translate-y-1/2
            text-[#666]
          "
        />

        <Input
          ref={ref}
          type={
            visible
              ? "text"
              : "password"
          }
          error={error}
          className={cn(
            "pl-[50px] pr-[52px]",
            className,
          )}
          {...props}
        />

        <button
          type="button"
          aria-label={
            visible
              ? "Ẩn mật khẩu"
              : "Hiện mật khẩu"
          }
          onClick={() =>
            setVisible(
              (value) => !value,
            )
          }
          className="
            absolute right-[14px] top-1/2
            flex h-8 w-8
            -translate-y-1/2
            items-center justify-center
            rounded-md
            text-[#4c4c4c]
            hover:bg-[#f5f5f5]
          "
        >
          {visible ? (
            <EyeOff
              size={19}
              strokeWidth={1.8}
            />
          ) : (
            <Eye
              size={19}
              strokeWidth={1.8}
            />
          )}
        </button>
      </div>
    );
  });
