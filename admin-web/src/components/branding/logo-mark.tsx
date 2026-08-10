import { cn } from "@/lib/utils/cn";

interface LogoMarkProps {
  className?: string;
}

export function LogoMark({
  className,
}: LogoMarkProps) {
  return (
    <div
      className={cn(
        "flex h-8 w-8 items-center justify-center",
        "rounded-[6px] bg-[#f5221d]",
        className,
      )}
      aria-hidden="true"
    >
      <svg
        width="24"
        height="24"
        viewBox="0 0 34 34"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <rect
          x="1.2"
          y="1.2"
          width="31.6"
          height="31.6"
          rx="2.8"
          stroke="white"
          strokeWidth="2.4"
        />

        <path
          d="M7 4.8V29.2M12 4.8V29.2M17 4.8V29.2M22 4.8V29.2M27 4.8V29.2"
          stroke="white"
          strokeWidth="1.2"
        />

        <path
          d="M4.8 9H29.2M4.8 14H29.2M4.8 19H29.2M4.8 24H29.2"
          stroke="white"
          strokeWidth="1.2"
        />
      </svg>
    </div>
  );
}
