import {
  ArrowDownRight,
  ArrowUpRight,
  Minus,
} from "lucide-react";

interface MetricCardProps {
  title: string;

  value:
    | string
    | number;

  icon?: React.ReactNode;

  description?: string;

  change?: number;

  suffix?: string;
}

export function MetricCard({
  title,
  value,
  icon,
  description,
  change,
  suffix,
}: MetricCardProps) {
  const positive =
    change !==
      undefined &&
    change > 0;

  const negative =
    change !==
      undefined &&
    change < 0;

  return (
    <div
      className="
        rounded-[11px]
        border
        border-[#e8e3dc]
        bg-white
        p-4
        shadow-[0_2px_8px_rgba(0,0,0,0.025)]
      "
    >
      <div
        className="
          flex
          items-start
          justify-between
          gap-3
        "
      >
        <div>
          <div
            className="
              text-[11px]
              text-[#858585]
            "
          >
            {title}
          </div>

          <div
            className="
              mt-2
              text-[24px]
              font-semibold
              tracking-[-0.5px]
              text-[#292929]
            "
          >
            {value}

            {suffix && (
              <span
                className="
                  ml-1
                  text-[12px]
                  font-normal
                  text-[#888]
                "
              >
                {suffix}
              </span>
            )}
          </div>
        </div>

        {icon && (
          <div
            className="
              flex h-10 w-10
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
      </div>

      {(change !==
        undefined ||
        description) && (
        <div
          className="
            mt-3
            flex
            items-center
            gap-2
            text-[10px]
          "
        >
          {change !==
            undefined && (
            <span
              className={
                positive
                  ? "flex items-center gap-1 text-[#16975b]"
                  : negative
                    ? "flex items-center gap-1 text-[#ef241c]"
                    : "flex items-center gap-1 text-[#888]"
              }
            >
              {positive ? (
                <ArrowUpRight
                  size={12}
                />
              ) : negative ? (
                <ArrowDownRight
                  size={12}
                />
              ) : (
                <Minus
                  size={12}
                />
              )}

              {Math.abs(
                change,
              )}
              %
            </span>
          )}

          {description && (
            <span
              className="text-[#929292]"
            >
              {description}
            </span>
          )}
        </div>
      )}
    </div>
  );
}
