"use client";

import { ArrowRight, UsersRound } from "lucide-react";
import Link from "next/link";
import { Button } from "@/components/ui/button";

interface RoleUsersPreviewProps {
  roleId: string;
  total: number;
}

export function RoleUsersPreview({ roleId, total }: RoleUsersPreviewProps) {
  return (
    <div className="flex flex-col gap-4 rounded-[9px] border border-[#e9e4dc] bg-[#faf9f7] p-4 sm:flex-row sm:items-center sm:justify-between">
      <div className="flex items-center gap-3">
        <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-[9px] bg-[#eef5ff] text-[#3973b8]">
          <UsersRound size={18} />
        </div>
        <div>
          <div className="text-[20px] font-semibold text-[#333]">{total}</div>
          <div className="text-[10px] text-[#888]">người dùng đang được gán vai trò này</div>
        </div>
      </div>
      <Link href={`/nguoi-dung?roleId=${roleId}`}>
        <Button type="button" variant="outline" className="h-[36px] gap-2 text-[10px]">
          Xem người dùng
          <ArrowRight size={13} />
        </Button>
      </Link>
    </div>
  );
}
