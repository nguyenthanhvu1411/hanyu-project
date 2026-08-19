"use client";

import { useCallback, useEffect, useMemo, useState } from "react";

import { Combobox, type ComboboxOption } from "@/components/ui/combobox";
import { courseApi } from "@/features/course/api/course.api";
import { identityApi } from "@/features/identity/identity.api";
import { lessonApi } from "@/features/lesson/api/lesson.api";
import { quizAttemptsApi } from "@/features/quiz/quiz-attempts.api";
import { quizApi } from "@/features/quiz/quiz.api";
import { reviewApi } from "@/features/review/review.api";
import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

interface EntitySelectorProps {
  value?: string;
  onValueChange: (value: string) => void;
  disabled?: boolean;
  clearable?: boolean;
  className?: string;
  placeholder?: string;
}

interface RemoteEntitySelectorProps extends EntitySelectorProps {
  loadOptions: (search: string) => Promise<ComboboxOption[]>;
  searchPlaceholder?: string;
  emptyText?: string;
}

function RemoteEntitySelector({ value, onValueChange, loadOptions, disabled, clearable = true, className, placeholder, searchPlaceholder, emptyText }: RemoteEntitySelectorProps) {
  const [options, setOptions] = useState<ComboboxOption[]>([]);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(false);
  const [selectedCache, setSelectedCache] = useState<ComboboxOption | null>(null);

  const load = useCallback(async (keyword: string) => {
    setLoading(true);
    try {
      const next = await loadOptions(keyword);
      setOptions(next);
      if (value) {
        const selected = next.find((item) => item.value === value);
        if (selected) setSelectedCache(selected);
      }
    } finally {
      setLoading(false);
    }
  }, [loadOptions, value]);

  useEffect(() => {
    const timer = window.setTimeout(() => void load(search), search ? 250 : 0);
    return () => window.clearTimeout(timer);
  }, [load, search]);

  const mergedOptions = useMemo(() => {
    if (!selectedCache || options.some((item) => item.value === selectedCache.value)) return options;
    return [selectedCache, ...options];
  }, [options, selectedCache]);

  return <Combobox value={value} onValueChange={(next) => { const selected = mergedOptions.find((item) => item.value === next) ?? null; setSelectedCache(selected); onValueChange(next); }} options={mergedOptions} placeholder={placeholder} searchPlaceholder={searchPlaceholder} emptyText={emptyText} disabled={disabled} clearable={clearable} className={className} loading={loading} remoteSearch onSearchChange={setSearch} />;
}

export function UserSelector(props: EntitySelectorProps) {
  const loadOptions = useCallback(async (search: string) => {
    const result = await identityApi.users.list({ page: 1, pageSize: 30, search: search || undefined, includeDeleted: false });
    return result.items.map((user) => ({ value: user.id, label: user.displayName || user.email, description: user.displayName ? user.email : undefined }));
  }, []);
  return <RemoteEntitySelector {...props} loadOptions={loadOptions} placeholder={props.placeholder ?? "Chọn người dùng"} searchPlaceholder="Tìm theo tên hoặc email..." emptyText="Không tìm thấy người dùng." />;
}

export function VocabularySelector(props: EntitySelectorProps) {
  const loadOptions = useCallback(async (search: string) => {
    const result = await lessonApi.listVocabularyOptions(search);
    return result.items.map((item) => ({ value: String(item.id), label: `${item.simplified} · ${item.pinyin}`, description: `${item.primaryMeaningVi}${item.hskCode ? ` · ${item.hskCode}` : ""}` }));
  }, []);
  return <RemoteEntitySelector {...props} loadOptions={loadOptions} placeholder={props.placeholder ?? "Chọn từ vựng"} searchPlaceholder="Tìm Hanzi, Pinyin hoặc nghĩa..." emptyText="Không tìm thấy từ vựng." />;
}

export function LessonSelector(props: EntitySelectorProps) {
  const loadOptions = useCallback(async (search: string) => {
    const result = await lessonApi.list({ search: search || undefined, page: 1, pageSize: 30, includeDeleted: false });
    return result.items.map((item) => ({ value: String(item.id), label: item.titleVi, description: [item.hskCode, item.courseTitleVi, item.chapterTitleVi].filter(Boolean).join(" · ") }));
  }, []);
  return <RemoteEntitySelector {...props} loadOptions={loadOptions} placeholder={props.placeholder ?? "Chọn bài học"} searchPlaceholder="Tìm theo tiêu đề bài học..." emptyText="Không tìm thấy bài học." />;
}

export function CourseSelector(props: EntitySelectorProps) {
  const loadOptions = useCallback(async (search: string) => {
    const result = await courseApi.list({ search: search || undefined, page: 1, pageSize: 30, includeDeleted: false });
    return result.items.map((item) => ({ value: String(item.id), label: item.titleVi, description: [item.code, item.hskCode].filter(Boolean).join(" · ") }));
  }, []);
  return <RemoteEntitySelector {...props} loadOptions={loadOptions} placeholder={props.placeholder ?? "Chọn khóa học"} searchPlaceholder="Tìm theo tên hoặc mã khóa học..." emptyText="Không tìm thấy khóa học." />;
}

export function QuizSelector(props: EntitySelectorProps) {
  const loadOptions = useCallback(async (search: string) => {
    const result = await quizApi.list({ q: search || undefined, page: 1, pageSize: 30 });
    return result.items.map((item) => ({ value: String(item.id), label: item.titleVi, description: item.lessonTitleVi ? `Bài học: ${item.lessonTitleVi}` : "Bài kiểm tra độc lập" }));
  }, []);
  return <RemoteEntitySelector {...props} loadOptions={loadOptions} placeholder={props.placeholder ?? "Chọn bài kiểm tra"} searchPlaceholder="Tìm theo tên bài kiểm tra..." emptyText="Không tìm thấy bài kiểm tra." />;
}

interface QuizAttemptSelectorProps extends EntitySelectorProps { userId?: string; quizId?: number; }

export function QuizAttemptSelector({ userId, quizId, ...props }: QuizAttemptSelectorProps) {
  const loadOptions = useCallback(async () => {
    const result = await quizAttemptsApi.list({ userId: userId || undefined, quizId, page: 1, pageSize: 30 });
    return result.items.map((item) => ({
      value: String(item.id),
      label: `${item.quizTitleVi} · lượt ${item.attemptNumber}`,
      description: `${item.userDisplayName || item.userEmail || "Học viên"}${item.percentage != null ? ` · ${Number(item.percentage).toFixed(1)}%` : ""}`,
    }));
  }, [quizId, userId]);
  return <RemoteEntitySelector {...props} loadOptions={loadOptions} placeholder={props.placeholder ?? "Chọn lượt làm bài"} searchPlaceholder="Danh sách lượt làm gần nhất" emptyText="Chưa có lượt làm bài phù hợp." />;
}

interface FlashcardSessionSelectorProps extends EntitySelectorProps { userId?: string; }

export function FlashcardSessionSelector({ userId, ...props }: FlashcardSessionSelectorProps) {
  const loadOptions = useCallback(async () => {
    const result = await reviewApi.sessions.list({ userId: userId || undefined, page: 1, pageSize: 30 });
    return result.items.map((item) => ({ value: String(item.id), label: `Flashcard ${new Date(item.startedAt).toLocaleString("vi-VN")}`, description: `${item.currentIndex}/${item.totalItems} mục · chính xác ${item.accuracyPercent}%` }));
  }, [userId]);
  return <RemoteEntitySelector {...props} loadOptions={loadOptions} placeholder={props.placeholder ?? "Chọn phiên flashcard"} searchPlaceholder="Danh sách phiên gần nhất" emptyText="Chưa có phiên flashcard phù hợp." />;
}

interface AudioAssetLookup {
  id: number;
  storagePath: string;
  publicUrl?: string | null;
  voice?: string | null;
  provider?: string | null;
  languageCode?: string | null;
  durationMs?: number | null;
  status: number;
}

export function AudioAssetSelector(props: EntitySelectorProps) {
  const loadOptions = useCallback(async (search: string) => {
    const result = await apiClient<PagedResult<AudioAssetLookup>>("/admin/audio-assets?page=1&pageSize=100");
    const keyword = search.trim().toLocaleLowerCase();
    return result.items
      .filter((item) => !keyword || `${item.storagePath} ${item.voice ?? ""} ${item.provider ?? ""} ${item.languageCode ?? ""}`.toLocaleLowerCase().includes(keyword))
      .slice(0, 30)
      .map((item) => ({
        value: String(item.id),
        label: item.voice || item.storagePath.split("/").pop() || item.storagePath,
        description: [item.languageCode, item.provider, item.durationMs ? `${Math.round(item.durationMs / 100) / 10}s` : null].filter(Boolean).join(" · "),
      }));
  }, []);
  return <RemoteEntitySelector {...props} loadOptions={loadOptions} placeholder={props.placeholder ?? "Chọn audio"} searchPlaceholder="Tìm theo file, giọng đọc hoặc provider..." emptyText="Không tìm thấy audio phù hợp." />;
}
