"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { GraduationCap } from "lucide-react";
import { Controller, useForm } from "react-hook-form";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { FormSection } from "@/components/forms/form-section";
import { FormRow } from "@/components/forms/form-row";
import { FormField } from "@/components/forms/form-field";
import { FormActions } from "@/components/forms/form-actions";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import { ConcurrencyConflictDialog } from "@/components/common/concurrency-conflict-dialog";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import { hskLevelSchema, type HskLevelFormValues } from "../../schemas/hsk-level.schema";
import { useCreateHskLevel, useUpdateHskLevel } from "../../hooks/use-hsk-levels";
import type { AdminHskLevelDto } from "@/dto/learning/hsk-level.dto";

interface HskLevelFormProps {
  item?: AdminHskLevelDto;
}

export function HskLevelForm({ item }: HskLevelFormProps) {
  const router = useRouter();
  const creating = !item;
  const createMutation = useCreateHskLevel();
  const updateMutation = useUpdateHskLevel(item?.id ?? 0);
  const [conflictOpen, setConflictOpen] = useState(false);

  const form = useForm<HskLevelFormValues>({
    resolver: zodResolver(hskLevelSchema),
    defaultValues: {
      id: item?.id ?? 1,
      code: item?.code ?? "HSK1",
      nameVi: item?.nameVi ?? "",
      sortOrder: item?.sortOrder ?? 1,
      isActive: item?.isActive ?? true,
    },
  });

  const loading = createMutation.isPending || updateMutation.isPending;

  async function submit(values: HskLevelFormValues) {
    try {
      if (creating) {
        await createMutation.mutateAsync({
          id: values.id,
          code: values.code.trim().toUpperCase(),
          nameVi: values.nameVi.trim(),
          sortOrder: values.sortOrder,
          isActive: values.isActive,
        });
        appToast.success("Tạo cấp độ HSK thành công.");
      } else {
        await updateMutation.mutateAsync({
          code: values.code.trim().toUpperCase(),
          nameVi: values.nameVi.trim(),
          sortOrder: values.sortOrder,
          isActive: values.isActive,
          concurrencyToken: item.concurrencyToken,
          version: item.version,
        });
        appToast.success("Cập nhật cấp độ HSK thành công.");
      }
      router.push("/cap-do-hsk");
      router.refresh();
    } catch (error) {
      const apiError = normalizeApiError(error);
      if (
        apiError.status === 409 &&
        (apiError.code?.toLowerCase().includes("concurrency") ||
          apiError.code?.toLowerCase().includes("version"))
      ) {
        setConflictOpen(true);
        return;
      }
      if (apiError.status === 409) {
        appToast.error("Không thể lưu dữ liệu", apiError.message);
        return;
      }
      appToast.error(
        creating ? "Không thể tạo cấp độ HSK" : "Không thể cập nhật cấp độ HSK",
        apiError.message
      );
    }
  }

  return (
    <>
      <form onSubmit={form.handleSubmit(submit)} className="space-y-5">
        <FormSection
          title="Thông tin cấp độ HSK"
          description="Thiết lập mã, tên, thứ tự hiển thị và trạng thái cấp độ."
          icon={<GraduationCap size={18} />}
        >
          <div className="space-y-4">
            <FormRow columns={2}>
              <FormField
                label="Cấp HSK"
                required
                error={form.formState.errors.id?.message}
                description={
                  creating
                    ? "ID của cấp độ HSK."
                    : "ID không thể thay đổi sau khi tạo."
                }
              >
                <Input
                  type="number"
                  min={1}
                  max={9}
                  disabled={!creating}
                  value={form.watch("id")}
                  onChange={(event) => {
                    const value = Number(event.target.value);
                    form.setValue("id", value, { shouldValidate: true });
                    if (creating && Number.isFinite(value)) {
                      form.setValue("code", `HSK${value}`);
                      form.setValue("sortOrder", value);
                    }
                  }}
                />
              </FormField>

              <FormField
                label="Mã cấp độ"
                required
                error={form.formState.errors.code?.message}
              >
                <Input {...form.register("code")} placeholder="Ví dụ: HSK1" />
              </FormField>
            </FormRow>

            <FormRow columns={2}>
              <FormField
                label="Tên cấp độ"
                required
                error={form.formState.errors.nameVi?.message}
              >
                <Input
                  {...form.register("nameVi")}
                  placeholder="Ví dụ: HSK 1 - Sơ cấp"
                />
              </FormField>

              <FormField
                label="Thứ tự hiển thị"
                required
                error={form.formState.errors.sortOrder?.message}
              >
                <Input
                  type="number"
                  min={0}
                  value={form.watch("sortOrder")}
                  onChange={(event) => {
                    form.setValue("sortOrder", Number(event.target.value), {
                      shouldValidate: true,
                    });
                  }}
                />
              </FormField>
            </FormRow>

            <FormField label="Trạng thái">
              <Controller
                name="isActive"
                control={form.control}
                render={({ field }) => (
                  <Switch
                    checked={field.value}
                    onCheckedChange={field.onChange}
                    label={field.value ? "Đang hoạt động" : "Ngừng hoạt động"}
                    description={
                      field.value
                        ? "Cấp độ có thể được sử dụng trong hệ thống."
                        : "Cấp độ sẽ không được sử dụng cho dữ liệu mới."
                    }
                  />
                )}
              />
            </FormField>
          </div>
        </FormSection>

        <FormActions
          loading={loading}
          submitText={creating ? "Tạo cấp độ HSK" : "Lưu thay đổi"}
          onCancel={() => router.push("/cap-do-hsk")}
        />
      </form>

      <ConcurrencyConflictDialog
        open={conflictOpen}
        onCancel={() => setConflictOpen(false)}
        onReload={() => {
          window.location.reload();
        }}
      />
    </>
  );
}
