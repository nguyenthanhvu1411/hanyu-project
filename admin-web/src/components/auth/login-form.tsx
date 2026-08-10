"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { ArrowRight } from "lucide-react";
import { useSearchParams } from "next/navigation";
import { useForm } from "react-hook-form";
import { z } from "zod";

import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { FormField } from "@/components/forms/form-field";
import { appToast } from "@/components/ui/toast";
import { useAuth } from "@/features/identity/auth/hooks/use-auth";
import { normalizeApiError } from "@/lib/api/api-error";
import { applyApiFormErrors } from "@/lib/api/form-error";
import { AUTH_ERROR_CODES } from "@/constants/auth.constants";

const loginSchema = z.object({
  email: z.string().trim().min(1, "Vui lòng nhập email.").email("Email không hợp lệ."),
  password: z.string().min(1, "Vui lòng nhập mật khẩu."),
  rememberMe: z.boolean(),
});

type LoginFormValues = z.infer<typeof loginSchema>;

export function LoginForm() {
  const { login } = useAuth();
  const searchParams = useSearchParams();

  const form = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: "",
      password: "",
      rememberMe: true,
    },
  });

  async function submit(values: LoginFormValues) {
    try {
      const next = searchParams.get("next");
      await login(values, isSafeRedirect(next) ? next! : "/tong-quan");
    } catch (error) {
      const apiError = normalizeApiError(error) as any;

      if (apiError.code === AUTH_ERROR_CODES.INVALID_CREDENTIALS) {
        form.setError("password", {
          message: "Email hoặc mật khẩu không chính xác.",
        });
        return;
      }

      if (apiError.status === 423 || apiError.code === AUTH_ERROR_CODES.ACCOUNT_LOCKED) {
        appToast.error("Tài khoản đang bị khóa.", apiError.message);
        return;
      }
      
      const applied = applyApiFormErrors(apiError, form.setError);

      if (!applied) {
        appToast.error("Đăng nhập thất bại", apiError.message);
      }
    }
  }

  return (
    <form onSubmit={form.handleSubmit(submit)} className="space-y-4">
      <FormField
        label="Email"
        required
        error={form.formState.errors.email?.message}
      >
        <Input
          type="email"
          autoComplete="email"
          placeholder="Nhập địa chỉ email"
          {...form.register("email")}
        />
      </FormField>

      <FormField
        label="Mật khẩu"
        required
        error={form.formState.errors.password?.message}
      >
        <Input
          type="password"
          autoComplete="current-password"
          placeholder="Nhập mật khẩu"
          {...form.register("password")}
        />
      </FormField>

      <div className="flex items-center justify-between gap-3">
        <label className="flex cursor-pointer items-center gap-2 text-[11px] text-[#555]">
          <Checkbox
            checked={form.watch("rememberMe")}
            onCheckedChange={(checked) =>
              form.setValue("rememberMe", checked === true)
            }
          />
          Ghi nhớ đăng nhập
        </label>

        <a
          href="/quen-mat-khau"
          className="text-[11px] font-medium text-[#16975b] hover:underline"
        >
          Quên mật khẩu?
        </a>
      </div>

      <Button
        type="submit"
        loading={form.formState.isSubmitting}
        className="h-[44px] w-full gap-2 text-[13px]"
      >
        Đăng nhập
        <ArrowRight size={16} />
      </Button>
    </form>
  );
}

function isSafeRedirect(value: string | null) {
  if (!value) {
    return false;
  }

  return value.startsWith("/") && !value.startsWith("//");
}
