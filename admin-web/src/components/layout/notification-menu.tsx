"use client";

import {
  Bell,
  CheckCheck,
} from "lucide-react";

import {
  useEffect,
  useRef,
  useState,
} from "react";

import {
  Button,
} from "@/components/ui/button";

export function NotificationMenu() {
  const [open, setOpen] =
    useState(false);

  const ref =
    useRef<HTMLDivElement>(
      null,
    );

  useEffect(() => {
    function handleClick(
      event: MouseEvent,
    ) {
      if (
        ref.current &&
        !ref.current.contains(
          event.target as Node,
        )
      ) {
        setOpen(false);
      }
    }

    document.addEventListener(
      "mousedown",
      handleClick,
    );

    return () =>
      document.removeEventListener(
        "mousedown",
        handleClick,
      );
  }, []);

  return (
    <div
      ref={ref}
      className="relative"
    >
      <Button
        type="button"
        size="icon"
        variant="ghost"
        onClick={() =>
          setOpen(
            (value) =>
              !value,
          )
        }
        className="
          relative
          h-9 w-9
          text-[#626262]
        "
      >
        <Bell
          size={19}
        />

        <span
          className="
            absolute
            right-[5px]
            top-[5px]
            h-[7px]
            w-[7px]
            rounded-full
            border-2
            border-white
            bg-[#ef241c]
          "
        />
      </Button>

      {open && (
        <div
          className="
            absolute
            right-0
            top-[45px]
            w-[340px]
            overflow-hidden
            rounded-[12px]
            border
            border-[#e8e3dc]
            bg-white
            shadow-[0_18px_45px_rgba(0,0,0,0.10)]
          "
        >
          <div
            className="
              flex
              items-center
              justify-between
              border-b
              border-[#eee9e2]
              px-4
              py-3
            "
          >
            <div>
              <div
                className="
                  text-[14px]
                  font-semibold
                  text-[#292929]
                "
              >
                Thông báo
              </div>

              <div
                className="
                  mt-[2px]
                  text-[11px]
                  text-[#8c8c8c]
                "
              >
                Bạn có 3 thông báo mới
              </div>
            </div>

            <button
              type="button"
              className="
                flex
                items-center
                gap-1
                text-[11px]
                text-[#16975b]
              "
            >
              <CheckCheck
                size={14}
              />

              Đánh dấu đã đọc
            </button>
          </div>

          <div className="max-h-[360px] overflow-y-auto">
            <NotificationItem
              title="Có người dùng mới đăng ký"
              description="Tài khoản Nguyễn Văn A vừa được tạo."
              time="2 phút trước"
              unread
            />

            <NotificationItem
              title="Khóa học mới được cập nhật"
              description="HSK 3 - Giao tiếp cơ bản vừa thay đổi nội dung."
              time="25 phút trước"
              unread
            />

            <NotificationItem
              title="Bài kiểm tra đã hoàn tất"
              description="Đợt kiểm tra HSK 2 vừa có kết quả."
              time="1 giờ trước"
              unread
            />

            <NotificationItem
              title="Hệ thống"
              description="Không có sự cố hệ thống trong 24 giờ qua."
              time="Hôm qua"
            />
          </div>

          <button
            type="button"
            className="
              h-11
              w-full
              border-t
              border-[#eee9e2]
              text-[12px]
              font-medium
              text-[#16975b]
              hover:bg-[#fafafa]
            "
          >
            Xem tất cả thông báo
          </button>
        </div>
      )}
    </div>
  );
}

interface NotificationItemProps {
  title: string;
  description: string;
  time: string;
  unread?: boolean;
}

function NotificationItem({
  title,
  description,
  time,
  unread = false,
}: NotificationItemProps) {
  return (
    <button
      type="button"
      className="
        relative
        block
        w-full
        border-b
        border-[#f1ede7]
        px-4
        py-3
        text-left
        transition
        last:border-b-0
        hover:bg-[#fffaf8]
      "
    >
      {unread && (
        <span
          className="
            absolute
            right-4
            top-4
            h-2
            w-2
            rounded-full
            bg-[#ef241c]
          "
        />
      )}

      <div
        className="
          pr-5
          text-[13px]
          font-medium
          text-[#333]
        "
      >
        {title}
      </div>

      <div
        className="
          mt-1
          pr-5
          text-[11px]
          leading-[17px]
          text-[#777]
        "
      >
        {description}
      </div>

      <div
        className="
          mt-[5px]
          text-[10px]
          text-[#a0a0a0]
        "
      >
        {time}
      </div>
    </button>
  );
}
