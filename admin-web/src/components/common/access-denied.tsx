"use client";

import {
  ArrowLeft,
  Home,
  ShieldX,
} from "lucide-react";

import Link from "next/link";

import {
  Button,
} from "@/components/ui/button";

import {
  ROUTES,
} from "@/constants/route.constants";

interface AccessDeniedProps {
  title?: string;

  description?: string;

  showBackButton?: boolean;

  showHomeButton?: boolean;
}

export function AccessDenied({
  title = "Bạn không có quyền truy cập",
  description = "Tài khoản hiện tại không được cấp quyền để truy cập chức năng này.",
  showBackButton = true,
  showHomeButton = true,
}: AccessDeniedProps) {
  return (
    <div
      className="
        flex
        min-h-[420px]
        flex-col
        items-center
        justify-center
        px-5
        py-12
        text-center
      "
    >
      <div
        className="
          flex h-16 w-16
          items-center
          justify-center
          rounded-[16px]
          bg-[#fff0ee]
          text-[#ef241c]
        "
      >
        <ShieldX
          size={30}
          strokeWidth={1.7}
        />
      </div>

      <h2
        className="
          mt-5
          text-[18px]
          font-semibold
          text-[#333]
        "
      >
        {title}
      </h2>

      <p
        className="
          mt-2
          max-w-[440px]
          text-[12px]
          leading-[19px]
          text-[#858585]
        "
      >
        {description}
      </p>

      <div
        className="
          mt-6
          flex
          flex-wrap
          justify-center
          gap-2
        "
      >
        {showBackButton && (
          <Button
            type="button"
            variant="outline"
            onClick={() =>
              history.back()
            }
            className="
              h-[38px]
              gap-2
              text-[12px]
            "
          >
            <ArrowLeft
              size={15}
            />

            Quay lại
          </Button>
        )}

        {showHomeButton && (
          <Link
            href={
              ROUTES.TONG_QUAN
            }
          >
            <Button
              className="
                h-[38px]
                gap-2
                text-[12px]
              "
            >
              <Home
                size={15}
              />

              Về tổng quan
            </Button>
          </Link>
        )}
      </div>
    </div>
  );
}
