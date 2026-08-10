# Plan cho Auth API Nâng Cao (Quên mật khẩu, Quản lý Session, Đổi mật khẩu)

## Mục tiêu
Hoàn thiện toàn bộ luồng Authentication và Account Management cho người dùng, bao gồm các tính năng:
- Quên mật khẩu & Đặt lại mật khẩu (có token giới hạn thời gian).
- Đổi mật khẩu (yêu cầu mật khẩu hiện tại).
- Quản lý phiên đăng nhập (Sessions): Liệt kê session, thu hồi session cụ thể, thu hồi tất cả.
- Lấy thông tin User (Get Me).
- Fix lại phân quyền `[AllowAnonymous]` và `[Authorize]` trên Controller.

## Các hạng mục cần thực hiện

### 1. Domain Layer
- **Tạo Entity mới**: `PasswordResetToken.cs` (Chứa token hash, thời hạn sử dụng, cờ đánh dấu đã sử dụng).
- **Cập nhật Interface Repositories**: 
  - Tạo `IPasswordResetTokenRepository.cs`.
  - Bổ sung methods `GetByPublicIdAsync` và `GetActiveByUserIdAsync` vào `IRefreshTokenRepository.cs`.

### 2. Infrastructure Layer
- **Database Context**: Thêm `DbSet<PasswordResetToken>` vào `AppDbContext.cs`.
- **Implement Repositories**: 
  - Tạo `PasswordResetTokenRepository.cs`.
  - Cập nhật `RefreshTokenRepository.cs` với các methods mới.
- **Dịch vụ Ngoài (External Services)**:
  - Tạo `ConsoleEmailSender.cs` (hiện thực `IEmailSender` in log ra console phục vụ test).

### 3. Application Layer
- **DTOs**: Bổ sung các record `ForgotPasswordRequest`, `ResetPasswordRequest`, `ChangePasswordRequest`, `UserMeResponse`, `SessionResponse` vào `AuthDtos.cs`.
- **Validators**: Viết validator cho các DTO mới trong `AuthValidators.cs` (Yêu cầu mật khẩu mạnh, email hợp lệ).
- **Services**: 
  - Khai báo `IEmailSender.cs`.
  - Cập nhật `IAuthService.cs` thêm các hàm tương ứng.
  - Hiện thực logic trong `AuthService.cs` (Hash token, kiểm tra hạn, chống dò email, đổi mật khẩu).

### 4. API / Controller Layer (Web)
- **AuthController**:
  - Gắn `[Authorize]` ở cấp class, dùng `[AllowAnonymous]` cho các endpoint công khai (Login, Register, Forgot, Reset).
  - Thêm các endpoint: `forgot-password`, `reset-password`, `change-password`, `sessions`, `sessions/{id}/revoke`, `logout-all`.
- **UsersController**:
  - Tạo mới với endpoint `GET /me` trả về thông tin user hiện tại (trích xuất `PublicId` từ JWT Claim).
- **Dependency Injection**: 
  - Đăng ký `IPasswordResetTokenRepository` và `IEmailSender` trong `Program.cs`.

### 5. Migration & Verification
- Tạo migration mới `AddPasswordResetToken`.
- Chạy `dotnet ef database update`.
- Xác minh bằng các script: `security_scan.py` và `lint_runner.py` (nếu có).

---
*Note: Logic về Swagger cấu hình extension đã được thực hiện trước đó theo yêu cầu.*
