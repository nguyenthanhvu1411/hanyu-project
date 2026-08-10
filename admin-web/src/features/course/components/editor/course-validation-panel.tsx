import type { CourseValidationResult } from "../../types/curriculum.types";

export function CourseValidationPanel({ result }: { result: CourseValidationResult }) {
  if (result.isValid) {
    return (
      <div className="rounded-xl border border-emerald-200 bg-emerald-50 p-4">
        <p className="font-medium text-emerald-800">Khóa học hợp lệ</p>
        <p className="mt-1 text-sm text-emerald-700">
          Có thể gửi duyệt hoặc xuất bản theo workflow.
        </p>
      </div>
    );
  }

  return (
    <div className="rounded-xl border border-amber-200 bg-amber-50 p-4">
      <h3 className="font-medium text-amber-900">
        Cần xử lý {result.issues.length} vấn đề
      </h3>
      <ul className="mt-3 space-y-2">
        {result.issues.map((issue, index) => (
          <li key={`${issue.code}-${index}`} className="text-sm text-amber-800">
            <strong>{issue.code}</strong> {" — "} {issue.message}
          </li>
        ))}
      </ul>
    </div>
  );
}
