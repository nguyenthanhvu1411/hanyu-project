import { z } from "zod";

export const hskLevelSchema = z.object({
  code: z
    .string()
    .trim()
    .min(1, "Vui lòng nhập mã HSK.")
    .max(20, "Mã HSK tối đa 20 ký tự."),

  nameVi: z
    .string()
    .trim()
    .min(1, "Vui lòng nhập tên cấp độ.")
    .max(100, "Tên cấp độ tối đa 100 ký tự."),

  sortOrder: z
    .number()
    .int("Thứ tự phải là số nguyên.")
    .min(0, "Thứ tự không được âm."),
});

export type HskLevelFormValues = z.infer<typeof hskLevelSchema>;
