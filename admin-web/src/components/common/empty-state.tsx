import {
  Inbox,
} from "lucide-react";

import {
  Button,
} from "@/components/ui/button";

interface EmptyStateProps {
  title?: string;

  description?: string;

  icon?: React.ReactNode;

  actionLabel?: string;

  onAction?: () => void;
}

export function EmptyState({
  title = "Chưa có dữ liệu",
  description =
    "Hiện chưa có dữ liệu để hiển thị.",
  icon,
  actionLabel,
  onAction,
}: EmptyStateProps) {
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
          bg-[#faf5ec]
          text-[#ba9c70]
        "
      >
        {icon ?? (
          <Inbox
            size={25}
          />
        )}
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
          max-w-[360px]
          text-[11px]
          leading-[17px]
          text-[#8e8e8e]
        "
      >
        {description}
      </p>

      {actionLabel &&
        onAction && (
          <Button
            type="button"
            onClick={
              onAction
            }
            className="
              mt-5
              h-[37px]
              text-[11px]
            "
          >
            {actionLabel}
          </Button>
        )}
    </div>
  );
}
