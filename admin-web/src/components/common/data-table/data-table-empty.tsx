import {
  Inbox,
} from "lucide-react";

interface DataTableEmptyProps {
  title?: string;
  description?: string;
}

export function DataTableEmpty({
  title = "Chưa có dữ liệu",
  description = "Không tìm thấy dữ liệu phù hợp.",
}: DataTableEmptyProps) {
  return (
    <div
      className="
        flex min-h-[240px]
        flex-col
        items-center
        justify-center
        px-4
        text-center
      "
    >
      <div
        className="
          flex h-12 w-12
          items-center
          justify-center
          rounded-full
          bg-[#faf6ef]
          text-[#c7a875]
        "
      >
        <Inbox
          size={22}
        />
      </div>

      <div
        className="
          mt-3
          text-[13px]
          font-medium
          text-[#555]
        "
      >
        {title}
      </div>

      <div
        className="
          mt-1
          text-[11px]
          text-[#999]
        "
      >
        {description}
      </div>
    </div>
  );
}
