"use client";

import { FormEvent, useCallback, useEffect, useMemo, useState } from "react";
import { Check, Link2, Pencil, Plus, RefreshCw, Tag, Trash2, Unlink, X } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
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

const EMPTY_OPTION: QuizQuestionOptionRequest = {
  optionText: "",
  optionPinyin: null,
  isCorrect: false,
  sortOrder: 0,
  explanationVi: null,
};

const EMPTY_PAIR: QuizMatchingPairRequest = {
  leftText: "",
  rightText: "",
  leftPinyin: null,
  rightPinyin: null,
  sortOrder: 0,
};

const EMPTY_TAG: QuizTagRequest = {
  slug: "",
  name: "",
  nameVi: null,
  descriptionVi: null,
};

function supportsOptions(type: QuizQuestionType) {
  return [
    QuizQuestionType.MeaningChoice,
    QuizQuestionType.PinyinChoice,
    QuizQuestionType.HanziChoice,
    QuizQuestionType.TrueFalse,
    QuizQuestionType.MultipleChoice,
  ].includes(type);
}

export function QuizQuestionContentManager({ quizId, questionId, questionType }: QuizQuestionContentManagerProps) {
  const [options, setOptions] = useState<AdminQuizQuestionOption[]>([]);
  const [pairs, setPairs] = useState<AdminQuizMatchingPair[]>([]);
  const [tags, setTags] = useState<AdminQuizTag[]>([]);
  const [optionForm, setOptionForm] = useState<QuizQuestionOptionRequest>(EMPTY_OPTION);
  const [pairForm, setPairForm] = useState<QuizMatchingPairRequest>(EMPTY_PAIR);
  const [tagForm, setTagForm] = useState<QuizTagRequest>(EMPTY_TAG);
  const [editingOptionId, setEditingOptionId] = useState<number | null>(null);
  const [editingPairId, setEditingPairId] = useState<number | null>(null);
  const [editingTagId, setEditingTagId] = useState<number | null>(null);
  const [working, setWorking] = useState(false);

  const load = useCallback(async () => {
    try {
      const [optionData, pairData, tagData] = await Promise.all([
        quizApi.listQuestionOptions(quizId, questionId),
        quizApi.listMatchingPairs(quizId, questionId),
        quizApi.listTags(),
      ]);
      setOptions([...optionData].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id));
      setPairs([...pairData].sort((a, b) => a.sortOrder - b.sortOrder || a.id - b.id));
      setTags([...tagData].sort((a, b) => a.name.localeCompare(b.name)));
    } catch (caught) {
      appToast.error("Không thể tải cấu hình câu hỏi", normalizeApiError(caught).message);
    }
  }, [questionId, quizId]);

  useEffect(() => { void load(); }, [load]);

  const nextOptionOrder = useMemo(() => options.length ? Math.max(...options.map((item) => item.sortOrder)) + 1 : 0, [options]);
  const nextPairOrder = useMemo(() => pairs.length ? Math.max(...pairs.map((item) => item.sortOrder)) + 1 : 0, [pairs]);

  function resetOption() {
    setEditingOptionId(null);
    setOptionForm({ ...EMPTY_OPTION, sortOrder: nextOptionOrder });
  }

  function resetPair() {
    setEditingPairId(null);
    setPairForm({ ...EMPTY_PAIR, sortOrder: nextPairOrder });
  }

  function resetTag() {
    setEditingTagId(null);
    setTagForm(EMPTY_TAG);
  }

  async function saveOption(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!optionForm.optionText.trim()) return appToast.error("Thiếu đáp án", "Vui lòng nhập nội dung lựa chọn.");
    setWorking(true);
    try {
      const payload = {
        ...optionForm,
        optionText: optionForm.optionText.trim(),
        optionPinyin: optionForm.optionPinyin?.trim() || null,
        explanationVi: optionForm.explanationVi?.trim() || null,
      };
      if (editingOptionId) await quizApi.updateQuestionOption(quizId, questionId, editingOptionId, payload);
      else await quizApi.createQuestionOption(quizId, questionId, payload);
      appToast.success(editingOptionId ? "Đã cập nhật lựa chọn." : "Đã thêm lựa chọn.");
      resetOption();
      await load();
    } catch (caught) {
      appToast.error("Không thể lưu lựa chọn", normalizeApiError(caught).message);
    } finally { setWorking(false); }
  }

  async function deleteOption(optionId: number) {
    if (!window.confirm("Xóa lựa chọn này?")) return;
    setWorking(true);
    try {
      await quizApi.deleteQuestionOption(quizId, questionId, optionId);
      appToast.success("Đã xóa lựa chọn.");
      await load();
    } catch (caught) { appToast.error("Không thể xóa lựa chọn", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function savePair(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!pairForm.leftText.trim() || !pairForm.rightText.trim()) return appToast.error("Thiếu dữ liệu", "Hai vế của cặp ghép đều bắt buộc.");
    setWorking(true);
    try {
      const payload = {
        ...pairForm,
        leftText: pairForm.leftText.trim(),
        rightText: pairForm.rightText.trim(),
        leftPinyin: pairForm.leftPinyin?.trim() || null,
        rightPinyin: pairForm.rightPinyin?.trim() || null,
      };
      if (editingPairId) await quizApi.updateMatchingPair(quizId, questionId, editingPairId, payload);
      else await quizApi.createMatchingPair(quizId, questionId, payload);
      appToast.success(editingPairId ? "Đã cập nhật cặp ghép." : "Đã thêm cặp ghép.");
      resetPair();
      await load();
    } catch (caught) { appToast.error("Không thể lưu cặp ghép", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function deletePair(pairId: number) {
    if (!window.confirm("Xóa cặp ghép này?")) return;
    setWorking(true);
    try {
      await quizApi.deleteMatchingPair(quizId, questionId, pairId);
      appToast.success("Đã xóa cặp ghép.");
      await load();
    } catch (caught) { appToast.error("Không thể xóa cặp ghép", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function saveTag(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!tagForm.slug.trim() || !tagForm.name.trim()) return appToast.error("Thiếu Tag", "Slug và tên Tag là bắt buộc.");
    setWorking(true);
    try {
      const payload = {
        ...tagForm,
        slug: tagForm.slug.trim().toLowerCase(),
        name: tagForm.name.trim(),
        nameVi: tagForm.nameVi?.trim() || null,
        descriptionVi: tagForm.descriptionVi?.trim() || null,
      };
      if (editingTagId) await quizApi.updateTag(editingTagId, payload);
      else await quizApi.createTag(payload);
      appToast.success(editingTagId ? "Đã cập nhật Tag." : "Đã tạo Tag.");
      resetTag();
      await load();
    } catch (caught) { appToast.error("Không thể lưu Tag", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  async function tagAction(action: () => Promise<void>, success: string) {
    setWorking(true);
    try { await action(); appToast.success(success); await load(); }
    catch (caught) { appToast.error("Không thể cập nhật Tag", normalizeApiError(caught).message); }
    finally { setWorking(false); }
  }

  return (
    <div className="mt-4 grid gap-3 border-t pt-4 xl:grid-cols-3">
      <Card className={!supportsOptions(questionType) ? "opacity-70" : ""}>
        <CardHeader className="flex flex-row items-center justify-between gap-2">
          <div><CardTitle className="text-[13px]">Lựa chọn</CardTitle><p className="mt-1 text-[10px] text-muted-foreground">Dùng cho câu trắc nghiệm/chọn đáp án.</p></div>
          <Button size="icon" variant="outline" onClick={() => void load()} aria-label="Làm mới"><RefreshCw size={13} /></Button>
        </CardHeader>
        <CardContent className="space-y-3">
          {!supportsOptions(questionType) ? <div className="rounded-md bg-muted p-3 text-[10px] text-muted-foreground">Loại câu hỏi hiện tại thường không dùng Options.</div> : null}
          <form onSubmit={saveOption} className="space-y-2">
            <Input value={optionForm.optionText} onChange={(e) => setOptionForm((v) => ({ ...v, optionText: e.target.value }))} placeholder="Nội dung lựa chọn" />
            <Input value={optionForm.optionPinyin ?? ""} onChange={(e) => setOptionForm((v) => ({ ...v, optionPinyin: e.target.value || null }))} placeholder="Pinyin (tùy chọn)" />
            <div className="grid grid-cols-2 gap-2"><Input type="number" min={0} value={optionForm.sortOrder} onChange={(e) => setOptionForm((v) => ({ ...v, sortOrder: Number(e.target.value) }))} /><label className="flex items-center gap-2 rounded-md border px-3 text-[10px]"><input type="checkbox" checked={optionForm.isCorrect} onChange={(e) => setOptionForm((v) => ({ ...v, isCorrect: e.target.checked }))} /> Đáp án đúng</label></div>
            <Textarea value={optionForm.explanationVi ?? ""} onChange={(e) => setOptionForm((v) => ({ ...v, explanationVi: e.target.value || null }))} placeholder="Giải thích lựa chọn" />
            <div className="flex gap-2"><Button size="sm" type="submit" disabled={working}><Plus size={13} />{editingOptionId ? "Lưu" : "Thêm"}</Button>{editingOptionId ? <Button size="sm" variant="outline" type="button" onClick={resetOption}><X size={13} />Hủy</Button> : null}</div>
          </form>
          <div className="space-y-2">{options.map((option) => <div key={option.id} className="rounded-md border p-2 text-[10px]"><div className="flex items-start justify-between gap-2"><div><div className="font-medium">{option.optionText}</div>{option.optionPinyin ? <div className="text-muted-foreground">{option.optionPinyin}</div> : null}</div><div className="flex gap-1">{option.isCorrect ? <Badge variant="success"><Check size={10} /> Đúng</Badge> : null}<Button size="icon" variant="ghost" onClick={() => { setEditingOptionId(option.id); setOptionForm({ optionText: option.optionText, optionPinyin: option.optionPinyin, isCorrect: option.isCorrect, sortOrder: option.sortOrder, explanationVi: option.explanationVi }); }}><Pencil size={12} /></Button><Button size="icon" variant="ghost" onClick={() => void deleteOption(option.id)}><Trash2 size={12} /></Button></div></div></div>)}</div>
        </CardContent>
      </Card>

      <Card className={questionType !== QuizQuestionType.Matching ? "opacity-70" : ""}>
        <CardHeader><CardTitle className="text-[13px]">Matching Pairs</CardTitle><p className="mt-1 text-[10px] text-muted-foreground">Các cặp trái/phải cho câu ghép cặp.</p></CardHeader>
        <CardContent className="space-y-3">
          {questionType !== QuizQuestionType.Matching ? <div className="rounded-md bg-muted p-3 text-[10px] text-muted-foreground">Chỉ cần cấu hình khi loại câu hỏi là Ghép cặp.</div> : null}
          <form onSubmit={savePair} className="space-y-2"><div className="grid grid-cols-2 gap-2"><Input value={pairForm.leftText} onChange={(e) => setPairForm((v) => ({ ...v, leftText: e.target.value }))} placeholder="Vế trái" /><Input value={pairForm.rightText} onChange={(e) => setPairForm((v) => ({ ...v, rightText: e.target.value }))} placeholder="Vế phải" /></div><div className="grid grid-cols-2 gap-2"><Input value={pairForm.leftPinyin ?? ""} onChange={(e) => setPairForm((v) => ({ ...v, leftPinyin: e.target.value || null }))} placeholder="Pinyin trái" /><Input value={pairForm.rightPinyin ?? ""} onChange={(e) => setPairForm((v) => ({ ...v, rightPinyin: e.target.value || null }))} placeholder="Pinyin phải" /></div><Input type="number" min={0} value={pairForm.sortOrder} onChange={(e) => setPairForm((v) => ({ ...v, sortOrder: Number(e.target.value) }))} /><div className="flex gap-2"><Button size="sm" type="submit" disabled={working}><Plus size={13} />{editingPairId ? "Lưu" : "Thêm"}</Button>{editingPairId ? <Button size="sm" variant="outline" type="button" onClick={resetPair}><X size={13} />Hủy</Button> : null}</div></form>
          <div className="space-y-2">{pairs.map((pair) => <div key={pair.id} className="rounded-md border p-2 text-[10px]"><div className="flex items-center justify-between gap-2"><span>{pair.leftText} ↔ {pair.rightText}</span><div className="flex gap-1"><Button size="icon" variant="ghost" onClick={() => { setEditingPairId(pair.id); setPairForm({ leftText: pair.leftText, rightText: pair.rightText, leftPinyin: pair.leftPinyin, rightPinyin: pair.rightPinyin, sortOrder: pair.sortOrder }); }}><Pencil size={12} /></Button><Button size="icon" variant="ghost" onClick={() => void deletePair(pair.id)}><Trash2 size={12} /></Button></div></div></div>)}</div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader><CardTitle className="text-[13px]">Tags</CardTitle><p className="mt-1 text-[10px] text-muted-foreground">Quản lý Tag và gắn/gỡ theo API thật. Backend chưa trả membership hiện tại.</p></CardHeader>
        <CardContent className="space-y-3">
          <form onSubmit={saveTag} className="space-y-2"><div className="grid grid-cols-2 gap-2"><Input value={tagForm.slug} onChange={(e) => setTagForm((v) => ({ ...v, slug: e.target.value }))} placeholder="slug" /><Input value={tagForm.name} onChange={(e) => setTagForm((v) => ({ ...v, name: e.target.value }))} placeholder="Name" /></div><Input value={tagForm.nameVi ?? ""} onChange={(e) => setTagForm((v) => ({ ...v, nameVi: e.target.value || null }))} placeholder="Tên tiếng Việt" /><Textarea value={tagForm.descriptionVi ?? ""} onChange={(e) => setTagForm((v) => ({ ...v, descriptionVi: e.target.value || null }))} placeholder="Mô tả" /><div className="flex gap-2"><Button size="sm" type="submit" disabled={working}><Tag size={13} />{editingTagId ? "Lưu Tag" : "Tạo Tag"}</Button>{editingTagId ? <Button size="sm" variant="outline" type="button" onClick={resetTag}><X size={13} />Hủy</Button> : null}</div></form>
          <div className="space-y-2">{tags.map((tag) => <div key={tag.id} className="rounded-md border p-2 text-[10px]"><div className="flex items-start justify-between gap-2"><div><div className="font-medium">{tag.nameVi || tag.name}</div><div className="text-muted-foreground">#{tag.slug} · {tag.isActive ? "Active" : "Inactive"}</div></div><div className="flex flex-wrap justify-end gap-1"><Button size="icon" variant="ghost" title="Gắn tag" onClick={() => void tagAction(() => quizApi.attachTag(quizId, questionId, tag.id), "Đã gắn Tag vào câu hỏi.")}><Link2 size={12} /></Button><Button size="icon" variant="ghost" title="Gỡ tag" onClick={() => void tagAction(() => quizApi.detachTag(quizId, questionId, tag.id), "Đã gỡ Tag khỏi câu hỏi.")}><Unlink size={12} /></Button><Button size="icon" variant="ghost" title="Sửa tag" onClick={() => { setEditingTagId(tag.id); setTagForm({ slug: tag.slug, name: tag.name, nameVi: tag.nameVi, descriptionVi: tag.descriptionVi }); }}><Pencil size={12} /></Button><Button size="icon" variant="ghost" title={tag.isActive ? "Tạm dừng" : "Kích hoạt"} onClick={() => void tagAction(() => tag.isActive ? quizApi.deactivateTag(tag.id) : quizApi.activateTag(tag.id), tag.isActive ? "Đã tạm dừng Tag." : "Đã kích hoạt Tag.")}><RefreshCw size={12} /></Button><Button size="icon" variant="ghost" title="Xóa tag" onClick={() => { if (window.confirm("Xóa Tag này?")) void tagAction(() => quizApi.deleteTag(tag.id), "Đã xóa Tag."); }}><Trash2 size={12} /></Button></div></div></div>)}</div>
        </CardContent>
      </Card>
    </div>
  );
}
