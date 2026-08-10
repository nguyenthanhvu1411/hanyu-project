import {
  cn,
} from "@/lib/utils/cn";

type StatusVariant =
  | "success"
  | "warning"
  | "danger"
  | "neutral"
  | "info";

interface StatusBadgeProps {
  children: React.ReactNode;
  variant?: StatusVariant;
}

const styles: Record<
  StatusVariant,
  string
> = {
  success:
    "bg-[#edf8f2] text-[#148553]",

  warning:
    "bg-[#fff7e4] text-[#b87b11]",

  danger:
    "bg-[#fff0ee] text-[#db2b24]",

  neutral:
    "bg-[#f2f2f2] text-[#666]",

  info:
    "bg-[#eef5ff] text-[#3973b8]",
};

export function StatusBadge({
  children,
  variant = "neutral",
}: StatusBadgeProps) {
  return (
    <span
      className={cn(
        "inline-flex",
        "items-center",
        "rounded-full",
        "px-[9px]",
        "py-[4px]",
        "text-[10px]",
        "font-semibold",
        styles[
          variant
        ],
      )}
    >
      {children}
    </span>
  );
}
