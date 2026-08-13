"use client";

import { Bold, Eye, Heading2, Italic, Link2, List, Quote, Save } from "lucide-react";
import { useEffect, useRef, useState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Modal } from "@/components/ui/modal";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { Textarea } from "@/components/ui/textarea";

import {
  LessonSectionType,
  lessonSectionTypeLabels,
  type AdminLessonSection,
  type CreateLessonSectionRequest,
} from "../types/lesson.types";

interface LessonSectionModalProps {
  open: boolean;
  section?: AdminLessonSection | null;
  defaultSortOrder: number;
  loading?: boolean;
  onClose: () => void;
  onSubmit: (request: CreateLessonSectionRequest) => void | Promise<void>;
}

interface DraftState {
  sectionType: LessonSectionType;
  titleVi: string;
  contentVi: string;
  sortOrder: number;
  isRequired: boolean;
  estimatedSeconds: number;
}

const sectionTypeOptions = Object.entries(lessonSectionTypeLabels).map(([value, label]) => ({ value, label }));

export function LessonSectionModal({
  open,
  section,
  defaultSortOrder,
  loading = false,
  onClose,
  onSubmit,
}: LessonSectionModalProps) {
  const [draft, setDraft] = useState<DraftState>(() => toDraft(section, defaultSortOrder));
  const [preview, setPreview] = useState(false);
  const textareaRef = useRef<HTMLTextAreaElement | null>(null);

  useEffect(() => {
    if (!open) return;
    setDraft(toDraft(section, defaultSortOrder));
    setPreview(false);
  }, [open, section, defaultSortOrder]);

  function setField<K extends keyof DraftState>(key: K, value: DraftState[K]) {
    setDraft((current) => ({ ...current, [key]: value }));
  }

  function insertMarkup(before: string, after = "", placeholder = "nội dung") {
    const textarea = textareaRef.current;
    const value = draft.contentVi;
    if (!textarea) {
      setField("contentVi", `${value}${before}${placeholder}${after}`);
      return;
    }

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selected = value.slice(start, end) || placeholder;
    const next = `${value.slice(0, start)}${before}${selected}${after}${value.slice(end)}`;
    setField("contentVi", next);

    window.requestAnimationFrame(() => {
      textarea.focus();
      const cursor = start + before.length + selected.length + after.length;
      textarea.setSelectionRange(cursor, cursor);
    });
  }

  async function submit() {
    await onSubmit({
      sectionType: draft.sectionType,
      titleVi: draft.titleVi.trim() || null,
      contentVi: draft.contentVi.trim() || null,
      sortOrder: Math.max(0, draft.sortOrder),
      isRequired: draft.isRequired,
      estimatedSeconds: draft.estimatedSeconds > 0 ? draft.estimatedSeconds : null,
    });
  }

  return (
    <Modal
      open={open}
      onClose={onClose}
      size="lg"
      title={section ? "Chỉnh sửa section" : "Thêm section mới"}
      description="Quản lý loại nội dung, thời lượng, trạng thái bắt buộc và nội dung Markdown của section."
      footer={
        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" size="md" disabled={loading} onClick={onClose}>Hủy</Button>
          <Button type="button" size="md" loading={loading} onClick={() => void submit()} className="gap-2">
            <Save size={14} /> {section ? "Lưu thay đổi" : "Thêm section"}
          </Button>
        </div>
      }
    >
      <div className="space-y-4">
        <div className="grid gap-4 md:grid-cols-2">
          <Field label="Loại section">
            <Select
              value={String(draft.sectionType)}
              onValueChange={(value) => setField("sectionType", Number(value) as LessonSectionType)}
              options={sectionTypeOptions}
            />
          </Field>
          <Field label="Tiêu đề">
            <Input
              value={draft.titleVi}
              onChange={(event) => setField("titleVi", event.target.value)}
              placeholder="Ví dụ: Mẫu câu chào hỏi"
            />
          </Field>
          <Field label="Thứ tự">
            <Input
              type="number"
              min={0}
              value={draft.sortOrder}
              onChange={(event) => setField("sortOrder", Number(event.target.value) || 0)}
            />
          </Field>
          <Field label="Thời lượng ước tính (giây)">
            <Input
              type="number"
              min={1}
              value={draft.estimatedSeconds}
              onChange={(event) => setField("estimatedSeconds", Number(event.target.value) || 0)}
            />
          </Field>
        </div>

        <div className="flex items-center justify-between rounded-[9px] border border-[#e7e2db] bg-[#faf9f7] px-3 py-2.5">
          <div>
            <div className="text-[13px] font-medium text-[#444]">Yêu cầu hoàn thành</div>
            <div className="mt-0.5 text-[12px] text-[#888]">Section bắt buộc sẽ được tính vào tiến độ Lesson.</div>
          </div>
          <Switch checked={draft.isRequired} onCheckedChange={(value) => setField("isRequired", value)} />
        </div>

        <div>
          <div className="mb-2 flex flex-wrap items-center justify-between gap-2">
            <span className="text-[13px] font-medium text-[#555]">Nội dung section</span>
            <div className="flex gap-1">
              <Button type="button" variant={!preview ? "secondary" : "ghost"} size="sm" onClick={() => setPreview(false)}>Soạn thảo</Button>
              <Button type="button" variant={preview ? "secondary" : "ghost"} size="sm" onClick={() => setPreview(true)} className="gap-1.5">
                <Eye size={13} /> Xem trước
              </Button>
            </div>
          </div>

          {!preview ? (
            <>
              <div className="mb-2 flex flex-wrap gap-1 rounded-[8px] border border-[#e7e2db] bg-[#faf9f7] p-1.5">
                <ToolbarButton label="Tiêu đề" onClick={() => insertMarkup("## ", "", "Tiêu đề")}><Heading2 size={14} /></ToolbarButton>
                <ToolbarButton label="Đậm" onClick={() => insertMarkup("**", "**")}><Bold size={14} /></ToolbarButton>
                <ToolbarButton label="Nghiêng" onClick={() => insertMarkup("*", "*")}><Italic size={14} /></ToolbarButton>
                <ToolbarButton label="Danh sách" onClick={() => insertMarkup("- ", "", "Mục nội dung")}><List size={14} /></ToolbarButton>
                <ToolbarButton label="Trích dẫn" onClick={() => insertMarkup("> ", "", "Ghi chú quan trọng")}><Quote size={14} /></ToolbarButton>
                <ToolbarButton label="Liên kết" onClick={() => insertMarkup("[", "](https://)", "văn bản liên kết")}><Link2 size={14} /></ToolbarButton>
              </div>
              <Textarea
                ref={textareaRef}
                value={draft.contentVi}
                onChange={(event) => setField("contentVi", event.target.value)}
                className="min-h-[320px] font-mono text-[13px] leading-6"
                placeholder={"## Mục tiêu\n\nNội dung bài học...\n\n- Ý chính 1\n- Ý chính 2"}
              />
            </>
          ) : (
            <div className="min-h-[320px] rounded-[9px] border border-[#e7e2db] bg-white p-5">
              <MarkdownPreview content={draft.contentVi} />
            </div>
          )}
        </div>
      </div>
    </Modal>
  );
}

function toDraft(section: AdminLessonSection | null | undefined, defaultSortOrder: number): DraftState {
  return {
    sectionType: section?.sectionType ?? LessonSectionType.Introduction,
    titleVi: section?.titleVi ?? "",
    contentVi: section?.contentVi ?? "",
    sortOrder: section?.sortOrder ?? defaultSortOrder,
    isRequired: section?.isRequired ?? true,
    estimatedSeconds: section?.estimatedSeconds ?? 120,
  };
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <label className="block space-y-1.5">
      <span className="text-[13px] font-medium text-[#555]">{label}</span>
      {children}
    </label>
  );
}

function ToolbarButton({ label, onClick, children }: { label: string; onClick: () => void; children: React.ReactNode }) {
  return (
    <Button type="button" variant="ghost" size="sm" title={label} aria-label={label} onClick={onClick}>
      {children}
    </Button>
  );
}

function MarkdownPreview({ content }: { content: string }) {
  if (!content.trim()) return <div className="text-[13px] text-[#999]">Chưa có nội dung để xem trước.</div>;

  return (
    <div className="space-y-2 text-[14px] leading-6 text-[#444]">
      {content.split("\n").map((line, index) => {
        const value = line.trim();
        if (!value) return <div key={index} className="h-2" />;
        if (value.startsWith("## ")) return <h3 key={index} className="pt-2 text-[18px] font-semibold text-[#2f2f2f]">{value.slice(3)}</h3>;
        if (value.startsWith("### ")) return <h4 key={index} className="pt-1 text-[16px] font-semibold text-[#333]">{value.slice(4)}</h4>;
        if (value.startsWith("- ")) return <div key={index} className="flex gap-2"><span>•</span><span>{renderInline(value.slice(2))}</span></div>;
        if (value.startsWith("> ")) return <blockquote key={index} className="border-l-3 border-[#efaca8] bg-[#fff8f7] px-3 py-2 text-[#666]">{renderInline(value.slice(2))}</blockquote>;
        return <p key={index}>{renderInline(value)}</p>;
      })}
    </div>
  );
}

function renderInline(text: string) {
  const parts = text.split(/(\*\*[^*]+\*\*|\*[^*]+\*)/g).filter(Boolean);
  return parts.map((part, index) => {
    if (part.startsWith("**") && part.endsWith("**")) return <strong key={index}>{part.slice(2, -2)}</strong>;
    if (part.startsWith("*") && part.endsWith("*")) return <em key={index}>{part.slice(1, -1)}</em>;
    return <span key={index}>{part}</span>;
  });
}
