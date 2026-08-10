"use client";

import { useCallback, useEffect, useState } from "react";
import Link from "next/link";
import { khoaHocApi } from "@/features/khoa-hoc/api/khoa-hoc.api";
import type { AdminCourseListItem, AdminCourseQuery } from "@/features/khoa-hoc/types/khoa-hoc.types";
import type { PagedResult } from "@/lib/api/api-result";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { getContentStatusLabel } from "@/lib/constants/content-status";
import { Plus } from "lucide-react";

export default function DanhSachKhoaHocPage() {
  const [duLieu, setDuLieu] = useState<PagedResult<AdminCourseListItem>>({
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
  });

  const [boLoc, setBoLoc] = useState<AdminCourseQuery>({
    page: 1,
    pageSize: 20,
    sortBy: "sortorder",
  });

  const [dangTai, setDangTai] = useState(true);
  const [loi, setLoi] = useState<string | null>(null);

  const taiDanhSach = useCallback(async () => {
    try {
      setDangTai(true);
      setLoi(null);

      const result = await khoaHocApi.danhSach(boLoc);
      setDuLieu(result);
    } catch (error) {
      setLoi(error instanceof Error ? error.message : "Không thể tải danh sách khóa học.");
    } finally {
      setDangTai(false);
    }
  }, [boLoc]);

  useEffect(() => {
    void taiDanhSach();
  }, [taiDanhSach]);

  return (
    <div className="space-y-6 p-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Khóa học</h1>
          <p className="text-sm text-muted-foreground mt-1">
            Quản lý khóa học, chương học và nội dung.
          </p>
        </div>

        <Link href="/khoa-hoc/them-moi">
          <Button>
            <Plus className="mr-2 h-4 w-4" />
            Thêm khóa học
          </Button>
        </Link>
      </div>

      <div className="flex items-center gap-3">
        <Input
          type="search"
          placeholder="Tìm mã, slug, tên khóa học..."
          className="max-w-sm"
          value={boLoc.search ?? ""}
          onChange={(event) =>
            setBoLoc((current) => ({
              ...current,
              page: 1,
              search: event.target.value,
            }))
          }
        />
      </div>

      {loi && (
        <div className="rounded-md border border-destructive/50 bg-destructive/10 p-3 text-sm text-destructive">
          {loi}
        </div>
      )}

      <div className="rounded-md border bg-card">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-16">STT</TableHead>
              <TableHead>Khóa học</TableHead>
              <TableHead>HSK</TableHead>
              <TableHead className="text-center">Chương</TableHead>
              <TableHead>Trạng thái</TableHead>
              <TableHead className="text-right">Thao tác</TableHead>
            </TableRow>
          </TableHeader>

          <TableBody>
            {!dangTai && duLieu.items.map((item, index) => (
              <TableRow key={item.id}>
                <TableCell>
                  {(duLieu.page - 1) * duLieu.pageSize + index + 1}
                </TableCell>
                <TableCell>
                  <div className="font-medium">{item.titleVi}</div>
                  <div className="mt-1 flex items-center gap-2 text-xs text-muted-foreground">
                    <span>{item.code}</span>
                    <span>&middot;</span>
                    <span>{item.slug}</span>
                  </div>
                </TableCell>
                <TableCell>
                  {item.hskCode ? (
                    <Badge variant="primary">{item.hskCode}</Badge>
                  ) : (
                    <span className="text-muted-foreground">—</span>
                  )}
                </TableCell>
                <TableCell className="text-center">{item.chapterCount}</TableCell>
                <TableCell>
                  <Badge variant="info">{getContentStatusLabel(item.status)}</Badge>
                </TableCell>
                <TableCell className="text-right">
                  <Link 
                    href={`/khoa-hoc/${item.id}`} 
                    className="text-[#ef241c] hover:underline text-sm font-medium"
                  >
                    Chi tiết
                  </Link>
                </TableCell>
              </TableRow>
            ))}

            {dangTai && (
              <TableRow>
                <TableCell colSpan={6} className="h-24 text-center">
                  Đang tải dữ liệu...
                </TableCell>
              </TableRow>
            )}

            {!dangTai && duLieu.items.length === 0 && (
              <TableRow>
                <TableCell colSpan={6} className="h-24 text-center text-muted-foreground">
                  Chưa có khóa học.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
