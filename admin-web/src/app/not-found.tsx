import {
  SearchX,
} from "lucide-react";

import Link from "next/link";

import {
  Button,
} from "@/components/ui/button";

import {
  ROUTES,
} from "@/constants/route.constants";

export default function NotFound() {
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
          w-full
          max-w-[460px]
          text-center
        "
      >
        <div
          className="
            mx-auto
            flex h-16 w-16
            items-center
            justify-center
            rounded-full
            bg-white
            text-[#ef241c]
            shadow-sm
          "
        >
          <SearchX size={30} />
        </div>

        <h1
          className="
            mt-5
            text-[24px]
            font-bold
          "
        >
          Không tìm thấy trang
        </h1>

        <p
          className="
            mt-2
            text-[14px]
            text-[#777]
          "
        >
          Đường dẫn bạn truy cập không tồn tại
          hoặc đã được thay đổi.
        </p>

        <Link href={ROUTES.DANG_NHAP}>
          <Button className="mt-6">
            Về trang đăng nhập
          </Button>
        </Link>
      </div>
    </main>
  );
}
