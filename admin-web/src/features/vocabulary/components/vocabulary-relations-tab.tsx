"use client";

import { Link2, Search, Trash2 } from "lucide-react";
import { useCallback, useEffect, useMemo, useState } from "react";

import { Button } from "@/components/ui/button";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Input } from "@/components/ui/input";
import { Select } from "@/components/ui/select";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { VocabularyEditorEmpty, VocabularyEditorRow, VocabularyEditorSection } from "./vocabulary-editor-ui";

interface RelationDto {
  id: number;
  vocabularyId: number;
  relatedVocabularyId: number;
  relatedSimplified: string;
  relatedPinyin: string;
  relatedMeaningVi: string;
  relationType: number;
  noteVi: string | null;
}

interface VocabularyLookupDto {
  id: number;
  simplified: string;
  pinyin: string;
  primaryMeaningVi: string;
}

interface PagedResult<T> {
  items: T[];
}

const RELATION_OPTIONS = [
  { value: "0", label: "Liên quan" },
  { value: "1", label: "Dễ nhầm" },
  { value: "2", label: "Đồng nghĩa" },
  { value: "3", label: "Trái nghĩa" },
];

export function VocabularyRelationsTab({ vocabularyId }: { vocabularyId: number }) {
  const [items, setItems] = useState<RelationDto[]>([]);
  const [candidates, setCandidates] = useState<VocabularyLookupDto[]>([]);
  const [editing, setEditing] = useState<RelationDto | null>(null);
  const [deleting, setDeleting] = useState<RelationDto | null>(null);
  const [query, setQuery] = useState("");
  const [selectedId, setSelectedId] = useState("");
  const [relationType, setRelationType] = useState("0");
  const [noteVi, setNoteVi] = useState("");
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setItems(await apiClient<RelationDto[]>(API_ENDPOINTS.VOCABULARY.RELATIONS(vocabularyId)));
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể tải quan hệ từ vựng.");
    } finally {
      setLoading(false);
    }
  }, [vocabularyId]);

  useEffect(() => { void load(); }, [load]);

  useEffect(() => {
    const timer = window.setTimeout(() => {
      void (async () => {
        try {
          const params = new URLSearchParams({ page: "1", pageSize: "50" });
          if (query.trim()) params.set("q", query.trim());
          const data = await apiClient<PagedResult<VocabularyLookupDto>>(`${API_ENDPOINTS.VOCABULARY.ROOT}?${params}`);
          const attached = new Set(items.map((item) => item.relatedVocabularyId));
          setCandidates(data.items.filter((item) => item.id !== vocabularyId && (!attached.has(item.id) || item.id === editing?.relatedVocabularyId)));
        } catch {
          setCandidates([]);
        }
      })();
    }, 250);
    return () => window.clearTimeout(timer);
  }, [editing?.relatedVocabularyId, items, query, vocabularyId]);

  const candidateOptions = useMemo(
    () => candidates.map((item) => ({
      value: String(item.id),
      label: `${item.simplified} — ${item.pinyin}`,
      description: item.primaryMeaningVi,
    })),
    [candidates],
  );

  function reset() {
    setEditing(null);
    setSelectedId("");
    setRelationType("0");
    setNoteVi("");
  }

  async function save() {
    if (!editing && !selectedId) {
      setError("Hãy chọn từ vựng liên quan.");
      return;
    }
    setBusy(true);
    setError(null);
    try {
      const body = editing
        ? { relationType: Number(relationType), noteVi: noteVi.trim() || null }
        : { relatedVocabularyId: Number(selectedId), relationType: Number(relationType), noteVi: noteVi.trim() || null };
      await apiClient(
        editing ? API_ENDPOINTS.VOCABULARY.RELATION(vocabularyId, editing.id) : API_ENDPOINTS.VOCABULARY.RELATIONS(vocabularyId),
        { method: editing ? "PUT" : "POST", body },
      );
      reset();
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể lưu quan hệ.");
    } finally {
      setBusy(false);
    }
  }

  async function remove() {
    if (!deleting) return;
    setBusy(true);
    try {
      await apiClient(API_ENDPOINTS.VOCABULARY.RELATION(vocabularyId, deleting.id), { method: "DELETE" });
      setDeleting(null);
      await load();
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể xóa quan hệ.");
    } finally {
      setBusy(false);
    }
  }

  return (
    <VocabularyEditorSection title="Quan hệ từ vựng" description="Liên kết đồng nghĩa, trái nghĩa, từ dễ nhầm hoặc có liên quan." icon={<Link2 size={18} />} error={error}>
      <div className="grid gap-3 rounded-[9px] bg-[#faf9f7] p-4 md:grid-cols-2">
        {!editing && (
          <div className="space-y-2">
            <div className="relative">
              <Search size={15} className="absolute left-3 top-3 text-[#999]" />
              <Input className="h-10 pl-9" value={query} onChange={(e) => setQuery(e.target.value)} placeholder="Tìm Hán tự, Pinyin, nghĩa..." />
            </div>
            <Select value={selectedId} onValueChange={setSelectedId} options={candidateOptions} placeholder="Chọn từ vựng liên quan" searchable searchPlaceholder="Tìm từ vựng..." />
          </div>
        )}

        <div className={editing ? "grid gap-3 md:col-span-2 md:grid-cols-2" : "space-y-2"}>
          <Select value={relationType} onValueChange={setRelationType} options={RELATION_OPTIONS} />
          <Input className="h-10" value={noteVi} onChange={(e) => setNoteVi(e.target.value)} placeholder="Ghi chú quan hệ" />
        </div>

        <div className="flex gap-2 md:col-span-2">
          <Button type="button" size="md" loading={busy} onClick={() => void save()}>{editing ? "Lưu quan hệ" : "Thêm quan hệ"}</Button>
          {editing && <Button type="button" size="md" variant="outline" disabled={busy} onClick={reset}>Hủy</Button>}
        </div>
      </div>

      <div className="mt-4 space-y-2">
        {items.length === 0 && !loading && <VocabularyEditorEmpty text="Chưa có quan hệ từ vựng." />}
        {items.map((item) => (
          <VocabularyEditorRow key={item.id} title={`${item.relatedSimplified} — ${item.relatedPinyin}`} subtitle={`${RELATION_OPTIONS[item.relationType]?.label ?? "Liên quan"} · ${item.relatedMeaningVi}${item.noteVi ? ` · ${item.noteVi}` : ""}`}>
            <Button type="button" size="sm" variant="outline" onClick={() => { setEditing(item); setRelationType(String(item.relationType)); setNoteVi(item.noteVi ?? ""); }}>Sửa</Button>
            <Button type="button" size="sm" variant="ghost" className="text-[#c93b33]" onClick={() => setDeleting(item)}><Trash2 size={14} className="mr-1" />Xóa</Button>
          </VocabularyEditorRow>
        ))}
      </div>

      <ConfirmDialog open={Boolean(deleting)} title="Xóa quan hệ từ vựng?" description="Liên kết này sẽ bị xóa khỏi từ vựng. Hành động này không thể hoàn tác." confirmLabel="Xóa quan hệ" loading={busy} onClose={() => setDeleting(null)} onConfirm={remove} />
    </VocabularyEditorSection>
  );
}
