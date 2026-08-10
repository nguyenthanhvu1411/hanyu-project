import {
  cn,
} from "@/lib/utils/cn";

interface ScrollAreaProps {
  children: React.ReactNode;

  className?: string;
}

export function ScrollArea({
  children,
  className,
}: ScrollAreaProps) {
  return (
    <div
      className={cn(
        "scrollbar-thin",
        "overflow-auto",
        className,
      )}
    >
      {children}
    </div>
  );
}
