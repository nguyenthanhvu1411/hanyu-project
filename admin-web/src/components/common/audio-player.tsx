"use client";

import {
  Pause,
  Play,
  RotateCcw,
  Volume2,
} from "lucide-react";

import {
  useRef,
  useState,
} from "react";

import {
  cn,
} from "@/lib/utils/cn";

interface AudioPlayerProps {
  src: string;

  title?: string;

  className?: string;
}

export function AudioPlayer({
  src,
  title,
  className,
}: AudioPlayerProps) {
  const audioRef =
    useRef<HTMLAudioElement>(
      null,
    );

  const [
    playing,
    setPlaying,
  ] = useState(false);

  const [
    duration,
    setDuration,
  ] = useState(0);

  const [
    currentTime,
    setCurrentTime,
  ] = useState(0);

  function togglePlay() {
    const audio =
      audioRef.current;

    if (!audio) {
      return;
    }

    if (
      audio.paused
    ) {
      void audio.play();
    } else {
      audio.pause();
    }
  }

  function reset() {
    const audio =
      audioRef.current;

    if (!audio) {
      return;
    }

    audio.currentTime = 0;

    setCurrentTime(0);
  }

  function seek(
    value: number,
  ) {
    const audio =
      audioRef.current;

    if (!audio) {
      return;
    }

    audio.currentTime =
      value;

    setCurrentTime(
      value,
    );
  }

  return (
    <div
      className={cn(
        "rounded-[10px]",
        "border",
        "border-[#e7e2db]",
        "bg-[#faf9f7]",
        "p-3",
        className,
      )}
    >
      <audio
        ref={
          audioRef
        }
        src={src}
        preload="metadata"
        onLoadedMetadata={(
          event,
        ) =>
          setDuration(
            event.currentTarget
              .duration ||
              0,
          )
        }
        onTimeUpdate={(
          event,
        ) =>
          setCurrentTime(
            event.currentTarget
              .currentTime,
          )
        }
        onPlay={() =>
          setPlaying(true)
        }
        onPause={() =>
          setPlaying(false)
        }
        onEnded={() =>
          setPlaying(false)
        }
      />

      <div
        className="
          flex
          items-center
          gap-3
        "
      >
        <button
          type="button"
          onClick={
            togglePlay
          }
          className="
            flex h-9 w-9
            shrink-0
            items-center
            justify-center
            rounded-full
            bg-[#ef241c]
            text-white
            transition
            hover:bg-[#d91f19]
          "
        >
          {playing ? (
            <Pause
              size={16}
              fill="currentColor"
            />
          ) : (
            <Play
              size={16}
              fill="currentColor"
            />
          )}
        </button>

        <div className="min-w-0 flex-1">
          {title && (
            <div
              className="
                mb-2
                truncate
                text-[11px]
                font-medium
                text-[#444]
              "
            >
              {title}
            </div>
          )}

          <input
            type="range"
            min={0}
            max={
              duration ||
              0
            }
            step={0.1}
            value={
              currentTime
            }
            onChange={(
              event,
            ) =>
              seek(
                Number(
                  event
                    .target
                    .value,
                ),
              )
            }
            className="w-full"
          />

          <div
            className="
              mt-1
              flex
              justify-between
              text-[9px]
              text-[#929292]
            "
          >
            <span>
              {formatTime(
                currentTime,
              )}
            </span>

            <span>
              {formatTime(
                duration,
              )}
            </span>
          </div>
        </div>

        <div
          className="
            flex
            items-center
            gap-1
            text-[#777]
          "
        >
          <Volume2
            size={16}
          />

          <button
            type="button"
            onClick={reset}
            className="
              flex h-8 w-8
              items-center
              justify-center
              rounded-[6px]
              hover:bg-white
              hover:text-[#ef241c]
            "
          >
            <RotateCcw
              size={14}
            />
          </button>
        </div>
      </div>
    </div>
  );
}

function formatTime(
  seconds: number,
) {
  if (
    !Number.isFinite(
      seconds,
    )
  ) {
    return "00:00";
  }

  const minute =
    Math.floor(
      seconds / 60,
    );

  const second =
    Math.floor(
      seconds % 60,
    );

  return `${String(
    minute,
  ).padStart(
    2,
    "0",
  )}:${String(
    second,
  ).padStart(
    2,
    "0",
  )}`;
}
