"use client";

import { BookOpenText, Trash2 } from "lucide-react";
import { useCallback, useEffect, useState } from "react";

import { Button } from "@/components/ui/button";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Input } from "@/components/ui/input";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { VocabularyEditorEmpty, VocabularyEditorRow, VocabularyEditorSection } from "./vocabulary-editor-ui";

interface MeaningDto {
  id: number;
  vocabularyId: number;
  meaningVi: string;
  senseOrder: number;
  usageNoteVi: string | null;
}

export function VocabularyMeaningsTab({ vocabularyId }: { vocabularyId: number }) {
  const [items, setItems] = useState<MeaningDto[]>([]);
  const [editing, setEditing] = useState<MeaningDto | null>(null);
  const [deleting, setDeleting] = useState<MeaningDto | null>(null);
  const [meaningVi, setMeaningVi] = useState("");
  const [senseOrder, setSenseOrder] = useState("1");
  const [usageNoteVi, setUsageNoteVi] = useState("");
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setItems(await apiClient<MeaningDto[]>(API_ENDPOINTS.VOCABULARY.MEANINGS(vocabularyId)));
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể tải nghĩa từ vựng.");
    } finally {
      setLoading(false);
    }
  }, [vocabularyId]);

  useEffect(() => { void load(); }, [load]);

  function reset() {
    setEditing(null);
    setMeaningVi("");
    setSenseOrder(String(Math.max(1, items.length + 1)));
    setUsageNoteVi("");
  }

  async function save() {
    if (!meaningVi.trim()) {
      setError("Nghĩa tiếng Việt là bắt buộc.");
      return;
    }
    setBusy(true);
    setError(null);
    try {
      await apiClient(
        editing ? API_ENDPOINTS.VOCABULARY.MEANING(vocabularyId, editing.id) : API_ENDPOINTS.VOCABULARY.MEANINGS(vocabularyId),
        {
          method: editing ? "PUT" : "POST",
          body: {
            meaningVi: meaningVi.trim(),
            senseOrder: Number(senseOrder) || 1,
            usageNoteVi: usageNoteVi.trim() || null,
          },
        },
      );
      reset();
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể lưu nghĩa từ vựng.");
    } finally {
      setBusy(false);
    }
  }

  async function remove() {
    if (!deleting) return;
    setBusy(true);
    try {
      await apiClient(API_ENDPOINTS.VOCABULARY.MEANING(vocabularyId, deleting.id), { method: "DELETE" });
      setDeleting(null);
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể xóa nghĩa từ vựng.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <VocabularyEditorSection title="Nghĩa từ vựng" description="Quản lý nhiều nghĩa theo thứ tự ngữ nghĩa." icon={<BookOpenText size={18} />} error={error}>
      <div className="grid gap-3 rounded-[9px] bg-[#faf9f7] p-4 md:grid-cols-[1fr_120px_1fr_auto]">
        <Input className="h-10" value={meaningVi} onChange={(e) => setMeaningVi(e.target.value)} placeholder="Nghĩa tiếng Việt" />
        <Input className="h-10" inputMode="numeric" value={senseOrder} onChange={(e) => setSenseOrder(e.target.value.replace(/\D/g, ""))} placeholder="Thứ tự" />
        <Input className="h-10" value={usageNoteVi} onChange={(e) => setUsageNoteVi(e.target.value)} placeholder="Ghi chú cách dùng" />
        <div className="flex gap-2">
          <Button type="button" size="md" loading={busy} onClick={() => void save()}>{editing ? "Lưu" : "Thêm"}</Button>
          {editing && <Button type="button" size="md" variant="outline" disabled={busy} onClick={reset}>Hủy</Button>}
        </div>
      </div>

      <div className="mt-4 space-y-2">
        {items.length === 0 && !loading && <VocabularyEditorEmpty text="Chưa có nghĩa bổ sung." />}
        {[...items].sort((a, b) => a.senseOrder - b.senseOrder).map((item) => (
          <VocabularyEditorRow key={item.id} title={`${item.senseOrder}. ${item.meaningVi}`} subtitle={item.usageNoteVi || "Không có ghi chú sử dụng"}>
            <Button type="button" size="sm" variant="outline" onClick={() => { setEditing(item); setMeaningVi(item.meaningVi); setSenseOrder(String(item.senseOrder)); setUsageNoteVi(item.usageNoteVi ?? ""); }}>Sửa</Button>
            <Button type="button" size="sm" variant="ghost" className="text-[#c93b33]" onClick={() => setDeleting(item)}><Trash2 size={14} className="mr-1" />Xóa</Button>
          </VocabularyEditorRow>
        ))}
      </div>

      <ConfirmDialog open={Boolean(deleting)} title="Xóa nghĩa từ vựng?" description={deleting ? `Nghĩa “${deleting.meaningVi}” sẽ bị xóa khỏi từ vựng.` : ""} confirmLabel="Xóa nghĩa" loading={busy} onClose={() => setDeleting(null)} onConfirm={remove} />
    </VocabularyEditorSection>
  );
}
