import { z } from "zod";

export const hskLevelSchema = z.object({
  id: z
    .number()
    .int("Cấp HSK phải là số nguyên.")
    .min(1, "Cấp HSK phải từ 1 trở lên.")
    .max(9, "Cấp HSK tối đa là 9."),

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

  isActive: z.boolean(),
});

export type HskLevelFormValues = z.infer<typeof hskLevelSchema>;
