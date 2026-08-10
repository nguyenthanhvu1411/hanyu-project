"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { GraduationCap } from "lucide-react";
import { useForm } from "react-hook-form";
import { useRouter } from "next/navigation";

import { FormSection } from "@/components/forms/form-section";
import { FormRow } from "@/components/forms/form-row";
import { FormField } from "@/components/forms/form-field";
import { FormActions } from "@/components/forms/form-actions";
import { Input } from "@/components/ui/input";
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

  const form = useForm<HskLevelFormValues>({
    resolver: zodResolver(hskLevelSchema),
    defaultValues: {
      code: item?.code ?? "",
      nameVi: item?.nameVi ?? "",
      sortOrder: item?.sortOrder ?? 0,
    },
  });

  const loading = createMutation.isPending || updateMutation.isPending;

  async function submit(values: HskLevelFormValues) {
    const request = {
      code: values.code.trim().toUpperCase(),
      nameVi: values.nameVi.trim(),
      sortOrder: values.sortOrder,
    };

    try {
      if (creating) {
        await createMutation.mutateAsync(request);
        appToast.success("Tạo cấp độ HSK thành công.");
      } else {
        await updateMutation.mutateAsync(request);
        appToast.success("Cập nhật cấp độ HSK thành công.");
      }

      router.push("/cap-do-hsk");
      router.refresh();
    } catch (error) {
      const apiError = normalizeApiError(error);
      appToast.error(
        creating ? "Không thể tạo cấp độ HSK" : "Không thể cập nhật cấp độ HSK",
        apiError.message,
      );
    }
  }

  return (
    <form onSubmit={form.handleSubmit(submit)} className="space-y-5">
      <FormSection
        title="Thông tin cấp độ HSK"
        description="Thiết lập mã, tên và thứ tự hiển thị theo đúng dữ liệu backend. Trạng thái được quản lý bằng thao tác kích hoạt/ngừng hoạt động ở danh sách."
        icon={<GraduationCap size={18} />}
      >
        <div className="space-y-4">
          <FormRow columns={2}>
            <FormField
              label="Mã cấp độ"
              required
              error={form.formState.errors.code?.message}
            >
              <Input {...form.register("code")} placeholder="Ví dụ: HSK1" />
            </FormField>

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
          </FormRow>

          <FormRow columns={2}>
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
        </div>
      </FormSection>

      <FormActions
        loading={loading}
        submitText={creating ? "Tạo cấp độ HSK" : "Lưu thay đổi"}
        onCancel={() => router.push("/cap-do-hsk")}
      />
    </form>
  );
}
