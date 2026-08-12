"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { BookOpenText, Link2, MessageSquareText, Plus, Search, Trash2, Volume2 } from "lucide-react";

import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { getContentStatusLabel } from "@/lib/constants/content-status";
import { VocabularyForm } from "./vocabulary-form";

type TabKey = "general" | "meanings" | "examples" | "relations" | "audio";

interface MeaningDto {
  id: number;
  vocabularyId: number;
  meaningVi: string;
  senseOrder: number;
  usageNoteVi: string | null;
}

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

interface AudioAssetDto {
  id: number;
  storagePath: string;
  publicUrl: string | null;
  kind: number;
  mimeType: string;
  fileSizeBytes: number | null;
  durationMs: number | null;
  voice: string | null;
  provider: string | null;
  languageCode: string | null;
  checksum: string | null;
  status: number;
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

interface VocabularyLookupDto {
  id: number;
  simplified: string;
  pinyin: string;
  primaryMeaningVi: string;
}

interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

const TABS: Array<{ key: TabKey; label: string }> = [
  { key: "general", label: "Thông tin chung" },
  { key: "meanings", label: "Nghĩa" },
  { key: "examples", label: "Ví dụ" },
  { key: "relations", label: "Quan hệ" },
  { key: "audio", label: "Audio" },
];

export function VocabularyEditorTabs({ vocabularyId }: { vocabularyId: number }) {
  const [activeTab, setActiveTab] = useState<TabKey>("general");

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap gap-1 rounded-[10px] border border-[#e8e3dc] bg-white p-1.5">
        {TABS.map((tab) => (
          <button
            key={tab.key}
            type="button"
            onClick={() => setActiveTab(tab.key)}
            className={`rounded-[7px] px-4 py-2 text-[11px] font-medium transition ${
              activeTab === tab.key
                ? "bg-[#fff0ee] text-[#d92720]"
                : "text-[#666] hover:bg-[#f7f6f3]"
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {activeTab === "general" && <VocabularyForm vocabularyId={vocabularyId} />}
      {activeTab === "meanings" && <MeaningManager vocabularyId={vocabularyId} />}
      {activeTab === "examples" && <ExampleManager vocabularyId={vocabularyId} />}
      {activeTab === "relations" && <RelationManager vocabularyId={vocabularyId} />}
      {activeTab === "audio" && <AudioManager vocabularyId={vocabularyId} />}
    </div>
  );
}

function MeaningManager({ vocabularyId }: { vocabularyId: number }) {
  const [items, setItems] = useState<MeaningDto[]>([]);
  const [editing, setEditing] = useState<MeaningDto | null>(null);
  const [meaningVi, setMeaningVi] = useState("");
  const [senseOrder, setSenseOrder] = useState("1");
  const [usageNoteVi, setUsageNoteVi] = useState("");
  const state = useAsyncSection();

  const load = useCallback(async () => {
    state.start();
    try {
      setItems(await apiClient<MeaningDto[]>(API_ENDPOINTS.VOCABULARY.MEANINGS(vocabularyId)));
      state.success();
    } catch (error) {
      state.fail(error, "Không thể tải nghĩa từ vựng.");
    }
  }, [state, vocabularyId]);

  useEffect(() => { void load(); }, [load]);

  function reset() {
    setEditing(null);
    setMeaningVi("");
    setSenseOrder(String(Math.max(1, items.length + 1)));
    setUsageNoteVi("");
  }

  async function save() {
    if (!meaningVi.trim()) return state.setError("Nghĩa tiếng Việt là bắt buộc.");
    state.setBusy(true);
    state.setError(null);
    try {
      const body = { meaningVi: meaningVi.trim(), senseOrder: Number(senseOrder) || 1, usageNoteVi: usageNoteVi.trim() || null };
      await apiClient(
        editing ? API_ENDPOINTS.VOCABULARY.MEANING(vocabularyId, editing.id) : API_ENDPOINTS.VOCABULARY.MEANINGS(vocabularyId),
        { method: editing ? "PUT" : "POST", body },
      );
      reset();
      await load();
    } catch (error) {
      state.fail(error, "Không thể lưu nghĩa từ vựng.");
    } finally {
      state.setBusy(false);
    }
  }

  return (
    <EditorSection title="Nghĩa từ vựng" description="Quản lý nhiều nghĩa theo thứ tự ngữ nghĩa." icon={<BookOpenText size={16} />} error={state.error}>
      <div className="grid gap-3 rounded-[9px] bg-[#faf9f7] p-3 md:grid-cols-[1fr_100px_1fr_auto]">
        <input className="editor-input" value={meaningVi} onChange={(e) => setMeaningVi(e.target.value)} placeholder="Nghĩa tiếng Việt" />
        <input className="editor-input" value={senseOrder} onChange={(e) => setSenseOrder(e.target.value.replace(/\D/g, ""))} placeholder="Thứ tự" />
        <input className="editor-input" value={usageNoteVi} onChange={(e) => setUsageNoteVi(e.target.value)} placeholder="Ghi chú cách dùng" />
        <div className="flex gap-2">
          <button type="button" disabled={state.busy} onClick={() => void save()} className="primary-btn">{editing ? "Lưu" : "Thêm"}</button>
          {editing && <button type="button" onClick={reset} className="secondary-btn">Hủy</button>}
        </div>
      </div>

      <div className="mt-4 space-y-2">
        {items.length === 0 && !state.loading && <EmptyText text="Chưa có nghĩa bổ sung." />}
        {[...items].sort((a, b) => a.senseOrder - b.senseOrder).map((item) => (
          <RowCard key={item.id} title={`${item.senseOrder}. ${item.meaningVi}`} subtitle={item.usageNoteVi || "Không có ghi chú sử dụng"}>
            <button type="button" onClick={() => { setEditing(item); setMeaningVi(item.meaningVi); setSenseOrder(String(item.senseOrder)); setUsageNoteVi(item.usageNoteVi ?? ""); }} className="secondary-btn">Sửa</button>
            <DeleteButton onClick={async () => { if (!confirm(`Xóa nghĩa “${item.meaningVi}”?`)) return; await apiClient(API_ENDPOINTS.VOCABULARY.MEANING(vocabularyId, item.id), { method: "DELETE" }); await load(); }} />
          </RowCard>
        ))}
      </div>
    </EditorSection>
  );
}

function ExampleManager({ vocabularyId }: { vocabularyId: number }) {
  const [items, setItems] = useState<ExampleDto[]>([]);
  const [editing, setEditing] = useState<ExampleDto | null>(null);
  const [zh, setZh] = useState("");
  const [pinyin, setPinyin] = useState("");
  const [vi, setVi] = useState("");
  const [difficulty, setDifficulty] = useState("1");
  const [sourceNote, setSourceNote] = useState("");
  const [audioAssetId, setAudioAssetId] = useState("");
  const state = useAsyncSection();

  const load = useCallback(async () => {
    state.start();
    try { setItems(await apiClient<ExampleDto[]>(API_ENDPOINTS.VOCABULARY.EXAMPLES(vocabularyId))); state.success(); }
    catch (error) { state.fail(error, "Không thể tải ví dụ từ vựng."); }
  }, [state, vocabularyId]);
  useEffect(() => { void load(); }, [load]);

  function reset() { setEditing(null); setZh(""); setPinyin(""); setVi(""); setDifficulty("1"); setSourceNote(""); setAudioAssetId(""); }

  async function save() {
    if (!zh.trim() || !pinyin.trim() || !vi.trim()) return state.setError("Câu tiếng Trung, Pinyin và tiếng Việt là bắt buộc.");
    state.setBusy(true); state.setError(null);
    try {
      const body = { sentenceZh: zh.trim(), sentencePinyin: pinyin.trim(), sentenceVi: vi.trim(), difficulty: Number(difficulty), audioAssetId: audioAssetId ? Number(audioAssetId) : null, sourceNote: sourceNote.trim() || null };
      await apiClient(editing ? API_ENDPOINTS.VOCABULARY.EXAMPLE(vocabularyId, editing.id) : API_ENDPOINTS.VOCABULARY.EXAMPLES(vocabularyId), { method: editing ? "PUT" : "POST", body });
      reset(); await load();
    } catch (error) { state.fail(error, "Không thể lưu ví dụ."); }
    finally { state.setBusy(false); }
  }

  async function workflow(item: ExampleDto, action: "submit-review" | "approve" | "publish" | "archive" | "restore") {
    const paths = {
      "submit-review": API_ENDPOINTS.VOCABULARY.EXAMPLE_SUBMIT_REVIEW(vocabularyId, item.id),
      approve: API_ENDPOINTS.VOCABULARY.EXAMPLE_APPROVE(vocabularyId, item.id),
      publish: API_ENDPOINTS.VOCABULARY.EXAMPLE_PUBLISH(vocabularyId, item.id),
      archive: API_ENDPOINTS.VOCABULARY.EXAMPLE_ARCHIVE(vocabularyId, item.id),
      restore: API_ENDPOINTS.VOCABULARY.EXAMPLE_RESTORE(vocabularyId, item.id),
    };
    await apiClient(paths[action], { method: "POST" }); await load();
  }

  return (
    <EditorSection title="Ví dụ từ vựng" description="Câu ví dụ có workflow riêng trước khi xuất bản." icon={<MessageSquareText size={16} />} error={state.error}>
      <div className="grid gap-3 rounded-[9px] bg-[#faf9f7] p-3 md:grid-cols-2">
        <input className="editor-input" value={zh} onChange={(e) => setZh(e.target.value)} placeholder="Câu tiếng Trung" />
        <input className="editor-input" value={pinyin} onChange={(e) => setPinyin(e.target.value)} placeholder="Pinyin" />
        <input className="editor-input" value={vi} onChange={(e) => setVi(e.target.value)} placeholder="Dịch tiếng Việt" />
        <div className="grid grid-cols-2 gap-2">
          <select className="editor-input" value={difficulty} onChange={(e) => setDifficulty(e.target.value)}><option value="1">Dễ</option><option value="2">Trung bình</option><option value="3">Khó</option></select>
          <input className="editor-input" value={audioAssetId} onChange={(e) => setAudioAssetId(e.target.value.replace(/\D/g, ""))} placeholder="AudioAsset ID" />
        </div>
        <input className="editor-input md:col-span-2" value={sourceNote} onChange={(e) => setSourceNote(e.target.value)} placeholder="Nguồn / ghi chú" />
        <div className="flex gap-2 md:col-span-2"><button type="button" disabled={state.busy} onClick={() => void save()} className="primary-btn">{editing ? "Lưu ví dụ" : "Thêm ví dụ"}</button>{editing && <button type="button" onClick={reset} className="secondary-btn">Hủy</button>}</div>
      </div>

      <div className="mt-4 space-y-2">
        {items.length === 0 && !state.loading && <EmptyText text="Chưa có câu ví dụ." />}
        {items.map((item) => (
          <RowCard key={item.id} title={item.sentenceZh} subtitle={`${item.sentencePinyin} · ${item.sentenceVi} · ${getContentStatusLabel(item.status)}`}>
            <button type="button" onClick={() => { setEditing(item); setZh(item.sentenceZh); setPinyin(item.sentencePinyin); setVi(item.sentenceVi); setDifficulty(String(item.difficulty)); setAudioAssetId(item.audioAssetId ? String(item.audioAssetId) : ""); setSourceNote(item.sourceNote ?? ""); }} className="secondary-btn">Sửa</button>
            {item.status === 0 && <button type="button" onClick={() => void workflow(item, "submit-review")} className="secondary-btn">Gửi duyệt</button>}
            {item.status === 1 && <button type="button" onClick={() => void workflow(item, "approve")} className="secondary-btn">Duyệt</button>}
            {item.status === 2 && <button type="button" onClick={() => void workflow(item, "publish")} className="secondary-btn">Xuất bản</button>}
            {item.status === 3 && <button type="button" onClick={() => void workflow(item, "archive")} className="secondary-btn">Lưu trữ</button>}
            {item.status === 4 && <button type="button" onClick={() => void workflow(item, "restore")} className="secondary-btn">Khôi phục</button>}
            <DeleteButton onClick={async () => { if (!confirm("Xóa ví dụ này?")) return; await apiClient(API_ENDPOINTS.VOCABULARY.EXAMPLE(vocabularyId, item.id), { method: "DELETE" }); await load(); }} />
          </RowCard>
        ))}
      </div>
    </EditorSection>
  );
}

function RelationManager({ vocabularyId }: { vocabularyId: number }) {
  const [items, setItems] = useState<RelationDto[]>([]);
  const [candidates, setCandidates] = useState<VocabularyLookupDto[]>([]);
  const [query, setQuery] = useState("");
  const [selectedId, setSelectedId] = useState("");
  const [relationType, setRelationType] = useState("0");
  const [noteVi, setNoteVi] = useState("");
  const [editing, setEditing] = useState<RelationDto | null>(null);
  const state = useAsyncSection();

  const load = useCallback(async () => {
    state.start();
    try { setItems(await apiClient<RelationDto[]>(API_ENDPOINTS.VOCABULARY.RELATIONS(vocabularyId))); state.success(); }
    catch (error) { state.fail(error, "Không thể tải quan hệ từ vựng."); }
  }, [state, vocabularyId]);
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
        } catch { setCandidates([]); }
      })();
    }, 250);
    return () => window.clearTimeout(timer);
  }, [editing?.relatedVocabularyId, items, query, vocabularyId]);

  function reset() { setEditing(null); setSelectedId(""); setRelationType("0"); setNoteVi(""); }
  async function save() {
    if (!editing && !selectedId) return state.setError("Hãy chọn từ vựng liên quan.");
    state.setBusy(true); state.setError(null);
    try {
      const body = editing
        ? { relationType: Number(relationType), noteVi: noteVi.trim() || null }
        : { relatedVocabularyId: Number(selectedId), relationType: Number(relationType), noteVi: noteVi.trim() || null };
      await apiClient(editing ? API_ENDPOINTS.VOCABULARY.RELATION(vocabularyId, editing.id) : API_ENDPOINTS.VOCABULARY.RELATIONS(vocabularyId), { method: editing ? "PUT" : "POST", body });
      reset(); await load();
    } catch (error) { state.fail(error, "Không thể lưu quan hệ."); }
    finally { state.setBusy(false); }
  }

  const labels = ["Liên quan", "Dễ nhầm", "Đồng nghĩa", "Trái nghĩa"];
  return (
    <EditorSection title="Quan hệ từ vựng" description="Liên kết đồng nghĩa, trái nghĩa, từ dễ nhầm hoặc có liên quan." icon={<Link2 size={16} />} error={state.error}>
      <div className="grid gap-3 rounded-[9px] bg-[#faf9f7] p-3 md:grid-cols-2">
        {!editing && <div className="space-y-2"><div className="relative"><Search size={14} className="absolute left-3 top-3 text-[#999]" /><input className="editor-input pl-9" value={query} onChange={(e) => setQuery(e.target.value)} placeholder="Tìm Hán tự, Pinyin, nghĩa..." /></div><select className="editor-input" value={selectedId} onChange={(e) => setSelectedId(e.target.value)}><option value="">Chọn từ vựng liên quan</option>{candidates.map((item) => <option key={item.id} value={item.id}>{item.simplified} — {item.pinyin} — {item.primaryMeaningVi}</option>)}</select></div>}
        <div className={editing ? "md:col-span-2 grid gap-3 md:grid-cols-2" : "space-y-2"}><select className="editor-input" value={relationType} onChange={(e) => setRelationType(e.target.value)}>{labels.map((label, index) => <option key={label} value={index}>{label}</option>)}</select><input className="editor-input" value={noteVi} onChange={(e) => setNoteVi(e.target.value)} placeholder="Ghi chú quan hệ" /></div>
        <div className="flex gap-2 md:col-span-2"><button type="button" onClick={() => void save()} className="primary-btn">{editing ? "Lưu quan hệ" : "Thêm quan hệ"}</button>{editing && <button type="button" onClick={reset} className="secondary-btn">Hủy</button>}</div>
      </div>
      <div className="mt-4 space-y-2">{items.length === 0 && !state.loading && <EmptyText text="Chưa có quan hệ từ vựng." />}{items.map((item) => <RowCard key={item.id} title={`${item.relatedSimplified} — ${item.relatedPinyin}`} subtitle={`${labels[item.relationType] ?? "Liên quan"} · ${item.relatedMeaningVi}${item.noteVi ? ` · ${item.noteVi}` : ""}`}><button type="button" className="secondary-btn" onClick={() => { setEditing(item); setRelationType(String(item.relationType)); setNoteVi(item.noteVi ?? ""); }}>Sửa</button><DeleteButton onClick={async () => { if (!confirm("Xóa quan hệ này?")) return; await apiClient(API_ENDPOINTS.VOCABULARY.RELATION(vocabularyId, item.id), { method: "DELETE" }); await load(); }} /></RowCard>)}</div>
    </EditorSection>
  );
}

function AudioManager({ vocabularyId }: { vocabularyId: number }) {
  const [vocabulary, setVocabulary] = useState<VocabularyDto | null>(null);
  const [assets, setAssets] = useState<AudioAssetDto[]>([]);
  const [selectedId, setSelectedId] = useState("");
  const state = useAsyncSection();

  const load = useCallback(async () => {
    state.start();
    try {
      const [vocab, audioPage] = await Promise.all([
        apiClient<VocabularyDto>(API_ENDPOINTS.VOCABULARY.DETAIL(vocabularyId)),
        apiClient<PagedResult<AudioAssetDto>>(`${API_ENDPOINTS.VOCABULARY.AUDIO_ASSETS}?page=1&pageSize=100`),
      ]);
      setVocabulary(vocab); setSelectedId(vocab.audioAssetId ? String(vocab.audioAssetId) : ""); setAssets(audioPage.items.filter((item) => item.kind === 0)); state.success();
    } catch (error) { state.fail(error, "Không thể tải Audio Asset."); }
  }, [state, vocabularyId]);
  useEffect(() => { void load(); }, [load]);

  const selectedAsset = useMemo(() => assets.find((item) => String(item.id) === selectedId) ?? null, [assets, selectedId]);

  async function attach() {
    if (!vocabulary) return;
    state.setBusy(true); state.setError(null);
    try {
      await apiClient(API_ENDPOINTS.VOCABULARY.DETAIL(vocabularyId), {
        method: "PUT",
        body: {
          hskLevelId: vocabulary.hskLevelId,
          simplified: vocabulary.simplified,
          traditional: vocabulary.traditional,
          pinyin: vocabulary.pinyin,
          pinyinNormalized: vocabulary.pinyinNormalized,
          primaryMeaningVi: vocabulary.primaryMeaningVi,
          notesVi: vocabulary.notesVi,
          difficulty: vocabulary.difficulty,
          partOfSpeechId: vocabulary.partOfSpeechId,
          topicId: vocabulary.topicId,
          audioAssetId: selectedId ? Number(selectedId) : null,
          version: vocabulary.version,
        },
      });
      await load();
    } catch (error) { state.fail(error, "Không thể gắn Audio Asset vào từ vựng."); }
    finally { state.setBusy(false); }
  }

  return (
    <EditorSection title="Audio phát âm" description="Chọn Audio Asset loại Vocabulary; trạng thái Published được ưu tiên cho nội dung public." icon={<Volume2 size={16} />} error={state.error}>
      <div className="rounded-[9px] bg-[#faf9f7] p-3">
        <div className="grid gap-3 md:grid-cols-[1fr_auto]">
          <select className="editor-input" value={selectedId} onChange={(e) => setSelectedId(e.target.value)}><option value="">Không gắn audio</option>{assets.map((asset) => <option key={asset.id} value={asset.id}>#{asset.id} · {asset.storagePath} · {getContentStatusLabel(asset.status)}</option>)}</select>
          <button type="button" disabled={state.busy} onClick={() => void attach()} className="primary-btn">Lưu Audio</button>
        </div>
      </div>
      {selectedAsset ? <div className="mt-4 rounded-[9px] border border-[#e8e3dc] p-4"><div className="text-[11px] font-semibold text-[#444]">Audio #{selectedAsset.id}</div><div className="mt-1 break-all text-[10px] text-[#888]">{selectedAsset.storagePath}</div><div className="mt-2 text-[10px] text-[#777]">{selectedAsset.mimeType} · {selectedAsset.durationMs ? `${Math.round(selectedAsset.durationMs / 1000)}s` : "chưa có thời lượng"} · {getContentStatusLabel(selectedAsset.status)}</div>{selectedAsset.publicUrl && <audio controls src={selectedAsset.publicUrl} className="mt-3 h-9 w-full" />}</div> : <div className="mt-4"><EmptyText text="Từ vựng hiện chưa gắn audio." /></div>}
      <p className="mt-3 text-[10px] leading-4 text-[#999]">Upload file vật lý vẫn dùng Media Upload hiện có; tab này quản lý lựa chọn AudioAsset và gắn vào Vocabulary thay cho nhập ID thủ công.</p>
    </EditorSection>
  );
}

function useAsyncSection() {
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  return useMemo(() => ({ loading, busy, error, setBusy, setError, start: () => { setLoading(true); setError(null); }, success: () => setLoading(false), fail: (value: unknown, fallback: string) => { setLoading(false); setError(value instanceof Error ? value.message : fallback); } }), [busy, error, loading]);
}

function EditorSection({ title, description, icon, error, children }: { title: string; description: string; icon: React.ReactNode; error: string | null; children: React.ReactNode }) {
  return <section className="rounded-[11px] border border-[#e8e3dc] bg-white p-4"><div className="mb-4 flex items-start gap-2"><span className="mt-0.5 text-[#ef241c]">{icon}</span><div><h2 className="text-[13px] font-semibold text-[#333]">{title}</h2><p className="mt-1 text-[11px] text-[#888]">{description}</p></div></div>{error && <div className="mb-3 rounded-[8px] border border-[#f0cfcb] bg-[#fff5f4] px-3 py-2 text-[11px] text-[#b9433d]">{error}</div>}{children}<style jsx global>{`.editor-input{height:38px;width:100%;border-radius:7px;border:1px solid #dfdbd4;background:#fff;padding:0 12px;font-size:11px;color:#444;outline:none}.editor-input:focus{border-color:#ef5b55}.primary-btn{height:38px;border-radius:7px;background:#ef241c;padding:0 14px;font-size:11px;font-weight:600;color:#fff}.primary-btn:hover{background:#d91f18}.primary-btn:disabled{opacity:.55}.secondary-btn{height:32px;border-radius:6px;border:1px solid #ddd8d1;background:#fff;padding:0 10px;font-size:10px;color:#555}.secondary-btn:hover{background:#f7f6f3}`}</style></section>;
}

function RowCard({ title, subtitle, children }: { title: string; subtitle: string; children: React.ReactNode }) {
  return <div className="flex flex-col gap-3 rounded-[9px] border border-[#e8e3dc] px-3 py-3 sm:flex-row sm:items-center sm:justify-between"><div className="min-w-0"><div className="text-[11px] font-semibold text-[#444]">{title}</div><div className="mt-1 text-[10px] leading-4 text-[#888]">{subtitle}</div></div><div className="flex shrink-0 flex-wrap gap-1.5">{children}</div></div>;
}

function DeleteButton({ onClick }: { onClick: () => void | Promise<void> }) {
  return <button type="button" onClick={() => void onClick()} className="inline-flex h-8 items-center gap-1 rounded-[6px] px-2 text-[10px] text-[#c93b33] hover:bg-[#fff0ee]"><Trash2 size={12} /> Xóa</button>;
}

function EmptyText({ text }: { text: string }) {
  return <div className="rounded-[9px] border border-dashed border-[#ddd8d1] px-4 py-7 text-center text-[11px] text-[#999]"><Plus size={15} className="mx-auto mb-2" />{text}</div>;
}
