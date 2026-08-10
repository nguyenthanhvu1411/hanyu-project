"use client";

import {
  ChevronRight,
  Home,
} from "lucide-react";

import Link from "next/link";

import {
  usePathname,
} from "next/navigation";

import {
  ROUTES,
} from "@/constants/route.constants";

const LABELS: Record<
  string,
  string
> = {
  "tong-quan":
    "Tổng quan",

  "nguoi-dung":
    "Người dùng",

  "vai-tro":
    "Vai trò",

  "quyen-han":
    "Quyền hạn",

  "cap-do-hsk":
    "Cấp độ HSK",

  "khoa-hoc":
    "Khóa học",

  "chuong-hoc":
    "Chương học",

  "bai-giang":
    "Bài giảng",

  "tu-vung":
    "Từ vựng",

  "nghia-tu-vung":
    "Nghĩa từ vựng",

  "vi-du-tu-vung":
    "Ví dụ từ vựng",

  "quan-he-tu-vung":
    "Quan hệ từ vựng",

  "chu-de-tu-vung":
    "Chủ đề từ vựng",

  "loai-tu":
    "Loại từ",

  "muc-tieu-hoc-tap":
    "Mục tiêu học tập",

  "hoat-dong-hoc-tap":
    "Hoạt động học tập",

  "tong-hop-hoc-tap":
    "Tổng hợp học tập",

  "ngan-hang-cau-hoi":
    "Ngân hàng câu hỏi",

  "cau-hoi":
    "Câu hỏi",

  "bai-kiem-tra":
    "Bài kiểm tra",

  "luot-lam-bai":
    "Lượt làm bài",

  "thong-bao":
    "Thông báo",

  "nhat-ky-he-thong":
    "Nhật ký hệ thống",

  "cau-hinh-he-thong":
    "Cấu hình hệ thống",

  "ho-so":
    "Hồ sơ",

  "them-moi":
    "Thêm mới",

  "chinh-sua":
    "Chỉnh sửa",

  "bao-mat":
    "Bảo mật",

  "thong-ke":
    "Thống kê",

  "hoc-vien":
    "Học viên",
};

function formatSegment(
  segment: string,
) {
  if (
    LABELS[segment]
  ) {
    return LABELS[
      segment
    ];
  }

  if (
    segment.length >
    18
  ) {
    return "Chi tiết";
  }

  return segment
    .split("-")
    .map(
      (word) =>
        word
          .charAt(0)
          .toUpperCase() +
        word.slice(1),
    )
    .join(" ");
}

export function Breadcrumb() {
  const pathname =
    usePathname();

  const segments =
    pathname
      .split("/")
      .filter(Boolean);

  if (
    pathname ===
    ROUTES.TONG_QUAN
  ) {
    return null;
  }

  return (
    <nav
      aria-label="Breadcrumb"
      className="
        flex
        min-w-0
        items-center
        gap-1
        overflow-hidden
        text-[11px]
        text-[#8a8a8a]
      "
    >
      <Link
        href={
          ROUTES.TONG_QUAN
        }
        className="
          flex
          shrink-0
          items-center
          gap-1
          transition
          hover:text-[#ef241c]
        "
      >
        <Home
          size={13}
        />

        <span className="hidden sm:inline">
          Tổng quan
        </span>
      </Link>

      {segments.map(
        (
          segment,
          index,
        ) => {
          const href =
            "/" +
            segments
              .slice(
                0,
                index +
                  1,
              )
              .join(
                "/",
              );

          const isLast =
            index ===
            segments.length -
              1;

          return (
            <div
              key={
                href
              }
              className="
                flex
                min-w-0
                items-center
                gap-1
              "
            >
              <ChevronRight
                size={12}
                className="shrink-0"
              />

              {isLast ? (
                <span
                  className="
                    max-w-[180px]
                    truncate
                    font-medium
                    text-[#555]
                  "
                >
                  {formatSegment(
                    segment,
                  )}
                </span>
              ) : (
                <Link
                  href={
                    href
                  }
                  className="
                    max-w-[160px]
                    truncate
                    transition
                    hover:text-[#ef241c]
                  "
                >
                  {formatSegment(
                    segment,
                  )}
                </Link>
              )}
            </div>
          );
        },
      )}
    </nav>
  );
}
