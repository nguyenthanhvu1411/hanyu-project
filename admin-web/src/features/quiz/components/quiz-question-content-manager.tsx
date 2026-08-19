"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { Check, Link2, Pencil, Plus, RefreshCw, Trash2, Unlink, X } from "lucide-react";

import { EmptyState } from "@/components/common/empty-state";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import { Textarea } from "@/components/ui/textarea";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";

import { quizApi } from "../quiz.api";
import type {
  AdminQuizMatchingPair,
  AdminQuizQuestionOption,
  AdminQuizTag,
  QuizMatchingPairRequest,
  QuizQuestionOptionRequest,
  QuizTagRequest,
} from "../quiz.types";
import { QuizQuestionType } from "../quiz.types";

interface QuizQuestionContentManagerProps {
  quizId: number;
  questionId: number;
  questionType: QuizQuestionType;
}

type DeleteTarget =
  | { kind: "option"; id: number; label: string }
  | { kind: "pair"; id: number; label: string }
  | { kind: "tag"; id: number; label: string }
  | null;

const EMPTY_OPTION: QuizQuestionOptionRequest = { optionText: "", optionPinyin: null, isCorrect: false, sortOrder: 0, explanationVi: null };
const EMPTY_PAIR: QuizMatchingPairRequest = { leftText: "", rightText: "", leftPinyin: null, rightPinyin: null, sortOrder: 0 };
const EMPTY_TAG: QuizTagRequest = { slug: "", name: "", nameVi: null, descriptionVi: null };

function supportsOptions(type: QuizQuestionType) {
  return [QuizQuestionType.MeaningChoice, QuizQuestionType.PinyinChoice, QuizQuestionType.HanziChoice, QuizQuestionType.TrueFalse, QuizQuestionType.MultipleChoice].includes(type);
}

export function QuizQuestionContentManager({ quizId, questionId, questionType }: QuizQuestionContentManagerProps) {
  const [options, setOptions] = useState<AdminQuizQuestionOption[]>([]);
  const [pairs, setPairs] = useState<AdminQuizMatchingPair[]>([]);
  const [tags, setTags] = useState<AdminQuizTag[]>([]);
  const [attachedTags, setAttachedTags] = useState<AdminQuizTag[]>([]);
  const [optionForm, setOptionForm] = useState<QuizQuestionOptionRequest>(EMPTY_OPTION);
  const [pairForm, setPairForm] = useState<QuizMatchingPairRequest>(EMPTY_PAIR);
  const [tagForm, setTagForm] = useState<QuizTagRequest>(EMPTY_TAG);
  const [editingOptionId, setEditingOptionId] = useState<number | null>(null);
  const [editingPairId, setEditingPairId] = useState<number | null>(null);
  const [editingTagId, setEditingTagId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<DeleteTarget>(null);
  const [working, setWorking] = useState(false);
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [optionData, pairData, tagData, membership] = await Promise.all([
        quizApi.listQuestionOptions(quizId, questionId),
        quizApi.listMatchingPairs(quizId, questionId),
        quizApi.listTags(),
        quizApi.listQuestionTags(quizId, questionId),
      ]);
      setOptions([...optionData].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id));
      setPairs([...pairData].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id));
      setTags([...tagData].sort((a, b) => (a.nameVi || a.name).localeCompare(b.nameVi || b.name)));
      setAttachedTags(membership);
    } catch (caught) {
      appToast.error("Không thể tải cấu hình câu hỏi", normalizeApiError(caught).message);
    } finally { setLoading(false); }
  }, [questionId, quizId]);

  useEffect(() => { void load(); }, [load]);

  const attachedTagIds = useMemo(() => new Set(attachedTags.map((item) => item.id)), [attachedTags]);
  const nextOptionOrder = useMemo(() => options.length ? Math.max(...options.map((item) => item.sortOrder)) + 1 : 0, [options]);
  const nextPairOrder = useMemo(() => pairs.length ? Math.max(...pairs.map((item) => item.sortOrder)) + 1 : 0, [pairs]);

  function resetOption() { setEditingOptionId(null); setOptionForm({ ...EMPTY_OPTION, sortOrder: nextOptionOrder }); }
  function resetPair() { setEditingPairId(null); setPairForm({ ...EMPTY_PAIR, sortOrder: nextPairOrder }); }
  function resetTag() { setEditingTagId(null); setTagForm(EMPTY_TAG); }

  async function saveOption(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!optionForm.optionText.trim()) { appToast.error("Thiếu đáp án", "Vui lòng nhập nội dung lựa chọn."); return; }
    setWorking(true);
    try {
      const payload = { ...optionForm, optionText: optionForm.optionText.trim(), optionPinyin: optionForm.optionPinyin?.trim() || null, explanationVi: optionForm.explanationVi?.trim() || null };
      if (editingOptionId) await quizApi.updateQuestionOption(quizId, questionId, editingOptionId, payload);
      else await quizApi.createQuestionOption(quizId, questionId, payload);
      appToast.success(editingOptionId ? "Đã cập nhật lựa chọn." : "Đã thêm lựa chọn.");
      resetOption(); await load();
    } catch (caught) { appToast.error("Không thể lưu lựa chọn", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function savePair(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!pairForm.leftText.trim() || !pairForm.rightText.trim()) { appToast.error("Thiếu dữ liệu", "Hai vế của cặp ghép đều bắt buộc."); return; }
    setWorking(true);
    try {
      const payload = { ...pairForm, leftText: pairForm.leftText.trim(), rightText: pairForm.rightText.trim(), leftPinyin: pairForm.leftPinyin?.trim() || null, rightPinyin: pairForm.rightPinyin?.trim() || null };
      if (editingPairId) await quizApi.updateMatchingPair(quizId, questionId, editingPairId, payload);
      else await quizApi.createMatchingPair(quizId, questionId, payload);
      appToast.success(editingPairId ? "Đã cập nhật cặp ghép." : "Đã thêm cặp ghép.");
      resetPair(); await load();
    } catch (caught) { appToast.error("Không thể lưu cặp ghép", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function saveTag(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!tagForm.slug.trim() || !tagForm.name.trim()) { appToast.error("Thiếu Tag", "Slug và tên Tag là bắt buộc."); return; }
    setWorking(true);
    try {
      const payload = { ...tagForm, slug: tagForm.slug.trim().toLowerCase(), name: tagForm.name.trim(), nameVi: tagForm.nameVi?.trim() || null, descriptionVi: tagForm.descriptionVi?.trim() || null };
      if (editingTagId) await quizApi.updateTag(editingTagId, payload);
      else await quizApi.createTag(payload);
      appToast.success(editingTagId ? "Đã cập nhật Tag." : "Đã tạo Tag.");
      resetTag(); await load();
    } catch (caught) { appToast.error("Không thể lưu Tag", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function membership(tag: AdminQuizTag) {
    setWorking(true);
    try {
      if (attachedTagIds.has(tag.id)) {
        await quizApi.detachTag(quizId, questionId, tag.id);
        appToast.success(`Đã gỡ Tag “${tag.nameVi || tag.name}”.`);
      } else {
        await quizApi.attachTag(quizId, questionId, tag.id);
        appToast.success(`Đã gắn Tag “${tag.nameVi || tag.name}”.`);
      }
      await load();
    } catch (caught) { appToast.error("Không thể cập nhật Tag", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function toggleTag(tag: AdminQuizTag) {
    setWorking(true);
    try {
      if (tag.isActive) await quizApi.deactivateTag(tag.id);
      else await quizApi.activateTag(tag.id);
      appToast.success(tag.isActive ? "Đã tạm dừng Tag." : "Đã kích hoạt Tag.");
      await load();
    } catch (caught) { appToast.error("Không thể cập nhật Tag", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function confirmDelete() {
    if (!deleteTarget) return;
    setWorking(true);
    try {
      if (deleteTarget.kind === "option") await quizApi.deleteQuestionOption(quizId, questionId, deleteTarget.id);
      if (deleteTarget.kind === "pair") await quizApi.deleteMatchingPair(quizId, questionId, deleteTarget.id);
      if (deleteTarget.kind === "tag") await quizApi.deleteTag(deleteTarget.id);
      appToast.success("Đã xóa dữ liệu.");
      setDeleteTarget(null); await load();
    } catch (caught) { appToast.error("Không thể xóa dữ liệu", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  return (
    <>
      <div className="mt-4 grid gap-3 border-t pt-4 xl:grid-cols-3">
        <Card className={!supportsOptions(questionType) ? "opacity-70" : ""}>
          <CardHeader className="flex flex-row items-center justify-between gap-2"><div><CardTitle className="text-[13px]">Lựa chọn</CardTitle><p className="mt-1 text-[10px] text-muted-foreground">Đáp án cho câu hỏi dạng lựa chọn.</p></div><Button size="icon" variant="outline" onClick={() => void load()} aria-label="Làm mới"><RefreshCw size={13} /></Button></CardHeader>
          <CardContent className="space-y-3">
            {!supportsOptions(questionType) ? <div className="rounded-md bg-muted p-3 text-[10px] text-muted-foreground">Loại câu hỏi hiện tại không yêu cầu Options.</div> : null}
            <form onSubmit={saveOption} className="space-y-2">
              <Input value={optionForm.optionText} onChange={(event) => setOptionForm((current) => ({ ...current, optionText: event.target.value }))} placeholder="Nội dung lựa chọn" />
              <Input value={optionForm.optionPinyin ?? ""} onChange={(event) => setOptionForm((current) => ({ ...current, optionPinyin: event.target.value || null }))} placeholder="Pinyin (tùy chọn)" />
              <div className="grid grid-cols-2 gap-2"><Input type="number" min={0} value={optionForm.sortOrder} onChange={(event) => setOptionForm((current) => ({ ...current, sortOrder: Number(event.target.value) }))} /><div className="rounded-md border px-3 py-2"><Switch checked={optionForm.isCorrect} onCheckedChange={(checked) => setOptionForm((current) => ({ ...current, isCorrect: checked }))} label="Đáp án đúng" /></div></div>
              <Textarea value={optionForm.explanationVi ?? ""} onChange={(event) => setOptionForm((current) => ({ ...current, explanationVi: event.target.value || null }))} placeholder="Giải thích lựa chọn" />
              <div className="flex gap-2"><Button size="sm" type="submit" loading={working}><Plus size={13} />{editingOptionId ? "Lưu" : "Thêm"}</Button>{editingOptionId ? <Button size="sm" variant="outline" type="button" onClick={resetOption}><X size={13} />Hủy</Button> : null}</div>
            </form>
            {options.length === 0 && !loading ? <EmptyState title="Chưa có lựa chọn" description="Thêm các đáp án cho câu hỏi này." /> : <div className="space-y-2">{options.map((option) => <div key={option.id} className="rounded-md border p-2 text-[10px]"><div className="flex items-start justify-between gap-2"><div><div className="font-medium">{option.optionText}</div>{option.optionPinyin ? <div className="text-muted-foreground">{option.optionPinyin}</div> : null}</div><div className="flex gap-1">{option.isCorrect ? <Badge variant="success"><Check size={10} />Đúng</Badge> : null}<Button size="icon" variant="ghost" aria-label="Sửa lựa chọn" onClick={() => { setEditingOptionId(option.id); setOptionForm({ optionText: option.optionText, optionPinyin: option.optionPinyin, isCorrect: option.isCorrect, sortOrder: option.sortOrder, explanationVi: option.explanationVi }); }}><Pencil size={12} /></Button><Button size="icon" variant="dangerGhost" aria-label="Xóa lựa chọn" onClick={() => setDeleteTarget({ kind: "option", id: option.id, label: option.optionText })}><Trash2 size={12} /></Button></div></div></div>)}</div>}
          </CardContent>
        </Card>

        <Card className={questionType !== QuizQuestionType.Matching ? "opacity-70" : ""}>
          <CardHeader><CardTitle className="text-[13px]">Matching Pairs</CardTitle><p className="mt-1 text-[10px] text-muted-foreground">Các cặp trái/phải cho câu ghép cặp.</p></CardHeader>
          <CardContent className="space-y-3">
            {questionType !== QuizQuestionType.Matching ? <div className="rounded-md bg-muted p-3 text-[10px] text-muted-foreground">Chỉ cần cấu hình khi loại câu hỏi là Ghép cặp.</div> : null}
            <form onSubmit={savePair} className="space-y-2"><div className="grid grid-cols-2 gap-2"><Input value={pairForm.leftText} onChange={(event) => setPairForm((current) => ({ ...current, leftText: event.target.value }))} placeholder="Vế trái" /><Input value={pairForm.rightText} onChange={(event) => setPairForm((current) => ({ ...current, rightText: event.target.value }))} placeholder="Vế phải" /></div><div className="grid grid-cols-2 gap-2"><Input value={pairForm.leftPinyin ?? ""} onChange={(event) => setPairForm((current) => ({ ...current, leftPinyin: event.target.value || null }))} placeholder="Pinyin trái" /><Input value={pairForm.rightPinyin ?? ""} onChange={(event) => setPairForm((current) => ({ ...current, rightPinyin: event.target.value || null }))} placeholder="Pinyin phải" /></div><Input type="number" min={0} value={pairForm.sortOrder} onChange={(event) => setPairForm((current) => ({ ...current, sortOrder: Number(event.target.value) }))} /><div className="flex gap-2"><Button size="sm" type="submit" loading={working}><Plus size={13} />{editingPairId ? "Lưu" : "Thêm"}</Button>{editingPairId ? <Button size="sm" variant="outline" type="button" onClick={resetPair}><X size={13} />Hủy</Button> : null}</div></form>
            {pairs.length === 0 && !loading ? <EmptyState title="Chưa có cặp ghép" description="Tạo các cặp dữ liệu cho câu Matching." /> : <div className="space-y-2">{pairs.map((pair) => <div key={pair.id} className="rounded-md border p-2 text-[10px]"><div className="flex items-center justify-between gap-2"><span>{pair.leftText} ↔ {pair.rightText}</span><div className="flex gap-1"><Button size="icon" variant="ghost" aria-label="Sửa cặp ghép" onClick={() => { setEditingPairId(pair.id); setPairForm({ leftText: pair.leftText, rightText: pair.rightText, leftPinyin: pair.leftPinyin, rightPinyin: pair.rightPinyin, sortOrder: pair.sortOrder }); }}><Pencil size={12} /></Button><Button size="icon" variant="dangerGhost" aria-label="Xóa cặp ghép" onClick={() => setDeleteTarget({ kind: "pair", id: pair.id, label: `${pair.leftText} ↔ ${pair.rightText}` })}><Trash2 size={12} /></Button></div></div></div>)}</div>}
          </CardContent>
        </Card>

        <Card>
          <CardHeader><CardTitle className="text-[13px]">Tags</CardTitle><p className="mt-1 text-[10px] text-muted-foreground">Membership được đọc trực tiếp từ backend; trạng thái gắn/gỡ luôn phản ánh dữ liệu hiện tại.</p></CardHeader>
          <CardContent className="space-y-3">
            <form onSubmit={saveTag} className="space-y-2"><Input value={tagForm.slug} onChange={(event) => setTagForm((current) => ({ ...current, slug: event.target.value }))} placeholder="slug" /><Input value={tagForm.name} onChange={(event) => setTagForm((current) => ({ ...current, name: event.target.value }))} placeholder="Tên tag" /><Input value={tagForm.nameVi ?? ""} onChange={(event) => setTagForm((current) => ({ ...current, nameVi: event.target.value || null }))} placeholder="Tên tiếng Việt" /><Textarea value={tagForm.descriptionVi ?? ""} onChange={(event) => setTagForm((current) => ({ ...current, descriptionVi: event.target.value || null }))} placeholder="Mô tả" /><div className="flex gap-2"><Button size="sm" type="submit" loading={working}><Plus size={13} />{editingTagId ? "Lưu Tag" : "Tạo Tag"}</Button>{editingTagId ? <Button size="sm" variant="outline" type="button" onClick={resetTag}><X size={13} />Hủy</Button> : null}</div></form>
            {tags.length === 0 && !loading ? <EmptyState title="Chưa có Tag" description="Tạo Tag để phân loại và tái sử dụng câu hỏi." /> : <div className="space-y-2">{tags.map((tag) => { const attached = attachedTagIds.has(tag.id); return <div key={tag.id} className="rounded-md border p-2 text-[10px]"><div className="flex items-start justify-between gap-2"><div className="min-w-0"><div className="flex flex-wrap items-center gap-1"><span className="font-medium">{tag.nameVi || tag.name}</span><Badge variant={attached ? "success" : "default"}>{attached ? "Đã gắn" : "Chưa gắn"}</Badge>{!tag.isActive ? <Badge variant="warning">Tạm dừng</Badge> : null}</div><div className="mt-1 text-muted-foreground">{tag.slug}</div></div><div className="flex flex-wrap justify-end gap-1"><Button size="icon" variant={attached ? "dangerGhost" : "outline"} aria-label={attached ? "Gỡ Tag" : "Gắn Tag"} disabled={working || !tag.isActive} onClick={() => void membership(tag)}>{attached ? <Unlink size={12} /> : <Link2 size={12} />}</Button><Button size="icon" variant="ghost" aria-label="Sửa Tag" onClick={() => { setEditingTagId(tag.id); setTagForm({ slug: tag.slug, name: tag.name, nameVi: tag.nameVi, descriptionVi: tag.descriptionVi }); }}><Pencil size={12} /></Button><Button size="sm" variant="ghost" disabled={working} onClick={() => void toggleTag(tag)}>{tag.isActive ? "Tạm dừng" : "Kích hoạt"}</Button><Button size="icon" variant="dangerGhost" aria-label="Xóa Tag" onClick={() => setDeleteTarget({ kind: "tag", id: tag.id, label: tag.nameVi || tag.name })}><Trash2 size={12} /></Button></div></div></div>; })}</div>}
          </CardContent>
        </Card>
      </div>

      <ConfirmDialog open={Boolean(deleteTarget)} title="Xóa dữ liệu?" description={deleteTarget ? `“${deleteTarget.label}” sẽ bị xóa. Thao tác này chỉ thực hiện khi backend cho phép.` : ""} confirmLabel="Xóa" loading={working} onClose={() => setDeleteTarget(null)} onConfirm={confirmDelete} />
    </>
  );
}
