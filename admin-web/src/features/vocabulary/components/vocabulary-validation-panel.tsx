"use client";

import { AlertTriangle, CheckCircle2, RefreshCw } from "lucide-react";
import { useCallback, useEffect, useState } from "react";

import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";

interface ValidationIssue {
  code: string;
  message: string;
  field: string | null;
  severity: "error" | "warning";
}

interface ValidationResult {
  isValid: boolean;
  issues: ValidationIssue[];
  errors: string[];
  warnings: string[];
}

export function VocabularyValidationPanel({ vocabularyId }: { vocabularyId: number }) {
  const [reviewResult, setReviewResult] = useState<ValidationResult | null>(null);
  const [publishResult, setPublishResult] = useState<ValidationResult | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const [review, publish] = await Promise.all([
        apiClient<ValidationResult>(API_ENDPOINTS.VOCABULARY.VALIDATE(vocabularyId)),
        apiClient<ValidationResult>(API_ENDPOINTS.VOCABULARY.VALIDATE(vocabularyId, true)),
      ]);
      setReviewResult(review);
      setPublishResult(publish);
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : "Không thể kiểm tra workflow từ vựng.");
    } finally {
      setLoading(false);
    }
  }, [vocabularyId]);

  useEffect(() => {
    void load();
  }, [load]);

  return (
    <section className="rounded-[11px] border border-[#e8e3dc] bg-white p-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h2 className="text-[13px] font-semibold text-[#333]">Workflow Validation</h2>
          <p className="mt-1 text-[11px] leading-5 text-[#888]">
            Review yêu cầu metadata và nghĩa hợp lệ. Publish siết thêm Topic, Ví dụ và Audio Published.
          </p>
        </div>
        <button
          type="button"
          disabled={loading}
          onClick={() => void load()}
          className="inline-flex h-[34px] items-center justify-center gap-2 rounded-[7px] border border-[#ddd8d1] px-3 text-[10px] font-medium text-[#555] hover:bg-[#f7f6f3] disabled:opacity-50"
        >
          <RefreshCw size={13} className={loading ? "animate-spin" : ""} />
          Kiểm tra lại
        </button>
      </div>

      {error && (
        <div className="mt-3 rounded-[8px] border border-[#f0cfcb] bg-[#fff5f4] px-3 py-2 text-[11px] text-[#b9433d]">
          {error}
        </div>
      )}

      <div className="mt-4 grid gap-4 lg:grid-cols-2">
        <ValidationColumn title="Trước khi gửi duyệt" result={reviewResult} loading={loading} />
        <ValidationColumn title="Trước khi xuất bản" result={publishResult} loading={loading} />
      </div>
    </section>
  );
}

function ValidationColumn({
  title,
  result,
  loading,
}: {
  title: string;
  result: ValidationResult | null;
  loading: boolean;
}) {
  if (loading && !result) {
    return <div className="rounded-[9px] bg-[#faf9f7] p-4 text-[11px] text-[#888]">Đang kiểm tra...</div>;
  }

  if (!result) return null;

  return (
    <div className="rounded-[9px] border border-[#ece7e0] p-4">
      <div className="flex items-center justify-between gap-3">
        <div className="text-[11px] font-semibold text-[#444]">{title}</div>
        <span className={`inline-flex items-center gap-1 rounded-full px-2 py-1 text-[10px] font-medium ${result.isValid ? "bg-[#eaf7ef] text-[#217a46]" : "bg-[#fff0ee] text-[#c93b33]"}`}>
          {result.isValid ? <CheckCircle2 size={12} /> : <AlertTriangle size={12} />}
          {result.isValid ? "Hợp lệ" : "Chưa hợp lệ"}
        </span>
      </div>

      <div className="mt-3 flex gap-3 text-[10px] text-[#777]">
        <span>{result.errors.length} lỗi</span>
        <span>{result.warnings.length} cảnh báo</span>
      </div>

      <div className="mt-3 space-y-2">
        {result.issues.length === 0 && (
          <div className="rounded-[7px] bg-[#f0faf4] px-3 py-2 text-[10px] text-[#2d7048]">
            Không phát hiện vấn đề workflow.
          </div>
        )}
        {result.issues.map((issue) => (
          <div
            key={`${issue.code}-${issue.field ?? ""}`}
            className={`rounded-[7px] border px-3 py-2 ${issue.severity === "error" ? "border-[#f0cfcb] bg-[#fff7f6]" : "border-[#eedfac] bg-[#fffaf0]"}`}
          >
            <div className={`text-[10px] font-semibold ${issue.severity === "error" ? "text-[#b9433d]" : "text-[#8a6817]"}`}>
              {issue.code}
            </div>
            <div className="mt-1 text-[10px] leading-4 text-[#555]">{issue.message}</div>
            {issue.field && <div className="mt-1 text-[9px] text-[#999]">Field: {issue.field}</div>}
          </div>
        ))}
      </div>
    </div>
  );
}
