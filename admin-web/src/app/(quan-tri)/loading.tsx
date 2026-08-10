export default function AdminLoading() {
  return (
    <div
      className="
        flex min-h-[400px]
        items-center
        justify-center
      "
    >
      <div
        className="
          flex
          items-center
          gap-3
          text-[12px]
          text-[#777]
        "
      >
        <div
          className="
            h-5 w-5
            animate-spin
            rounded-full
            border-2
            border-[#ef241c]/20
            border-t-[#ef241c]
          "
        />

        Đang tải dữ liệu...
      </div>
    </div>
  );
}
