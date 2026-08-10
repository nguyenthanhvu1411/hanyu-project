import {
  cn,
} from "@/lib/utils/cn";

interface PageContainerProps {
  children: React.ReactNode;
  className?: string;
}

export function PageContainer({
  children,
  className,
}: PageContainerProps) {
  return (
    <main
      className={cn(
        "w-full",
        "space-y-5",
        "px-5",
        "py-5",
        "lg:px-7",
        "lg:py-6",
        "overflow-visible",
        className,
      )}
    >
      {children}
    </main>
  );
}
