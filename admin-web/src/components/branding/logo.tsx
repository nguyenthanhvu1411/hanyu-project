import Link from "next/link";

import { BrandName } from "./brand-name";
import { LogoMark } from "./logo-mark";

import { ROUTES } from "@/constants/route.constants";
import { cn } from "@/lib/utils/cn";

interface LogoProps {
  className?: string;
  showName?: boolean;
}

export function Logo({
  className,
  showName = true,
}: LogoProps) {
  return (
    <Link
      href={ROUTES.DANG_NHAP}
      className={cn(
        "inline-flex items-center gap-3",
        className,
      )}
    >
      <LogoMark />

      {showName && <BrandName />}
    </Link>
  );
}
