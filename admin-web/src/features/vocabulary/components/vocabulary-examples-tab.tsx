"use client";

import { MessageSquareText, Trash2 } from "lucide-react";
import { useCallback, useEffect, useState } from "react";

import { Button } from "@/components/ui/button";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { getContentStatusLabel } from "@/lib/constants/content-status";
import { AudioAssetPicker } from "./audio-asset-picker";
import { VocabularyEditorEmpty, VocabularyEditorRow, VocabularyEditorSection } from "./vocabulary-editor-ui";

interface ExampleDto {
  id: number;
  vocabularyId: number;
  audioAssetId: number | null;
  sentenceZh: string;
  sentencePinyin: string;
  sentenceVi: string;
  difficulty: number;
  status: number;
  sourceNote: string | null;
}

const DIFFICULTY_OPTIONS = [
  { value: "1", label: "Dễ" },
  { value: "2", label: "Trung bình" },
  { value: "3", label: "Khó" },
];

export function VocabularyExamplesTab({ vocabularyId }: { vocabularyId: number }) {
  const [items, setItems] = useState<ExampleDto[]>([]);
  const [editing, setEditing] = useState<ExampleDto | null>(null);
  const [deleting, setDeleting] = useState<ExampleDto | null>(null);
  const [zh, setZh] = useState("");
  const [pinyin, setPinyin] = useState("");
  const [vi, setVi] = useState("");
  const [difficulty, setDifficulty] = useState("1");
  const [sourceNote, setSourceNote] = useState("");
  const [audioAssetId, setAudioAssetId] = useState<number | null>(null);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setItems(await apiClient<ExampleDto[]>(API_ENDPOINTS.VOCABULARY.EXAMPLES(vocabularyId)));
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể tải ví dụ từ vựng.");
    } finally {
      setLoading(false);
    }
  }, [vocabularyId]);

  useEffect(() => { void load(); }, [load]);

  function reset() {
    setEditing(null);
    setZh("");
    setPinyin("");
    setVi("");
    setDifficulty("1");
    setSourceNote("");
    setAudioAssetId(null);
  }

  async function changeAudio(nextAudioAssetId: number | null) {
    setAudioAssetId(nextAudioAssetId);
    if (!editing) return;

    const updated = await apiClient<ExampleDto>(API_ENDPOINTS.VOCABULARY.EXAMPLE_AUDIO(vocabularyId, editing.id), {
      method: "PUT",
      body: { audioAssetId: nextAudioAssetId },
    });
    setEditing(updated);
    setAudioAssetId(updated.audioAssetId);
    await load();
  }

  async function save() {
    if (!zh.trim() || !pinyin.trim() || !vi.trim()) {
      setError("Câu tiếng Trung, Pinyin và tiếng Việt là bắt buộc.");
      return;
    }

    setBusy(true);
    setError(null);
    try {
      await apiClient(
        editing ? API_ENDPOINTS.VOCABULARY.EXAMPLE(vocabularyId, editing.id) : API_ENDPOINTS.VOCABULARY.EXAMPLES(vocabularyId),
        {
          method: editing ? "PUT" : "POST",
          body: {
            sentenceZh: zh.trim(),
            sentencePinyin: pinyin.trim(),
            sentenceVi: vi.trim(),
            difficulty: Number(difficulty),
            audioAssetId,
            sourceNote: sourceNote.trim() || null,
          },
        },
      );
      reset();
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể lưu ví dụ.");
    } finally {
      setBusy(false);
    }
  }

  async function workflow(item: ExampleDto, action: "submit-review" | "approve" | "publish" | "archive" | "restore") {
    const paths = {
      "submit-review": API_ENDPOINTS.VOCABULARY.EXAMPLE_SUBMIT_REVIEW(vocabularyId, item.id),
      approve: API_ENDPOINTS.VOCABULARY.EXAMPLE_APPROVE(vocabularyId, item.id),
      publish: API_ENDPOINTS.VOCABULARY.EXAMPLE_PUBLISH(vocabularyId, item.id),
      archive: API_ENDPOINTS.VOCABULARY.EXAMPLE_ARCHIVE(vocabularyId, item.id),
      restore: API_ENDPOINTS.VOCABULARY.EXAMPLE_RESTORE(vocabularyId, item.id),
    };
    setBusy(true);
    setError(null);
    try {
      await apiClient(paths[action], { method: "POST" });
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể cập nhật workflow ví dụ.");
    } finally {
      setBusy(false);
    }
  }

  async function remove() {
    if (!deleting) return;
    setBusy(true);
    try {
      await apiClient(API_ENDPOINTS.VOCABULARY.EXAMPLE(vocabularyId, deleting.id), { method: "DELETE" });
      setDeleting(null);
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể xóa ví dụ.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <VocabularyEditorSection title="Ví dụ từ vựng" description="Câu ví dụ có workflow riêng trước khi xuất bản." icon={<MessageSquareText size={18} />} error={error}>
      <div className="grid gap-3 rounded-[9px] bg-[#faf9f7] p-4 md:grid-cols-2">
        <Input className="h-10" value={zh} onChange={(e) => setZh(e.target.value)} placeholder="Câu tiếng Trung" />
        <Input className="h-10" value={pinyin} onChange={(e) => setPinyin(e.target.value)} placeholder="Pinyin" />
        <Input className="h-10" value={vi} onChange={(e) => setVi(e.target.value)} placeholder="Dịch tiếng Việt" />
        <Select value={difficulty} onValueChange={setDifficulty} options={DIFFICULTY_OPTIONS} />
        <Input className="h-10 md:col-span-2" value={sourceNote} onChange={(e) => setSourceNote(e.target.value)} placeholder="Nguồn / ghi chú" />

        <div className="md:col-span-2">
          <AudioAssetPicker value={audioAssetId} onChange={changeAudio} kind={1} title="Audio câu ví dụ" description="Chọn audio câu ví dụ có sẵn hoặc upload trực tiếp lên Backblaze B2. Khi đang sửa, thay đổi audio được lưu qua command riêng." disabled={busy} />
        </div>

        <div className="flex gap-2 md:col-span-2">
          <Button type="button" size="md" loading={busy} onClick={() => void save()}>{editing ? "Lưu ví dụ" : "Thêm ví dụ"}</Button>
          {editing && <Button type="button" size="md" variant="outline" disabled={busy} onClick={reset}>Hủy</Button>}
        </div>
      </div>

      <div className="mt-4 space-y-2">
        {items.length === 0 && !loading && <VocabularyEditorEmpty text="Chưa có câu ví dụ." />}
        {items.map((item) => (
          <VocabularyEditorRow key={item.id} title={item.sentenceZh} subtitle={`${item.sentencePinyin} · ${item.sentenceVi} · ${getContentStatusLabel(item.status)}${item.audioAssetId ? ` · Audio #${item.audioAssetId}` : " · Chưa có audio"}`}>
            <Button type="button" size="sm" variant="outline" onClick={() => { setEditing(item); setZh(item.sentenceZh); setPinyin(item.sentencePinyin); setVi(item.sentenceVi); setDifficulty(String(item.difficulty)); setAudioAssetId(item.audioAssetId); setSourceNote(item.sourceNote ?? ""); }}>Sửa</Button>
            {item.status === 0 && <Button type="button" size="sm" variant="outline" onClick={() => void workflow(item, "submit-review")}>Gửi duyệt</Button>}
            {item.status === 1 && <Button type="button" size="sm" variant="outline" onClick={() => void workflow(item, "approve")}>Duyệt</Button>}
            {item.status === 2 && <Button type="button" size="sm" variant="outline" onClick={() => void workflow(item, "publish")}>Xuất bản</Button>}
            {item.status === 3 && <Button type="button" size="sm" variant="outline" onClick={() => void workflow(item, "archive")}>Lưu trữ</Button>}
            {item.status === 4 && <Button type="button" size="sm" variant="outline" onClick={() => void workflow(item, "restore")}>Khôi phục</Button>}
            <Button type="button" size="sm" variant="ghost" className="text-[#c93b33]" onClick={() => setDeleting(item)}><Trash2 size={14} className="mr-1" />Xóa</Button>
          </VocabularyEditorRow>
        ))}
      </div>

      <ConfirmDialog open={Boolean(deleting)} title="Xóa câu ví dụ?" description="Câu ví dụ sẽ bị xóa khỏi từ vựng. Hành động này không thể hoàn tác." confirmLabel="Xóa ví dụ" loading={busy} onClose={() => setDeleting(null)} onConfirm={remove} />
    </VocabularyEditorSection>
  );
}
