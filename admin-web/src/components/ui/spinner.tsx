import {
  Loader2,
} from "lucide-react";

import {
  cn,
} from "@/lib/utils/cn";

interface SpinnerProps {
  size?:
    | "sm"
    | "md"
    | "lg";

  className?: string;
}

const sizes = {
  sm: 14,
  md: 20,
  lg: 28,
};

export function Spinner({
  size = "md",
  className,
}: SpinnerProps) {
  return (
    <Loader2
      size={
        sizes[size]
      }
      className={cn(
        "animate-spin",
        "text-[#ef241c]",
        className,
      )}
    />
  );
}
