interface DataTableToolbarProps {
  left?: React.ReactNode;
  right?: React.ReactNode;
}

export function DataTableToolbar({
  left,
  right,
}: DataTableToolbarProps) {
  return (
    <div
      className="
        flex
        flex-col
        gap-3
        border-b
        border-[#eee9e2]
        px-4
        py-3
        sm:flex-row
        sm:items-center
        sm:justify-between
      "
    >
      <div
        className="
          flex
          flex-1
          flex-col
          gap-2
          sm:flex-row
          sm:items-center
        "
      >
        {left}
      </div>

      {right && (
        <div
          className="
            flex
            flex-wrap
            items-center
            gap-2
          "
        >
          {right}
        </div>
      )}
    </div>
  );
}
