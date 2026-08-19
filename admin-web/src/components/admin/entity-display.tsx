"use client";

import { useEffect, useState } from "react";

import { Skeleton } from "@/components/ui/skeleton";
import { identityApi } from "@/features/identity/identity.api";
import { lessonApi } from "@/features/lesson/api/lesson.api";
import { apiClient } from "@/lib/api/api-client";

type EntityKind = "user" | "lesson" | "vocabulary";

interface EntityLabel {
  label: string;
  description?: string;
}

const cache = new Map<string, EntityLabel>();

function cacheKey(kind: EntityKind, id: string | number) {
  return `${kind}:${id}`;
}

async function loadEntity(kind: EntityKind, id: string | number): Promise<EntityLabel> {
  const key = cacheKey(kind, id);
  const cached = cache.get(key);
  if (cached) return cached;

  let result: EntityLabel;

  if (kind === "user") {
    const user = await identityApi.users.get(String(id));
    result = {
      label: user.displayName || user.email,
      description: user.displayName ? user.email : undefined,
    };
  } else if (kind === "lesson") {
    const lesson = await lessonApi.getById(Number(id));
    result = {
      label: lesson.titleVi,
      description: [lesson.hskCode, lesson.courseTitleVi].filter(Boolean).join(" · "),
    };
  } else {
    const vocabulary = await apiClient<{
      simplified: string;
      pinyin: string;
      primaryMeaningVi: string;
      hskCode?: string | null;
    }>(`/admin/vocabularies/${Number(id)}`);
    result = {
      label: `${vocabulary.simplified} · ${vocabulary.pinyin}`,
      description: `${vocabulary.primaryMeaningVi}${vocabulary.hskCode ? ` · ${vocabulary.hskCode}` : ""}`,
    };
  }

  cache.set(key, result);
  return result;
}

interface EntityDisplayProps {
  kind: EntityKind;
  id?: string | number | null;
  label?: string | null;
  description?: string | null;
  compact?: boolean;
}

export function EntityDisplay({
  kind,
  id,
  label,
  description,
  compact = false,
}: EntityDisplayProps) {
  const [data, setData] = useState<EntityLabel | null>(
    label ? { label, description: description ?? undefined } : null,
  );
  const [loading, setLoading] = useState(Boolean(id && !label));

  useEffect(() => {
    if (label) {
      setData({ label, description: description ?? undefined });
      setLoading(false);
      return;
    }

    if (id === undefined || id === null || id === "") {
      setData(null);
      setLoading(false);
      return;
    }

    let active = true;
    setLoading(true);
    void loadEntity(kind, id)
      .then((next) => {
        if (active) setData(next);
      })
      .catch(() => {
        if (active) setData({ label: "Không tìm thấy đối tượng" });
      })
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, [description, id, kind, label]);

  if (loading) {
    return <Skeleton className={compact ? "h-4 w-24" : "h-8 w-36"} />;
  }

  if (!data) return <span className="text-[#aaa]">—</span>;

  return (
    <div className="min-w-0">
      <div className="truncate font-medium text-[#444]">{data.label}</div>
      {!compact && data.description ? (
        <div className="mt-0.5 truncate text-[10px] text-[#999]">{data.description}</div>
      ) : null}
    </div>
  );
}

export function UserDisplay(props: Omit<EntityDisplayProps, "kind">) {
  return <EntityDisplay {...props} kind="user" />;
}

export function LessonDisplay(props: Omit<EntityDisplayProps, "kind">) {
  return <EntityDisplay {...props} kind="lesson" />;
}

export function VocabularyDisplay(props: Omit<EntityDisplayProps, "kind">) {
  return <EntityDisplay {...props} kind="vocabulary" />;
}
