"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { Controller, useForm } from "react-hook-form";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { FormSection } from "@/components/forms/form-section";
import { FormField } from "@/components/forms/form-field";
import { FormRow } from "@/components/forms/form-row";
import { FormActions } from "@/components/forms/form-actions";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { MultiSelect } from "@/components/common/multi-select";
import { appToast } from "@/components/ui/toast";
import { PermissionMatrix } from "./permission-matrix";
import { ConcurrencyConflictDialog } from "@/components/common/concurrency-conflict-dialog";
import { normalizeApiError } from "@/lib/api/api-error";
import type { AdminRoleDto } from "@/dto/identity/admin-role.dto";
import { adminRoleSchema, type AdminRoleFormValues } from "../schemas/admin-role.schema";
import { useAdminPermissions } from "../hooks/use-admin-permissions";
import { useCreateAdminRole, useUpdateAdminRole } from "../hooks/use-admin-roles";
import { isConcurrencyConflict } from "../utils/identity-error.util";

export function RoleForm({ role }: { role?: AdminRoleDto }) {
  const router = useRouter();
  const [conflictOpen, setConflictOpen] = useState(false);

  const permissions = useAdminPermissions({ page: 1, pageSize: 1000 });
  const create = useCreateAdminRole();
  const update = useUpdateAdminRole(role?.id ?? "");

  const form = useForm<AdminRoleFormValues>({
    resolver: zodResolver(adminRoleSchema) as never,
    defaultValues: {
      code: role?.code ?? "",
      name: role?.name ?? "",
      description: role?.description ?? "",
      permissionIds: role?.permissions?.map((permission) => permission.id) ?? [],
    },
  });

  async function submit(values: AdminRoleFormValues) {
    try {
      if (!role) {
        await create.mutateAsync(values);
        appToast.success("Tạo vai trò thành công.");
      } else {
        await update.mutateAsync({
          ...values,
          concurrencyToken: role.concurrencyToken,
        });
        appToast.success("Cập nhật vai trò thành công.");
      }
      router.push("/vai-tro");
    } catch (error) {
      const apiError = normalizeApiError(error);
      if (isConcurrencyConflict(apiError)) {
        setConflictOpen(true);
        return;
      }
      appToast.error(role ? "Cập nhật thất bại" : "Tạo mới thất bại", apiError.message);
    }
  }

  return (
    <>
      <form onSubmit={form.handleSubmit(submit)} className="space-y-5">
        <FormSection title="Thông tin vai trò">
          <FormRow columns={2}>
            <FormField label="Mã vai trò" required error={form.formState.errors.code?.message}>
              <Input {...form.register("code")} disabled={role?.isSystem} className="h-[42px]" />
            </FormField>

            <FormField label="Tên vai trò" required error={form.formState.errors.name?.message}>
              <Input {...form.register("name")} className="h-[42px]" />
            </FormField>
          </FormRow>

          <FormField label="Mô tả" error={form.formState.errors.description?.message}>
            <Textarea {...form.register("description")} />
          </FormField>
        </FormSection>

        <FormSection title="Quyền hạn" description="Chọn các quyền được gán cho vai trò.">
          <Controller
            name="permissionIds"
            control={form.control}
            render={({ field }) => (
              <PermissionMatrix
                permissions={permissions.data?.items ?? []}
                value={field.value}
                onChange={(ids) => field.onChange(ids)}
              />
            )}
          />
        </FormSection>

        <FormActions
          loading={create.isPending || update.isPending}
          submitText={role ? "Lưu vai trò" : "Tạo vai trò"}
          onCancel={() => router.push("/vai-tro")}
        />
      </form>

      <ConcurrencyConflictDialog
        open={conflictOpen}
        onCancel={() => setConflictOpen(false)}
        onReload={() => {
          // Normal behavior: reload page to get fresh data
          window.location.reload();
        }}
      />
    </>
  );
}
