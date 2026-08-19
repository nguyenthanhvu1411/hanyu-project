import { cn } from "@/lib/utils/cn";

type BadgeVariant =
  | "default"
  | "primary"
  | "success"
  | "warning"
  | "danger"
  | "info"
  | "secondary"
  | "destructive";

interface BadgeProps {
  children: React.ReactNode;
  variant?: BadgeVariant;
  className?: string;
}

const variants: Record<BadgeVariant, string> = {
  default: "bg-[#f2f2f2] text-[#666]",
  primary: "bg-[#fff0ee] text-[#d9342d]",
  success: "bg-[#edf8f2] text-[#168152]",
  warning: "bg-[#fff7e4] text-[#b77c14]",
  danger: "bg-[#fff0ee] text-[#e33730]",
  info: "bg-[#eef5ff] text-[#3973b8]",
  secondary: "bg-[#f2f2f2] text-[#666]",
  destructive: "bg-[#fff0ee] text-[#e33730]",
};

export function Badge({ children, variant = "default", className }: BadgeProps) {
  return (
    <span
      className={cn(
        "inline-flex items-center rounded-full px-2.5 py-1 text-[12px] font-medium",
        variants[variant],
        className,
      )}
    >
      {children}
    </span>
  );
}
