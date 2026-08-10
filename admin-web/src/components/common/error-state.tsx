import {
  AlertTriangle,
  RefreshCw,
} from "lucide-react";

import {
  Button,
} from "@/components/ui/button";

interface ErrorStateProps {
  title?: string;

  description?: string;

  retryText?: string;

  onRetry?: () => void;
}

export function ErrorState({
  title = "Không thể tải dữ liệu",
  description =
    "Đã xảy ra lỗi trong quá trình tải dữ liệu. Vui lòng thử lại.",
  retryText = "Thử lại",
  onRetry,
}: ErrorStateProps) {
  return (
    <div
      className="
        flex
        min-h-[280px]
        flex-col
        items-center
        justify-center
        px-5
        py-10
        text-center
      "
    >
      <div
        className="
          flex h-14 w-14
          items-center
          justify-center
          rounded-[14px]
          bg-[#fff0ee]
          text-[#ef241c]
        "
      >
        <AlertTriangle
          size={25}
        />
      </div>

      <h3
        className="
          mt-4
          text-[14px]
          font-semibold
          text-[#444]
        "
      >
        {title}
      </h3>

      <p
        className="
          mt-1
          max-w-[380px]
          text-[11px]
          leading-[17px]
          text-[#8c8c8c]
        "
      >
        {description}
      </p>

      {onRetry && (
        <Button
          type="button"
          variant="outline"
          onClick={
            onRetry
          }
          className="
            mt-5
            h-[37px]
            gap-2
            text-[11px]
          "
        >
          <RefreshCw
            size={14}
          />

          {retryText}
        </Button>
      )}
    </div>
  );
}
