import { LoginForm } from "@/components/auth/login-form";
import { AuthShell } from "@/components/auth/auth-shell";

export default function LoginPage() {
  return (
    <AuthShell>
      <div className="w-full max-w-[480px]">
        <div>
          <h1 className="text-[25px] font-semibold tracking-[-0.4px] text-[#202124]">
            Chào mừng bạn <span className="text-[#ef241c]">trở lại</span>
          </h1>

          <p className="mt-2 text-[12px] leading-[19px] text-[#777]">
            Đăng nhập để tiếp tục quản trị hệ thống HanYu.
          </p>
        </div>

        <div className="mt-7">
          <LoginForm />
        </div>
      </div>
    </AuthShell>
  );
}
