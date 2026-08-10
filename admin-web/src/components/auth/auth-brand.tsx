import Image from "next/image";
import Link from "next/link";

export function AuthBrand() {
  return (
    <Link href="/" className="flex items-center gap-3 select-none">
      <div className="relative w-10 h-10 flex-shrink-0">
        <Image
          src="/logo/logo.svg"
          alt="Học Tiếng Trung Logo"
          fill
          className="object-contain"
          priority
        />
      </div>
      <span className="text-lg font-bold text-gray-900 tracking-tight">
        HỌC TIẾNG TRUNG
      </span>
    </Link>
  );
}
