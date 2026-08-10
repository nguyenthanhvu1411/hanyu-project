import { z } from "zod";

export const loginSchema = z.object({
  email: z
    .string()
    .trim()
    .min(1, "Vui lòng nhập email.")
    .email("Vui lòng nhập email hợp lệ."),

  password: z
    .string()
    .min(
      6,
      "Mật khẩu cần có ít nhất 6 ký tự.",
    ),

  rememberMe: z.boolean(),
});

export type LoginFormValues =
  z.infer<typeof loginSchema>;
