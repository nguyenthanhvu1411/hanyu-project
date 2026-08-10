import {
  UserRound,
} from "lucide-react";

import {
  cn,
} from "@/lib/utils/cn";

interface AvatarProps {
  src?: string | null;

  alt?: string;

  name?: string;

  size?:
    | "sm"
    | "md"
    | "lg"
    | "xl";

  className?: string;
}

const sizes = {
  sm:
    "h-7 w-7 text-[9px]",

  md:
    "h-9 w-9 text-[11px]",

  lg:
    "h-12 w-12 text-[13px]",

  xl:
    "h-16 w-16 text-[16px]",
};

export function Avatar({
  src,
  alt = "",
  name,
  size = "md",
  className,
}: AvatarProps) {
  const initials =
    name
      ?.split(/\s+/)
      .filter(Boolean)
      .slice(-2)
      .map(
        (word) =>
          word[0]?.toUpperCase(),
      )
      .join("") ?? "";

  return (
    <div
      className={cn(
        "flex",
        "shrink-0",
        "items-center",
        "justify-center",
        "overflow-hidden",
        "rounded-full",
        "bg-[#fff0ee]",
        "font-semibold",
        "text-[#ef241c]",
        sizes[size],
        className,
      )}
    >
      {src ? (
        <img
          src={src}
          alt={alt}
          className="
            h-full
            w-full
            object-cover
          "
        />
      ) : initials ? (
        initials
      ) : (
        <UserRound
          size="50%"
        />
      )}
    </div>
  );
}
