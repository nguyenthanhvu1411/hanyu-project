import {
  Loader2,
} from "lucide-react";

interface PageLoadingProps {
  text?: string;
}

export function PageLoading({
  text = "Đang tải dữ liệu...",
}: PageLoadingProps) {
  return (
    <div
      className="
        flex
        min-h-[320px]
        flex-col
        items-center
        justify-center
      "
    >
      <Loader2
        size={26}
        className="
          animate-spin
          text-[#ef241c]
        "
      />

      <div
        className="
          mt-3
          text-[11px]
          text-[#858585]
        "
      >
        {text}
      </div>
    </div>
  );
}
