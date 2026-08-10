interface StatisticCardProps {
  label: string;

  value:
    | number
    | string;

  icon?: React.ReactNode;

  helper?: string;

  accent?:
    | "red"
    | "green"
    | "gold"
    | "blue";
}

const styles = {
  red:
    "bg-[#fff0ee] text-[#ef241c]",

  green:
    "bg-[#edf8f2] text-[#16975b]",

  gold:
    "bg-[#fff7e4] text-[#c58c24]",

  blue:
    "bg-[#eef5ff] text-[#3973b8]",
};

export function StatisticCard({
  label,
  value,
  icon,
  helper,
  accent = "red",
}: StatisticCardProps) {
  return (
    <div
      className="
        flex
        items-center
        gap-3
        rounded-[10px]
        border
        border-[#e8e3dc]
        bg-white
        p-3
      "
    >
      {icon && (
        <div
          className={`
            flex h-10 w-10
            shrink-0
            items-center
            justify-center
            rounded-[9px]
            ${styles[accent]}
          `}
        >
          {icon}
        </div>
      )}

      <div className="min-w-0">
        <div
          className="
            text-[10px]
            text-[#888]
          "
        >
          {label}
        </div>

        <div
          className="
            mt-[2px]
            text-[18px]
            font-semibold
            text-[#333]
          "
        >
          {value}
        </div>

        {helper && (
          <div
            className="
              mt-[2px]
              truncate
              text-[9px]
              text-[#aaa]
            "
          >
            {helper}
          </div>
        )}
      </div>
    </div>
  );
}
