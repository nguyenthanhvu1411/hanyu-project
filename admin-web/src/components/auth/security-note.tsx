import {
  LockKeyhole,
} from "lucide-react";

export function SecurityNote() {
  return (
    <div
      className="
        mx-auto flex
        max-w-[365px]
        items-start
        justify-center
        gap-[12px]
        text-[12px]
        leading-[18px]
        text-[#666]
      "
    >
      <div
        className="
          mt-[1px]
          flex h-8 w-8
          shrink-0 items-center justify-center
          rounded-full
          border border-[#1b9a60]
          text-[#1b9a60]
        "
      >
        <LockKeyhole
          size={16}
          strokeWidth={1.9}
        />
      </div>

      <span>
        Chúng tôi cam kết bảo mật
        thông tin của bạn
        <br />
        với công nghệ mã hóa tiên tiến.
      </span>
    </div>
  );
}
