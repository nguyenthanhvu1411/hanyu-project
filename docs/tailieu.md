# TÀI LIỆU THIẾT KẾ HỆ THỐNG API - HANYU (CHINESE LEARNING)
*(Bản đầy đủ 21 phân hệ nghiệp vụ - Trích xuất từ api.md & Database Schema)*

Tài liệu này định nghĩa cấu trúc API cho hệ thống HanYu dựa trên 2 phân hệ chính: Quản trị (Admin) và Người dùng (Public).

---

## 1. TIÊU CHUẨN THIẾT KẾ CHUNG (API GUIDELINES)

### 1.1. Quy ước Khóa Định Danh (MANDATORY)
- **Admin Base Path (`/api/v1/admin/*`)**: Dành riêng cho phân hệ Quản trị.
  - **Bắt buộc dùng `{id}` (BIGINT)** trên toàn bộ API Path và Payload để tối ưu hiệu năng Query/JOIN trực tiếp cho Backend. Không dùng UUID trong Admin API.
- **Public Base Path (`/api/v1/public/*`)**: Dành cho Học viên, Guest và Frontend Clients.
  - **Bắt buộc dùng `{uuid}` (UUID)** trên mọi API Path và Response để ẩn số lượng bản ghi và chặn triệt để rủi ro IDOR / Enumeration attack. Tuyệt đối không để lộ `{id}` (BIGINT) ra ngoài.

### 1.2. Chính Sách Cưỡng Bức & Bảo Mật (Enforced Policies)
- **Rate Limiting:** Public API (100 req/min), Auth/Sensitive (5 req/min), Admin API (300 req/min).
- **Idempotency:** Bắt buộc Header `Idempotency-Key` cho thanh toán, nộp bài kiểm tra, tạo đơn hàng, đồng bộ tiến độ offline.
- **Response Format:** Bọc trong cấu trúc chuẩn ProblemDetails khi xảy ra lỗi. API danh sách luôn phân trang.
- **Outbox Pattern:** Các API tạo sự kiện quan trọng (Thanh toán, Hoàn thành bài học) phải lưu song song vào bảng nghiệp vụ và bảng `outbox_events` chung 1 Transaction để đảm bảo tính nhất quán (P0).

---

## 2. NHÓM API QUẢN TRỊ (ADMIN) - `/api/v1/admin/*`
*Sử dụng tham số định danh `{id}` (BIGINT)*

### 2.1. Phân hệ Xác thực & Quản lý User (Identity - Module 1)
- `POST /api/v1/admin/auth/login`: Xác thực nhân viên nội bộ.
- `POST /api/v1/admin/auth/logout`: Đăng xuất và thu hồi Access Token.
- `GET /api/v1/admin/users`: Tra cứu danh sách người dùng.
- `GET /api/v1/admin/users/{id}`: Xem chi tiết người dùng.
- `PATCH /api/v1/admin/users/{id}/status`: Khóa hoặc mở khóa tài khoản (Ghi audit logs).
- `POST /api/v1/admin/users/{id}/roles`: Phân quyền (Yêu cầu four-eyes principle đối với quyền cấp cao).
- `POST /api/v1/admin/users/{id}/force-logout`: Buộc thu hồi mọi sessions đang active.

### 2.2. Phân hệ Từ điển (Dictionary - Module 3)
- `GET /api/v1/admin/words`: Quản lý danh sách từ vựng.
- `POST /api/v1/admin/words`: Tạo từ vựng mới (Validate bắt buộc Hán tự Giản thể, Pinyin, và Nghĩa).
- `PUT /api/v1/admin/words/{id}`: Sửa từ vựng.
- `POST /api/v1/admin/words/import`: Tải lên file Excel để Import từ vựng hàng loạt qua Worker bất đồng bộ.
- `GET /api/v1/admin/imports/{id}`: Kiểm tra tiến độ job Import.

### 2.3. Phân hệ Khóa học (Courses - Module 2)
- `GET /api/v1/admin/courses`: Quản lý danh sách khóa học.
- `POST /api/v1/admin/courses`: Tạo khóa học mới (Trạng thái mặc định Draft).
- `PUT /api/v1/admin/courses/{id}`: Cập nhật thông tin (Bắt buộc Concurrency Token chống ghi đè).
- `POST /api/v1/admin/courses/{id}/submit-review`: Gửi duyệt khóa học.
- `POST /api/v1/admin/courses/{id}/approve`: Duyệt nội dung khóa học.
- `POST /api/v1/admin/courses/{id}/publish`: Xuất bản (Snapshot nội dung lưu qua `content_revisions`).
- `POST /api/v1/admin/courses/{id}/rollback/{revisionId}`: Khôi phục khóa học về version cũ.

### 2.4. Phân hệ Bài tập & Kiểm tra (Assessment - Module 4 & 5)
- `GET /api/v1/admin/exercises`: Quản lý bài tập.
- `POST /api/v1/admin/exercises/{id}/questions`: Thêm câu hỏi thi. Nếu là đề thi thật bắt buộc Dual-review.

### 2.5. Phân hệ Cấu Hình & Báo Cáo (Modules 9, 15, 16, 20)
- `GET /api/v1/admin/audit-logs`: Truy vết nhật ký thay đổi an ninh/nghiệp vụ. (Immutable).
- `GET /api/v1/admin/feature-flags`: Quản lý Feature Flags của hệ thống.
- `PATCH /api/v1/admin/feature-flags/{key}`: Bật/Tắt tính năng.
- `GET /api/v1/admin/settings`: Quản lý app settings, tham số Runtime.
- `GET /api/v1/admin/reports/cost`: Báo cáo chi phí (AI, Storage, SMS) theo thời gian.
- `POST /api/v1/admin/api-keys`: Khởi tạo API Key B2B (Mã thật hiện 1 lần duy nhất, lưu DB dạng hash).
- `POST /api/v1/admin/api-keys/{id}/revoke`: Hủy API Key B2B.

### 2.6. Phân hệ Thanh Toán & Support (Modules 10, 11, 14, 17)
- `POST /api/v1/admin/refunds`: Xét duyệt hoàn tiền (Bắt buộc four-eyes approval với số tiền lớn).
- `GET /api/v1/admin/support/tickets`: Quản lý yêu cầu hỗ trợ (Tìm theo trace_id).
- `PATCH /api/v1/admin/support/tickets/{id}/assign`: Điều phối ticket cho nhân viên CS.
- `POST /api/v1/admin/certificates/{id}/revoke`: Thu hồi chứng chỉ đã cấp.

---

## 3. NHÓM API NGƯỜI DÙNG (PUBLIC) - `/api/v1/public/*`
*Sử dụng tham số định danh `{uuid}` (UUID)*

### 3.1. Xác thực & Thiết bị (Identity - Modules 1 & 8)
- `POST /api/v1/public/auth/register`: Đăng ký tài khoản (Tạo pending user, gửi Email).
- `POST /api/v1/public/auth/login`: Đăng nhập (Giới hạn brute force, ghi vào `login_attempts`).
- `POST /api/v1/public/auth/refresh`: Xoay vòng Refresh Token. Phát hiện Reuse sẽ block family.
- `POST /api/v1/public/auth/logout`: Thu hồi session của thiết bị hiện tại.
- `POST /api/v1/public/auth/reset-password`: Đặt lại mật khẩu qua OTP.
- `GET /api/v1/public/users/me`: Hồ sơ người dùng.
- `POST /api/v1/public/devices/register`: Đăng ký Push Notification Token cho iOS/Android.

### 3.2. Nội Dung Khóa Học & Bài Học (Discovery - Module 2)
- `GET /api/v1/public/courses`: Khám phá khóa học (Cache CDN, chỉ trả về khóa học đã Published).
- `GET /api/v1/public/courses/{uuid}`: Chi tiết một khóa học.
- `POST /api/v1/public/courses/{uuid}/bookmark`: Yêu thích khóa học.
- `GET /api/v1/public/lessons/{uuid}`: Vào bài học (Kiểm tra khóa chặn Prerequisite).

### 3.3. Từ Điển & Tìm Kiếm (Dictionary & Search - Modules 3 & 10)
- `GET /api/v1/public/dictionary/search?q={query}`: Tra cứu từ điển đa năng (Pinyin, Hán Tự, Âm Hán Việt).
- `GET /api/v1/public/dictionary/words/{uuid}`: Chi tiết từ, chữ Hán, ngữ pháp, audio.
- `GET /api/v1/public/search?q={query}`: Global Search trên toàn app (Khóa học, Bài học, Từ vựng).

### 3.4. Tiến Độ, SRS & Đồng Bộ Offline (Modules 5 & 6)
- `POST /api/v1/public/lessons/{uuid}/complete`: Hoàn thành học phần (Bắt buộc kèm Idempotency-Key và timeSpent).
- `GET /api/v1/public/srs/due`: Lấy danh sách Flashcard cần ôn.
- `POST /api/v1/public/srs/cards/{uuid}/review`: Đẩy kết quả ôn tập SRS để FSRS tính lại interval tiếp theo.
- `GET /api/v1/public/progress/me`: Truy xuất thống kê 9 kỹ năng (Listening, Speaking, Reading...).
- `POST /api/v1/public/sync/batch`: API đồng bộ hóa dữ liệu từ Local App lên Server khi có mạng lại.

### 3.5. Kiểm Tra, Đánh Giá & AI Tutor (Modules 4 & 12, 13)
- `POST /api/v1/public/exercises/{uuid}/attempts`: Bắt đầu làm bài thi (Tạo Snapshot cứng).
- `POST /api/v1/public/attempts/{uuid}/submit`: Nộp bài thi.
- `POST /api/v1/public/ai/conversations/{uuid}/messages`: Chat với AI Tutor (Có Audit Model Safety/Moderation).
- `POST /api/v1/public/pronunciation/grade`: Upload file `.wav`/`.mp3` đa thành phần để AI chấm điểm phát âm. (Cơ chế Timeout mềm).

### 3.6. Lớp Học Ảo (Classroom B2B - Module 12)
- `POST /api/v1/public/classes/join`: Tham gia lớp ảo theo join_code 8 ký tự.
- `POST /api/v1/public/assignments/{uuid}/submissions`: Học viên làm và nộp bài được giáo viên giao.

### 3.7. Cộng Đồng & Hỗ Trợ Pháp Lý (Modules 17, 18, 19, 21)
- `GET /api/v1/public/leaderboards`: Xem hạng Gamification toàn cầu/lớp học.
- `POST /api/v1/public/comments`: Đăng bình luận (Gửi qua Filter AI Spam Moderation).
- `POST /api/v1/public/support/tickets`: Mở Support Ticket.
- `GET /api/v1/public/legal/{type}`: Lấy điều khoản và Privacy Policy mới nhất.

### 3.8. Thanh Toán (Payment - Module 11)
- `POST /api/v1/public/orders`: Khởi tạo đơn hàng mua Subscription. (Idempotency-Key bắt buộc).
- `POST /api/v1/public/webhooks/{provider}`: (Chỉ máy chủ gọi) Nhận Callback thanh toán. Trích xuất signature. Log vào `payment_webhooks_log`.
- `GET /api/v1/public/invoices/me`: Tra cứu hóa đơn của cá nhân.

---

## 4. CHI TIẾT NGHIỆP VỤ CƯỠNG BỨC (SECURITY & AUDIT)

1. **Transaction Outbox (P0):** Bất kỳ API nào sinh ra Event quan trọng (Hoàn thành bài, Thanh toán) đều PHẢI ghi dữ liệu và tạo Event vào bảng `outbox_events` trong *cùng một Transaction* DB. Điều này chống lỗi gửi queue thất bại.
2. **Data Immutable:** Bảng `attempts` (đề nộp), `payment_transactions`, `audit_logs` chỉ hỗ trợ INSERT. API sửa xóa trên các resource này bị chốt cứng `403 Forbidden` tại Application Layer.
3. **Field-level Encryption:** Log Webhook thanh toán, Hội thoại AI, Consent... nếu có chứa nhạy cảm cá nhân sẽ bắt buộc được băm hoặc mã hóa (Encryption at rest).
4. **Idempotency Flow:** Khi gọi POST `/api/v1/public/lessons/{uuid}/complete` kèm Header `Idempotency-Key: xxx`, Middleware tự động chặn nếu Client bấm nút 2 lần, trả về kết quả Cached của lần 1.
