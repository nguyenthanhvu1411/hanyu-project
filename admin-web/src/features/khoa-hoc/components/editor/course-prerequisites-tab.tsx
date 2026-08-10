"use client";

import { FormEvent, useState } from "react";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import { Button } from "@/components/ui/button";

export function CoursePrerequisitesTab({
  editor,
}: {
  editor: CourseEditorController;
}) {
  const [requiredCourseId, setRequiredCourseId] = useState("");
  const [isRequired, setRequired] = useState(true);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const id = Number(requiredCourseId);

    if (id <= 0) {
      return;
    }

    const created = await editor.createPrerequisite({
      requiredCourseId: id,
      isRequired,
      sortOrder: editor.prerequisites.length,
    });

    if (created) {
      setRequiredCourseId("");
    }
  }

  return (
    <section className="space-y-4">
      <div>
        <h2 className="text-lg font-semibold">Khóa học tiên quyết</h2>
        <p className="mt-1 text-sm text-neutral-500">
          Những khóa học người học nên hoặc bắt buộc hoàn thành trước.
        </p>
      </div>

      {editor.canEdit && (
        <form
          onSubmit={submit}
          className="flex flex-wrap items-end gap-3 rounded-xl border bg-white p-4"
        >
          <label className="min-w-72 flex-1 space-y-1">
            <span className="text-sm font-medium">Course ID</span>
            <input
              type="number"
              min={1}
              value={requiredCourseId}
              onChange={(e) => setRequiredCourseId(e.target.value)}
              placeholder="Nhập Course ID..."
              className="h-10 w-full rounded-lg border px-3 text-sm"
            />
          </label>

          <label className="flex h-10 items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={isRequired}
              onChange={(e) => setRequired(e.target.checked)}
              className="h-4 w-4 rounded border-gray-300 text-red-600 focus:ring-red-500"
            />
            Bắt buộc
          </label>

          <Button type="submit" variant="danger" disabled={editor.saving}>
            Thêm
          </Button>
        </form>
      )}

      <div className="overflow-hidden rounded-xl border bg-white">
        {editor.prerequisites.length === 0 ? (
          <div className="p-10 text-center text-sm text-neutral-500">
            Khóa học này không có điều kiện tiên quyết.
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="border-b bg-neutral-50">
              <tr>
                <th className="px-4 py-3 text-left">Khóa học</th>
                <th className="px-4 py-3 text-left">Loại</th>
                <th className="px-4 py-3 text-right">Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {editor.prerequisites.map((item) => (
                <tr key={item.id} className="border-b last:border-0">
                  <td className="px-4 py-3">
                    <div className="font-medium">
                      {item.requiredCourseTitleVi}
                    </div>
                    <div className="mt-1 text-xs text-neutral-500">
                      {item.requiredCourseCode}
                    </div>
                  </td>
                  <td className="px-4 py-3">
                    {item.isRequired ? "Bắt buộc" : "Khuyến nghị"}
                  </td>
                  <td className="px-4 py-3 text-right">
                    {editor.canEdit && (
                      <button
                        type="button"
                        className="text-red-600 hover:underline"
                        onClick={() => {
                          if (window.confirm("Xóa điều kiện tiên quyết này?")) {
                            void editor.deletePrerequisite(item);
                          }
                        }}
                      >
                        Xóa
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </section>
  );
}
