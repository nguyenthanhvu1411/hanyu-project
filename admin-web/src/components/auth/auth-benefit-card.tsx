import { CheckCircle2, Star, Users } from "lucide-react";

export function AuthBenefitCard() {
  return (
    <div className="bg-white/95 backdrop-blur-sm rounded-2xl p-6 shadow-[0_8px_30px_rgb(0,0,0,0.08)] max-w-[480px]">
      <div className="flex gap-4 items-start mb-6">
        <div className="w-12 h-12 rounded-full bg-orange-50 flex items-center justify-center flex-shrink-0 border border-orange-100">
          <CheckCircle2 className="w-6 h-6 text-orange-500" />
        </div>
        <div>
          <h3 className="text-base font-semibold text-gray-900 mb-1">
            Hành trình vững vàng – Tiến bộ mỗi ngày
          </h3>
          <p className="text-sm text-gray-600 leading-relaxed">
            Hàng triệu người học đã tin tưởng lựa chọn Học Tiếng Trung để chinh phục tiếng Trung một cách hiệu quả và bền vững.
          </p>
        </div>
      </div>

      <div className="grid grid-cols-3 gap-4 pt-4 border-t border-gray-100">
        <div className="flex items-center gap-2">
          <div className="w-8 h-8 rounded-full bg-green-50 flex items-center justify-center flex-shrink-0">
            <span className="text-green-600 font-bold text-xs">HSK</span>
          </div>
          <span className="text-xs text-gray-600 leading-tight">Lộ trình chuẩn HSK<br/>bài bản, khoa học</span>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-8 h-8 rounded-full bg-orange-50 flex items-center justify-center flex-shrink-0">
            <Star className="w-4 h-4 text-orange-500" />
          </div>
          <span className="text-xs text-gray-600 leading-tight">Giảng dạy bởi giáo viên<br/>kinh nghiệm</span>
        </div>
        <div className="flex items-center gap-2">
          <div className="w-8 h-8 rounded-full bg-red-50 flex items-center justify-center flex-shrink-0">
            <Users className="w-4 h-4 text-red-500" />
          </div>
          <span className="text-xs text-gray-600 leading-tight">Cộng đồng học tập<br/>sôi nổi, hỗ trợ 24/7</span>
        </div>
      </div>
    </div>
  );
}
