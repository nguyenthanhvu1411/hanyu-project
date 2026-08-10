import {
  z,
} from "zod";

export const adminUserSchema =
  z.object({
    email:
      z
        .string()
        .trim()
        .min(
          1,
          "Vui lòng nhập email.",
        )
        .email(
          "Email không hợp lệ.",
        ),

    displayName:
      z
        .string()
        .trim()
        .min(
          2,
          "Tên hiển thị cần ít nhất 2 ký tự.",
        )
        .max(
          100,
          "Tên hiển thị tối đa 100 ký tự.",
        ),

    locale:
      z
        .string()
        .default(
          "vi",
        ),

    status:
      z.enum([
        "active",
        "locked",
        "disabled",
        "pending",
      ]),

    roleIds:
      z
        .array(
          z.string(),
        )
        .default([]),
  });

export const createAdminUserSchema =
  adminUserSchema.extend({
    password:
      z
        .string()
        .min(
          6,
          "Mật khẩu cần ít nhất 6 ký tự.",
        ),

    emailVerified:
      z.boolean(),
  });

export type AdminUserFormValues =
  z.infer<
    typeof adminUserSchema
  >;

export type CreateAdminUserFormValues =
  z.infer<
    typeof createAdminUserSchema
  >;
