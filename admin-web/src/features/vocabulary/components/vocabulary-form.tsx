"use client";

import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";

interface HskLevelDto {
  id: number;
  code: string;
  nameVi: string;
  isActive?: boolean;
}

interface TopicDto {
  id: number;
  slug: string;
  nameVi: string;
  status: number;
}

interface PartOfSpeechDto {
  id: number;
  code: string;
  nameVi: string;
  nameEn: string | null;
}

interface VocabularyDto {
  id: number;
  hskLevelId: number;
  partOfSpeechId: number | null;
  topicId: number | null;
  audioAssetId: number | null;
  simplified: string;
  traditional: string | null;
  pinyin: string;
  pinyinNormalized: string;
  primaryMeaningVi: string;
  notesVi: string | null;
  difficulty: number;
  version: number;
}

interface VocabularyFormProps {
  vocabularyId?: number;
}

interface FormValues {
  hskLevelId: string;
  simplified: string;
  traditional: string;
  pinyin: string;
  primaryMeaningVi: string;
  notesVi: string;
  difficulty: string;
  partOfSpeechId: string;
  topicId: string;
  audioAssetId: string;
  version: number;
}

const EMPTY_FORM: FormValues = {
  hskLevelId: "",
  simplified: "",
  traditional: "",
  pinyin: "",
  primaryMeaningVi: "",
  notesVi: "",
  difficulty: "1",
  partOfSpeechId: "",
  topicId: "",
  audioAssetId: "",
  version: 0,
};

function normalizePinyin(value: string) {
  return value
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/ü/g, "v")
    .replace(/Ü/g, "v")
    .toLowerCase()
    .replace(/[^a-z0-9]/g, "");
}

export function VocabularyForm({ vocabularyId }: VocabularyFormProps) {
  const router = useRouter();
  const [form, setForm] = useState<FormValues>(EMPTY_FORM);
  const [hskLevels, setHskLevels] = useState<HskLevelDto[]>([]);
  const [topics, setTopics] = useState<TopicDto[]>([]);
  const [partsOfSpeech, setPartsOfSpeech] = useState<PartOfSpeechDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isEditing = Boolean(vocabularyId);

  useEffect(() => {
    let cancelled = false;

    void (async () => {
      setLoading(true);
      setError(null);
      try {
        const [hskData, topicData, partData, vocabulary] = await Promise.all([
          apiClient<HskLevelDto[]>(API_ENDPOINTS.LEARNING.HSK_LEVELS),
          apiClient<TopicDto[]>(API_ENDPOINTS.VOCABULARY.TOPICS),
          apiClient<PartOfSpeechDto[]>(API_ENDPOINTS.VOCABULARY.PARTS_OF_SPEECH),
          vocabularyId
            ? apiClient<VocabularyDto>(API_ENDPOINTS.VOCABULARY.DETAIL(vocabularyId))
            : Promise.resolve(null),
        ]);

        if (cancelled) return;
        setHskLevels(hskData);
        setTopics(topicData);
        setPartsOfSpeech(partData);

        if (vocabulary) {
          setForm({
            hskLevelId: String(vocabulary.hskLevelId),
            simplified: vocabulary.simplified,
            traditional: vocabulary.traditional ?? "",
            pinyin: vocabulary.pinyin,
            primaryMeaningVi: vocabulary.primaryMeaningVi,
            notesVi: vocabulary.notesVi ?? "",
            difficulty: String(vocabulary.difficulty),
            partOfSpeechId: vocabulary.partOfSpeechId ? String(vocabulary.partOfSpeechId) : "",
            topicId: vocabulary.topicId ? String(vocabulary.topicId) : "",
            audioAssetId: vocabulary.audioAssetId ? String(vocabulary.audioAssetId) : "",
            version: vocabulary.version,
          });
        }
      } catch (exception) {
        if (!cancelled) {
          setError(exception instanceof Error ? exception.message : "Không thể tải dữ liệu biên tập từ vựng.");
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [vocabularyId]);

  const publishedTopics = useMemo(
    () => topics.filter((topic) => topic.status === 3 || String(topic.id) === form.topicId),
    [form.topicId, topics],
  );

  function update<K extends keyof FormValues>(key: K, value: FormValues[K]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    const hskLevelId = Number(form.hskLevelId);
    const simplified = form.simplified.trim();
    const pinyin = form.pinyin.trim();
    const primaryMeaningVi = form.primaryMeaningVi.trim();

    if (!Number.isSafeInteger(hskLevelId) || hskLevelId <= 0 || !simplified || !pinyin || !primaryMeaningVi) {
      setError("HSK, chữ giản thể, Pinyin và nghĩa chính là bắt buộc.");
      return;
    }

    const payload = {
      hskLevelId,
      simplified,
      traditional: form.traditional.trim() || null,
      pinyin,
      pinyinNormalized: normalizePinyin(pinyin),
      primaryMeaningVi,
      notesVi: form.notesVi.trim() || null,
      difficulty: Number(form.difficulty),
      partOfSpeechId: form.partOfSpeechId ? Number(form.partOfSpeechId) : null,
      topicId: form.topicId ? Number(form.topicId) : null,
      audioAssetId: form.audioAssetId ? Number(form.audioAssetId) : null,
    };

    setSubmitting(true);
    try {
      if (vocabularyId) {
        await apiClient(API_ENDPOINTS.VOCABULARY.DETAIL(vocabularyId), {
          method: "PUT",
          body: { ...payload, version: form.version },
        });
        router.push(`/tu-vung/${vocabularyId}`);
      } else {
        const created = await apiClient<VocabularyDto>(API_ENDPOINTS.VOCABULARY.ROOT, {
          method: "POST",
          body: payload,
        });
        router.push(`/tu-vung/${created.id}`);
      }
      router.refresh();
    } catch (exception) {
      setError(exception instanceof Error ? exception.message : "Không thể lưu từ vựng.");
    } finally {
      setSubmitting(false);
    }
  }

  if (loading) {
    return (
      <div className="rounded-[11px] border border-[#e8e3dc] bg-white p-6 text-[11px] text-[#777]">
        Đang tải dữ liệu biên tập...
      </div>
    );
  }

  if (error && isEditing && !form.simplified) {
    return <ErrorState title="Không thể tải từ vựng" description={error} />;
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-5">
      {error && (
        <div className="rounded-[8px] border border-[#f0cfcb] bg-[#fff5f4] px-3 py-2 text-[11px] text-[#b9433d]">
          {error}
        </div>
      )}

      <section className="rounded-[11px] border border-[#e8e3dc] bg-white p-4">
        <div className="mb-4">
          <h2 className="text-[13px] font-semibold text-[#333]">Thông tin chung</h2>
          <p className="mt-1 text-[11px] text-[#888]">Quản lý Hán tự, Pinyin, nghĩa chính và phân loại từ vựng.</p>
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          <Field label="Chữ giản thể *">
            <input value={form.simplified} onChange={(e) => update("simplified", e.target.value)} placeholder="你好" className="field-input text-[16px]" />
          </Field>
          <Field label="Chữ phồn thể">
            <input value={form.traditional} onChange={(e) => update("traditional", e.target.value)} placeholder="你好" className="field-input text-[16px]" />
          </Field>
          <Field label="Pinyin *">
            <input value={form.pinyin} onChange={(e) => update("pinyin", e.target.value)} placeholder="nǐ hǎo" className="field-input" />
          </Field>
          <Field label="Nghĩa chính tiếng Việt *">
            <input value={form.primaryMeaningVi} onChange={(e) => update("primaryMeaningVi", e.target.value)} placeholder="Xin chào" className="field-input" />
          </Field>
        </div>
      </section>

      <section className="rounded-[11px] border border-[#e8e3dc] bg-white p-4">
        <div className="mb-4">
          <h2 className="text-[13px] font-semibold text-[#333]">Phân loại</h2>
          <p className="mt-1 text-[11px] text-[#888]">HSK là bắt buộc; Chủ đề và Loại từ dùng danh mục quản trị hiện có.</p>
        </div>

        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          <Field label="Cấp độ HSK *">
            <select value={form.hskLevelId} onChange={(e) => update("hskLevelId", e.target.value)} className="field-input">
              <option value="">Chọn HSK</option>
              {hskLevels.filter((item) => item.isActive !== false || String(item.id) === form.hskLevelId).map((item) => (
                <option key={item.id} value={item.id}>{item.code} — {item.nameVi}</option>
              ))}
            </select>
          </Field>

          <Field label="Loại từ">
            <select value={form.partOfSpeechId} onChange={(e) => update("partOfSpeechId", e.target.value)} className="field-input">
              <option value="">Chưa phân loại</option>
              {partsOfSpeech.map((item) => (
                <option key={item.id} value={item.id}>{item.nameVi} ({item.code})</option>
              ))}
            </select>
          </Field>

          <Field label="Chủ đề">
            <select value={form.topicId} onChange={(e) => update("topicId", e.target.value)} className="field-input">
              <option value="">Chưa gắn chủ đề</option>
              {publishedTopics.map((item) => (
                <option key={item.id} value={item.id} disabled={item.status !== 3}>{item.nameVi}{item.status !== 3 ? " (không Published)" : ""}</option>
              ))}
            </select>
          </Field>

          <Field label="Độ khó">
            <select value={form.difficulty} onChange={(e) => update("difficulty", e.target.value)} className="field-input">
              <option value="1">1 — Dễ</option>
              <option value="2">2 — Trung bình</option>
              <option value="3">3 — Khó</option>
            </select>
          </Field>
        </div>
      </section>

      <section className="rounded-[11px] border border-[#e8e3dc] bg-white p-4">
        <h2 className="text-[13px] font-semibold text-[#333]">Ghi chú và Audio</h2>
        <div className="mt-4 grid gap-4 md:grid-cols-[1fr_220px]">
          <Field label="Ghi chú nội bộ">
            <textarea value={form.notesVi} onChange={(e) => update("notesVi", e.target.value)} rows={5} placeholder="Ghi chú cho biên tập viên..." className="field-input h-auto py-2 leading-5" />
          </Field>
          <Field label="AudioAsset ID">
            <input value={form.audioAssetId} onChange={(e) => update("audioAssetId", e.target.value.replace(/\D/g, ""))} inputMode="numeric" placeholder="Để trống nếu chưa có" className="field-input" />
            <p className="mt-1 text-[10px] leading-4 text-[#999]">Audio picker/upload sẽ được chuyển sang tab Audio ở bước tiếp theo.</p>
          </Field>
        </div>
      </section>

      <div className="flex justify-end gap-2">
        <button type="button" disabled={submitting} onClick={() => router.back()} className="h-[38px] rounded-[7px] border border-[#ddd8d1] px-4 text-[11px] font-medium text-[#555] hover:bg-[#f7f6f3] disabled:opacity-50">Hủy</button>
        <button type="submit" disabled={submitting} className="h-[38px] rounded-[7px] bg-[#ef241c] px-5 text-[11px] font-semibold text-white hover:bg-[#d91f18] disabled:opacity-50">{submitting ? "Đang lưu..." : isEditing ? "Lưu thay đổi" : "Tạo từ vựng"}</button>
      </div>

      <style jsx>{`
        .field-input {
          height: 38px;
          width: 100%;
          border-radius: 7px;
          border: 1px solid #dfdbd4;
          background: white;
          padding-left: 12px;
          padding-right: 12px;
          font-size: 11px;
          color: #444;
          outline: none;
        }
        .field-input:focus { border-color: #ef5b55; }
      `}</style>
    </form>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <label className="block space-y-1.5">
      <span className="text-[11px] font-medium text-[#555]">{label}</span>
      {children}
    </label>
  );
}
