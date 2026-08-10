"use client";

import {
  zodResolver,
} from "@hookform/resolvers/zod";

import {
  Save,
  UserRound,
} from "lucide-react";

import {
  Controller,
  useForm,
} from "react-hook-form";
import { useState } from "react";

import {
  useRouter,
} from "next/navigation";

import {
  FormField,
} from "@/components/forms/form-field";

import {
  FormRow,
} from "@/components/forms/form-row";

import {
  FormSection,
} from "@/components/forms/form-section";

import {
  FormActions,
} from "@/components/forms/form-actions";

import {
  Input,
} from "@/components/ui/input";

import {
  Select,
} from "@/components/ui/select";

import {
  MultiSelect,
} from "@/components/common/multi-select";

import {
  Switch,
} from "@/components/ui/switch";

import {
  appToast,
} from "@/components/ui/toast";

import {
  ConcurrencyConflictDialog,
} from "@/components/common/concurrency-conflict-dialog";

import {
  normalizeApiError,
} from "@/lib/api/api-error";

import {
  isConcurrencyConflict,
} from "../utils/identity-error.util";

import type {
  AdminUserDetailDto,
} from "@/dto/identity/admin-user.dto";

import {
  createAdminUserSchema,
  adminUserSchema,
  type CreateAdminUserFormValues,
} from "../schemas/admin-user.schema";

import {
  USER_STATUS_OPTIONS,
} from "../identity.constants";

import {
  useAdminRoles,
} from "../hooks/use-admin-roles";

import {
  useCreateAdminUser,
  useUpdateAdminUser,
} from "../hooks/use-admin-users";

interface UserFormProps {
  user?: AdminUserDetailDto;
}

export function UserForm({
  user,
}: UserFormProps) {
  const router =
    useRouter();

  const creating =
    !user;

  const rolesQuery =
    useAdminRoles({
      page: 1,
      pageSize: 100,
    });

  const createMutation =
    useCreateAdminUser();

  const updateMutation =
    useUpdateAdminUser(
      user?.id ?? "",
    );

  const [conflictOpen, setConflictOpen] = useState(false);

  const form =
    useForm<CreateAdminUserFormValues>({
      resolver:
        zodResolver(
          creating
            ? createAdminUserSchema
            : adminUserSchema,
        ) as never,

      values: {
        email:
          user?.email ??
          "",

        displayName:
          user?.displayName ??
          "",

        password:
          "",

        locale:
          user?.locale ??
          "vi",

        status:
          user?.status ??
          "active",

        roleIds:
          user?.roles?.map(
            (role) =>
              role.id,
          ) ?? [],

        emailVerified:
          Boolean(
            user?.emailVerifiedAt,
          ),
      },
    });

  async function submit(
    values:
      CreateAdminUserFormValues,
  ) {
    try {
      if (creating) {
        await createMutation.mutateAsync({
          email:
            values.email,

          displayName:
            values.displayName,

          password:
            values.password,

          locale:
            values.locale,

          status:
            values.status,

          roleIds:
            values.roleIds,

          emailVerified:
            values.emailVerified,
        });

        appToast.success(
          "Tạo người dùng thành công.",
        );
      } else {
        await updateMutation.mutateAsync({
          email:
            values.email,

          displayName:
            values.displayName,

          locale:
            values.locale,

          status:
            values.status,

          concurrencyToken:
            user.concurrencyToken,
        });

        appToast.success(
          "Cập nhật người dùng thành công.",
        );
      }

      router.push(
        "/nguoi-dung",
      );
    } catch (error) {
      const apiError =
        normalizeApiError(
          error,
        );

      if (
        isConcurrencyConflict(
          apiError,
        )
      ) {
        setConflictOpen(
          true,
        );

        return;
      }

      appToast.error(
        creating
          ? "Không thể tạo người dùng"
          : "Không thể cập nhật người dùng",
        apiError.message,
      );
    }
  }

  const loading =
    createMutation.isPending ||
    updateMutation.isPending;

  return (
    <form
      onSubmit={
        form.handleSubmit(
          submit,
        )
      }
      className="space-y-5"
    >
      <FormSection
        title="Thông tin tài khoản"
        description="Thông tin nhận diện và trạng thái người dùng."
        icon={
          <UserRound
            size={18}
          />
        }
      >
        <FormRow columns={2}>
          <FormField
            label="Email"
            required
            error={
              form.formState
                .errors.email
                ?.message
            }
          >
            <Input
              {...form.register(
                "email",
              )}
              error={
                Boolean(
                  form
                    .formState
                    .errors
                    .email,
                )
              }
              className="h-[42px]"
            />
          </FormField>

          <FormField
            label="Tên hiển thị"
            required
            error={
              form.formState
                .errors
                .displayName
                ?.message
            }
          >
            <Input
              {...form.register(
                "displayName",
              )}
              className="h-[42px]"
            />
          </FormField>
        </FormRow>

        {creating && (
          <FormRow columns={2}>
            <FormField
              label="Mật khẩu"
              required
              error={
                form
                  .formState
                  .errors
                  .password
                  ?.message
              }
            >
              <Input
                type="password"
                {...form.register(
                  "password",
                )}
                className="h-[42px]"
              />
            </FormField>

            <FormField
              label="Xác minh email"
            >
              <Controller
                name="emailVerified"
                control={
                  form.control
                }
                render={({
                  field,
                }) => (
                  <Switch
                    checked={
                      field.value
                    }
                    onCheckedChange={
                      field.onChange
                    }
                    label="Đánh dấu email đã xác minh"
                  />
                )}
              />
            </FormField>
          </FormRow>
        )}

        <FormRow columns={2}>
          <FormField
            label="Ngôn ngữ"
          >
            <Controller
              name="locale"
              control={
                form.control
              }
              render={({
                field,
              }) => (
                <Select
                  value={
                    field.value
                  }
                  onValueChange={
                    field.onChange
                  }
                  options={[
                    {
                      label:
                        "Tiếng Việt",
                      value:
                        "vi",
                    },
                    {
                      label:
                        "English",
                      value:
                        "en",
                    },
                  ]}
                />
              )}
            />
          </FormField>

          <FormField
            label="Trạng thái"
            required
          >
            <Controller
              name="status"
              control={
                form.control
              }
              render={({
                field,
              }) => (
                <Select
                  value={
                    field.value
                  }
                  onValueChange={
                    field.onChange
                  }
                  options={
                    USER_STATUS_OPTIONS.map(
                      (
                        option,
                      ) => ({
                        ...option,
                      }),
                    )
                  }
                />
              )}
            />
          </FormField>
        </FormRow>
      </FormSection>

      {creating && (
        <FormSection
          title="Vai trò"
          description="Gán vai trò ban đầu cho người dùng."
        >
          <Controller
            name="roleIds"
            control={
              form.control
            }
            render={({
              field,
            }) => (
              <MultiSelect
                value={
                  field.value
                }
                onValueChange={(
                  values,
                ) =>
                  field.onChange(
                    values,
                  )
                }
                options={
                  rolesQuery.data
                    ?.items.map(
                      (
                        role,
                      ) => ({
                        value:
                          role.id,

                        label:
                          role.name,

                        description:
                          role.code,
                      }),
                    ) ??
                  []
                }
              />
            )}
          />
        </FormSection>
      )}

      <FormActions
        loading={
          loading
        }
        submitText={
          creating
            ? "Tạo người dùng"
            : "Lưu thay đổi"
        }
        onCancel={() =>
          router.push(
            "/nguoi-dung",
          )
        }
      />
      <ConcurrencyConflictDialog
        open={
          conflictOpen
        }
        onCancel={() =>
          setConflictOpen(
            false,
          )
        }
        onReload={() => {
          window.location.reload();
        }}
      />
    </form>
  );
}
