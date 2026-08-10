import {
  APP_CONSTANTS,
} from "@/constants/app.constants";

export function AdminFooter() {
  return (
    <footer
      className="
        mt-auto
        border-t
        border-[#ebe6df]
        bg-white
      "
    >
      <div
        className="
          mx-auto
          flex
          min-h-[52px]
          max-w-[1600px]
          flex-col
          items-center
          justify-between
          gap-2
          px-5
          py-3
          text-[11px]
          text-[#8a8a8a]
          sm:flex-row
          lg:px-6
        "
      >
        <span>
          {
            APP_CONSTANTS.COPYRIGHT
          }
        </span>

        <div className="flex items-center gap-4">
          <button
            type="button"
            className="hover:text-[#ef241c]"
          >
            Điều khoản sử dụng
          </button>

          <button
            type="button"
            className="hover:text-[#ef241c]"
          >
            Chính sách bảo mật
          </button>
        </div>
      </div>
    </footer>
  );
}
