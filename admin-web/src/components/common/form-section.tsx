import { cn } from "@/lib/utils/cn";

interface FormSectionProps {
  title: string;

  description?: string;

  icon?: React.ReactNode;

  children: React.ReactNode;

  className?: string;

  contentClassName?: string;

  actions?: React.ReactNode;
}

export function FormSection({
  title,
  description,
  icon,
  children,
  className,
  contentClassName,
  actions,
}: FormSectionProps) {
  return (
    <section
      className={cn(
        "relative",
        "rounded-[11px]",
        "border border-[#e8e3dc]",
        "bg-white",
        "shadow-[0_2px_10px_rgba(0,0,0,0.025)]",

        // QUAN TRỌNG
        "overflow-visible",

        className,
      )}
    >
      <div
        className="
          flex
          min-h-[66px]
          items-center
          justify-between
          gap-4
          border-b
          border-[#eee9e2]
          px-5
          py-4
        "
      >
        <div className="flex min-w-0 items-start gap-3">
          {icon && (
            <div
              className="
                flex
                h-10 w-10
                shrink-0
                items-center
                justify-center
                rounded-[9px]
                bg-[#fff0ee]
                text-[#ef241c]
              "
            >
              {icon}
            </div>
          )}

          <div className="min-w-0">
            <h2
              className="
                text-[14px]
                font-semibold
                leading-[20px]
                text-[#292929]
              "
            >
              {title}
            </h2>

            {description && (
              <p
                className="
                  mt-[2px]
                  text-[10px]
                  leading-[16px]
                  text-[#888]
                "
              >
                {description}
              </p>
            )}
          </div>
        </div>

        {actions && (
          <div className="shrink-0">
            {actions}
          </div>
        )}
      </div>

      <div
        className={cn(
          "relative p-5",

          // cũng phải visible
          "overflow-visible",

          contentClassName,
        )}
      >
        {children}
      </div>
    </section>
  );
}
