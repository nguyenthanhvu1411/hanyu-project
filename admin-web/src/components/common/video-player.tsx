"use client";

import {
  Maximize2,
  PlayCircle,
} from "lucide-react";

import {
  useRef,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

interface VideoPlayerProps {
  src: string;

  poster?: string;

  title?: string;

  className?: string;
}

export function VideoPlayer({
  src,
  poster,
  title,
  className,
}: VideoPlayerProps) {
  const ref =
    useRef<HTMLVideoElement>(
      null,
    );

  async function fullscreen() {
    await ref.current?.requestFullscreen?.();
  }

  return (
    <div
      className={cn(
        "overflow-hidden",
        "rounded-[10px]",
        "border",
        "border-[#e6e1da]",
        "bg-black",
        className,
      )}
    >
      <div className="relative aspect-video">
        <video
          ref={ref}
          src={src}
          poster={poster}
          controls
          preload="metadata"
          className="
            h-full
            w-full
            object-contain
          "
        />

        <button
          type="button"
          onClick={
            fullscreen
          }
          className="
            absolute
            right-3
            top-3
            flex h-8 w-8
            items-center
            justify-center
            rounded-[6px]
            bg-black/50
            text-white
            backdrop-blur
          "
        >
          <Maximize2
            size={14}
          />
        </button>
      </div>

      {title && (
        <div
          className="
            flex
            items-center
            gap-2
            bg-white
            px-3
            py-2
            text-[10px]
            text-[#666]
          "
        >
          <PlayCircle
            size={13}
          />

          {title}
        </div>
      )}
    </div>
  );
}
