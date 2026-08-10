"use client";

import { Save, ShieldCheck } from "lucide-react";
import { useEffect, useState } from "react";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { ErrorState } from "@/components/common/error-state";
import { PageLoading } from "@/components/common/page-loading";
import { ConcurrencyConflictDialog } from "@/components/common/concurrency-conflict-dialog";
import { appToast } from "@/components/ui/toast";
import { normalizeApiError } from "@/lib/api/api-error";
import { isConcurrencyConflict } from "../utils/identity-error.util";
import { useAdminRoles } from "../hooks/use-admin-roles";
import { useAdminUser, useReplaceUserRoles } from "../hooks/use-admin-users";

interface UserRoleManagerProps {
  userId: string;
}

export function UserRoleManager({ userId }: UserRoleManagerProps) {
  const userQuery = useAdminUser(userId);
  const rolesQuery = useAdminRoles({
    page: 1,
    pageSize: 100,
    sortBy: "name",
    sortDirection: "asc",
  });
  const mutation = useReplaceUserRoles(userId);

  const [selectedRoleIds, setSelectedRoleIds] = useState<string[]>([]);
  const [conflictOpen, setConflictOpen] = useState(false);

  useEffect(() => {
    if (!userQuery.data) {
      return;
    }
    setSelectedRoleIds(userQuery.data.roles?.map((role) => role.id) ?? []);
  }, [userQuery.data]);

  if (userQuery.isLoading || rolesQuery.isLoading) {
    return <PageLoading text="Đang tải vai trò..." />;
  }

  if (userQuery.isError || rolesQuery.isError || !userQuery.data) {
    return (
      <ErrorState
        onRetry={() => {
          void userQuery.refetch();
          void rolesQuery.refetch();
        }}
      />
    );
  }

  const user = userQuery.data;
  const roles = rolesQuery.data?.items ?? [];

  function toggleRole(roleId: string) {
    setSelectedRoleIds((current) =>
      current.includes(roleId)
        ? current.filter((id) => id !== roleId)
        : [...current, roleId]
    );
  }

  async function save() {
    try {
      await mutation.mutateAsync({
        roleIds: selectedRoleIds,
        concurrencyToken: user.concurrencyToken,
      });
      appToast.success("Cập nhật vai trò thành công.");
    } catch (error) {
      const apiError = normalizeApiError(error);
      
      if (apiError.status === 409 && apiError.code === "AUTH.LAST_SUPER_ADMIN") {
        appToast.error(
          "Không thể thay đổi vai trò",
          "Hệ thống phải còn ít nhất một SuperAdmin."
        );
        return;
      }

      if (isConcurrencyConflict(apiError)) {
        setConflictOpen(true);
        return;
      }
      appToast.error("Không thể cập nhật vai trò", apiError.message);
    }
  }

  const changed = !sameIds(
    selectedRoleIds,
    user.roles?.map((role) => role.id) ?? []
  );

  return (
    <>
      <div className="space-y-4">
        <Alert variant="info" title="Phân quyền theo vai trò">
          Thay đổi tại đây sẽ thay thế toàn bộ tập vai trò hiện tại của người dùng.
        </Alert>

        <div className="overflow-hidden rounded-[11px] border border-[#e8e3dc] bg-white">
          <div className="flex items-center justify-between gap-3 border-b border-[#eee9e2] px-4 py-3">
            <div>
              <div className="text-[13px] font-semibold text-[#333]">
                Vai trò hệ thống
              </div>
              <div className="mt-[2px] text-[10px] text-[#888]">
                Đã chọn {selectedRoleIds.length} vai trò
              </div>
            </div>

            <Button
              type="button"
              disabled={!changed}
              loading={mutation.isPending}
              onClick={save}
              className="h-[38px] gap-2 text-[11px]"
            >
              <Save size={14} />
              Lưu vai trò
            </Button>
          </div>

          <div className="grid gap-3 p-4 md:grid-cols-2 xl:grid-cols-3">
            {roles.map((role) => {
              const active = selectedRoleIds.includes(role.id);

              return (
                <div
                  key={role.id}
                  onClick={() => toggleRole(role.id)}
                  className={`flex cursor-pointer items-start gap-3 rounded-[9px] border p-3 text-left transition ${
                    active
                      ? "border-[#efc5c1] bg-[#fff7f5]"
                      : "border-[#e8e3dc] bg-white hover:bg-[#faf9f7]"
                  }`}
                >
                  <Checkbox
                    checked={active}
                    onCheckedChange={() => toggleRole(role.id)}
                  />

                  <div className="min-w-0 flex-1">
                    <div className="flex items-center gap-2">
                      <ShieldCheck
                        size={14}
                        className={active ? "text-[#ef241c]" : "text-[#888]"}
                      />
                      <span className="truncate text-[12px] font-semibold text-[#444]">
                        {role.name}
                      </span>
                    </div>

                    <div className="mt-1 text-[9px] text-[#999]">{role.code}</div>

                    {role.description && (
                      <div className="mt-2 line-clamp-2 text-[10px] leading-[15px] text-[#858585]">
                        {role.description}
                      </div>
                    )}

                    {role.isSystem && (
                      <span className="mt-2 inline-flex rounded-full bg-[#fff0ee] px-2 py-[3px] text-[9px] text-[#d8322b]">
                        Vai trò hệ thống
                      </span>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      </div>

      <ConcurrencyConflictDialog
        open={conflictOpen}
        onCancel={() => setConflictOpen(false)}
        onReload={async () => {
          await userQuery.refetch();
          setConflictOpen(false);
        }}
      />
    </>
  );
}

function sameIds(a: string[], b: string[]) {
  if (a.length !== b.length) return false;
  const sortedA = [...a].sort();
  const sortedB = [...b].sort();
  return sortedA.every((val, index) => val === sortedB[index]);
}
