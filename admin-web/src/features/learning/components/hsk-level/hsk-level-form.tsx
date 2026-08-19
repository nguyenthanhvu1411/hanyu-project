"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { GraduationCap, Info, ListOrdered } from "lucide-react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";

import { FormActions } from "@/components/forms/form-actions";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { FormSection } from "@/components/forms/form-section";
import { Input } from "@/components/ui/input";
import { appToast } from "@/components/ui/toast";
import type { AdminHskLevelDto } from "@/dto/learning/hsk-level.dto";
import { normalizeApiError } from "@/lib/api/api-error";

import { useCreateHskLevel, useUpdateHskLevel } from "../../hooks/use-hsk-levels";
import { hskLevelSchema, type HskLevelFormValues } from "../../schemas/hsk-level.schema";
import { HskLevelStatusBadge } from "./hsk-level-status-badge";

interface HskLevelFormProps {
  item?: AdminHskLevelDto;
}

export function HskLevelForm({ item }: HskLevelFormProps) {
  const router = useRouter();
  const creating = !item;
  const createMutation = useCreateHskLevel();
  const updateMutation = useUpdateHskLevel(item?.id ?? 0);

  const form = useForm<HskLevelFormValues>({
    resolver: zodResolver(hskLevelSchema),
    defaultValues: {
      code: item?.code ?? "",
      nameVi: item?.nameVi ?? "",
      sortOrder: item?.sortOrder ?? 0,
    },
  });

  const loading = createMutation.isPending || updateMutation.isPending;
  const dirty = form.formState.isDirty;

  async function submit(values: HskLevelFormValues) {
    const request = {
      code: values.code.trim().toUpperCase(),
      nameVi: values.nameVi.trim(),
      sortOrder: values.sortOrder,
    };

    try {
      if (creating) {
        const created = await createMutation.mutateAsync(request);
        appToast.success("Tạo cấp độ HSK thành công.");
        router.replace(`/cap-do-hsk/${created.id}`);
      } else {
        const updated = await updateMutation.mutateAsync(request);
        appToast.success("Cập nhật cấp độ HSK thành công.");
        router.replace(`/cap-do-hsk/${updated.id}`);
      }

      router.refresh();
    } catch (error) {
      const apiError = normalizeApiError(error);
      appToast.error(
        creating ? "Không thể tạo cấp độ HSK" : "Không thể cập nhật cấp độ HSK",
        apiError.message,
      );
    }
  }

  const cancelHref = item ? `/cap-do-hsk/${item.id}` : "/cap-do-hsk";

  return (
    <form onSubmit={form.handleSubmit(submit)} className="space-y-5">
      {item ? (
        <FormSection
          title="Cấp độ đang chỉnh sửa"
          description="Thông tin định danh hiện tại. Trạng thái kích hoạt được quản lý riêng tại trang chi tiết."
          icon={<Info size={18} />}
        >
          <div className="grid gap-3 md:grid-cols-3">
            <ReadOnlyInfo label="Mã hiện tại" value={item.code} />
            <ReadOnlyInfo label="ID nội bộ" value={String(item.id)} />
            <div className="rounded-[8px] border border-[#eee9e2] bg-[#fcfbf9] px-3.5 py-3">
              <div className="text-[10px] font-medium text-[#929292]">Trạng thái</div>
              <div className="mt-[6px]">
                <HskLevelStatusBadge isActive={item.isActive} />
              </div>
            </div>
          </div>
        </FormSection>
      ) : null}

      <FormSection
        title="Thông tin cấp độ HSK"
        description="Thiết lập mã, tên hiển thị và thứ tự trong danh mục HSK. Các giá trị này được Course, Lesson và Vocabulary sử dụng làm dữ liệu tham chiếu."
        icon={<GraduationCap size={18} />}
      >
        <div className="space-y-4">
          <FormRow columns={2}>
            <FormField
              label="Mã cấp độ"
              required
              description="Ví dụ HSK1, HSK2. Mã sẽ được chuẩn hóa thành chữ hoa khi lưu."
              error={form.formState.errors.code?.message}
            >
              <Input
                {...form.register("code")}
                placeholder="Ví dụ: HSK1"
                autoComplete="off"
                maxLength={20}
              />
            </FormField>

            <FormField
              label="Tên cấp độ"
              required
              description="Tên tiếng Việt hiển thị trong danh mục lựa chọn của admin-web."
              error={form.formState.errors.nameVi?.message}
            >
              <Input
                {...form.register("nameVi")}
                placeholder="Ví dụ: HSK 1 - Sơ cấp"
                autoComplete="off"
                maxLength={100}
              />
            </FormField>
          </FormRow>

          <FormRow columns={2}>
            <FormField
              label="Thứ tự hiển thị"
              required
              description="Giá trị nhỏ hơn sẽ được hiển thị trước trong danh sách và các picker."
              error={form.formState.errors.sortOrder?.message}
            >
              <div className="relative">
                <ListOrdered
                  size={15}
                  className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-[#999]"
                />
                <Input
                  type="number"
                  min={0}
                  step={1}
                  className="pl-9"
                  value={form.watch("sortOrder")}
                  onChange={(event) => {
                    form.setValue("sortOrder", Number(event.target.value), {
                      shouldValidate: true,
                      shouldDirty: true,
                    });
                  }}
                />
              </div>
            </FormField>
          </FormRow>
        </div>
      </FormSection>

      <FormActions
        loading={loading}
        disabled={!creating && !dirty}
        submitText={creating ? "Tạo cấp độ HSK" : "Lưu thay đổi"}
        onCancel={() => router.push(cancelHref)}
      />
    </form>
  );
}

function ReadOnlyInfo({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-[8px] border border-[#eee9e2] bg-[#fcfbf9] px-3.5 py-3">
      <div className="text-[10px] font-medium text-[#929292]">{label}</div>
      <div className="mt-[6px] truncate text-[12px] font-semibold text-[#333]">
        {value}
      </div>
    </div>
  );
}
