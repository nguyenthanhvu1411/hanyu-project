"use client";

import { BookOpenText, Languages, Pencil, Volume2 } from "lucide-react";
import Link from "next/link";
import { useEffect, useState } from "react";

import { ErrorState } from "@/components/common/error-state";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { ContentStatus, getContentStatusLabel } from "@/lib/constants/content-status";

interface VocabularyDto {
  id: number;
  hskLevelId: number;
  hskCode: string;
  hskNameVi: string;
  partOfSpeechId: number | null;
  partOfSpeechCode: string | null;
  partOfSpeechNameVi: string | null;
  topicId: number | null;
  topicSlug: string | null;
  topicNameVi: string | null;
  audioAssetId: number | null;
  simplified: string;
  traditional: string | null;
  pinyin: string;
  pinyinNormalized: string;
  primaryMeaningVi: string;
  notesVi: string | null;
  difficulty: number;
  status: ContentStatus;
  version: number;
  publishedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export function VocabularyDetail({ vocabularyId }: { vocabularyId: number }) {
  const [item, setItem] = useState<VocabularyDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;
    void (async () => {
      setLoading(true);
      setError(null);
      try {
        const data = await apiClient<VocabularyDto>(API_ENDPOINTS.VOCABULARY.DETAIL(vocabularyId));
        if (!cancelled) setItem(data);
      } catch (exception) {
        if (!cancelled) {
          setError(exception instanceof Error ? exception.message : "Không thể tải chi tiết từ vựng.");
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [vocabularyId]);

  if (loading) {
    return <div className="rounded-[11px] border border-[#e8e3dc] bg-white p-6 text-[11px] text-[#777]">Đang tải chi tiết...</div>;
  }

  if (error || !item) {
    return <ErrorState title="Không thể tải từ vựng" description={error ?? "Không tìm thấy dữ liệu."} />;
  }

  return (
    <div className="space-y-5">
      <section className="rounded-[11px] border border-[#e8e3dc] bg-white p-5">
        <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
          <div className="flex items-start gap-4">
            <div className="flex h-14 w-14 shrink-0 items-center justify-center rounded-[12px] bg-[#fff0ee] text-[#ef241c]">
              <Languages size={24} />
            </div>
            <div>
              <div className="flex flex-wrap items-baseline gap-2">
                <h1 className="text-[26px] font-semibold text-[#222]">{item.simplified}</h1>
                {item.traditional && item.traditional !== item.simplified && (
                  <span className="text-[14px] text-[#999]">{item.traditional}</span>
                )}
              </div>
              <div className="mt-1 text-[13px] text-[#666]">{item.pinyin}</div>
              <div className="mt-2 text-[12px] font-medium text-[#444]">{item.primaryMeaningVi}</div>
            </div>
          </div>

          <Link
            href={`/tu-vung/${item.id}/chinh-sua`}
            className="inline-flex h-[36px] items-center justify-center gap-2 rounded-[7px] bg-[#ef241c] px-4 text-[11px] font-semibold text-white hover:bg-[#d91f18]"
          >
            <Pencil size={14} /> Chỉnh sửa
          </Link>
        </div>
      </section>

      <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <InfoCard label="Cấp độ HSK" value={`${item.hskCode} — ${item.hskNameVi}`} />
        <InfoCard label="Loại từ" value={item.partOfSpeechNameVi || "Chưa phân loại"} />
        <InfoCard label="Chủ đề" value={item.topicNameVi || "Chưa gắn chủ đề"} />
        <InfoCard label="Trạng thái" value={getContentStatusLabel(item.status)} />
      </section>

      <section className="rounded-[11px] border border-[#e8e3dc] bg-white p-4">
        <h2 className="flex items-center gap-2 text-[13px] font-semibold text-[#333]">
          <BookOpenText size={15} /> Thông tin biên tập
        </h2>
        <div className="mt-4 grid gap-4 md:grid-cols-2">
          <InfoRow label="Độ khó" value={String(item.difficulty)} />
          <InfoRow label="Pinyin normalized" value={item.pinyinNormalized} />
          <InfoRow label="Revision kỹ thuật" value={`v${item.version}`} />
          <InfoRow label="Ngày xuất bản" value={item.publishedAt ? new Date(item.publishedAt).toLocaleString("vi-VN") : "Chưa xuất bản"} />
          <InfoRow label="Cập nhật" value={new Date(item.updatedAt).toLocaleString("vi-VN")} />
          <InfoRow label="Tạo lúc" value={new Date(item.createdAt).toLocaleString("vi-VN")} />
        </div>
        {item.notesVi && (
          <div className="mt-4 rounded-[8px] bg-[#faf9f7] p-3">
            <div className="text-[10px] font-medium uppercase tracking-wide text-[#999]">Ghi chú</div>
            <p className="mt-1 whitespace-pre-wrap text-[11px] leading-5 text-[#555]">{item.notesVi}</p>
          </div>
        )}
      </section>

      <section className="rounded-[11px] border border-[#e8e3dc] bg-white p-4">
        <h2 className="flex items-center gap-2 text-[13px] font-semibold text-[#333]">
          <Volume2 size={15} /> Nội dung mở rộng
        </h2>
        <p className="mt-1 text-[11px] leading-5 text-[#777]">
          Vocabulary Editor sẽ nối tiếp Nghĩa, Ví dụ, Quan hệ và Audio vào cùng workspace này. Audio hiện tại: {item.audioAssetId ? `#${item.audioAssetId}` : "chưa gắn"}.
        </p>
        <div className="mt-3 flex flex-wrap gap-2">
          <Link href={`/tu-vung/${item.id}/nghia`} className="detail-tab">Nghĩa</Link>
          <Link href={`/tu-vung/${item.id}/vi-du`} className="detail-tab">Ví dụ</Link>
          <Link href={`/tu-vung/${item.id}/quan-he`} className="detail-tab">Quan hệ</Link>
        </div>
      </section>

      <style jsx>{`
        :global(.detail-tab) {
          border: 1px solid #e0dcd5;
          border-radius: 7px;
          padding: 8px 12px;
          font-size: 11px;
          color: #555;
          background: white;
        }
        :global(.detail-tab:hover) { background: #f7f6f3; }
      `}</style>
    </div>
  );
}

function InfoCard({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-[10px] border border-[#e8e3dc] bg-white p-4">
      <div className="text-[10px] uppercase tracking-wide text-[#999]">{label}</div>
      <div className="mt-2 text-[12px] font-semibold text-[#444]">{value}</div>
    </div>
  );
}

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex items-start justify-between gap-4 border-b border-[#f0ede8] pb-2 text-[11px] last:border-b-0">
      <span className="text-[#888]">{label}</span>
      <span className="text-right font-medium text-[#444]">{value}</span>
    </div>
  );
}
