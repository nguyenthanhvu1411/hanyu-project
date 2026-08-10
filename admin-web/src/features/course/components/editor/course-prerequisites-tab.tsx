"use client";

import { FormEvent, useMemo, useState } from "react";
import { Link2 } from "lucide-react";
import { DataTable } from "@/components/common/data-table/data-table";
import { DataTableActions } from "@/components/common/data-table/data-table-actions";
import { EmptyState } from "@/components/common/empty-state";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { FormSection } from "@/components/forms/form-section";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import type { DataTableColumn } from "@/types/table.types";
import type { CourseEditorController } from "../../hooks/use-course-editor";
import type { CoursePrerequisite } from "../../types/curriculum.types";

export function CoursePrerequisitesTab({ editor }: { editor: CourseEditorController }) {
  const [requiredCourseId, setRequiredCourseId] = useState("");
  const [isRequired, setRequired] = useState(true);

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
        <DataTableActions
          onDelete={() => {
            if (window.confirm("Xóa điều kiện tiên quyết này?")) void editor.deletePrerequisite(item);
          }}
        />
      ) : null,
    },
  ], [editor]);

  return (
    <div className="space-y-4">
      {editor.canEdit ? (
        <form onSubmit={submit}>
          <FormSection
            title="Thêm khóa học tiên quyết"
            description="Gắn khóa học cần hoàn thành hoặc nên học trước khóa học hiện tại."
            icon={<Link2 size={18} />}
          >
            <FormRow columns={3}>
              <FormField label="Course ID" required description="Backend dùng long RequiredCourseId.">
                <Input type="number" min={1} value={requiredCourseId} onChange={(e) => setRequiredCourseId(e.target.value)} placeholder="Nhập Course ID" />
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
