import {
  Activity,
  BookOpen,
  Languages,
  UsersRound,
} from "lucide-react";

import {
  PageContainer,
} from "@/components/layout/page-container";

import {
  PageHeader,
} from "@/components/layout/page-header";

export default function DashboardPage() {
  return (
    <PageContainer>
      <PageHeader
        title="Tổng quan"
        description="Theo dõi nhanh tình trạng hệ thống Học Tiếng Trung."
      />

      <div
        className="
          grid
          gap-4
          sm:grid-cols-2
          xl:grid-cols-4
        "
      >
        <DashboardCard
          icon={
            <UsersRound
              size={21}
            />
          }
          label="Người dùng"
          value="12,480"
          change="+8.4%"
        />

        <DashboardCard
          icon={
            <BookOpen
              size={21}
            />
          }
          label="Khóa học"
          value="42"
          change="+3"
        />

        <DashboardCard
          icon={
            <Languages
              size={21}
            />
          }
          label="Từ vựng"
          value="8,624"
          change="+126"
        />

        <DashboardCard
          icon={
            <Activity
              size={21}
            />
          }
          label="Hoạt động hôm nay"
          value="3,248"
          change="+12.6%"
        />
      </div>
    </PageContainer>
  );
}

interface DashboardCardProps {
  icon: React.ReactNode;
  label: string;
  value: string;
  change: string;
}

function DashboardCard({
  icon,
  label,
  value,
  change,
}: DashboardCardProps) {
  return (
    <div
      className="
        rounded-[12px]
        border
        border-[#e9e4dc]
        bg-white
        p-4
        shadow-[0_2px_8px_rgba(0,0,0,0.025)]
      "
    >
      <div className="flex items-start justify-between">
        <div
          className="
            flex h-10 w-10
            items-center
            justify-center
            rounded-[9px]
            bg-[#fff0ee]
            text-[#ef241c]
          "
        >
          {icon}
        </div>

        <span
          className="
            rounded-full
            bg-[#edf8f2]
            px-2
            py-1
            text-[10px]
            font-semibold
            text-[#16975b]
          "
        >
          {change}
        </span>
      </div>

      <div
        className="
          mt-4
          text-[11px]
          text-[#858585]
        "
      >
        {label}
      </div>

      <div
        className="
          mt-1
          text-[24px]
          font-semibold
          tracking-[-0.4px]
          text-[#282828]
        "
      >
        {value}
      </div>
    </div>
  );
}
