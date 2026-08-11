"use client";

import { useEffect, useMemo, useState } from "react";
import { ImageIcon, Loader2 } from "lucide-react";

import { getStorageObjectKey, mediaApi } from "@/features/system/api/media.api";

interface StorageImageProps {
  value?: string | null;
  alt: string;
  className?: string;
  emptyClassName?: string;
}

export function StorageImage({
  value,
  alt,
  className = "h-full w-full object-cover",
  emptyClassName = "min-h-[180px]",
}: StorageImageProps) {
  const objectKey = useMemo(() => getStorageObjectKey(value), [value]);
  const [resolvedUrl, setResolvedUrl] = useState<string | null>(null);
  const [loading, setLoading] = useState(Boolean(objectKey));
  const [failed, setFailed] = useState(false);

  useEffect(() => {
    let active = true;
    setFailed(false);

    if (!objectKey) {
      setResolvedUrl(value?.trim() || null);
      setLoading(false);
      return () => {
        active = false;
      };
    }

    setLoading(true);
    void mediaApi
      .getReadUrl(objectKey)
      .then((result) => {
        if (active) setResolvedUrl(result.url);
      })
      .catch(() => {
        if (active) {
          setResolvedUrl(null);
          setFailed(true);
        }
      })
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, [objectKey, value]);

  if (loading) {
    return (
      <div className={`grid place-items-center bg-[#faf9f7] ${emptyClassName}`}>
        <Loader2 size={20} className="animate-spin text-[#999]" />
      </div>
    );
  }

  if (!resolvedUrl || failed) {
    return (
      <div className={`flex flex-col items-center justify-center gap-2 bg-[#faf9f7] text-[#999] ${emptyClassName}`}>
        <ImageIcon size={22} />
        <span className="text-[11px]">Chưa có ảnh bìa</span>
      </div>
    );
  }

  // eslint-disable-next-line @next/next/no-img-element
  return <img src={resolvedUrl} alt={alt} className={className} onError={() => setFailed(true)} />;
}
