import {
  z,
} from "zod";

export const adminRoleSchema =
  z.object({
    code:
      z
        .string()
        .trim()
        .min(
          2,
          "Vui lòng nhập mã vai trò.",
        )
        .max(
          50,
          "Mã tối đa 50 ký tự.",
        )
        .regex(
          /^[A-Z0-9_.-]+$/,
          "Mã vai trò chỉ dùng chữ in hoa, số, _, . hoặc -.",
        ),

    name:
      z
        .string()
        .trim()
        .min(
          2,
          "Vui lòng nhập tên vai trò.",
        )
        .max(
          100,
          "Tên tối đa 100 ký tự.",
        ),

    description:
      z
        .string()
        .max(
          1000,
          "Mô tả quá dài.",
        )
        .optional(),

    permissionIds:
      z
        .array(
          z.string(),
        )
        .default([]),
  });

export type AdminRoleFormValues =
  z.infer<
    typeof adminRoleSchema
  >;
