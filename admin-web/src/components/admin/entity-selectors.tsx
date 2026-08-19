"use client";

import { useCallback, useEffect, useMemo, useState } from "react";

import { Combobox, type ComboboxOption } from "@/components/ui/combobox";
import { courseApi } from "@/features/course/api/course.api";
import { identityApi } from "@/features/identity/identity.api";
import { lessonApi } from "@/features/lesson/api/lesson.api";
import { reviewApi } from "@/features/review/review.api";

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

function RemoteEntitySelector({
  value,
  onValueChange,
  loadOptions,
  disabled,
  clearable = true,
  className,
  placeholder,
  searchPlaceholder,
  emptyText,
}: RemoteEntitySelectorProps) {
  const [options, setOptions] = useState<ComboboxOption[]>([]);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(false);
  const [selectedCache, setSelectedCache] = useState<ComboboxOption | null>(null);

  const load = useCallback(
    async (keyword: string) => {
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
    },
    [loadOptions, value],
  );

  useEffect(() => {
    const timer = window.setTimeout(() => void load(search), search ? 250 : 0);
    return () => window.clearTimeout(timer);
  }, [load, search]);

  const mergedOptions = useMemo(() => {
    if (!selectedCache || options.some((item) => item.value === selectedCache.value)) {
      return options;
    }
    return [selectedCache, ...options];
  }, [options, selectedCache]);

  return (
    <Combobox
      value={value}
      onValueChange={(next) => {
        const selected = mergedOptions.find((item) => item.value === next) ?? null;
        setSelectedCache(selected);
        onValueChange(next);
      }}
      options={mergedOptions}
      placeholder={placeholder}
      searchPlaceholder={searchPlaceholder}
      emptyText={emptyText}
      disabled={disabled}
      clearable={clearable}
      className={className}
      loading={loading}
      remoteSearch
      onSearchChange={setSearch}
    />
  );
}

export function UserSelector(props: EntitySelectorProps) {
  const loadOptions = useCallback(async (search: string) => {
    const result = await identityApi.users.list({
      page: 1,
      pageSize: 30,
      search: search || undefined,
      includeDeleted: false,
    });

    return result.items.map((user) => ({
      value: user.id,
      label: user.displayName || user.email,
      description: user.displayName ? user.email : undefined,
    }));
  }, []);

  return (
    <RemoteEntitySelector
      {...props}
      loadOptions={loadOptions}
      placeholder={props.placeholder ?? "Chọn người dùng"}
      searchPlaceholder="Tìm theo tên hoặc email..."
      emptyText="Không tìm thấy người dùng."
    />
  );
}

export function VocabularySelector(props: EntitySelectorProps) {
  const loadOptions = useCallback(async (search: string) => {
    const result = await lessonApi.listVocabularyOptions(search);

    return result.items.map((item) => ({
      value: String(item.id),
      label: `${item.simplified} · ${item.pinyin}`,
      description: `${item.primaryMeaningVi}${item.hskCode ? ` · ${item.hskCode}` : ""}`,
    }));
  }, []);

  return (
    <RemoteEntitySelector
      {...props}
      loadOptions={loadOptions}
      placeholder={props.placeholder ?? "Chọn từ vựng"}
      searchPlaceholder="Tìm Hanzi, Pinyin hoặc nghĩa..."
      emptyText="Không tìm thấy từ vựng."
    />
  );
}

export function LessonSelector(props: EntitySelectorProps) {
  const loadOptions = useCallback(async (search: string) => {
    const result = await lessonApi.list({
      search: search || undefined,
      page: 1,
      pageSize: 30,
      includeDeleted: false,
    });

    return result.items.map((item) => ({
      value: String(item.id),
      label: item.titleVi,
      description: [item.hskCode, item.courseTitleVi, item.chapterTitleVi]
        .filter(Boolean)
        .join(" · "),
    }));
  }, []);

  return (
    <RemoteEntitySelector
      {...props}
      loadOptions={loadOptions}
      placeholder={props.placeholder ?? "Chọn bài học"}
      searchPlaceholder="Tìm theo tiêu đề bài học..."
      emptyText="Không tìm thấy bài học."
    />
  );
}

export function CourseSelector(props: EntitySelectorProps) {
  const loadOptions = useCallback(async (search: string) => {
    const result = await courseApi.list({
      search: search || undefined,
      page: 1,
      pageSize: 30,
      includeDeleted: false,
    });

    return result.items.map((item) => ({
      value: String(item.id),
      label: item.titleVi,
      description: [item.code, item.hskCode].filter(Boolean).join(" · "),
    }));
  }, []);

  return (
    <RemoteEntitySelector
      {...props}
      loadOptions={loadOptions}
      placeholder={props.placeholder ?? "Chọn khóa học"}
      searchPlaceholder="Tìm theo tên hoặc mã khóa học..."
      emptyText="Không tìm thấy khóa học."
    />
  );
}

interface FlashcardSessionSelectorProps extends EntitySelectorProps {
  userId?: string;
}

export function FlashcardSessionSelector({ userId, ...props }: FlashcardSessionSelectorProps) {
  const loadOptions = useCallback(async () => {
    const result = await reviewApi.sessions.list({
      userId: userId || undefined,
      page: 1,
      pageSize: 30,
    });

    return result.items.map((item) => ({
      value: String(item.id),
      label: `Flashcard ${new Date(item.startedAt).toLocaleString("vi-VN")}`,
      description: `${item.currentIndex}/${item.totalItems} mục · chính xác ${item.accuracyPercent}%`,
    }));
  }, [userId]);

  return (
    <RemoteEntitySelector
      {...props}
      loadOptions={loadOptions}
      placeholder={props.placeholder ?? "Chọn phiên flashcard"}
      searchPlaceholder="Danh sách phiên gần nhất"
      emptyText="Chưa có phiên flashcard phù hợp."
    />
  );
}
