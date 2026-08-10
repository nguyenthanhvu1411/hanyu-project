interface DataTableSkeletonProps {
  rows?: number;
  columns?: number;
}

export function DataTableSkeleton({
  rows = 6,
  columns = 6,
}: DataTableSkeletonProps) {
  return (
    <div className="animate-pulse">
      {Array.from({
        length: rows,
      }).map(
        (
          _,
          row,
        ) => (
          <div
            key={
              row
            }
            className="
              flex
              gap-3
              border-b
              border-[#eee]
              px-4
              py-4
            "
          >
            {Array.from({
              length:
                columns,
            }).map(
              (
                _,
                col,
              ) => (
                <div
                  key={
                    col
                  }
                  className="
                    h-4
                    flex-1
                    rounded
                    bg-[#efefef]
                  "
                />
              ),
            )}
          </div>
        ),
      )}
    </div>
  );
}
