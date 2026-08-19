"use client";

import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";

import { ErrorState } from "@/components/common/error-state";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
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

const DIFFICULTY_OPTIONS = [
  { value: "1", label: "1 — Dễ" },
  { value: "2", label: "2 — Trung bình" },
  { value: "3", label: "3 — Khó" },
];

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
          vocabularyId ? apiClient<VocabularyDto>(API_ENDPOINTS.VOCABULARY.DETAIL(vocabularyId)) : Promise.resolve(null),
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
      } catch (caught) {
        if (!cancelled) setError(caught instanceof Error ? caught.message : "Không thể tải dữ liệu biên tập từ vựng.");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    return () => { cancelled = true; };
  }, [vocabularyId]);

  const publishedTopics = useMemo(
    () => topics.filter((topic) => topic.status === 3 || String(topic.id) === form.topicId),
    [form.topicId, topics],
  );

  const hskOptions = useMemo(
    () => hskLevels
      .filter((item) => item.isActive !== false || String(item.id) === form.hskLevelId)
      .map((item) => ({ value: String(item.id), label: `${item.code} — ${item.nameVi}` })),
    [form.hskLevelId, hskLevels],
  );

  const partOptions = useMemo(
    () => partsOfSpeech.map((item) => ({ value: String(item.id), label: `${item.nameVi} (${item.code})`, description: item.nameEn ?? undefined })),
    [partsOfSpeech],
  );

  const topicOptions = useMemo(
    () => publishedTopics.map((item) => ({ value: String(item.id), label: item.nameVi, description: item.slug, disabled: item.status !== 3 })),
    [publishedTopics],
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
        const created = await apiClient<VocabularyDto>(API_ENDPOINTS.VOCABULARY.ROOT, { method: "POST", body: payload });
        router.push(`/tu-vung/${created.id}`);
      }
      router.refresh();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể lưu từ vựng.");
    } finally {
      setSubmitting(false);
    }
  }

  if (loading) {
    return <Card><CardContent className="p-6 text-[13px] text-[#777]">Đang tải dữ liệu biên tập...</CardContent></Card>;
  }

  if (error && isEditing && !form.simplified) {
    return <ErrorState title="Không thể tải từ vựng" description={error} />;
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <Alert variant="danger">{error}</Alert>}

      <FormCard title="Thông tin chung" description="Quản lý Hán tự, Pinyin và nghĩa chính của từ vựng.">
        <div className="grid gap-4 md:grid-cols-2">
          <Field label="Chữ giản thể *"><Input value={form.simplified} onChange={(e) => update("simplified", e.target.value)} placeholder="你好" className="h-10 text-[16px]" /></Field>
          <Field label="Chữ phồn thể"><Input value={form.traditional} onChange={(e) => update("traditional", e.target.value)} placeholder="你好" className="h-10 text-[16px]" /></Field>
          <Field label="Pinyin *"><Input value={form.pinyin} onChange={(e) => update("pinyin", e.target.value)} placeholder="nǐ hǎo" className="h-10" /></Field>
          <Field label="Nghĩa chính tiếng Việt *"><Input value={form.primaryMeaningVi} onChange={(e) => update("primaryMeaningVi", e.target.value)} placeholder="Xin chào" className="h-10" /></Field>
        </div>
      </FormCard>

      <FormCard title="Phân loại" description="HSK là bắt buộc; Chủ đề và Loại từ dùng danh mục quản trị hiện có.">
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          <Field label="Cấp độ HSK *"><Select value={form.hskLevelId} onValueChange={(value) => update("hskLevelId", value)} options={hskOptions} placeholder="Chọn HSK" /></Field>
          <Field label="Loại từ"><Select value={form.partOfSpeechId} onValueChange={(value) => update("partOfSpeechId", value)} options={partOptions} placeholder="Chưa phân loại" clearable searchable /></Field>
          <Field label="Chủ đề"><Select value={form.topicId} onValueChange={(value) => update("topicId", value)} options={topicOptions} placeholder="Chưa gắn chủ đề" clearable searchable /></Field>
          <Field label="Độ khó"><Select value={form.difficulty} onValueChange={(value) => update("difficulty", value)} options={DIFFICULTY_OPTIONS} /></Field>
        </div>
      </FormCard>

      <FormCard title="Ghi chú nội bộ" description="Audio phát âm được quản lý riêng trong tab Audio để tránh nhập ID thủ công hoặc vô tình thay đổi liên kết.">
        <Field label="Ghi chú cho biên tập viên">
          <Textarea value={form.notesVi} onChange={(e) => update("notesVi", e.target.value)} rows={5} placeholder="Ghi chú cho biên tập viên..." />
        </Field>
      </FormCard>

      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" size="md" disabled={submitting} onClick={() => router.back()}>Hủy</Button>
        <Button type="submit" size="md" loading={submitting}>{isEditing ? "Lưu thay đổi" : "Tạo từ vựng"}</Button>
      </div>
    </form>
  );
}

function FormCard({ title, description, children }: { title: string; description: string; children: React.ReactNode }) {
  return (
    <Card>
      <CardContent className="p-4">
        <div className="mb-4">
          <h2 className="text-[16px] font-semibold text-[#333]">{title}</h2>
          <p className="mt-1 text-[13px] leading-5 text-[#777]">{description}</p>
        </div>
        {children}
      </CardContent>
    </Card>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <label className="block space-y-1.5">
      <span className="text-[13px] font-medium text-[#555]">{label}</span>
      {children}
    </label>
  );
}
