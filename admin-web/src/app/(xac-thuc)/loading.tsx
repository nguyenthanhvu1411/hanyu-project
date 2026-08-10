export default function AuthLoading() {
  return (
    <main
      className="
        flex min-h-screen
        items-center justify-center
        bg-[#fffaf0]
      "
    >
      <div
        className="
          flex items-center gap-3
          text-[14px]
          text-[#666]
        "
      >
        <span
          className="
            h-5 w-5
            animate-spin
            rounded-full
            border-2
            border-[#ef241c]/20
            border-t-[#ef241c]
          "
        />

        Đang tải...
      </div>
    </main>
  );
}
