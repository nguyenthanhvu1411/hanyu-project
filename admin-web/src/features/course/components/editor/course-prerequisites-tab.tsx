"use client";

import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link2 } from "lucide-react";
import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableActions } from "@/components/common/data-table/data-table-actions";
import { EmptyState } from "@/components/common/empty-state";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { FormSection } from "@/components/forms/form-section";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Select } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { courseApi } from "../../api/course.api";
import type { AdminCourseListItem } from "../../types/course.types";
import type { DataTableColumn } from "@/types/table.types";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import type { CoursePrerequisite } from "../../types/curriculum.types";

export function CoursePrerequisitesTab({ editor }: { editor: CourseEditorController }) {
  const [requiredCourseId, setRequiredCourseId] = useState("");
  const [isRequired, setRequired] = useState(true);
  const [courses, setCourses] = useState<AdminCourseListItem[]>([]);
  const [courseLoading, setCourseLoading] = useState(true);

  useEffect(() => {
    let active = true;
    setCourseLoading(true);

    void courseApi
      .list({ page: 1, pageSize: 100, isActive: true, sortBy: "sortorder", sortDescending: false })
      .then((result) => {
        if (active) setCourses(result.items ?? []);
      })
      .finally(() => {
        if (active) setCourseLoading(false);
      });

    return () => {
      active = false;
    };
  }, []);

  const prerequisiteIds = useMemo(
    () => new Set(editor.prerequisites.map((item) => item.requiredCourseId)),
    [editor.prerequisites],
  );

  const courseOptions = useMemo(
    () =>
      courses
        .filter((course) => course.id !== editor.course?.id && !prerequisiteIds.has(course.id))
        .map((course) => ({
          value: String(course.id),
          label: `${course.code} — ${course.titleVi}`,
          description: course.hskCode ? `Cấp độ ${course.hskCode}` : "Chưa phân loại HSK",
        })),
    [courses, editor.course?.id, prerequisiteIds],
  );

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const id = Number(requiredCourseId);
    if (!Number.isSafeInteger(id) || id <= 0) return;

    const created = await editor.createPrerequisite({
      requiredCourseId: id,
      isRequired,
      sortOrder: editor.prerequisites.length,
    });
    if (created) setRequiredCourseId("");
  }

  const columns = useMemo<DataTableColumn<CoursePrerequisite>[]>(() => [
    {
      id: "course",
      header: "Khóa học",
      cell: (item) => (
        <div>
          <div className="text-[12px] font-semibold text-[#333]">{item.requiredCourseTitleVi}</div>
          <div className="mt-0.5 text-[10px] text-[#888]">{item.requiredCourseCode} · PublicId {item.requiredCoursePublicId.slice(0, 8)}…</div>
        </div>
      ),
    },
    {
      id: "required",
      header: "Loại",
      width: "140px",
      cell: (item) => <Badge variant={item.isRequired ? "warning" : "default"}>{item.isRequired ? "Bắt buộc" : "Khuyến nghị"}</Badge>,
    },
    {
      id: "actions",
      header: "Thao tác",
      align: "center",
      width: "90px",
      cell: (item) => editor.canEdit ? (
        <DataTableActions onDelete={() => void editor.deletePrerequisite(item)} />
      ) : null,
    },
  ], [editor]);

  return (
    <div className="space-y-4">
      {editor.canEdit ? (
        <form onSubmit={submit}>
          <FormSection
            title="Thêm khóa học tiên quyết"
            description="Chọn khóa học cần hoàn thành hoặc nên học trước. Admin không phải nhập ID nội bộ thủ công."
            icon={<Link2 size={18} />}
          >
            <FormRow columns={3}>
              <FormField label="Khóa học" required description="Danh mục lấy trực tiếp từ backend Course.">
                <Select
                  value={requiredCourseId}
                  onValueChange={setRequiredCourseId}
                  options={courseOptions}
                  placeholder={courseLoading ? "Đang tải khóa học..." : "Chọn khóa học tiên quyết"}
                  disabled={courseLoading || editor.saving}
                />
              </FormField>
              <FormField label="Mức độ">
                <Switch checked={isRequired} onCheckedChange={setRequired} label={isRequired ? "Bắt buộc" : "Khuyến nghị"} />
              </FormField>
              <div className="flex items-end">
                <Button type="submit" disabled={editor.saving || !requiredCourseId} className="w-full">Thêm tiên quyết</Button>
              </div>
            </FormRow>
          </FormSection>
        </form>
      ) : null}

      {editor.prerequisites.length === 0 ? (
        <EmptyState title="Chưa có khóa học tiên quyết" description="Khóa học này hiện không yêu cầu điều kiện tiên quyết." />
      ) : (
        <DataTable
          data={editor.prerequisites}
          columns={columns}
          rowKey={(item) => item.id}
          loading={false}
          selectable={false}
          page={1}
          pageSize={Math.max(1, editor.prerequisites.length)}
          totalItems={editor.prerequisites.length}
          totalPages={1}
          onPageChange={() => undefined}
          onPageSizeChange={() => undefined}
        />
      )}
    </div>
  );
}
