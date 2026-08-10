# KIẾN TRÚC & TIÊU CHUẨN KỸ THUẬT CỐT LÕI (CORE ARCHITECTURE)

Tài liệu này định nghĩa toàn bộ các tiêu chuẩn kỹ thuật (Non-functional requirements & Infrastructure) bắt buộc phải tuân thủ khi phát triển backend HanYu, dựa trên yêu cầu Enterprise do người dùng chỉ định.

## 1. API & Nghiệp Vụ (API Standards)
- **Versioning:** `/api/v1/...`
- **Error Handling:** 100% sử dụng chuẩn `ProblemDetails` (RFC 7807) thông qua `GlobalExceptionHandler`.
- **Idempotency:** Bắt buộc có header `Idempotency-Key` cho các API `POST/PUT/PATCH` thay đổi trạng thái nhạy cảm (như hoàn thành bài học, thanh toán).
- **Concurrency:** Sử dụng `ConcurrencyToken` (Optimistic Concurrency) khi cập nhật dữ liệu.
- **Thống nhất định danh:** Nội bộ (Admin) dùng Primary Key `bigint` tự tăng. Công khai (Public) dùng `UUID` (Guid).

## 2. Database & Storage
- **Database:** Transaction bắt buộc cho các thao tác nhiều bước (ví dụ Outbox Pattern). Sử dụng Soft Delete (`IsDeleted`) và Audit Fields (`CreatedAt`, `UpdatedAt`, `CreatedBy`).
- **Storage:** Phân tách rõ Public Bucket (ảnh, avatar) và Private Bucket (bản ghi âm, dữ liệu cá nhân - dùng Signed URL). Cần giới hạn dung lượng, kiểm tra MIME/Magic number chống file độc hại.

## 3. Authentication & Phân Quyền (Security)
- **JWT:** Áp dụng Access Token (ngắn hạn) và Refresh Token (dài hạn, có cơ chế Rotation).
- **Phân quyền (RBAC/ABAC):** Quyền được chia nhỏ (ví dụ: `content.create`, `content.publish`) thay vì chỉ check Role. Application layer là chốt chặn cuối cùng. Kiểm soát nghiêm ngặt 5 lần đăng nhập sai/phút -> Khóa tài khoản.

## 4. Bảo mật (Security & Rate Limiting)
- **Headers & CORS:** Bật HSTS, CSP, X-Frame-Options. Whitelist CORS chặt chẽ.
- **Rate Limit:** 
  - Login: 5 req/phút/IP.
  - Search: 60 req/phút/User.
  - Admin Write: 100 req/phút.
- Tránh lộ thông tin: Secret vault/Environment variables. Không log Token/Password.

## 5. Cache, Tracing & Background Jobs
- **Cache (Redis):** Cache danh sách khóa học public, từ điển. Cần có cơ chế Invalidation khi Admin publish nội dung.
- **Background Jobs:** Hỗ trợ Retry/Backoff/Dead-letter queue (Ví dụ: Chuyển đổi audio, tính điểm streak) thông qua Hangfire hoặc Quartz.NET.
- **Tracing & Logging:** Mọi Request phải sinh ra `Correlation ID` truyền xuyên suốt từ Frontend -> API -> DB -> Background. Ghi log Request Rate, Error Rate, Response Time.

---

# LỘ TRÌNH TRIỂN KHAI (ROADMAP ĐỂ ĐÁP ỨNG TIÊU CHUẨN NÀY)

Thay vì code toàn bộ cùng lúc (rất dễ vỡ hệ thống), chúng ta sẽ chèn các kiến trúc này vào dự án theo từng Phase (Epic):

### 🎯 Epic 1: Gia cố móng (Foundation Security & API) - *Nên làm ngay tiếp theo*
1. Setup **Global Exception Handler** trả về đúng cấu trúc ProblemDetails.
2. Setup Middleware kiểm tra **Idempotency-Key** dựa vào Redis/MemoryCache.
3. Setup **Rate Limiting** policy (Login, Search, Default).
4. Chuẩn hóa Audit Fields (`CreatedAt`, `UpdatedAt`, `IsDeleted`) vào base entity (EF Core Interceptors).

### 🎯 Epic 2: Authentication Nâng cao
1. Thêm Refresh-Token rotation vào AuthController.
2. Thêm khóa tài khoản khi sai 5 lần (Lockout).

### 🎯 Epic 3: Storage & Background Jobs
1. Cấu hình dịch vụ Cloud Storage (hoặc MinIO local) hỗ trợ Signed URLs.
2. Setup Hangfire/BackgroundService để xử lý Outbox Events (như cộng điểm bài học).

### 🎯 Epic 4: Observability (Logs/Metrics)
1. Cài đặt Serilog + OpenTelemetry (Sinh CorrelationId).
