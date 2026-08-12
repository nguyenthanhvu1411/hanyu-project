"use client";

import { useCallback, useEffect, useState } from "react";
import { AlertTriangle, CheckCircle2, RefreshCw, ShieldAlert } from "lucide-react";
import { toast } from "sonner";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";

import { lessonApi } from "../api/lesson.api";
import type { LessonValidationResult } from "../types/lesson.types";

interface LessonValidationPanelProps {
  lessonId: number;
}

export function LessonValidationPanel({ lessonId }: LessonValidationPanelProps) {
  const [result, setResult] = useState<LessonValidationResult | null>(null);
  const [loading, setLoading] = useState(true);

  const validate = useCallback(async (notify = false) => {
    setLoading(true);
    try {
      const next = await lessonApi.validate(lessonId);
      setResult(next);

      if (notify) {
        if (next.isValid) {
          toast.success("Bài giảng hợp lệ để tiếp tục quy trình.");
        } else {
          toast.error("Bài giảng còn lỗi cần xử lý trước khi gửi duyệt hoặc xuất bản.");
        }
      }
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Không thể kiểm tra bài giảng.");
    } finally {
      setLoading(false);
    }
  }, [lessonId]);

  useEffect(() => {
    void validate(false);
  }, [validate]);

  if (loading && !result) {
    return <Skeleton className="h-40 w-full rounded-[11px]" />;
  }

  const errors = result?.issues.filter((issue) => issue.severity === "error") ?? [];
  const warnings = result?.issues.filter((issue) => issue.severity === "warning") ?? [];

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between gap-3">
        <div>
          <CardTitle className="flex items-center gap-2">
            {result?.isValid ? <CheckCircle2 size={17} /> : <ShieldAlert size={17} />}
            Kiểm tra trước Review / Publish
          </CardTitle>
          <p className="mt-1 text-[11px] leading-5 text-[#777]">
            Đây là kết quả validation từ backend. Các lỗi bắt buộc phải được xử lý trước khi gửi duyệt hoặc xuất bản.
          </p>
        </div>
        <Button variant="outline" disabled={loading} onClick={() => void validate(true)} className="gap-2">
          <RefreshCw size={14} className={loading ? "animate-spin" : undefined} /> Kiểm tra lại
        </Button>
      </CardHeader>

      <CardContent className="space-y-4">
        <div className="flex flex-wrap gap-2">
          <Badge variant={result?.isValid ? "success" : "warning"}>
            {result?.isValid ? "Hợp lệ" : "Chưa hợp lệ"}
          </Badge>
          <Badge variant="info">{errors.length} lỗi</Badge>
          <Badge>{warnings.length} cảnh báo</Badge>
        </div>

        {result?.isValid ? (
          <div className="rounded-[9px] border border-[#dfe9df] bg-[#f7fbf7] px-4 py-3 text-[12px] leading-5 text-[#496149]">
            Không phát hiện lỗi chặn workflow. Lesson có thể tiếp tục sang bước Review hoặc Publish theo trạng thái hiện tại.
          </div>
        ) : (
          <div className="space-y-2">
            {errors.map((issue) => (
              <div key={`${issue.code}-${issue.field ?? "root"}`} className="rounded-[9px] border border-[#f1d1cd] bg-[#fff7f6] px-4 py-3">
                <div className="flex flex-wrap items-center gap-2">
                  <AlertTriangle size={14} className="text-[#b42318]" />
                  <span className="text-[12px] font-semibold text-[#8f2018]">{issue.message}</span>
                  {issue.field ? <Badge>{issue.field}</Badge> : null}
                </div>
                <div className="mt-1 text-[10px] text-[#9a6a66]">{issue.code}</div>
              </div>
            ))}
          </div>
        )}

        {warnings.length > 0 ? (
          <div className="space-y-2">
            {warnings.map((issue) => (
              <div key={`${issue.code}-${issue.field ?? "root"}`} className="rounded-[9px] border border-[#eadfbf] bg-[#fffaf0] px-4 py-3">
                <div className="text-[12px] font-medium text-[#7c6223]">{issue.message}</div>
                <div className="mt-1 text-[10px] text-[#9a854f]">{issue.code}{issue.field ? ` · ${issue.field}` : ""}</div>
              </div>
            ))}
          </div>
        ) : null}
      </CardContent>
    </Card>
  );
}
