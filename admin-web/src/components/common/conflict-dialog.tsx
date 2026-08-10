"use client";

import {
  TriangleAlert,
} from "lucide-react";

import {
  Button,
} from "@/components/ui/button";

interface ConflictDialogProps {
  open: boolean;

  title?: string;

  description: string;

  traceId?: string;

  onClose: () => void;
}

export function ConflictDialog({
  open,
  title =
    "Không thể thực hiện thao tác",
  description,
  traceId,
  onClose,
}: ConflictDialogProps) {
  if (!open) {
    return null;
  }

  return (
    <div
      className="
        fixed inset-0
        z-[140]
        flex items-center
        justify-center
        p-4
      "
    >
      <button
        type="button"
        onClick={
          onClose
        }
        className="
          absolute inset-0
          bg-black/40
        "
      />

      <div
        className="
          relative
          w-full
          max-w-[460px]
          rounded-[14px]
          bg-white
          shadow-2xl
        "
      >
        <div className="p-5">
          <div
            className="
              flex
              items-start
              gap-3
            "
          >
            <div
              className="
                flex h-10 w-10
                items-center
                justify-center
                rounded-[9px]
                bg-[#fff7e4]
                text-[#b67d18]
              "
            >
              <TriangleAlert
                size={19}
              />
            </div>

            <div>
              <h2
                className="
                  text-[14px]
                  font-semibold
                "
              >
                {title}
              </h2>

              <p
                className="
                  mt-2
                  text-[11px]
                  leading-[18px]
                  text-[#777]
                "
              >
                {description}
              </p>

              {traceId && (
                <div
                  className="
                    mt-3
                    font-mono
                    text-[9px]
                    text-[#aaa]
                  "
                >
                  Trace ID:{" "}
                  {traceId}
                </div>
              )}
            </div>
          </div>
        </div>

        <div
          className="
            flex
            justify-end
            border-t
            border-[#eee]
            px-5 py-3
          "
        >
          <Button
            type="button"
            onClick={
              onClose
            }
          >
            Đóng
          </Button>
        </div>
      </div>
    </div>
  );
}
