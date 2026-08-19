"use client";

import { useParams } from "next/navigation";
import { Clock3, Network } from "lucide-react";
import { useQuery } from "@tanstack/react-query";

import { EmptyState } from "@/components/common/empty-state";
import { ErrorState } from "@/components/common/error-state";
import { FormSection } from "@/components/forms/form-section";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { courseApi } from "@/features/course/api/course.api";
import type { CourseHistoryItem } from "@/features/course/types/course-insights.types";

export default function CourseHistoryPage() {
  const params = useParams<{ id: string }>();
  const courseId = Number(params.id);
  const query = useQuery({
    queryKey: ["course-history", courseId],
    queryFn: () => courseApi.history(courseId),
    enabled: Number.isSafeInteger(courseId) && courseId > 0,
  });

  if (!Number.isSafeInteger(courseId) || courseId <= 0) {
    return <ErrorState title="Khóa học không hợp lệ" description="CourseId phải là số nguyên dương." />;
  }

  if (query.isLoading) {
    return <Skeleton className="h-64 w-full rounded-[11px]" />;
  }

  if (query.error) {
    return (
      <ErrorState
        title="Không thể tải lịch sử khóa học"
        description={query.error instanceof Error ? query.error.message : "Không thể tải dữ liệu."}
        onRetry={() => void query.refetch()}
      />
    );
  }

  const events = query.data ?? [];

  return (
    <FormSection
      title="Lịch sử khóa học"
      description="Dữ liệu lấy trực tiếp từ AuditLog và các mốc lifecycle đã lưu của Course."
      icon={<Clock3 size={18} />}
    >
      {events.length === 0 ? (
        <EmptyState title="Chưa có lịch sử" description="Chưa ghi nhận sự kiện nào cho khóa học này." />
      ) : (
        <div className="space-y-3">
          {events.map((event, index) => (
            <HistoryRow key={`${event.id ?? "lifecycle"}-${event.action}-${event.occurredAt}-${index}`} event={event} />
          ))}
        </div>
      )}
    </FormSection>
  );
}

function HistoryRow({ event }: { event: CourseHistoryItem }) {
  const changes = safeParseChangedProperties(event.changedPropertiesJson);

  return (
    <div className="rounded-[9px] border border-[#e8e3dc] bg-white p-4">
      <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
        <div className="min-w-0">
          <div className="flex flex-wrap items-center gap-2">
            <div className="text-[12px] font-semibold text-[#333]">{event.label}</div>
            <Badge variant="default">{event.action}</Badge>
          </div>
          <div className="mt-1 text-[10px] text-[#888]">{formatDate(event.occurredAt)}</div>
          {changes.length > 0 ? (
            <div className="mt-2 flex flex-wrap gap-1.5">
              {changes.map((item) => (
                <Badge key={item} variant="info">{item}</Badge>
              ))}
            </div>
          ) : null}
        </div>

        <div className="flex flex-col items-start gap-1 text-[10px] text-[#777] md:items-end">
          <span>{event.userDisplayName || (event.userId ? `User ${event.userId.slice(0, 8)}…` : "Hệ thống")}</span>
          {event.ipAddress ? <span>IP: {event.ipAddress}</span> : null}
        </div>
      </div>

      {(event.oldValuesJson || event.newValuesJson || event.correlationId) ? (
        <details className="mt-3 rounded-[7px] bg-[#faf9f7] px-3 py-2 text-[10px] text-[#666]">
          <summary className="cursor-pointer select-none font-medium text-[#444]">Chi tiết audit</summary>
          <div className="mt-2 space-y-2">
            {event.correlationId ? (
              <div className="flex items-center gap-1.5"><Network size={12} /> Correlation: {event.correlationId}</div>
            ) : null}
            {event.oldValuesJson ? <AuditJson label="Giá trị cũ" value={event.oldValuesJson} /> : null}
            {event.newValuesJson ? <AuditJson label="Giá trị mới" value={event.newValuesJson} /> : null}
          </div>
        </details>
      ) : null}
    </div>
  );
}

function AuditJson({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <div className="font-medium text-[#555]">{label}</div>
      <pre className="mt-1 max-h-48 overflow-auto whitespace-pre-wrap break-words rounded-md border border-[#ece8e2] bg-white p-2 font-mono text-[9px]">
        {formatJson(value)}
      </pre>
    </div>
  );
}

function safeParseChangedProperties(value?: string | null): string[] {
  if (!value) return [];
  try {
    const parsed: unknown = JSON.parse(value);
    if (Array.isArray(parsed)) return parsed.filter((item): item is string => typeof item === "string");
  } catch {
    return [];
  }
  return [];
}

function formatJson(value: string) {
  try {
    return JSON.stringify(JSON.parse(value), null, 2);
  } catch {
    return value;
  }
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat("vi-VN", { dateStyle: "medium", timeStyle: "short" }).format(new Date(value));
}
