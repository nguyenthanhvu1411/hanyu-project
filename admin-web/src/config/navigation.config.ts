import {
  Activity,
  Bell,
  BookOpen,
  BookText,
  Brain,
  ChartNoAxesCombined,
  CircleUserRound,
  ClipboardCheck,
  FileQuestion,
  FolderKanban,
  Gauge,
  GraduationCap,
  Languages,
  LibraryBig,
  ListChecks,
  MessageSquareText,
  MonitorSmartphone,
  NotebookTabs,
  PanelsTopLeft,
  Settings,
  ShieldCheck,
  Tags,
  Target,
  UsersRound,
} from "lucide-react";

import { ROUTES } from "@/constants/route.constants";
import { PERMISSIONS } from "@/constants/permission.constants";
import type {
  NavigationGroup,
} from "@/types/navigation.types";

export const ADMIN_NAVIGATION: NavigationGroup[] = [
  {
    items: [
      {
        title: "Tổng quan",
        href: ROUTES.TONG_QUAN,
        icon: Gauge,
      },
    ],
  },

  {
    title: "Quản lý hệ thống",

    items: [
      {
        title: "Người dùng",
        href: ROUTES.NGUOI_DUNG,
        icon: UsersRound,
      },

      {
        title: "Vai trò",
        href: ROUTES.VAI_TRO,
        icon: ShieldCheck,
      },

      {
        title: "Quyền hạn",
        href: ROUTES.QUYEN_HAN,
        icon: ListChecks,
      },

      {
        title: "Phiên đăng nhập",
        href: ROUTES.PHIEN_DANG_NHAP,
        icon: MonitorSmartphone,
      },
    ],
  },

  {
    title: "Nội dung học tập",

    items: [
      {
        title: "Cấp độ HSK",
        href: ROUTES.CAP_DO_HSK,
        icon: GraduationCap,
        permission: PERMISSIONS.HSK_LEVELS.READ,
      },

      {
        title: "Khóa học",
        href: ROUTES.KHOA_HOC,
        icon: BookOpen,
      },

      {
        title: "Chương học",
        href: ROUTES.CHUONG_HOC,
        icon: LibraryBig,
      },

      {
        title: "Bài giảng",
        href: ROUTES.BAI_GIANG,
        icon: BookText,
      },

      {
        title: "Chủ đề",
        href: ROUTES.CHU_DE_TU_VUNG,
        icon: FolderKanban,
      },
    ],
  },

  {
    title: "Từ vựng",

    items: [
      {
        title: "Từ vựng",
        href: ROUTES.TU_VUNG,
        icon: Languages,
      },

      {
        title: "Nghĩa từ vựng",
        href: ROUTES.NGHIA_TU_VUNG,
        icon: NotebookTabs,
      },

      {
        title: "Ví dụ từ vựng",
        href: ROUTES.VI_DU_TU_VUNG,
        icon: MessageSquareText,
      },

      {
        title: "Quan hệ từ vựng",
        href: ROUTES.QUAN_HE_TU_VUNG,
        icon: PanelsTopLeft,
      },

      {
        title: "Loại từ",
        href: ROUTES.LOAI_TU,
        icon: Tags,
      },
    ],
  },

  {
    title: "Học tập",

    items: [
      {
        title: "Mục tiêu học tập",
        href: ROUTES.MUC_TIEU_HOC_TAP,
        icon: Target,
      },

      {
        title: "Hoạt động học tập",
        href: ROUTES.HOAT_DONG_HOC_TAP,
        icon: Activity,
      },

      {
        title: "Tổng hợp học tập",
        href: ROUTES.TONG_HOP_HOC_TAP,
        icon: ChartNoAxesCombined,
      },
    ],
  },

  {
    title: "Kiểm tra",

    items: [
      {
        title: "Ngân hàng câu hỏi",
        href: ROUTES.NGAN_HANG_CAU_HOI,
        icon: Brain,
      },

      {
        title: "Câu hỏi",
        href: ROUTES.CAU_HOI,
        icon: FileQuestion,
      },

      {
        title: "Bài kiểm tra",
        href: ROUTES.BAI_KIEM_TRA,
        icon: ClipboardCheck,
      },

      {
        title: "Lượt làm bài",
        href: ROUTES.LUOT_LAM_BAI,
        icon: ListChecks,
      },
    ],
  },

  {
    title: "Hệ thống",

    items: [
      {
        title: "Thông báo",
        href: ROUTES.THONG_BAO,
        icon: Bell,
      },

      {
        title: "Nhật ký hệ thống",
        href: ROUTES.NHAT_KY_HE_THONG,
        icon: Activity,
      },

      {
        title: "Cấu hình hệ thống",
        href: ROUTES.CAU_HINH_HE_THONG,
        icon: Settings,
      },

      {
        title: "Hồ sơ",
        href: ROUTES.HO_SO,
        icon: CircleUserRound,
      },
    ],
  },
];
