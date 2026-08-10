import Image from "next/image";

export function AuthHero() {
  return (
    <section
      className="
        relative hidden
        h-full
        w-full
        overflow-hidden
        bg-[#fff9ed]
        xl:flex
        xl:flex-col
      "
    >
      <Image
        src="/images/xac-thuc/dang-nhap-background.webp"
        alt="Nền trang đăng nhập"
        fill
        priority
        className="
          pointer-events-none
          object-contain
          object-center
        "
      />
    </section>
  );
}
