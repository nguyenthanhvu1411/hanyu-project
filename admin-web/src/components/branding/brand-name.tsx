import { cn } from "@/lib/utils/cn";

interface BrandNameProps {
  className?: string;
}

export function BrandName({
  className,
}: BrandNameProps) {
  return (
    <span
      className={cn(
        "whitespace-nowrap text-[17px]",
        "font-bold tracking-[-0.4px]",
        "text-[#202124]",
        className,
      )}
    >
      HỌC TIẾNG TRUNG
    </span>
  );
}
