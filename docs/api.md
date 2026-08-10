WEBSITE HỌC TIẾNG TRUNG
TỔNG HỢP THIẾT KẾ DATABASE & API
Bản hợp nhất từ tài liệu 13, 14 và 15 (FINAL)
Phần 1 — API Catalog: mô tả rõ chức năng từng endpoint
Phần 2 — Database Schema: bảng, cột, kiểu dữ liệu, khóa id (nội bộ) & uuid (public)
Production Readiness Documentation Set
 
MỤC LỤC
MỤC LỤC	1
PHẦN 0 — QUY ƯỚC CHUNG	1
0.1 Quy tắc khóa chính: id (nội bộ) & uuid (public)	1
0.2 Cột hệ thống chuẩn — áp dụng cho MỌI bảng nghiệp vụ	1
0.3 Đánh dấu mã hóa cấp cột (Field-level Encryption)	1
0.4 Partitioning — chốt cụ thể	1
0.5 Chuẩn API dùng chung	1
PHẦN 1 — API CATALOG	1
1. AUTH / IDENTITY — Xác thực & Tài khoản	1
2. COURSE / LESSON — Khóa học & Bài học	1
3. DICTIONARY — Từ điển	1
4. ASSESSMENT — Bài tập & Chấm điểm	1
5. PROGRESS & SRS — Tiến độ & Ôn tập	1
6. MEDIA — Tải lên & Truy xuất tài nguyên	1
7. NOTIFICATION — Thông báo & Email	1
8. GOVERNANCE / AUDIT — Nhật ký & Duyệt nội dung	1
9. SYSTEM — Hệ thống & Cấu hình công khai	1
10. SEARCH — Tìm kiếm toàn hệ thống	1
11. PAYMENT & SUBSCRIPTION — Thanh toán & Gói học	1
12. CLASSROOM — Lớp học	1
13. AI TUTOR — Trợ lý AI & Chấm phát âm	1
14. CERTIFICATE & PLACEMENT — Chứng chỉ & Xếp lớp	1
15. ADMIN — Feature Flags & Settings	1
16. ADMIN — Reports & Cost	1
17. SUPPORT — Hỗ trợ người dùng	1
18. COMMENT & REVIEW — Bình luận & Đánh giá	1
19. GAMIFICATION — Huy hiệu & Bảng xếp hạng	1
20. ADMIN — API Key B2B	1
21. LEGAL DOCUMENTS & HELP CENTER	1
PHẦN 2 — DATABASE SCHEMA	1
1. IDENTITY — Người dùng, Vai trò, Phiên đăng nhập	1
Bảng: users  —  [chuẩn]	1
Bảng: roles  —  [chuẩn]	1
Bảng: permissions  —  [chuẩn]	1
Bảng: role_permissions  —  [bảng nối]	1
Bảng: user_roles  —  [chuẩn]	1
Bảng: sessions  —  [chuẩn]	1
Bảng: login_attempts  —  [log-only]	1
Bảng: password_resets  —  [chuẩn]	1
Bảng: user_devices  —  [mới]	1
2. LEARNING CONTENT — Khóa học, Chương, Bài học	1
Bảng: courses  —  [chuẩn]	1
Bảng: chapters  —  [chuẩn]	1
Bảng: lessons  —  [chuẩn]	1
Bảng: lesson_prerequisites  —  [bảng nối]	1
Bảng: content_revisions  —  [immutable]	1
Bảng: course_bookmarks  —  [mới]	1
3. DICTIONARY — Từ vựng, Chữ Hán, Ngữ pháp	1
Bảng: words  —  [chuẩn]	1
Bảng: word_meanings  —  [chuẩn]	1
Bảng: characters  —  [chuẩn]	1
Bảng: word_characters  —  [bảng nối]	1
Bảng: grammar_points  —  [chuẩn]	1
4. TAXONOMY & VERSIONING — Chuẩn học thuật (HSK/CEFR)	1
Bảng: taxonomy_versions  —  [chuẩn]	1
Bảng: taxonomy_levels  —  [chuẩn]	1
Bảng: learning_outcomes  —  [chuẩn]	1
Bảng: cefr_mappings  —  [chuẩn]	1
Bảng: entity_taxonomy_links  —  [bảng nối dùng chung]	1
5. ASSESSMENT — Bài tập, Câu hỏi, Lượt làm bài	1
Bảng: exercises  —  [chuẩn]	1
Bảng: questions  —  [chuẩn]	1
Bảng: attempts  —  [immutable]	1
Bảng: answers  —  [chuẩn]	1
6. PROGRESS & SRS — Tiến độ và Ôn tập ngắt quãng	1
Bảng: progress  —  [chuẩn]	1
Bảng: skill_progress  —  [chuẩn]	1
Bảng: streaks  —  [chuẩn]	1
Bảng: srs_cards  —  [chuẩn]	1
Bảng: srs_reviews  —  [partition theo quý]	1
Bảng: learning_events  —  [immutable · partition theo tháng]	1
7. MEDIA — Tệp tin, Ảnh, Audio, Video	1
Bảng: media_assets  —  [chuẩn]	1
Bảng: uploads  —  [chuẩn]	1
8. NOTIFICATION — Thông báo và Email	1
Bảng: notifications  —  [partition theo tháng]	1
Bảng: notification_preferences  —  [chuẩn]	1
Bảng: email_logs  —  [log-only]	1
Bảng: email_templates  —  [mới]	1
Bảng: email_template_versions  —  [mới · immutable]	1
9. GOVERNANCE / AUDIT — Nhật ký & Hàng đợi duyệt	1
Bảng: audit_logs  —  [immutable · partition theo tháng]	1
Bảng: review_queue  —  [chuẩn]	1
10. PAYMENT & SUBSCRIPTION — Thanh toán, Gói học	1
Bảng: plans  —  [chuẩn]	1
Bảng: subscriptions  —  [chuẩn]	1
Bảng: orders  —  [chuẩn]	1
Bảng: invoices  —  [chuẩn]	1
Bảng: payment_transactions  —  [immutable]	1
Bảng: coupons  —  [chuẩn]	1
Bảng: refunds  —  [chuẩn]	1
Bảng: payment_webhooks_log  —  [immutable · partition theo quý]	1
11. CLASSROOM — Lớp học, Giáo viên, Bài tập giao	1
Bảng: classes  —  [chuẩn]	1
Bảng: class_members  —  [chuẩn]	1
Bảng: class_assignments  —  [chuẩn]	1
Bảng: homework_submissions  —  [chuẩn]	1
Bảng: teacher_class_permissions  —  [bảng nối]	1
12. AI TUTOR / SPEECH — Trợ lý AI, Chấm phát âm	1
Bảng: ai_conversations  —  [chuẩn]	1
Bảng: ai_messages  —  [partition theo tháng]	1
Bảng: ai_usage_logs  —  [log-only]	1
Bảng: pronunciation_attempts  —  [chuẩn]	1
Bảng: moderation_logs  —  [chuẩn]	1
13. HẠ TẦNG VẬN HÀNH (System-level)	1
Bảng: feature_flags  —  [chuẩn]	1
Bảng: feature_flag_audit  —  [log-only]	1
Bảng: app_settings  —  [chuẩn]	1
Bảng: outbox_events  —  [P0 · engine đồng bộ]	1
Bảng: inbox_events  —  [P0 · engine đồng bộ]	1
Bảng: idempotency_keys  —  [chuẩn · dùng chung mọi module]	1
Bảng: security_events  —  [partition theo tháng]	1
Bảng: rate_limit_counters  —  [tùy chọn]	1
14. SUPPORT & COMPLIANCE — Hỗ trợ & Pháp lý	1
Bảng: support_tickets  —  [chuẩn]	1
Bảng: ticket_messages  —  [chuẩn]	1
Bảng: consent_records  —  [immutable]	1
Bảng: data_export_requests  —  [chuẩn]	1
Bảng: data_deletion_requests  —  [chuẩn]	1
Bảng: legal_documents  —  [mới]	1
Bảng: legal_document_versions  —  [mới · immutable]	1
Bảng: help_categories  —  [mới]	1
Bảng: help_articles  —  [mới]	1
15. CONTENT QUALITY & DEPENDENCY	1
Bảng: content_quality_issues  —  [chuẩn]	1
Bảng: content_dependency_links  —  [chuẩn]	1
16. CERTIFICATE & PLACEMENT — Chứng chỉ & Xếp lớp	1
Bảng: certificate_templates  —  [chuẩn]	1
Bảng: certificates  —  [immutable]	1
Bảng: placement_test_sessions  —  [chuẩn]	1
Bảng: placement_test_results  —  [chuẩn]	1
17. REPORTING / ANALYTICS — Báo cáo tổng hợp	1
Bảng: daily_user_metrics  —  [materialized · log-only]	1
Bảng: cost_reports  —  [chuẩn]	1
18. COMMENT & REVIEW — Bình luận & Đánh giá	1
Bảng: comments  —  [chuẩn]	1
Bảng: comment_reports  —  [chuẩn]	1
Bảng: comment_moderation_logs  —  [log-only]	1
Bảng: course_reviews  —  [chuẩn]	1
Bảng: review_helpful_votes  —  [chuẩn]	1
19. GAMIFICATION — Huy hiệu, Bảng xếp hạng, Điểm thưởng	1
Bảng: badges  —  [chuẩn]	1
Bảng: user_badges  —  [chuẩn]	1
Bảng: leaderboards  —  [chuẩn]	1
Bảng: leaderboard_entries  —  [chuẩn]	1
Bảng: points_ledger  —  [immutable]	1
20. IMPORT / EXPORT JOB TRACKING	1
Bảng: import_jobs  —  [chuẩn]	1
Bảng: export_jobs  —  [chuẩn]	1
21. API KEY B2B	1
Bảng: api_keys  —  [chuẩn]	1
Bảng: api_key_scopes  —  [bảng nối]	1
PHỤ LỤC — TRANSACTIONAL OUTBOX / INBOX (P0)	1
Luồng xử lý Outbox Processor	1
Chống xử lý trùng phía Consumer (Inbox)	1
Governance	1

 
PHẦN 0 — QUY ƯỚC CHUNG
0.1 Quy tắc khóa chính: id (nội bộ) & uuid (public)
Mọi bảng nghiệp vụ dùng đồng thời 2 loại khóa:
Khóa	Kiểu	Vai trò
id	BIGINT (auto-increment)	Khóa chính nội bộ — dùng để JOIN và index giữa các bảng trong database, tốc độ nhanh, KHÔNG bao giờ lộ ra ngoài API/URL.
uuid	UUID (random, unique)	Định danh công khai — dùng trong mọi request/response của API (vd /api/v1/lessons/{uuid}), tránh lộ số lượng bản ghi và tránh đoán được ID kế tiếp (chống enumeration/IDOR).

0.2 Cột hệ thống chuẩn — áp dụng cho MỌI bảng nghiệp vụ
Danh sách dưới đây áp dụng mặc định cho tất cả bảng; ở Phần 2 (Database Schema) chỉ liệt kê thêm các cột nghiệp vụ đặc thù của từng bảng để tránh lặp lại.
Cột	Kiểu dữ liệu	Ghi chú
id	BIGINT	PK nội bộ, auto-increment — dùng để join/query nhanh giữa các bảng, KHÔNG lộ ra ngoài API
uuid	UUID	Định danh công khai, unique index — dùng trong mọi path/response của API (vd /api/v1/lessons/{uuid})
created_at	timestamptz	Thời điểm tạo, lưu theo UTC
updated_at	timestamptz	Thời điểm cập nhật gần nhất, lưu theo UTC
created_by	BIGINT	FK → users.id; null nếu do hệ thống tạo
updated_by	BIGINT	FK → users.id; null nếu do hệ thống cập nhật
deleted_at	timestamptz, null	Soft delete — bỏ trống nếu bản ghi còn hiệu lực
version	int, default 1	Optimistic concurrency — tăng 1 mỗi lần UPDATE, dùng để chống ghi đè xung đột
Ngoại lệ: bảng loại immutable (attempts, certificates, payment_transactions, audit_logs, security_events, outbox_events, learning_events...) và bảng log-only (login_attempts, email_logs...) không có updated_at/updated_by/deleted_at/version vì chỉ INSERT, không UPDATE.

0.3 Đánh dấu mã hóa cấp cột (Field-level Encryption)
Bảng.Cột	Mức phân loại	Xử lý
users.password_hash	Restricted	Hash Argon2id — không phải encryption
sessions.refresh_token_hash	Restricted	Hash, không lưu plaintext
payment_transactions.raw_response_json	Restricted	Field-level encryption bắt buộc
payment_webhooks_log.payload_json	Restricted	Field-level encryption bắt buộc
pronunciation_attempts.recording_media_id	Restricted (trỏ file)	Mã hóa tại storage layer, không phải cột DB
ai_messages.content_text	Confidential	Không field-level encrypt (cần search/debug) nhưng redact khi log ra ngoài DB
consent_records (toàn bảng)	Restricted	Encryption at rest bắt buộc, immutable
support_tickets/ticket_messages	Confidential	Encryption at rest tầng DB là đủ
password_resets.token_hash	Restricted	Hash, TTL ngắn
Nguyên tắc: chỉ field-level encryption khi (a) dữ liệu Restricted và (b) không cần truy vấn/search trực tiếp trên cột đó. Các trường hợp còn lại dùng encryption at rest tầng database/storage là đủ.

0.4 Partitioning — chốt cụ thể
Bảng	Partition key	Chu kỳ
learning_events	created_at	Theo tháng
audit_logs	occurred_at	Theo tháng
security_events	created_at	Theo tháng
ai_messages	created_at	Theo tháng
notifications	created_at	Theo tháng
srs_reviews	created_at	Theo quý
payment_webhooks_log	received_at	Theo quý
outbox_events / inbox_events	—	Không partition — bảng ngắn hạn, có job cleanup riêng
Quy tắc chung: partition tạo sẵn ở schema (không tạo bảng partition thật từ MVP), nhưng PK và index không được thiết kế chặn khả năng partition sau này.

0.5 Chuẩn API dùng chung
• Naming: bảng snake_case số nhiều; cột snake_case; API path kebab-case; API versioning /api/v1.
• Response envelope: { "data": {...}, "meta": { "page", "pageSize", "total" } }
• Error envelope theo ProblemDetails: { "type", "title", "status", "code", "traceId", "errors[]" } — mã lỗi nghiệp vụ ổn định, vd AUTH.INVALID_CREDENTIALS, LEARNING.LESSON_LOCKED, SYNC.CONFLICT, PAYMENT.ALREADY_PROCESSED.
• Mọi API ADMIN-* bắt buộc: kiểm tra Permission, ghi audit_logs với action rõ ràng, trả lỗi theo ProblemDetails.
• Idempotency-Key bắt buộc cho: thanh toán, nộp bài, import, tạo đơn hàng, cập nhật tiến độ offline.
 
PHẦN 1 — API CATALOG
Tổng cộng ~145 API, chia thành 21 nhóm theo module nghiệp vụ. Mỗi API được mô tả rõ chức năng thực hiện, không chỉ liệt kê path.

1. AUTH / IDENTITY — Xác thực & Tài khoản
API ID	Method	Path	Quyền / Actor	Chức năng
AUTH-001	POST	/api/v1/auth/register	Guest	Đăng ký tài khoản mới: tạo user ở trạng thái pending và gửi email xác minh.
AUTH-002	POST	/api/v1/auth/login	Guest	Đăng nhập: kiểm tra khóa tài khoản, giới hạn 5 lần/phút/IP, ghi log vào login_attempts.
AUTH-003	POST	/api/v1/auth/refresh	Guest (refresh cookie)	Làm mới access token; xoay vòng (rotation) refresh token; nếu phát hiện token bị dùng lại (reuse) thì thu hồi toàn bộ family.
AUTH-004	POST	/api/v1/auth/logout	Learner+	Đăng xuất thiết bị hiện tại, thu hồi session tương ứng.
AUTH-005	POST	/api/v1/auth/logout-all	Learner+	Đăng xuất toàn bộ thiết bị, thu hồi mọi session của tài khoản.
AUTH-006	POST	/api/v1/auth/forgot-password	Guest	Gửi yêu cầu quên mật khẩu; không tiết lộ email có tồn tại trong hệ thống hay không.
AUTH-007	POST	/api/v1/auth/reset-password	Guest (token)	Đặt lại mật khẩu bằng token dùng 1 lần, hết hạn sau 15 phút.
AUTH-008	GET	/api/v1/auth/sessions	Learner+	Liệt kê các thiết bị/phiên đang đăng nhập của tài khoản.
AUTH-009	POST	/api/v1/auth/sessions/{id}/revoke	Learner+	Thu hồi một phiên đăng nhập cụ thể (kiểm tra quyền sở hữu phiên).
USER-001	GET	/api/v1/users/me	Learner+	Lấy thông tin hồ sơ cá nhân của người dùng hiện tại.
USER-002	PATCH	/api/v1/users/me	Learner+	Cập nhật hồ sơ cá nhân; yêu cầu xác thực lại (re-auth) nếu đổi email.
USER-003	POST	/api/v1/users/me/data-export	Learner+	Yêu cầu xuất toàn bộ dữ liệu cá nhân; tạo job bất đồng bộ và gửi link tải qua email.
USER-004	POST	/api/v1/users/me/deletion-request	Learner+	Yêu cầu xóa tài khoản: soft-delete ngay và lên lịch anonymize dữ liệu.
ADMIN-USER-001	GET	/api/v1/admin/users	user.manage	Danh sách người dùng cho quản trị, hỗ trợ lọc/sắp xếp/phân trang.
ADMIN-USER-002	GET	/api/v1/admin/users/{id}	user.manage	Xem chi tiết một người dùng cụ thể.
ADMIN-USER-003	PATCH	/api/v1/admin/users/{id}/status	user.manage	Khóa hoặc mở khóa tài khoản người dùng; bắt buộc ghi audit.
ADMIN-USER-004	POST	/api/v1/admin/users/{id}/roles	user.manage (+ four-eyes nếu quyền cao)	Gán hoặc thu hồi vai trò của người dùng.
ADMIN-USER-005	POST	/api/v1/admin/users/{id}/force-logout	user.manage	Buộc đăng xuất toàn bộ phiên đang hoạt động của một người dùng.
DEVICE-001	POST	/api/v1/devices/register	Learner	Đăng ký push token của thiết bị để nhận thông báo đẩy.

2. COURSE / LESSON — Khóa học & Bài học
API ID	Method	Path	Quyền / Actor	Chức năng
COURSE-001	GET	/api/v1/courses	Public	Danh sách khóa học công khai (chỉ trả về bản đã published).
COURSE-002	GET	/api/v1/courses/{slug}	Public	Xem chi tiết một khóa học theo slug; có cache CDN.
LESSON-001	GET	/api/v1/lessons/{id}	Learner	Xem chi tiết bài học; kiểm tra điều kiện mở bài (prerequisite).
LESSON-002	POST	/api/v1/lessons/{id}/start	Learner	Bắt đầu học một bài: tạo phiên học (attempt/session), phát sự kiện lesson_started.
LESSON-003	POST	/api/v1/lessons/{id}/complete	Learner	Hoàn thành bài học: ghi Progress, tạo thẻ SRS mới; bắt buộc Idempotency-Key để chống ghi trùng.
ADMIN-COURSE-001	GET	/api/v1/admin/courses	content.view	Danh sách khóa học phục vụ quản trị nội dung.
ADMIN-COURSE-002	POST	/api/v1/admin/courses	content.create	Tạo khóa học mới ở trạng thái Draft.
ADMIN-COURSE-003	PUT	/api/v1/admin/courses/{id}	content.edit	Sửa thông tin khóa học; bắt buộc concurrency token để tránh ghi đè.
ADMIN-COURSE-004	POST	/api/v1/admin/courses/{id}/submit-review	content.create	Gửi khóa học đi duyệt (Draft → Review).
ADMIN-COURSE-005	POST	/api/v1/admin/courses/{id}/approve	content.review	Duyệt khóa học; người duyệt phải khác người tạo.
ADMIN-COURSE-006	POST	/api/v1/admin/courses/{id}/publish	content.publish	Xuất bản khóa học; tạo snapshot vào content_revisions.
ADMIN-COURSE-007	POST	/api/v1/admin/courses/{id}/reject	content.review	Từ chối duyệt; bắt buộc nêu lý do.
ADMIN-COURSE-008	POST	/api/v1/admin/courses/{id}/rollback/{revisionId}	content.publish	Khôi phục khóa học về một phiên bản nội dung trước đó.
ADMIN-LESSON-001..006	CRUD + workflow	/api/v1/admin/chapters, /api/v1/admin/lessons...	content.*	Tạo/sửa/gửi duyệt/duyệt/xuất bản/rollback cho Chapter và Lesson — áp dụng đúng khuôn mẫu như Course.
BOOKMARK-001	POST	/api/v1/courses/{id}/bookmark	Learner	Lưu khóa học vào danh sách yêu thích (idempotent nhờ unique constraint).
BOOKMARK-002	DELETE	/api/v1/courses/{id}/bookmark	Learner	Bỏ lưu khóa học khỏi danh sách yêu thích.

3. DICTIONARY — Từ điển
API ID	Method	Path	Quyền / Actor	Chức năng
DICT-001	GET	/api/v1/dictionary/search?q=	Public	Tìm kiếm từ điển: hỗ trợ chữ Hán, pinyin có/không dấu, tiếng Việt, âm Hán Việt.
DICT-002	GET	/api/v1/dictionary/words/{id}	Public	Xem chi tiết một từ vựng.
DICT-003	POST	/api/v1/users/me/saved-words	Learner	Lưu một từ vào sổ tay cá nhân để tra cứu nhanh sau này.
ADMIN-DICT-001	GET	/api/v1/admin/words	content.view	Danh sách từ vựng cho quản trị; lọc theo HSK/trạng thái duyệt/trường còn thiếu.
ADMIN-DICT-002	POST	/api/v1/admin/words	content.create	Tạo từ mới; validate bắt buộc simplified, pinyin, nghĩa tiếng Việt, HSK mapping trước khi cho publish.
ADMIN-DICT-003	POST	/api/v1/admin/words/import	content.create	Import từ vựng hàng loạt qua file Excel (qua Import Worker); trả về job_id để theo dõi.
ADMIN-DICT-004	GET	/api/v1/admin/imports/{jobId}	content.view	Theo dõi trạng thái job import: tổng dòng, số thành công, số lỗi, log lỗi để tải về.

4. ASSESSMENT — Bài tập & Chấm điểm
API ID	Method	Path	Quyền / Actor	Chức năng
EXAM-001	POST	/api/v1/exercises/{id}/attempts	Learner	Bắt đầu làm bài: tạo snapshot đề tại đúng thời điểm bắt đầu (chống gian lận khi admin sửa đề sau đó).
EXAM-002	POST	/api/v1/attempts/{id}/answers	Learner	Lưu tạm từng câu trả lời (autosave) trong lúc làm bài.
EXAM-003	POST	/api/v1/attempts/{id}/submit	Learner	Nộp bài; bắt buộc Idempotency-Key; chặn nộp trùng hoặc nộp sau khi đã hết giờ.
EXAM-004	GET	/api/v1/attempts/{id}/result	Learner	Xem kết quả bài đã làm.
ADMIN-EXAM-001	GET	/api/v1/admin/exercises	content.view	Danh sách bài tập cho quản trị.
ADMIN-EXAM-002	POST	/api/v1/admin/exercises/{id}/questions	content.create	Tạo câu hỏi mới; bắt buộc dual-review (người tạo khác người duyệt) nếu là câu hỏi thi chính thức.

5. PROGRESS & SRS — Tiến độ & Ôn tập
API ID	Method	Path	Quyền / Actor	Chức năng
PROG-001	GET	/api/v1/progress/me	Learner	Xem tiến độ học tổng hợp, tách riêng theo từng kỹ năng.
PROG-002	GET	/api/v1/srs/due	Learner	Danh sách thẻ SRS đã đến hạn ôn tập.
PROG-003	POST	/api/v1/srs/cards/{id}/review	Learner	Ghi kết quả ôn tập, tính lại interval theo thuật toán (algorithm_version); có lock chống review đồng thời trên 2 thiết bị.
SYNC-001	POST	/api/v1/sync/batch	Learner	Đồng bộ hàng loạt hành động học offline lên server; mỗi item mang client_id + idempotency_key riêng.
SYNC-002	GET	/api/v1/sync/status	Learner	Kiểm tra trạng thái đồng bộ dữ liệu offline.
ADMIN-PROG-001	GET	/api/v1/admin/users/{id}/progress	support / user.manage	Cho phép nhân viên hỗ trợ xem tiến độ học viên khi được cấp quyền; ghi audit.

6. MEDIA — Tải lên & Truy xuất tài nguyên
API ID	Method	Path	Quyền / Actor	Chức năng
MEDIA-001	POST	/api/v1/uploads/init	Learner+	Khởi tạo phiên upload: trả về pre-signed URL, giới hạn loại file (MIME) và dung lượng.
MEDIA-002	POST	/api/v1/uploads/complete	Learner+	Xác nhận upload hoàn tất; kích hoạt quét malware và re-encode bất đồng bộ.
MEDIA-003	GET	/api/v1/media/{id}/signed-url	Learner+	Lấy URL truy cập media có thời hạn ngắn; kiểm tra quyền sở hữu trước khi cấp.
ADMIN-MEDIA-001	GET	/api/v1/admin/media	content.view	Danh sách media cho quản trị; lọc theo trạng thái quét, loại, ngày.

7. NOTIFICATION — Thông báo & Email
API ID	Method	Path	Quyền / Actor	Chức năng
NOTI-001	GET	/api/v1/notifications	Learner+	Danh sách thông báo của người dùng hiện tại.
NOTI-002	PATCH	/api/v1/notifications/{id}/read	Learner+	Đánh dấu một thông báo là đã đọc.
NOTI-003	PATCH	/api/v1/users/me/notification-preferences	Learner+	Cập nhật tùy chọn nhận thông báo theo từng loại (quiet hours, tần suất...).
ADMIN-NOTI-001	POST	/api/v1/admin/notifications/broadcast	system.config	Gửi thông báo hàng loạt tới một nhóm người dùng.
ADMIN-EMAILTPL-001	GET	/api/v1/admin/email-templates	content.view	Danh sách mẫu email hiện có.
ADMIN-EMAILTPL-002	PUT	/api/v1/admin/email-templates/{code}	content.edit	Cập nhật mẫu email; luôn tạo version mới, không sửa đè version cũ.
ADMIN-EMAILTPL-003	POST	/api/v1/admin/email-templates/{code}/preview	content.view	Xem thử email trước khi gửi thật (không gửi).
ADMIN-EMAILTPL-004	POST	/api/v1/admin/email-templates/{code}/send-test	content.edit	Gửi thử email tới địa chỉ chỉ định; giới hạn 5 lần/giờ để chống lạm dụng.

8. GOVERNANCE / AUDIT — Nhật ký & Duyệt nội dung
API ID	Method	Path	Quyền / Actor	Chức năng
ADMIN-AUDIT-001	GET	/api/v1/admin/audit-logs	audit.view	Tra cứu nhật ký hành động quản trị (ai làm gì, khi nào, thay đổi trước/sau).
ADMIN-REVIEW-001	GET	/api/v1/admin/review-queue	content.review	Danh sách nội dung đang chờ được duyệt, sắp xếp theo hạn SLA.

9. SYSTEM — Hệ thống & Cấu hình công khai
API ID	Method	Path	Quyền / Actor	Chức năng
SYS-001	GET	/api/v1/health/live	None	Kiểm tra tiến trình ứng dụng còn hoạt động (liveness probe).
SYS-002	GET	/api/v1/health/ready	None	Kiểm tra ứng dụng đã sẵn sàng nhận request, có kiểm tra kết nối Database/Redis (readiness probe).
SYS-003	GET	/api/v1/version	None	Trả về phiên bản build hiện tại của backend.
SYS-004	GET	/api/v1/config/public	None	Trả cấu hình công khai không nhạy cảm (vd feature flag công khai); cache 5 phút.

10. SEARCH — Tìm kiếm toàn hệ thống
API ID	Method	Path	Quyền / Actor	Chức năng
SEARCH-001	GET	/api/v1/search?q=&type=course,lesson,word&page=	Public/Learner	Tìm kiếm tổng hợp trên nhiều loại nội dung (khóa học, bài học, từ vựng...) cùng lúc.

11. PAYMENT & SUBSCRIPTION — Thanh toán & Gói học
API ID	Method	Path	Quyền / Actor	Chức năng
PAY-001	POST	/api/v1/orders	Learner	Tạo đơn hàng mua gói học; kiểm tra coupon hợp lệ; sinh idempotency_key theo (user, plan, phút hiện tại) để chống double-click tạo 2 đơn.
PAY-002	POST	/api/v1/webhooks/{provider}	Xác thực bằng chữ ký HMAC	Nhận webhook xác nhận thanh toán từ cổng thanh toán; ghi log trước khi verify; chặn xử lý trùng qua event_id; cập nhật orders/payment_transactions trong 1 transaction; phát event PaymentConfirmed.
PAY-003	GET	/api/v1/subscriptions/me	Learner	Xem thông tin gói đăng ký hiện tại.
PAY-004	POST	/api/v1/subscriptions/me/cancel	Learner	Hủy gói đăng ký; bắt buộc ghi audit.
PAY-005	GET	/api/v1/invoices/me	Learner	Xem lịch sử hóa đơn.
ADMIN-PAY-001	POST	/api/v1/admin/refunds	payment.refund	Xử lý hoàn tiền; bắt buộc four-eyes approval nếu số tiền vượt ngưỡng; ghi cảnh báo nếu tần suất refund bất thường.

12. CLASSROOM — Lớp học
API ID	Method	Path	Quyền / Actor	Chức năng
CLASS-001	POST	/api/v1/classes	Teacher	Tạo lớp học mới; sinh mã tham gia (join_code) ngẫu nhiên 8 ký tự.
CLASS-002	POST	/api/v1/classes/join	Learner	Tham gia lớp bằng mã mời; kiểm tra lớp đang ở trạng thái active.
CLASS-003	POST	/api/v1/classes/{id}/assignments	Teacher	Giao bài tập cho lớp; kiểm tra quyền sở hữu lớp qua teacher_class_permissions.
CLASS-004	POST	/api/v1/assignments/{id}/submissions	Learner	Nộp bài tập được giao; idempotent theo cặp (assignment_id, user_id).
CLASS-005	PATCH	/api/v1/submissions/{id}/grade	Teacher	Chấm điểm bài nộp của học viên; bắt buộc ghi audit.

13. AI TUTOR — Trợ lý AI & Chấm phát âm
API ID	Method	Path	Quyền / Actor	Chức năng
AI-001	POST	/api/v1/ai/conversations/{id}/messages	Learner	Gửi tin nhắn cho AI Tutor: kiểm tra consent + quota còn lại trước khi gọi; input/output đều qua kiểm duyệt (moderation) trước khi trả về; ghi log chi phí bất kể thành công hay lỗi.
AI-002	POST	/api/v1/pronunciation/attempts	Learner	Nộp bản ghi âm để AI chấm điểm phát âm; nếu provider timeout thì trả trạng thái 'đang chấm điểm' thay vì báo lỗi cứng.

14. CERTIFICATE & PLACEMENT — Chứng chỉ & Xếp lớp
API ID	Method	Path	Quyền / Actor	Chức năng
CERT-001	GET	/api/v1/certificates/me	Learner	Xem danh sách chứng chỉ của bản thân.
CERT-002	GET	/api/v1/certificates/verify/{code}	Public	Tra cứu công khai xác thực chứng chỉ, không cần đăng nhập.
ADMIN-CERT-001	POST	/api/v1/admin/certificates/{id}/revoke	content.publish	Thu hồi chứng chỉ đã cấp; bắt buộc nêu lý do; ghi audit.
PLACE-001	POST	/api/v1/placement-tests	Learner/Guest	Bắt đầu bài kiểm tra xếp lớp; tạo snapshot đề bài.
PLACE-002	POST	/api/v1/placement-tests/{id}/submit	Learner/Guest	Nộp bài kiểm tra xếp lớp; tính kết quả và đề xuất cấp độ; idempotent.

15. ADMIN — Feature Flags & Settings
API ID	Method	Path	Quyền / Actor	Chức năng
ADMIN-FLAG-001	GET	/api/v1/admin/feature-flags	system.config	Danh sách cờ tính năng hiện có.
ADMIN-FLAG-002	PATCH	/api/v1/admin/feature-flags/{key}	system.config	Bật/tắt hoặc điều chỉnh phần trăm rollout của một cờ tính năng; ghi feature_flag_audit.
ADMIN-SETTINGS-001	GET	/api/v1/admin/settings?category=	system.config	Xem cấu hình hệ thống theo từng nhóm (static/runtime/business_rule/tenant).
ADMIN-SETTINGS-002	PUT	/api/v1/admin/settings/{key}	system.config	Cập nhật giá trị cấu hình; chặn sửa trực tiếp nếu is_secret_ref=true (phải qua secret vault riêng).

16. ADMIN — Reports & Cost
API ID	Method	Path	Quyền / Actor	Chức năng
ADMIN-REPORT-001	GET	/api/v1/admin/reports/cost?from=&to=&category=	report.view	Xem báo cáo chi phí vận hành theo khoảng thời gian và hạng mục (đọc từ cost_reports, không tính realtime).
ADMIN-REPORT-002	GET	/api/v1/admin/reports/export?type=users|revenue|content-health&format=csv	report.view	Xuất báo cáo ra file; chạy job bất đồng bộ, tải qua signed URL; cảnh báo nếu export số lượng lớn.

17. SUPPORT — Hỗ trợ người dùng
API ID	Method	Path	Quyền / Actor	Chức năng
SUPPORT-001	POST	/api/v1/support/tickets	Learner	Tạo yêu cầu hỗ trợ (ticket) mới.
SUPPORT-002	GET	/api/v1/support/tickets/{id}	Learner	Xem chi tiết một ticket; kiểm tra quyền sở hữu.
SUPPORT-003	POST	/api/v1/support/tickets/{id}/messages	Learner	Gửi tin nhắn trao đổi trong ticket.
ADMIN-SUPPORT-001	GET	/api/v1/admin/support/tickets?trace_id=	agent.support	Tra cứu ticket theo trace ID để hỗ trợ nhanh khi người dùng báo lỗi.
ADMIN-SUPPORT-002	PATCH	/api/v1/admin/support/tickets/{id}/assign	agent.support	Phân công ticket cho một nhân viên hỗ trợ xử lý.

18. COMMENT & REVIEW — Bình luận & Đánh giá
API ID	Method	Path	Quyền / Actor	Chức năng
COMMENT-001	POST	/api/v1/comments	Learner	Đăng bình luận trên bài học/khóa học/bài tập; kiểm tra entity tồn tại; rate-limit 20/phút chống spam.
COMMENT-002	GET	/api/v1/comments?entityType=&entityId=	Public/Learner	Xem danh sách bình luận của một đối tượng, phân trang kiểu cursor.
COMMENT-003	POST	/api/v1/comments/{id}/report	Learner	Báo cáo một bình luận vi phạm; tự động đẩy vào review_queue nếu vượt ngưỡng báo cáo.
ADMIN-COMMENT-001	PATCH	/api/v1/admin/comments/{id}/moderate	content.review	Ẩn/xóa/khôi phục bình luận; bắt buộc ghi comment_moderation_logs.
REVIEW-001	POST	/api/v1/courses/{id}/reviews	Learner (đã học ≥1 bài)	Đánh giá sao cho khóa học; mỗi người chỉ đánh giá 1 lần/khóa học.
REVIEW-002	GET	/api/v1/courses/{id}/reviews	Public	Xem danh sách đánh giá của một khóa học.
REVIEW-003	POST	/api/v1/reviews/{id}/helpful	Learner	Đánh dấu một đánh giá là hữu ích; idempotent.

19. GAMIFICATION — Huy hiệu & Bảng xếp hạng
API ID	Method	Path	Quyền / Actor	Chức năng
GAME-001	GET	/api/v1/badges/me	Learner	Xem các huy hiệu đã đạt được.
GAME-002	GET	/api/v1/leaderboards?type=weekly&scope=global	Public/Learner	Xem bảng xếp hạng; đọc dữ liệu đã tính sẵn, không tính realtime.
ADMIN-GAME-001	POST	/api/v1/admin/badges	content.create	Tạo huy hiệu mới cùng điều kiện đạt được.
ADMIN-GAME-002	POST	/api/v1/admin/leaderboards/{id}/recompute	system.config	Kích hoạt tính lại bảng xếp hạng thủ công khi cần.

20. ADMIN — API Key B2B
API ID	Method	Path	Quyền / Actor	Chức năng
ADMIN-APIKEY-001	POST	/api/v1/admin/api-keys	system.config	Tạo API key mới cho đối tác B2B; giá trị key thật chỉ hiển thị đúng 1 lần lúc tạo, sau đó chỉ lưu hash.
ADMIN-APIKEY-002	POST	/api/v1/admin/api-keys/{id}/revoke	system.config	Thu hồi một API key; bắt buộc ghi audit.
ADMIN-APIKEY-003	GET	/api/v1/admin/api-keys	system.config	Danh sách API key; không bao giờ trả lại giá trị key thật, chỉ trả metadata.

21. LEGAL DOCUMENTS & HELP CENTER
API ID	Method	Path	Quyền / Actor	Chức năng
LEGAL-001	GET	/api/v1/legal/{type}	Public	Xem nội dung điều khoản/chính sách đang có hiệu lực hiện hành.
ADMIN-LEGAL-001	POST	/api/v1/admin/legal/{type}/versions	content.publish	Xuất bản phiên bản điều khoản mới; requires_reconsent quyết định có buộc người dùng đồng ý lại hay không.
HELP-001	GET	/api/v1/help/articles	Public	Danh sách bài viết trong trung tâm trợ giúp.
HELP-002	GET	/api/v1/help/articles/{id}	Public	Xem chi tiết một bài viết trợ giúp; tăng view_count.
ADMIN-HELP-001..003	CRUD	/api/v1/admin/help/articles	content.create/edit/publish	Quản trị nội dung trung tâm trợ giúp theo quy trình draft/published đơn giản.

 
PHẦN 2 — DATABASE SCHEMA
Tổng cộng ~108 bảng, chia thành 21 nhóm module. Mỗi bảng liệt kê CHỈ các cột nghiệp vụ đặc thù — cột hệ thống chuẩn (id, uuid, created_at, updated_at, created_by, updated_by, deleted_at, version) xem tại Phần 0.2.

1. IDENTITY — Người dùng, Vai trò, Phiên đăng nhập
Quản lý tài khoản, xác thực, phân quyền và thiết bị.
Bảng: users  —  [chuẩn]
Tài khoản người dùng.
Cột	Kiểu dữ liệu	Ghi chú
email	varchar(255)	Duy nhất, không rỗng
email_verified_at	timestamptz, null	Thời điểm xác minh email
password_hash	varchar(255)	Băm bằng Argon2id — mức Restricted, không log
display_name	varchar(100)	Tên hiển thị, không rỗng
avatar_url	varchar(500), null	
status	enum	active / locked / disabled / pending
locale	varchar(10)	Mặc định 'vi'
last_login_at	timestamptz, null	
failed_login_count	int, default 0	Đếm số lần đăng nhập sai liên tiếp
locked_until	timestamptz, null	Thời điểm mở khóa tự động

Bảng: roles  —  [chuẩn]
Danh mục vai trò hệ thống.
Cột	Kiểu dữ liệu	Ghi chú
code	varchar(50)	Duy nhất, vd LEARNER, TEACHER, ADMIN
name	varchar(100)	
is_system	bool	Vai trò hệ thống — không cho xóa

Bảng: permissions  —  [chuẩn]
Danh mục quyền hạn chi tiết.
Cột	Kiểu dữ liệu	Ghi chú
code	varchar(100)	Duy nhất, vd content.publish, user.manage
description	text	
resource	varchar(50)	
action	varchar(50)	

Bảng: role_permissions  —  [bảng nối]
Gán quyền cho vai trò (không cần id/uuid riêng, PK ghép).
Cột	Kiểu dữ liệu	Ghi chú
role_id	BIGINT	FK → roles.id — PK ghép
permission_id	BIGINT	FK → permissions.id — PK ghép

Bảng: user_roles  —  [chuẩn]
Gán vai trò cho người dùng, hỗ trợ quyền tạm thời.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
role_id	BIGINT	FK → roles.id
granted_at	timestamptz	
granted_by	BIGINT	FK → users.id
expires_at	timestamptz, null	Quyền tạm thời nếu có giá trị

Bảng: sessions  —  [chuẩn]
Phiên đăng nhập / thiết bị.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
device_info	text	
ip	varchar(45)	
user_agent	text	
refresh_token_hash	varchar(255)	Restricted — chỉ lưu hash
family_id	UUID	Nhóm token để thu hồi hàng loạt khi phát hiện reuse
revoked_at	timestamptz, null	
expires_at	timestamptz	

Bảng: login_attempts  —  [log-only]
Ghi log mọi lần đăng nhập (thành công/thất bại), không sửa/xóa.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT, null	FK → users.id nếu xác định được
email	varchar(255)	
ip	varchar(45)	
success	bool	
reason	varchar(100)	Lý do thất bại nếu có

Bảng: password_resets  —  [chuẩn]
Yêu cầu đặt lại mật khẩu.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
token_hash	varchar(255)	Restricted
expires_at	timestamptz	Mặc định 15 phút
used_at	timestamptz, null	Token dùng 1 lần

Bảng: user_devices  —  [mới]
Thiết bị đăng ký nhận push notification (tách khỏi sessions).
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
device_id	varchar(255)	
platform	enum	ios / android / web
push_token	varchar(500)	
last_seen_at	timestamptz	
status	enum	active / inactive

2. LEARNING CONTENT — Khóa học, Chương, Bài học
Nội dung học chính, theo quy trình Draft → Review → Approved → Published → Archived.
Bảng: courses  —  [chuẩn]
Khóa học.
Cột	Kiểu dữ liệu	Ghi chú
slug	varchar(255)	Duy nhất, dùng cho URL công khai
title	varchar(255)	
description	text	
level_code	BIGINT	FK → taxonomy_levels.id
cover_image_url	varchar(500)	
status	enum	draft / review / approved / published / archived
published_at	timestamptz, null	
content_version	int	
superseded_by	BIGINT, null	FK → courses.id — trỏ tới bản thay thế

Bảng: chapters  —  [chuẩn]
Chương trong khóa học.
Cột	Kiểu dữ liệu	Ghi chú
course_id	BIGINT	FK → courses.id
title	varchar(255)	
order_index	int	
status	enum	draft / review / approved / published / archived

Bảng: lessons  —  [chuẩn]
Bài học trong chương.
Cột	Kiểu dữ liệu	Ghi chú
chapter_id	BIGINT	FK → chapters.id
title	varchar(255)	
type	enum	vocabulary / grammar / listening / speaking / reading / writing / mixed
order_index	int	
content_json	JSONB	Cấu trúc nội dung bài học
estimated_duration_seconds	int	
pass_score	numeric	Điểm tối thiểu để qua bài
status	enum	
content_version	int	

Bảng: lesson_prerequisites  —  [bảng nối]
Điều kiện tiên quyết giữa các bài học (chống vòng lặp ở application layer).
Cột	Kiểu dữ liệu	Ghi chú
lesson_id	BIGINT	FK → lessons.id
prerequisite_lesson_id	BIGINT	FK → lessons.id

Bảng: content_revisions  —  [immutable]
Snapshot phiên bản nội dung mỗi lần publish, phục vụ rollback.
Cột	Kiểu dữ liệu	Ghi chú
entity_type	varchar(50)	course / chapter / lesson
entity_id	BIGINT	
revision_number	int	
snapshot_json	JSONB	Toàn bộ nội dung tại thời điểm publish
status	enum	

Bảng: course_bookmarks  —  [mới]
Danh sách khóa học người dùng lưu để học sau (wishlist).
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id — unique cùng course_id
course_id	BIGINT	FK → courses.id

3. DICTIONARY — Từ vựng, Chữ Hán, Ngữ pháp
Dữ liệu từ điển tiếng Trung.
Bảng: words  —  [chuẩn]
Từ vựng.
Cột	Kiểu dữ liệu	Ghi chú
simplified	varchar(50)	Chữ giản thể
traditional	varchar(50)	Chữ phồn thể
pinyin_marked	varchar(100)	Pinyin có dấu thanh
pinyin_numeric	varchar(100)	Pinyin dạng số
part_of_speech	varchar(50)	
source	varchar(255)	Nguồn dữ liệu
license	varchar(100)	Loại giấy phép sử dụng
review_status	enum	Trạng thái kiểm duyệt
audio_url	varchar(500)	

Bảng: word_meanings  —  [chuẩn]
Nghĩa tiếng Việt của từ (1 từ có thể nhiều nghĩa).
Cột	Kiểu dữ liệu	Ghi chú
word_id	BIGINT	FK → words.id
vietnamese_meaning	text	
sino_vietnamese	varchar(255)	Âm Hán Việt
example_sentence	text	
example_translation	text	
order_index	int	

Bảng: characters  —  [chuẩn]
Chữ Hán đơn lẻ.
Cột	Kiểu dữ liệu	Ghi chú
character	varchar(10)	
radical	varchar(10)	Bộ thủ
stroke_count	int	Số nét
stroke_order_svg_url	varchar(500)	SVG thứ tự nét

Bảng: word_characters  —  [bảng nối]
Liên kết từ vựng ↔ chữ Hán cấu thành.
Cột	Kiểu dữ liệu	Ghi chú
word_id	BIGINT	FK → words.id
character_id	BIGINT	FK → characters.id
position	int	Vị trí chữ trong từ

Bảng: grammar_points  —  [chuẩn]
Điểm ngữ pháp.
Cột	Kiểu dữ liệu	Ghi chú
title	varchar(255)	
explanation	text	
structure_pattern	varchar(500)	
example_json	JSONB	

4. TAXONOMY & VERSIONING — Chuẩn học thuật (HSK/CEFR)
Quản lý phiên bản chuẩn học thuật, thay thế việc lưu rải rác 1 cột hsk_level ở nhiều bảng.
Bảng: taxonomy_versions  —  [chuẩn]
Phiên bản chuẩn (HSK 2.0/3.0, CEFR, nội bộ...).
Cột	Kiểu dữ liệu	Ghi chú
standard_code	varchar(20)	HSK / CEFR / INTERNAL
version_label	varchar(50)	vd HSK-3.0
effective_from	date	
effective_to	date, null	
is_active	bool	

Bảng: taxonomy_levels  —  [chuẩn]
Cấp độ trong một phiên bản chuẩn.
Cột	Kiểu dữ liệu	Ghi chú
taxonomy_version_id	BIGINT	FK → taxonomy_versions.id
level_code	varchar(50)	vd HSK3-L2
level_order	int	
display_name	varchar(100)	

Bảng: learning_outcomes  —  [chuẩn]
Mục tiêu đầu ra học tập theo cấp độ.
Cột	Kiểu dữ liệu	Ghi chú
taxonomy_level_id	BIGINT	FK → taxonomy_levels.id
description	text	
skill_type	enum	vocab / grammar / listening / speaking / reading / writing

Bảng: cefr_mappings  —  [chuẩn]
Ánh xạ sang chuẩn CEFR tương đương.
Cột	Kiểu dữ liệu	Ghi chú
taxonomy_level_id	BIGINT	FK → taxonomy_levels.id
cefr_level	enum	A1..C2
mapping_confidence	enum	exact / approximate

Bảng: entity_taxonomy_links  —  [bảng nối dùng chung]
Bảng NỐI DUY NHẤT gắn taxonomy cho Word/Lesson/Course/Question — thay thế mọi cột hsk_level rải rác.
Cột	Kiểu dữ liệu	Ghi chú
entity_type	varchar(20)	word / lesson / course / question
entity_id	BIGINT	
taxonomy_level_id	BIGINT	FK → taxonomy_levels.id

5. ASSESSMENT — Bài tập, Câu hỏi, Lượt làm bài
Chấm điểm và lưu kết quả bất biến (chống gian lận, chống sửa lịch sử).
Bảng: exercises  —  [chuẩn]
Bài tập gắn với 1 bài học.
Cột	Kiểu dữ liệu	Ghi chú
lesson_id	BIGINT	FK → lessons.id
type	enum	multiple_choice / fill_blank / listening / speaking / writing
config_json	JSONB	
order_index	int	

Bảng: questions  —  [chuẩn]
Câu hỏi trong bài tập.
Cột	Kiểu dữ liệu	Ghi chú
exercise_id	BIGINT	FK → exercises.id
content_json	JSONB	
correct_answer_json	JSONB	
difficulty	enum	
question_version	int	

Bảng: attempts  —  [immutable]
Lượt làm bài — snapshot đề & đáp án tại thời điểm bắt đầu, admin sửa câu hỏi sau đó KHÔNG ảnh hưởng kết quả cũ.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
exercise_id	BIGINT	FK → exercises.id
question_snapshot_json	JSONB	Snapshot đề bài
answer_snapshot_json	JSONB	Snapshot đáp án đúng tại thời điểm đó
scoring_rule_version	varchar(50)	
content_version	int	
score	numeric	
status	enum	in_progress / submitted / expired
started_at	timestamptz	
submitted_at	timestamptz, null	

Bảng: answers  —  [chuẩn]
Đáp án người dùng chọn cho từng câu trong 1 attempt.
Cột	Kiểu dữ liệu	Ghi chú
attempt_id	BIGINT	FK → attempts.id
question_id	BIGINT	FK → questions.id
selected_answer_json	JSONB	
is_correct	bool	
response_time_ms	int	

6. PROGRESS & SRS — Tiến độ và Ôn tập ngắt quãng
Theo dõi tiến độ theo từng kỹ năng (không chỉ 1 con số tổng), lịch ôn tập SRS, và toàn bộ sự kiện học tập.
Bảng: progress  —  [chuẩn]
Tiến độ theo từng bài học.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
lesson_id	BIGINT	FK → lessons.id
status	enum	not_started / in_progress / completed
best_score	numeric	
latest_score	numeric	
completed_at	timestamptz, null	
algorithm_version	varchar(50)	Version công thức tính điểm/mastery

Bảng: skill_progress  —  [chuẩn]
Tiến độ theo từng kỹ năng riêng biệt (9 kỹ năng).
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
skill_type	enum	vocab / grammar / listening / speaking / reading / writing / hanzi / pinyin / tone
mastery_score	numeric	

Bảng: streaks  —  [chuẩn]
Chuỗi ngày học liên tục.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
current_streak	int	
longest_streak	int	
last_active_date	date	
freeze_count	int	Số lần được 'đóng băng' streak

Bảng: srs_cards  —  [chuẩn]
Thẻ ôn tập ngắt quãng (Spaced Repetition).
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
word_id	BIGINT, null	FK → words.id (hoặc question_id tùy loại thẻ)
ease_factor	numeric	
interval_days	int	
due_date	date	
algorithm_version	varchar(50)	vd FSRS-v1
state	enum	new / learning / review / relearning

Bảng: srs_reviews  —  [partition theo quý]
Lịch sử mỗi lần ôn tập 1 thẻ SRS.
Cột	Kiểu dữ liệu	Ghi chú
card_id	BIGINT	FK → srs_cards.id
reviewed_at	timestamptz	
rating	int	Đánh giá độ khó của người học
previous_interval	int	
new_interval	int	

Bảng: learning_events  —  [immutable · partition theo tháng]
Toàn bộ sự kiện hành vi học tập (lesson_started, answer_submitted...), không chỉ lưu kết quả cuối.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
session_id	UUID	
event_name	varchar(100)	vd lesson_started, answer_submitted
entity_type	varchar(50)	
entity_id	BIGINT	
occurred_at	timestamptz	
metadata_json	JSONB	
app_version	varchar(20)	
device	varchar(50)	

7. MEDIA — Tệp tin, Ảnh, Audio, Video
Quản lý tài nguyên media và luồng upload an toàn.
Bảng: media_assets  —  [chuẩn]
Tài nguyên media đã xử lý xong.
Cột	Kiểu dữ liệu	Ghi chú
type	enum	image / audio / video / svg
storage_key	varchar(500)	
mime_type	varchar(100)	
size_bytes	bigint	
checksum	varchar(64)	
status	enum	pending_scan / clean / quarantined
uploaded_by	BIGINT	FK → users.id
related_entity_type	varchar(50)	
related_entity_id	BIGINT	
is_public	bool	

Bảng: uploads  —  [chuẩn]
Phiên upload tạm thời trước khi hoàn tất.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
purpose	enum	recording / avatar / import
status	enum	
storage_key	varchar(500)	
expires_at	timestamptz	

8. NOTIFICATION — Thông báo và Email
Thông báo trong app, email, và quản lý mẫu email có version.
Bảng: notifications  —  [partition theo tháng]
Thông báo gửi tới người dùng.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
type	varchar(50)	
title	varchar(255)	
body	text	
read_at	timestamptz, null	
sent_at	timestamptz	
channel	enum	in_app / email / push

Bảng: notification_preferences  —  [chuẩn]
Tùy chọn nhận thông báo theo loại.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
type	varchar(50)	
enabled	bool	
quiet_hours_start	time	
quiet_hours_end	time	

Bảng: email_logs  —  [log-only]
Nhật ký gửi email (theo dõi deliverability).
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
template_code	varchar(50)	
status	enum	queued / sent / bounced / complained
provider_message_id	varchar(255)	

Bảng: email_templates  —  [mới]
Mẫu email (định danh, không chứa nội dung — nội dung tách sang bảng version).
Cột	Kiểu dữ liệu	Ghi chú
code	varchar(50)	Duy nhất
subject	varchar(255)	
channel	varchar(50)	
category	varchar(50)	
status	enum	

Bảng: email_template_versions  —  [mới · immutable]
Nội dung cụ thể theo từng version, hỗ trợ preview/rollback không mất lịch sử.
Cột	Kiểu dữ liệu	Ghi chú
template_id	BIGINT	FK → email_templates.id
version_number	int	
body_html	text	
body_text	text	
is_active	bool	

9. GOVERNANCE / AUDIT — Nhật ký & Hàng đợi duyệt
Dùng chung cho mọi module.
Bảng: audit_logs  —  [immutable · partition theo tháng]
Nhật ký mọi hành động nhạy cảm (không sửa/xóa được).
Cột	Kiểu dữ liệu	Ghi chú
actor_id	BIGINT	
action	varchar(100)	
resource_type	varchar(50)	
resource_id	BIGINT	
before_json	JSONB	
after_json	JSONB	
ip	varchar(45)	
user_agent	text	
trace_id	varchar(64)	
occurred_at	timestamptz	

Bảng: review_queue  —  [chuẩn]
Hàng đợi nội dung chờ duyệt.
Cột	Kiểu dữ liệu	Ghi chú
content_type	varchar(50)	
content_id	BIGINT	
submitted_by	BIGINT	FK → users.id
assigned_to	BIGINT, null	FK → users.id
status	enum	
priority	enum	
sla_due_at	timestamptz	

10. PAYMENT & SUBSCRIPTION — Thanh toán, Gói học
Tách riêng module, không gắn logic thanh toán trực tiếp vào Course.
Bảng: plans  —  [chuẩn]
Gói học.
Cột	Kiểu dữ liệu	Ghi chú
code	varchar(50)	Duy nhất, vd PREMIUM_MONTHLY
name	varchar(100)	
price_amount	numeric(12,2)	
currency	char(3)	ISO 4217
billing_cycle	enum	monthly / yearly / lifetime
entitlements_json	JSONB	Quyền truy cập nội dung theo gói
status	enum	active / deprecated

Bảng: subscriptions  —  [chuẩn]
Đăng ký gói của người dùng.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
plan_id	BIGINT	FK → plans.id
status	enum	active / past_due / canceled / expired
current_period_start	timestamptz	
current_period_end	timestamptz	
cancel_at_period_end	bool	
canceled_at	timestamptz, null	

Bảng: orders  —  [chuẩn]
Đơn hàng mua gói.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
plan_id	BIGINT	FK → plans.id
coupon_id	BIGINT, null	FK → coupons.id
amount	numeric	
currency	char(3)	
status	enum	pending / paid / failed / refunded
idempotency_key	varchar(255)	Duy nhất — chống tạo trùng đơn

Bảng: invoices  —  [chuẩn]
Hóa đơn.
Cột	Kiểu dữ liệu	Ghi chú
order_id	BIGINT	FK → orders.id
invoice_number	varchar(50)	Duy nhất, tuần tự
issued_at	timestamptz	
pdf_storage_key	varchar(500)	
tax_amount	numeric	

Bảng: payment_transactions  —  [immutable]
Giao dịch thanh toán từ cổng thanh toán.
Cột	Kiểu dữ liệu	Ghi chú
order_id	BIGINT	FK → orders.id
provider	varchar(50)	vd VNPay / Momo / Stripe
provider_transaction_id	varchar(255)	Duy nhất
status	enum	
raw_response_json	JSONB	Restricted — field-level encryption bắt buộc
reconciled_at	timestamptz, null	
reconciliation_status	enum	matched / mismatched / pending

Bảng: coupons  —  [chuẩn]
Mã giảm giá.
Cột	Kiểu dữ liệu	Ghi chú
code	varchar(50)	Duy nhất
discount_type	enum	percent / fixed
discount_value	numeric	
max_redemptions	int	
redeemed_count	int	
valid_from	timestamptz	
valid_to	timestamptz	
applicable_plan_ids	JSONB	Mảng plan_id áp dụng

Bảng: refunds  —  [chuẩn]
Hoàn tiền.
Cột	Kiểu dữ liệu	Ghi chú
order_id	BIGINT	FK → orders.id
amount	numeric	
reason	text	
status	enum	requested / approved / processed / rejected
processed_by	BIGINT, null	FK → users.id
processed_at	timestamptz, null	

Bảng: payment_webhooks_log  —  [immutable · partition theo quý]
Log webhook thanh toán — event_id unique chống xử lý trùng.
Cột	Kiểu dữ liệu	Ghi chú
provider	varchar(50)	
event_id	varchar(255)	Duy nhất — chống xử lý trùng webhook
payload_json	JSONB	Restricted — field-level encryption bắt buộc
signature_verified	bool	
received_at	timestamptz	
processed_at	timestamptz, null	
processing_status	enum	

11. CLASSROOM — Lớp học, Giáo viên, Bài tập giao
Bảng: classes  —  [chuẩn]
Lớp học.
Cột	Kiểu dữ liệu	Ghi chú
teacher_id	BIGINT	FK → users.id
name	varchar(255)	
course_id	BIGINT, null	FK → courses.id — null nếu lớp tự do
status	enum	active / archived
join_code	varchar(8)	Duy nhất — mã mời tham gia

Bảng: class_members  —  [chuẩn]
Thành viên lớp.
Cột	Kiểu dữ liệu	Ghi chú
class_id	BIGINT	FK → classes.id
user_id	BIGINT	FK → users.id
role_in_class	enum	student / co_teacher
joined_at	timestamptz	
status	enum	active / removed

Bảng: class_assignments  —  [chuẩn]
Bài tập được giao cho lớp.
Cột	Kiểu dữ liệu	Ghi chú
class_id	BIGINT	FK → classes.id
lesson_id	BIGINT, null	FK → lessons.id
assigned_by	BIGINT	FK → users.id
due_at	timestamptz	
instructions	text	

Bảng: homework_submissions  —  [chuẩn]
Bài nộp của học viên cho bài tập được giao.
Cột	Kiểu dữ liệu	Ghi chú
assignment_id	BIGINT	FK → class_assignments.id
user_id	BIGINT	FK → users.id
attempt_id	BIGINT	FK → attempts.id
submitted_at	timestamptz	
teacher_score	numeric, null	Null nếu chấm tự động
teacher_feedback	text	
graded_by	BIGINT, null	FK → users.id
graded_at	timestamptz, null	

Bảng: teacher_class_permissions  —  [bảng nối]
Quyền giới hạn cho co-teacher.
Cột	Kiểu dữ liệu	Ghi chú
teacher_id	BIGINT	FK → users.id
class_id	BIGINT	FK → classes.id
can_grade	bool	
can_edit_assignment	bool	
can_remove_student	bool	

12. AI TUTOR / SPEECH — Trợ lý AI, Chấm phát âm
Bảng: ai_conversations  —  [chuẩn]
Hội thoại với AI Tutor.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
context_type	enum	tutor_chat / pronunciation
started_at	timestamptz	
ended_at	timestamptz, null	
status	enum	

Bảng: ai_messages  —  [partition theo tháng]
Tin nhắn trong hội thoại — nội dung Confidential, redact khi log ra ngoài.
Cột	Kiểu dữ liệu	Ghi chú
conversation_id	BIGINT	FK → ai_conversations.id
role	enum	user / assistant / system
content_text	text	Confidential — không log kèm PII thừa
moderation_flag	bool	
prompt_version	varchar(50)	
model_version	varchar(50)	

Bảng: ai_usage_logs  —  [log-only]
Chi phí và hiệu năng sử dụng AI.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
conversation_id	BIGINT, null	FK → ai_conversations.id
provider	varchar(50)	
model	varchar(50)	
input_tokens	int	
output_tokens	int	
cost_amount	numeric	
latency_ms	int	
request_id	varchar(100)	

Bảng: pronunciation_attempts  —  [chuẩn]
Kết quả chấm phát âm.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
word_id	BIGINT, null	FK → words.id (hoặc sentence_id)
recording_media_id	BIGINT	FK → media_assets.id
score	numeric	
phoneme_feedback_json	JSONB	
model_version	varchar(50)	

Bảng: moderation_logs  —  [chuẩn]
Log kiểm duyệt nội dung do AI hoặc người dùng tạo.
Cột	Kiểu dữ liệu	Ghi chú
source_type	enum	ai_message / comment / homework
source_id	BIGINT	
flagged_reason	text	
action_taken	enum	allowed / blocked / queued_review
reviewed_by	BIGINT, null	FK → users.id

13. HẠ TẦNG VẬN HÀNH (System-level)
Feature flag, cấu hình, Outbox/Inbox, idempotency, bảo mật vận hành.
Bảng: feature_flags  —  [chuẩn]
Cờ tính năng.
Cột	Kiểu dữ liệu	Ghi chú
key	varchar(100)	Duy nhất
description	text	
owner	varchar(100)	
environment	varchar(50)	
rollout_percentage	int	
target_user_segment_json	JSONB	
status	enum	active / scheduled_removal
expires_at	timestamptz, null	

Bảng: feature_flag_audit  —  [log-only]
Lịch sử thay đổi cờ tính năng.
Cột	Kiểu dữ liệu	Ghi chú
flag_id	BIGINT	FK → feature_flags.id
changed_by	BIGINT	
old_value_json	JSONB	
new_value_json	JSONB	
changed_at	timestamptz	

Bảng: app_settings  —  [chuẩn]
Cấu hình hệ thống — phân loại rõ, không gộp chung 1 bảng Settings.
Cột	Kiểu dữ liệu	Ghi chú
category	enum	static / runtime / feature_flag / business_rule / tenant
key	varchar(100)	Duy nhất trong category
value_json	JSONB	
value_type	varchar(20)	
is_secret_ref	bool	True → value chỉ chứa reference tới secret vault

Bảng: outbox_events  —  [P0 · engine đồng bộ]
Ghi sự kiện CÙNG transaction với nghiệp vụ chính, đảm bảo không mất event khi queue lỗi.
Cột	Kiểu dữ liệu	Ghi chú
aggregate_type	varchar(50)	vd Lesson, Order
aggregate_id	BIGINT	
event_type	varchar(100)	vd LessonCompleted, PaymentConfirmed
payload_json	JSONB	
event_schema_version	int	
status	enum	pending / processing / published / failed
retry_count	int, default 0	
max_retries	int, default 5	
next_retry_at	timestamptz	Tính theo exponential backoff
published_at	timestamptz, null	
trace_id	varchar(64)	

Bảng: inbox_events  —  [P0 · engine đồng bộ]
Chống consumer xử lý trùng 1 event (at-least-once delivery).
Cột	Kiểu dữ liệu	Ghi chú
event_id	UUID	= outbox_events.uuid — unique cùng consumer_name
consumer_name	varchar(100)	vd NotificationWorker
status	enum	received / processing / completed / failed
received_at	timestamptz	
processed_at	timestamptz, null	
error_message	text, null	

Bảng: idempotency_keys  —  [chuẩn · dùng chung mọi module]
Chống gửi trùng request cho các thao tác quan trọng.
Cột	Kiểu dữ liệu	Ghi chú
key	varchar(255)	Client cung cấp — unique theo (user_id, endpoint)
user_id	BIGINT, null	
endpoint	varchar(255)	vd POST /lessons/{id}/complete
request_hash	varchar(64)	SHA-256 body — phát hiện key trùng nhưng body khác
response_snapshot_json	JSONB	Trả lại y hệt response lần đầu
status	enum	in_progress / completed / failed
expires_at	timestamptz	TTL mặc định 24h

Bảng: security_events  —  [partition theo tháng]
Tách riêng khỏi audit_logs — sự kiện an ninh.
Cột	Kiểu dữ liệu	Ghi chú
event_type	enum	token_reuse / brute_force / privilege_escalation_attempt / impossible_travel / mass_scrape
actor_id	BIGINT, null	
ip	varchar(45)	
severity	enum	
detected_by	enum	rule_engine / manual
status	enum	open / investigating / resolved / false_positive

Bảng: rate_limit_counters  —  [tùy chọn]
Chỉ dùng nếu cần persist/audit rate-limit ngoài Redis.
Cột	Kiểu dữ liệu	Ghi chú
bucket_key	varchar(255)	vd login:ip:1.2.3.4
window_start	timestamptz	
count	int	
limit_value	int	

14. SUPPORT & COMPLIANCE — Hỗ trợ & Pháp lý
Bảng: support_tickets  —  [chuẩn]
Yêu cầu hỗ trợ người dùng.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
category	varchar(50)	
severity	enum	P1 / P2 / P3 / P4
status	enum	open / pending / resolved / closed
assigned_to	BIGINT, null	
sla_response_due_at	timestamptz	
sla_resolve_due_at	timestamptz	
trace_id_ref	varchar(64)	

Bảng: ticket_messages  —  [chuẩn]
Nội dung trao đổi trong 1 ticket.
Cột	Kiểu dữ liệu	Ghi chú
ticket_id	BIGINT	FK → support_tickets.id
sender_type	enum	user / agent
content	text	
attachments_json	JSONB	

Bảng: consent_records  —  [immutable]
Bằng chứng đồng ý của người dùng (AI, ghi âm, marketing...).
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
consent_type	enum	ai_usage / recording / marketing_email / data_processing
granted	bool	
version	int	
ip	varchar(45)	
granted_at	timestamptz	
revoked_at	timestamptz, null	

Bảng: data_export_requests  —  [chuẩn]
Yêu cầu xuất dữ liệu cá nhân.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
status	enum	queued / processing / ready / expired
file_storage_key	varchar(500)	
requested_at	timestamptz	
ready_at	timestamptz, null	
expires_at	timestamptz	

Bảng: data_deletion_requests  —  [chuẩn]
Yêu cầu xóa tài khoản.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
status	enum	queued / processing / completed
requested_at	timestamptz	
scheduled_purge_at	timestamptz	
completed_at	timestamptz, null	
verification_notes	text	

Bảng: legal_documents  —  [mới]
Loại văn bản pháp lý.
Cột	Kiểu dữ liệu	Ghi chú
type	enum	terms / privacy / cookie_policy
current_version_id	BIGINT	FK → legal_document_versions.id

Bảng: legal_document_versions  —  [mới · immutable]
Nội dung theo từng phiên bản văn bản pháp lý.
Cột	Kiểu dữ liệu	Ghi chú
document_id	BIGINT	FK → legal_documents.id
version_number	int	
content_markdown	text	
effective_from	timestamptz	
requires_reconsent	bool	True → buộc người dùng đồng ý lại

Bảng: help_categories  —  [mới]
Danh mục trung tâm trợ giúp.
Cột	Kiểu dữ liệu	Ghi chú
name	varchar(100)	
order_index	int	

Bảng: help_articles  —  [mới]
Bài viết trợ giúp.
Cột	Kiểu dữ liệu	Ghi chú
category_id	BIGINT	FK → help_categories.id
title	varchar(255)	
content_markdown	text	
status	enum	draft / published
view_count	int	

15. CONTENT QUALITY & DEPENDENCY
Bảng: content_quality_issues  —  [chuẩn]
Vấn đề chất lượng nội dung phát hiện tự động.
Cột	Kiểu dữ liệu	Ghi chú
entity_type	varchar(50)	
entity_id	BIGINT	
issue_type	enum	missing_pinyin / broken_audio / duplicate_content / invalid_hsk_mapping / orphan_reference...
severity	enum	
detected_at	timestamptz	
resolved_at	timestamptz, null	
resolved_by	BIGINT, null	

Bảng: content_dependency_links  —  [chuẩn]
Đồ thị phụ thuộc nội dung (prerequisite, references, media).
Cột	Kiểu dữ liệu	Ghi chú
from_entity_type	varchar(50)	
from_entity_id	BIGINT	
to_entity_type	varchar(50)	
to_entity_id	BIGINT	
dependency_type	enum	prerequisite / references / uses_media
is_valid	bool	Cập nhật bởi job kiểm tra định kỳ

16. CERTIFICATE & PLACEMENT — Chứng chỉ & Xếp lớp
Bảng: certificate_templates  —  [chuẩn]
Mẫu chứng chỉ.
Cột	Kiểu dữ liệu	Ghi chú
name	varchar(100)	
design_template_url	varchar(500)	
taxonomy_level_id	BIGINT	FK → taxonomy_levels.id

Bảng: certificates  —  [immutable]
Chứng chỉ đã cấp — không cấp trùng.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
template_id	BIGINT	FK → certificate_templates.id
course_id	BIGINT	FK → courses.id
issued_at	timestamptz	
certificate_number	varchar(50)	Duy nhất
revoked_at	timestamptz, null	
revoke_reason	text, null	
verification_code	varchar(50)	Duy nhất — tra cứu công khai

Bảng: placement_test_sessions  —  [chuẩn]
Phiên làm bài kiểm tra xếp lớp.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
started_at	timestamptz	
submitted_at	timestamptz, null	
status	enum	
question_snapshot_json	JSONB	

Bảng: placement_test_results  —  [chuẩn]
Kết quả và đề xuất cấp độ.
Cột	Kiểu dữ liệu	Ghi chú
session_id	BIGINT	FK → placement_test_sessions.id
recommended_level_taxonomy_id	BIGINT	FK → taxonomy_levels.id
score_breakdown_json	JSONB	
algorithm_version	varchar(50)	

17. REPORTING / ANALYTICS — Báo cáo tổng hợp
Bảng: daily_user_metrics  —  [materialized · log-only]
Chỉ số tổng hợp mỗi ngày, tính bởi Analytics Worker.
Cột	Kiểu dữ liệu	Ghi chú
metric_date	date	
dau	int	Daily Active Users
new_signups	int	
lessons_completed	int	
avg_session_minutes	numeric	
retention_d1	numeric	
retention_d7	numeric	

Bảng: cost_reports  —  [chuẩn]
Chi phí vận hành theo hạng mục.
Cột	Kiểu dữ liệu	Ghi chú
report_date	date	
category	enum	ai / media_bandwidth / storage / email
amount	numeric	
unit_count	int	
cost_per_active_user	numeric	

18. COMMENT & REVIEW — Bình luận & Đánh giá
Bảng: comments  —  [chuẩn]
Bình luận trên bài học/khóa học/bài tập.
Cột	Kiểu dữ liệu	Ghi chú
entity_type	enum	lesson / course / exercise
entity_id	BIGINT	
user_id	BIGINT	FK → users.id
content	text	
parent_comment_id	BIGINT, null	Bình luận trả lời (thread)
status	enum	visible / hidden / removed

Bảng: comment_reports  —  [chuẩn]
Báo cáo bình luận vi phạm.
Cột	Kiểu dữ liệu	Ghi chú
comment_id	BIGINT	FK → comments.id
reported_by	BIGINT	FK → users.id
reason	text	
status	enum	pending / reviewed / dismissed

Bảng: comment_moderation_logs  —  [log-only]
Lịch sử xử lý kiểm duyệt bình luận.
Cột	Kiểu dữ liệu	Ghi chú
comment_id	BIGINT	FK → comments.id
action	enum	hide / remove / restore
moderator_id	BIGINT	
reason	text	

Bảng: course_reviews  —  [chuẩn]
Đánh giá sao khóa học — 1 người 1 đánh giá/khóa.
Cột	Kiểu dữ liệu	Ghi chú
course_id	BIGINT	FK → courses.id
user_id	BIGINT	FK → users.id — unique cùng course_id
rating	int	1–5
review_text	text	
status	enum	visible / hidden

Bảng: review_helpful_votes  —  [chuẩn]
Vote hữu ích cho đánh giá.
Cột	Kiểu dữ liệu	Ghi chú
review_id	BIGINT	FK → course_reviews.id — unique cùng user_id
user_id	BIGINT	FK → users.id
is_helpful	bool	

19. GAMIFICATION — Huy hiệu, Bảng xếp hạng, Điểm thưởng
Bảng: badges  —  [chuẩn]
Danh mục huy hiệu.
Cột	Kiểu dữ liệu	Ghi chú
code	varchar(50)	Duy nhất
name	varchar(100)	
description	text	
icon_url	varchar(500)	
criteria_json	JSONB	Điều kiện đạt huy hiệu

Bảng: user_badges  —  [chuẩn]
Huy hiệu người dùng đã đạt.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id — unique cùng badge_id
badge_id	BIGINT	FK → badges.id
earned_at	timestamptz	

Bảng: leaderboards  —  [chuẩn]
Kỳ bảng xếp hạng.
Cột	Kiểu dữ liệu	Ghi chú
type	enum	weekly / monthly / all_time
scope	enum	global / class
period_start	date	
period_end	date	

Bảng: leaderboard_entries  —  [chuẩn]
Xếp hạng — tính sẵn bởi job định kỳ, KHÔNG realtime.
Cột	Kiểu dữ liệu	Ghi chú
leaderboard_id	BIGINT	FK → leaderboards.id
user_id	BIGINT	FK → users.id
score	numeric	
rank	int	

Bảng: points_ledger  —  [immutable]
Sổ cái điểm thưởng — chống cộng trùng.
Cột	Kiểu dữ liệu	Ghi chú
user_id	BIGINT	FK → users.id
points_delta	int	
reason_type	enum	lesson_complete / streak / badge
reference_id	BIGINT	

20. IMPORT / EXPORT JOB TRACKING
Bảng: import_jobs  —  [chuẩn]
Trạng thái job import dữ liệu hàng loạt (Excel...).
Cột	Kiểu dữ liệu	Ghi chú
type	enum	dictionary / course / user
file_storage_key	varchar(500)	
status	enum	queued / processing / completed / failed
total_rows	int	
success_rows	int	
error_rows	int	
error_log_storage_key	varchar(500)	
started_at	timestamptz, null	
completed_at	timestamptz, null	

Bảng: export_jobs  —  [chuẩn]
Trạng thái job xuất dữ liệu/báo cáo.
Cột	Kiểu dữ liệu	Ghi chú
type	enum	user_data / report / cost
requested_by	BIGINT	FK → users.id
status	enum	
file_storage_key	varchar(500)	
expires_at	timestamptz	

21. API KEY B2B
Bảng: api_keys  —  [chuẩn]
Khóa API cho tích hợp máy–máy. Giá trị thật chỉ hiển thị 1 lần lúc tạo.
Cột	Kiểu dữ liệu	Ghi chú
name	varchar(100)	
key_hash	varchar(255)	Restricted — chỉ lưu hash
owner_org	varchar(255)	
status	enum	active / revoked
expires_at	timestamptz, null	
last_used_at	timestamptz, null	

Bảng: api_key_scopes  —  [bảng nối]
Phạm vi quyền của 1 API key.
Cột	Kiểu dữ liệu	Ghi chú
api_key_id	BIGINT	FK → api_keys.id
scope_code	varchar(100)	vd dictionary.read, progress.read

 
PHỤ LỤC — TRANSACTIONAL OUTBOX / INBOX (P0)
Vấn đề: khi hoàn thành bài học cần INSERT progress + INSERT srs_cards + publish event LessonCompleted một cách ATOMIC. Nếu DB commit thành công nhưng publish message queue thất bại, dữ liệu có thể mất đồng bộ vĩnh viễn. Outbox pattern giải quyết bằng cách ghi event vào CÙNG transaction DB thay vì publish trực tiếp ra queue.
Luồng xử lý Outbox Processor
1. Ghi sự kiện: trong cùng transaction với nghiệp vụ chính, thêm dòng vào outbox_events với status=pending. Commit — dữ liệu nghiệp vụ và event đã an toàn cùng lúc.
2. Polling: Outbox Processor (worker riêng, queue critical) quét outbox_events WHERE status='pending' AND next_retry_at <= now() ORDER BY created_at LIMIT N FOR UPDATE SKIP LOCKED.
3. Publish: gửi payload lên Message Queue, partition key = aggregate_id để đảm bảo thứ tự theo từng entity.
4. Xác nhận: queue ACK thành công → UPDATE status='published'. Lỗi → retry_count += 1, tính next_retry_at theo exponential backoff (2^retry_count giây, cap 5 phút).
5. Dead-letter: nếu retry_count >= max_retries → status='failed', cảnh báo, chuyển sang xử lý thủ công, KHÔNG xóa để giữ khả năng replay.
6. Cleanup: job định kỳ archive/xóa event status='published' cũ hơn retention policy (vd 30 ngày).

Chống xử lý trùng phía Consumer (Inbox)
Consumer (vd NotificationWorker, AnalyticsWorker) insert vào inbox_events (event_id, consumer_name) TRƯỚC khi xử lý. Unique constraint trên (event_id, consumer_name) đảm bảo: nếu message queue redeliver (at-least-once delivery), insert vi phạm unique → bỏ qua, không xử lý lại. Đây chính là cơ chế chống cộng streak/gửi notification 2 lần.

Governance
• Event versioning: khi đổi cấu trúc data, tăng schemaVersion; consumer phải hỗ trợ đọc ít nhất 2 version gần nhất.
• Retention: outbox_events đã publish giữ tối thiểu 7 ngày để replay; inbox_events giữ tương đương để duy trì dedup.
• Monitoring: pending quá lâu (>5 phút) → alert; tỷ lệ failed tăng đột biến → alert.
• Không dùng Outbox cho các thao tác không cần đảm bảo tuyệt đối (vd cập nhật UI realtime không quan trọng).


