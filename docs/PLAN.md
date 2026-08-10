# Kế hoạch Triển khai Admin Users API (Backend)

## 1. Vấn đề hiện tại
Giao diện **Quản lý người dùng** trên Frontend đang gọi API đến `http://localhost:7126/api/v1/admin/users?page=1&pageSize=10` nhưng nhận lại lỗi `404 Not Found`. Nguyên nhân là vì dự án Backend (HanYu-Web) chưa có bất kỳ API quản lý người dùng nào được xây dựng cho Admin.

## 2. Nhiệm vụ
Xây dựng cụm API Admin Users ở Backend để cung cấp dữ liệu cho Frontend, tuân thủ Clean Architecture và CQRS bằng thư viện MediatR.

## 3. Chi tiết triển khai

### Tầng Application (CQRS)
Tạo thư mục `HanYu\Application\Features\Identity\Admin\Users` với các file sau:
- **AdminUserDtos.cs**: Chứa `AdminUserListItemDto`, `AdminUserDetailDto`.
- **GetUsersQuery.cs**: Xử lý việc lấy danh sách phân trang (sử dụng UserManager hoặc DbContext trực tiếp để truy vấn danh sách IdentityUser).
- **GetUserByIdQuery.cs**: Lấy chi tiết user.
- **CreateUserCommand.cs**: Tính năng tạo user mới.
- **UpdateUserCommand.cs**: Sửa thông tin user.
- **LockUserCommand.cs / UnlockUserCommand.cs**: Khóa / Mở khóa tài khoản.

### Tầng API
Tạo Controller `AdminUsersController.cs` tại `HanYu\API\Controllers\Admin\Identity\AdminUsersController.cs`:
- Gắn `[Route("api/v1/admin/users")]`.
- Gắn `[Authorize(Roles = "Admin")]`.
- Tạo các endpoint `[HttpGet]`, `[HttpPost]`, v.v. trỏ tới MediatR ISender.

## 4. Xác minh (Verification)
- Đảm bảo dự án biên dịch thành công.
- Tải lại trang Quản lý người dùng, bảng dữ liệu phải load được các bản ghi từ DB (`thanhvu1411`, `admin`, `testuser`) và không báo lỗi.
