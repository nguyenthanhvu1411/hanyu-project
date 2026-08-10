"use client";

import {
  AlertTriangle,
  RefreshCw,
} from "lucide-react";

import {
  Button,
} from "@/components/ui/button";

interface ErrorPageProps {
  reset: () => void;
}

export default function ErrorPage({
  reset,
}: ErrorPageProps) {
  return (
    <main
      className="
        flex min-h-screen
        items-center
        justify-center
        bg-[#fffaf0]
        p-6
      "
    >
      <div
        className="
          w-full max-w-[450px]
          rounded-[16px]
          border border-[#eee5da]
          bg-white
          p-8
          text-center
          shadow-sm
        "
      >
        <div
          className="
            mx-auto
            flex h-14 w-14
            items-center
            justify-center
            rounded-full
            bg-[#fff1ef]
            text-[#ef241c]
          "
        >
          <AlertTriangle size={28} />
        </div>

        <h1
          className="
            mt-5
            text-[20px]
            font-semibold
          "
        >
          Đã xảy ra lỗi
        </h1>

        <p
          className="
            mt-2
            text-[13px]
            leading-5
            text-[#777]
          "
        >
          Hệ thống gặp sự cố khi xử lý yêu cầu.
          Vui lòng thử lại.
        </p>

        <Button
          type="button"
          onClick={reset}
          className="mt-6"
        >
          <RefreshCw
            size={16}
            className="mr-2"
          />

          Thử lại
        </Button>
      </div>
    </main>
  );
}
