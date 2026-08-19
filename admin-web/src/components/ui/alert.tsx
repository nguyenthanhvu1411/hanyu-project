import {
  AlertCircle,
  CheckCircle2,
  Info,
  TriangleAlert,
} from "lucide-react";

import { cn } from "@/lib/utils/cn";

type AlertVariant =
  | "default"
  | "info"
  | "success"
  | "warning"
  | "danger";

interface AlertProps {
  title?: string;
  children: React.ReactNode;
  variant?: AlertVariant;
  className?: string;
}

const styles = {
  default: "border-[#e5e1da] bg-[#faf9f7] text-[#555]",
  info: "border-[#cfdef2] bg-[#f1f6fd] text-[#3973b8]",
  success: "border-[#cbe7d7] bg-[#edf8f2] text-[#168152]",
  warning: "border-[#f0ddb1] bg-[#fff8e7] text-[#af7917]",
  danger: "border-[#f1c9c5] bg-[#fff1ef] text-[#d9362f]",
};

const icons = {
  default: Info,
  info: Info,
  success: CheckCircle2,
  warning: TriangleAlert,
  danger: AlertCircle,
};

export function Alert({
  title,
  children,
  variant = "default",
  className,
}: AlertProps) {
  const Icon = icons[variant];

  return (
    <div
      className={cn(
        "flex items-start gap-3 rounded-[9px] border p-3",
        styles[variant],
        className,
      )}
    >
      <Icon size={17} className="mt-[1px] shrink-0" />

      <div className="min-w-0">
        {title && <div className="text-[13px] font-semibold">{title}</div>}
        <div className="mt-0.5 text-[13px] leading-5">{children}</div>
      </div>
    </div>
  );
}
