import * as React from "react";

import { cn } from "@/lib/utils/cn";

interface LabelProps
  extends React.LabelHTMLAttributes<HTMLLabelElement> {}

export function Label({
  className,
  ...props
}: LabelProps) {
  return (
    <label
      className={cn(
        "mb-[8px] block",
        "text-[14px]",
        "font-medium",
        "text-[#202124]",
        className,
      )}
      {...props}
    />
  );
}
