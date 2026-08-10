import {
  z,
} from "zod";

export const adminPermissionSchema =
  z.object({
    code:
      z
        .string()
        .trim()
        .min(
          3,
          "Vui lòng nhập mã quyền.",
        )
        .max(
          100,
          "Mã quyền tối đa 100 ký tự.",
        ),

    resource:
      z
        .string()
        .trim()
        .min(
          1,
          "Vui lòng nhập resource.",
        )
        .max(
          50,
        ),

    action:
      z
        .string()
        .trim()
        .min(
          1,
          "Vui lòng nhập action.",
        )
        .max(
          50,
        ),

    description:
      z
        .string()
        .max(
          1000,
        )
        .optional(),
  });

export type AdminPermissionFormValues =
  z.infer<
    typeof adminPermissionSchema
  >;
