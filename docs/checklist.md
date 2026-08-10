Checklist tổng thể khi đưa web học tiếng Trung lên Production
Để người dùng có trải nghiệm tốt, hệ thống không chỉ cần Frontend chạy đẹp và Backend trả API được. Cần hoàn thiện đồng thời 6 khối:
Frontend
Backend
Dữ liệu & nội dung học
Hạ tầng & triển khai
Giám sát & phục hồi
Vận hành & pháp lý
1. Backend cần thực hiện
1.1. API và nghiệp vụ
•	Thiết kế REST API thống nhất. 
•	API versioning: /api/v1. 
•	Pagination, filtering, sorting, searching. 
•	Chuẩn request và response. 
•	Chuẩn lỗi bằng ProblemDetails. 
•	Validation dữ liệu đầu vào. 
•	Idempotency, chống gửi trùng request. 
•	Transaction cho nghiệp vụ nhiều bước. 
•	Xử lý concurrency và xung đột dữ liệu. 
•	Swagger/OpenAPI. 
•	Kiểm soát tương thích khi nâng cấp API. 
Các nghiệp vụ chính của web học tiếng Trung:
•	Đăng ký, đăng nhập và onboarding. 
•	Lộ trình, khóa học, chương và bài học. 
•	Từ vựng, chữ Hán, bộ thủ và ngữ pháp. 
•	Bài tập, bài kiểm tra và chấm điểm. 
•	Tiến độ học tập. 
•	Streak và mục tiêu hằng ngày. 
•	SRS và lịch ôn tập. 
•	Luyện nghe, nói, đọc và viết. 
•	Quản lý lớp học, giáo viên và bài tập. 
•	Báo cáo và thống kê. 
•	AI Tutor nếu triển khai. 
1.2. Database và Storage
•	Thiết kế bảng và quan hệ dữ liệu đúng. 
•	Primary key nội bộ và UUID công khai. 
•	Foreign key, unique constraint và check constraint. 
•	Index cho truy vấn thường xuyên. 
•	Transaction và optimistic concurrency. 
•	Soft delete và audit fields. 
•	Migration có version. 
•	Seed dữ liệu hệ thống. 
•	Connection pooling. 
•	Slow-query monitoring. 
•	Backup tự động. 
•	Kiểm tra khôi phục backup thực tế. 
Storage cần phân chia:
•	Ảnh. 
•	Audio phát âm. 
•	Video bài học. 
•	SVG thứ tự nét. 
•	Phụ đề. 
•	File Excel import. 
•	Bản ghi âm của người học. 
Phải có:
•	Public/private bucket. 
•	Signed URL. 
•	Kiểm tra MIME và magic number. 
•	Giới hạn dung lượng. 
•	Quét file độc hại. 
•	Nén và chuyển đổi media. 
•	Chính sách lưu trữ và tự động xóa file tạm. 
1.3. Authentication và Permissions
•	Đăng ký và xác minh email. 
•	Đăng nhập, đăng xuất. 
•	Quên và đặt lại mật khẩu. 
•	Access token. 
•	Refresh-token rotation. 
•	Revoke token. 
•	Quản lý session và thiết bị. 
•	Khóa tài khoản khi đăng nhập sai nhiều lần. 
•	OAuth Google với PKCE. 
•	MFA cho tài khoản quản trị. 
•	Đăng xuất toàn bộ thiết bị. 
•	Yêu cầu xác thực lại với hành động nhạy cảm. 
Phân quyền nên theo cả vai trò và quyền:
Learner
Teacher
ContentEditor
Reviewer
Publisher
Analyst
Admin
SuperAdmin
Ví dụ quyền:
content.create
content.edit
content.review
content.publish
user.manage
report.view
Backend phải kiểm tra quyền thật sự. Frontend chỉ có nhiệm vụ ẩn hoặc hiện giao diện.
1.4. Security
•	RLS ở database nếu phù hợp. 
•	Kiểm tra quyền tại Application/API layer. 
•	Chống SQL Injection. 
•	Chống XSS. 
•	Chống CSRF. 
•	CORS whitelist. 
•	CSP và security headers. 
•	HSTS và HTTPS. 
•	Chống clickjacking. 
•	Chống path traversal. 
•	Chống SSRF. 
•	Sanitization nội dung HTML. 
•	Unicode NFC normalization. 
•	Quản lý secret bằng môi trường hoặc secret vault. 
•	Không ghi mật khẩu và token vào log. 
•	Quét lỗ hổng dependency và Docker image. 
•	Quản lý quyền truy cập storage. 
•	Chống bot, spam, scraping và credential stuffing. 
1.5. Rate Limiting và chống lạm dụng
Nên có chính sách riêng:
Đăng nhập: 5 lần/phút/IP
Tìm kiếm: 60 lần/phút/người dùng
Admin write: 100 lần/phút
AI và speech: giới hạn theo quota
Upload: giới hạn dung lượng và tần suất
Cần bổ sung:
•	Retry-After. 
•	Quota theo tài khoản. 
•	Giới hạn theo IP và user. 
•	CAPTCHA khi phát hiện bất thường. 
•	Chống tạo tài khoản hàng loạt. 
•	Chống phát audio/video hotlink. 
•	Giới hạn AI token và chi phí theo người dùng. 
1.6. Cache, CDN và Search
Cache
•	Redis distributed cache. 
•	Cache từ điển và khóa học công khai. 
•	Cache permission và cấu hình. 
•	Cache invalidation khi cập nhật nội dung. 
•	TTL rõ ràng. 
•	Distributed lock khi cần. 
•	Theo dõi cache hit/miss. 
CDN
•	Phân phối ảnh, audio, video và SVG. 
•	Cache-control. 
•	Version hoặc hash tên file. 
•	Image optimization. 
•	Video streaming nếu nội dung lớn. 
•	Chống hotlink. 
•	Signed CDN URL cho media riêng tư. 
Search
Tìm kiếm cần hỗ trợ:
•	Chữ Hán. 
•	Pinyin có dấu và không dấu. 
•	Tiếng Việt. 
•	Âm Hán Việt. 
•	Giản thể và phồn thể. 
•	HSK. 
•	Bộ thủ. 
•	Số nét. 
•	Tìm gần đúng và sửa lỗi chính tả. 
1.7. Queue và Background Jobs
Các tác vụ nên chạy nền:
•	Import Excel. 
•	Gửi email. 
•	Gửi notification. 
•	Chuyển đổi audio/video. 
•	Tạo thumbnail. 
•	Phân tích phát âm. 
•	Tạo báo cáo. 
•	Tính lại tiến độ. 
•	Tạo lịch SRS. 
•	Đồng bộ analytics. 
•	Dọn file và token hết hạn. 
Hệ thống job cần:
•	Retry có backoff. 
•	Timeout. 
•	Idempotency. 
•	Job cancellation. 
•	Dead-letter queue. 
•	Dashboard theo dõi. 
•	Cảnh báo khi job thất bại. 
1.8. Logging, Metrics và Tracing
Không chỉ ghi log lỗi. Production cần:
Logs
•	Application log. 
•	Request log. 
•	Security log. 
•	Audit log. 
•	Background-job log. 
Metrics
•	Request rate. 
•	Error rate. 
•	Response time. 
•	CPU và RAM. 
•	Database connection. 
•	Redis cache hit rate. 
•	Queue depth. 
•	Số người dùng đang hoạt động. 
•	Tỷ lệ hoàn thành bài học. 
•	Chi phí AI và media. 
Tracing
Theo dõi request xuyên suốt:
Frontend → API → Database → Redis → Storage → AI
Mỗi request nên có:
•	Request ID. 
•	Correlation ID. 
•	Trace ID. 
1.9. Availability, Scaling và Recovery
•	Stateless API để dễ scale ngang. 
•	Load balancer. 
•	Health checks: liveness và readiness. 
•	Autoscaling khi lưu lượng tăng. 
•	Database connection pooling. 
•	Read replica khi báo cáo lớn. 
•	Queue để giảm tải xử lý đồng thời. 
•	Timeout và retry có kiểm soát. 
•	Circuit breaker với dịch vụ ngoài. 
•	Graceful degradation. 
Ví dụ AI bị lỗi thì phần học cơ bản vẫn phải hoạt động.
Cần xác định:
•	RPO: chấp nhận mất tối đa bao nhiêu dữ liệu. 
•	RTO: chấp nhận hệ thống ngừng tối đa bao lâu. 
•	Backup database và storage. 
•	Restore test định kỳ. 
•	Runbook xử lý sự cố. 
•	Kịch bản database, Redis, CDN hoặc storage bị lỗi. 
2. Frontend cần thực hiện
2.1. UI và Design System
•	Thiết kế đồng nhất. 
•	Responsive cho desktop, tablet và mobile. 
•	Component dùng chung. 
•	Typography, màu sắc và khoảng cách chuẩn. 
•	Button, input, modal, table, toast. 
•	Loading state. 
•	Skeleton. 
•	Empty state. 
•	Error state. 
•	Success state. 
•	Dark mode nếu cần. 
•	Giao diện phù hợp với người học. 
Các màn hình quan trọng:
•	Đăng ký và đăng nhập. 
•	Onboarding. 
•	Lộ trình học. 
•	Trang bài học. 
•	Từ điển. 
•	Bài tập. 
•	SRS. 
•	Tiến độ. 
•	Hồ sơ. 
•	Trang quản trị. 
2.2. Routing và Layout
•	Public routes. 
•	Auth routes. 
•	Protected routes. 
•	Admin routes. 
•	Role/permission guard. 
•	Nested layout. 
•	Trang 404. 
•	Trang 403. 
•	Trang lỗi 500. 
•	Redirect sau đăng nhập. 
•	Giữ URL quay lại sau khi xác thực. 
•	SEO-friendly URL cho nội dung công khai. 
2.3. State Management
Phân chia rõ:
Local state
•	Modal. 
•	Tab. 
•	Giá trị UI tạm thời. 
Global state
•	Người dùng hiện tại. 
•	Ngôn ngữ. 
•	Theme. 
•	Permission. 
•	Trạng thái menu. 
Server state
•	Khóa học. 
•	Bài học. 
•	Từ vựng. 
•	Tiến độ. 
•	Notification. 
Nên có:
•	Cache invalidation. 
•	Optimistic update. 
•	Refetch. 
•	Retry. 
•	Deduplicate request. 
•	Conflict resolution. 
•	Tránh lưu cùng một dữ liệu ở nhiều nơi. 
2.4. Data Layer
•	API client dùng chung. 
•	Base URL theo environment. 
•	Timeout. 
•	Abort request. 
•	Tự động gắn access token. 
•	Refresh token an toàn. 
•	Chuẩn hóa response và error. 
•	Retry có điều kiện. 
•	Pagination. 
•	Infinite scroll. 
•	Upload có progress. 
•	Download và signed URL. 
•	Không gọi API trực tiếp rải rác trong component. 
Luồng nên là:
UI → Feature Hook/Service → API Client → Backend
2.5. Business Logic phía Frontend
Frontend chỉ giữ logic phục vụ trải nghiệm:
•	Điều khiển trình tự bài học. 
•	Hiển thị câu hỏi. 
•	Ghi nhận câu trả lời tạm. 
•	Phát và lặp audio. 
•	Countdown. 
•	Canvas luyện viết. 
•	Hiển thị tiến độ. 
•	Autosave. 
•	Khôi phục bài đang học. 
Các luật quan trọng như chấm điểm, mở khóa bài học và cấp chứng chỉ vẫn phải do Backend quyết định.
2.6. Auth phía Frontend
•	Form đăng nhập. 
•	Quản lý trạng thái session. 
•	Tự động refresh session. 
•	Route guard. 
•	Permission-based UI. 
•	Xử lý session hết hạn. 
•	Đăng xuất an toàn. 
•	Không để lộ token qua URL. 
•	Không lưu dữ liệu nhạy cảm trong LocalStorage. 
•	Xử lý nhiều tab trình duyệt. 
•	Hiển thị thông báo khi tài khoản bị khóa hoặc thu hồi session. 
2.7. Validation và Form UX
•	Validation phía client. 
•	Validation phía server. 
•	Hiển thị lỗi đúng trường. 
•	Giữ dữ liệu khi request lỗi. 
•	Dirty-state warning. 
•	Autosave bản nháp. 
•	Chống bấm submit nhiều lần. 
•	Loading khi gửi. 
•	Form hỗ trợ bàn phím. 
•	Thông báo rõ cách sửa lỗi. 
•	Upload có giới hạn loại và dung lượng file. 
2.8. Caching và Offline
•	Cache dữ liệu server. 
•	Cache asset bằng CDN. 
•	Browser cache. 
•	Service Worker nếu cần. 
•	IndexedDB cho dữ liệu offline. 
•	Lưu nháp bài tập. 
•	Khôi phục khi mất mạng. 
•	Queue thao tác chờ đồng bộ. 
•	Phát hiện online/offline. 
•	Xử lý xung đột dữ liệu. 
Điều quan trọng với web học:
Người dùng không được mất toàn bộ bài đang làm khi mạng bị ngắt.
2.9. Error Handling
•	Error boundary. 
•	Trang lỗi toàn cục. 
•	Lỗi theo component. 
•	Retry action. 
•	Fallback UI. 
•	Thông báo mất mạng. 
•	Xử lý timeout. 
•	Xử lý 401, 403, 404, 409, 422, 429 và 500. 
•	Gắn trace ID khi báo lỗi cho support. 
•	Không hiển thị stack trace hoặc thông tin nội bộ. 
2.10. Performance
•	Server rendering cho nội dung cần SEO. 
•	Lazy loading. 
•	Code splitting. 
•	Dynamic import. 
•	Image optimization. 
•	Font optimization. 
•	Preload audio cần thiết. 
•	Không tải toàn bộ khóa học cùng lúc. 
•	Pagination hoặc virtual list. 
•	Debounce tìm kiếm. 
•	Giảm re-render. 
•	Giảm JavaScript bundle. 
•	Theo dõi Core Web Vitals. 
Mục tiêu trải nghiệm:
•	Trang chính hiển thị nhanh. 
•	Chuyển bài học mượt. 
•	Audio phát gần như ngay lập tức. 
•	Không giật khi viết trên canvas. 
•	Không khóa giao diện khi upload hoặc chấm bài. 
2.11. Accessibility
•	Semantic HTML. 
•	Keyboard navigation. 
•	Focus management. 
•	ARIA. 
•	Độ tương phản. 
•	Font có thể phóng to. 
•	Reduced motion. 
•	Alt text. 
•	Caption và transcript. 
•	Điều khiển audio bằng bàn phím. 
•	Không truyền đạt thông tin chỉ bằng màu sắc. 
2.12. SEO và chia sẻ
Áp dụng cho từ điển, khóa học và bài viết công khai:
•	Metadata. 
•	Canonical URL. 
•	Sitemap. 
•	Robots.txt. 
•	Open Graph. 
•	Structured data. 
•	Breadcrumb. 
•	SSR/ISR. 
•	URL sạch. 
•	Trang không tồn tại trả đúng 404. 
2.13. Testing
Frontend cần:
•	Unit test. 
•	Component test. 
•	Integration test. 
•	E2E test. 
•	Accessibility test. 
•	Visual regression test. 
•	Browser compatibility test. 
•	Mobile test. 
•	Network-slow/offline test. 
•	Audio, microphone và canvas test. 
Các luồng quan trọng phải có E2E:
Đăng ký
Đăng nhập
Bắt đầu bài học
Nộp bài
Lưu tiến độ
Ôn tập SRS
Thanh toán nếu có
Quản trị và xuất bản nội dung
2.14. Monitoring và Analytics
Theo dõi kỹ thuật:
•	JavaScript error. 
•	Failed API request. 
•	Page load. 
•	Web Vitals. 
•	Browser và device. 
•	Phiên bản frontend. 
•	Release gây lỗi. 
Theo dõi sản phẩm:
•	Hoàn thành đăng ký. 
•	Hoàn thành onboarding. 
•	Bắt đầu và hoàn thành bài học. 
•	Vị trí người dùng bỏ dở. 
•	Câu hỏi sai nhiều. 
•	Từ khóa không tìm thấy. 
•	Tỷ lệ quay lại. 
•	Streak. 
•	Thời gian học. 
•	Audio play và pronunciation attempt. 
3. Nội dung và dữ liệu học tiếng Trung
Đây là phân hệ bắt buộc, không thuộc riêng Frontend hay Backend.
3.1. CMS và workflow nội dung
Draft → Review → Approved → Published → Archived
Cần có:
•	Người tạo. 
•	Người sửa. 
•	Người duyệt. 
•	Người xuất bản. 
•	Version history. 
•	Compare version. 
•	Rollback. 
•	Preview. 
•	Scheduled publishing. 
•	Audit log. 
3.2. Chuẩn hóa dữ liệu
•	Giản thể và phồn thể. 
•	Pinyin có dấu. 
•	Pinyin dạng số. 
•	Âm Hán Việt. 
•	Loại từ. 
•	Nghĩa tiếng Việt. 
•	Câu ví dụ. 
•	Audio. 
•	Bộ thủ. 
•	Số nét. 
•	Stroke order. 
•	HSK version. 
•	Nguồn và giấy phép sử dụng. 
3.3. Learning Engine
•	Điều kiện mở bài. 
•	Chấm điểm. 
•	Mastery theo kỹ năng. 
•	Placement test. 
•	Adaptive learning. 
•	Daily goal. 
•	Streak. 
•	Gợi ý bài tiếp theo. 
•	Xác định điểm yếu. 
•	SRS bằng SM-2 hoặc FSRS. 
•	Version thuật toán. 
Không nên chỉ lưu một con số tiến độ tổng. Nên theo dõi riêng:
Từ vựng
Ngữ pháp
Nghe
Nói
Đọc
Viết
Chữ Hán
Pinyin
Thanh điệu
4. DevOps và quy trình phát hành
•	Local, development, testing, staging và production riêng biệt. 
•	Git branching và pull request. 
•	Code review. 
•	CI/CD. 
•	Lint và type check. 
•	Unit và integration test. 
•	Security scan. 
•	Docker build. 
•	Deploy staging. 
•	Smoke test. 
•	Deploy production. 
•	Database migration an toàn. 
•	Rollback ứng dụng và database. 
•	Blue-green, rolling hoặc canary deployment. 
•	Feature flags. 
•	Infrastructure as Code. 
•	Secret management. 
•	Domain, DNS, SSL và CDN. 
•	Không sửa trực tiếp production thủ công nếu có thể tránh. 
5. Vận hành, pháp lý và hỗ trợ
Vận hành
•	Admin dashboard. 
•	Quản lý người dùng. 
•	Xem và hủy session. 
•	Xem lỗi import. 
•	Retry background job. 
•	Quản lý nội dung. 
•	Audit log. 
•	Feature flags. 
•	System settings. 
•	Maintenance mode. 
•	Incident runbook. 
•	Status page. 
Hỗ trợ người dùng
•	Feedback. 
•	Bug report. 
•	Help center. 
•	FAQ. 
•	Contact form. 
•	Ticket support. 
•	Tra cứu lỗi theo trace ID. 
•	Công cụ xem tiến độ người dùng khi được phép. 
Pháp lý và riêng tư
•	Điều khoản sử dụng. 
•	Chính sách riêng tư. 
•	Cookie consent. 
•	Quyền xóa tài khoản. 
•	Xuất dữ liệu cá nhân. 
•	Data retention. 
•	Bản quyền nội dung. 
•	Giấy phép audio, hình ảnh và giáo trình. 
•	Quy định xử lý bản ghi âm. 
•	Đồng ý trước khi dùng dữ liệu cho AI. 
•	Unsubscribe email. 
6. Thứ tự ưu tiên triển khai
P0 — Bắt buộc trước khi mở cho người dùng
•	Auth và permission. 
•	API và database ổn định. 
•	Nội dung học đã được kiểm duyệt. 
•	Validation. 
•	Security cơ bản. 
•	Backup và restore. 
•	Logging và error tracking. 
•	Responsive. 
•	Error handling. 
•	Core E2E tests. 
•	Monitoring và alert. 
•	CMS cơ bản. 
•	Tiến độ học và chấm điểm chính xác. 
•	HTTPS, secret management và rate limiting. 
P1 — Cần có để trải nghiệm tốt
•	Redis cache và CDN. 
•	SRS. 
•	Offline draft. 
•	Search tiếng Trung nâng cao. 
•	Notification. 
•	Analytics. 
•	Accessibility. 
•	Media optimization. 
•	Audit log đầy đủ. 
•	Feature flags. 
•	Load testing. 
•	Công cụ hỗ trợ vận hành. 
P2 — Mở rộng khi có người dùng và dữ liệu thật
•	Autoscaling nâng cao. 
•	Read replica. 
•	Search engine riêng. 
•	Adaptive learning. 
•	AI Tutor. 
•	Pronunciation scoring. 
•	Multi-tenancy. 
•	A/B testing. 
•	Multi-region. 
•	Recommendation engine. 
7. Tiêu chí production-ready cuối cùng
Một web học tiếng Trung chỉ nên mở rộng người dùng khi đáp ứng được:
Đúng nội dung
Đúng nghiệp vụ
Nhanh và dễ sử dụng
Không mất tiến độ
An toàn dữ liệu
Có thể giám sát
Có thể khôi phục
Có thể cập nhật an toàn
Có công cụ vận hành
Có quy trình hỗ trợ người dùng
Tóm gọn:
Frontend:
UI + Routing + State + Data + Auth + Forms
+ Offline + Performance + Accessibility
+ Error Handling + Testing + Monitoring

Backend:
API + Domain Logic + Database + Storage
+ Auth + Authorization + Security + Search
+ Cache + Queue + Media + Observability
+ Scaling + Backup + Recovery

Production:
Content Governance + DevOps + Analytics
+ Operations + Compliance + Cost Control
Checklist hiện tại đã đạt khoảng 85–90% mức production-ready cho một website học tiếng Trung. Các khối Frontend, Backend, bảo mật, dữ liệu học, DevOps, monitoring và phục hồi đã khá đầy đủ.
Tuy nhiên, để vận hành với người dùng thật lâu dài, nên bổ sung hoặc làm rõ thêm các phần sau.
1. Kiến trúc và quản lý yêu cầu
Trước khi phát triển cần có:
•	Product requirements. 
•	Business requirements. 
•	Functional và non-functional requirements. 
•	User roles và user journeys. 
•	Use case, acceptance criteria. 
•	Data flow và sequence diagram. 
•	Architecture Decision Record. 
•	Quy ước code và cấu trúc module. 
•	Definition of Done. 
•	Ma trận traceability từ yêu cầu đến API, giao diện và test case. 
Mục đích là tránh tình trạng có nhiều API và màn hình nhưng nghiệp vụ thực tế chưa hoàn chỉnh.
2. Quản lý taxonomy và phiên bản học thuật
Website học tiếng Trung cần một phân hệ chuẩn học thuật riêng:
•	HSK 2.0, HSK 3.0 hoặc chuẩn nội bộ. 
•	CEFR và chuẩn tương đương. 
•	Cấp độ khóa học. 
•	Chủ đề. 
•	Kỹ năng. 
•	Learning outcomes. 
•	Mapping từ vựng, ngữ pháp và chữ Hán vào từng chuẩn. 
•	Version của taxonomy. 
•	Ngày hiệu lực. 
•	Quy trình chuyển dữ liệu khi thay đổi chuẩn. 
Không nên chỉ lưu một trường HSKLevel, vì một từ có thể thuộc những phiên bản chuẩn khác nhau.
3. Content provenance và bản quyền
Mỗi nội dung nên có:
•	Nguồn dữ liệu. 
•	Tác giả hoặc đơn vị cung cấp. 
•	Loại giấy phép. 
•	Quyền sử dụng thương mại. 
•	Ngày nhập. 
•	Người kiểm duyệt. 
•	Phiên bản nội dung. 
•	File hoặc đường dẫn chứng minh nguồn. 
•	Trạng thái bản quyền. 
•	Ngày hết hạn giấy phép nếu có. 
Áp dụng cho:
•	Từ vựng. 
•	Câu ví dụ. 
•	Bài đọc. 
•	Audio. 
•	Hình ảnh. 
•	Video. 
•	SVG thứ tự nét. 
•	Giáo trình. 
4. Chất lượng nội dung tự động
Ngoài kiểm duyệt thủ công, nên xây dựng bộ kiểm tra tự động:
•	Từ thiếu pinyin. 
•	Pinyin sai định dạng. 
•	Thiếu nghĩa tiếng Việt. 
•	Thiếu loại từ. 
•	Audio không tồn tại. 
•	Audio không khớp nội dung. 
•	Link media bị hỏng. 
•	Câu hỏi không có đáp án đúng. 
•	Câu hỏi có nhiều đáp án đúng ngoài dự kiến. 
•	Bài học tham chiếu dữ liệu đã xóa. 
•	Quan hệ prerequisite bị vòng lặp. 
•	Nội dung trùng lặp. 
•	Ký tự giản thể/phồn thể không nhất quán. 
•	HSK mapping không hợp lệ. 
•	Bài học đã xuất bản nhưng còn nội dung nháp. 
Nên có dashboard hiển thị:
Content Health Score
Broken Media
Missing Fields
Duplicate Content
Unreviewed Changes
Invalid Dependencies
5. Learning event và dữ liệu hành vi
Không nên chỉ lưu kết quả cuối cùng. Cần hệ thống learning events:
lesson_started
lesson_paused
lesson_completed
question_viewed
answer_selected
answer_submitted
answer_corrected
audio_played
hint_opened
word_saved
review_completed
pronunciation_recorded
Mỗi event nên có:
•	User UUID. 
•	Session UUID. 
•	Course, lesson và question UUID. 
•	Timestamp. 
•	Device. 
•	App version. 
•	Thời gian phản hồi. 
•	Kết quả. 
•	Metadata cần thiết. 
Dữ liệu này phục vụ:
•	Phân tích điểm người học gặp khó khăn. 
•	Cải thiện bài học. 
•	Phát hiện câu hỏi lỗi. 
•	Gợi ý nội dung. 
•	Tính tiến độ chính xác. 
•	Phân tích retention. 
6. Quy tắc tính tiến độ
Cần định nghĩa rõ:
•	Khi nào bài học được xem là hoàn thành. 
•	Xem video bao nhiêu phần trăm là đủ. 
•	Điểm tối thiểu để qua bài. 
•	Có bắt buộc hoàn thành mọi bài tập không. 
•	Làm lại có thay thế điểm cũ không. 
•	Điểm cao nhất hay điểm gần nhất được sử dụng. 
•	Cách tính mastery. 
•	Cách tính thời gian học. 
•	Cách xử lý học offline. 
•	Cách xử lý người dùng học trên nhiều thiết bị. 
•	Khi thay đổi nội dung, tiến độ cũ còn hợp lệ không. 
Đây phải là quy tắc backend có version, không tính tùy ý phía frontend.
7. Version hóa Learning Engine
Các thuật toán sau cần có version:
•	Chấm điểm. 
•	Mastery. 
•	SRS. 
•	Placement test. 
•	Recommendation. 
•	Streak. 
•	Chứng chỉ. 
•	Pronunciation scoring. 
Ví dụ:
SRS Algorithm: FSRS-v1
Scoring Rule: LessonScore-v2
Mastery Model: SkillMastery-v3
Nhờ đó có thể biết kết quả của người dùng được tạo bởi thuật toán nào và tránh thay đổi công thức làm sai dữ liệu cũ.
8. Đồng bộ đa thiết bị và offline
Cần định nghĩa kỹ:
•	Client-generated ID. 
•	Idempotency key. 
•	Phiên bản bản ghi. 
•	Thời điểm cập nhật. 
•	Conflict resolution. 
•	Thứ tự đồng bộ event. 
•	Chống submit trùng. 
•	Đồng bộ khi thiết bị quay lại online. 
•	Hạn lưu dữ liệu offline. 
•	Mã hóa dữ liệu offline nhạy cảm. 
Ví dụ người dùng làm cùng một bài trên điện thoại và máy tính thì hệ thống phải biết:
•	Gộp kết quả. 
•	Chọn phiên mới nhất. 
•	Giữ kết quả cao nhất. 
•	Hay yêu cầu người dùng chọn. 
9. Compatibility và quản lý phiên bản frontend
Khi deploy frontend mới nhưng người dùng còn mở tab phiên bản cũ, có thể xảy ra lỗi API hoặc schema.
Cần có:
•	Build version. 
•	API compatibility matrix. 
•	Client-version header. 
•	Phát hiện phiên bản quá cũ. 
•	Yêu cầu reload ứng dụng. 
•	Cache busting. 
•	Service Worker update strategy. 
•	Không xóa API cũ ngay khi deploy. 
•	Grace period cho client cũ. 
10. Browser và thiết bị thực tế
Cần xác định ma trận hỗ trợ chính thức:
•	Chrome Windows. 
•	Edge Windows. 
•	Safari macOS. 
•	Safari iPhone/iPad. 
•	Chrome Android. 
•	Máy tính bảng. 
•	Màn hình cảm ứng. 
•	Microphone và tai nghe Bluetooth. 
•	Bút cảm ứng. 
•	Mạng 3G/4G yếu. 
Đặc biệt phải kiểm thử:
•	Ghi âm. 
•	Xin quyền microphone. 
•	Canvas viết chữ. 
•	Audio autoplay. 
•	Upload file. 
•	Fullscreen video. 
•	Bàn phím tiếng Trung. 
•	Pinyin IME. 
11. Email, notification và deliverability
Không chỉ gửi email được, cần bảo đảm email đến hộp thư:
•	SPF. 
•	DKIM. 
•	DMARC. 
•	Domain gửi riêng. 
•	Bounce handling. 
•	Complaint handling. 
•	Suppression list. 
•	Unsubscribe. 
•	Retry. 
•	Template versioning. 
•	Preview email. 
•	Tracking có sự đồng ý. 
•	Giới hạn tần suất gửi. 
Các thông báo học tập cần tránh gây phiền:
•	Quiet hours. 
•	Tần suất tối đa. 
•	Preference theo loại thông báo. 
•	Nhắc ôn tập có thể tắt. 
•	Không gửi nhiều thông báo trùng nhau. 
12. Payment và subscription nếu có thương mại
Nếu web có khóa học trả phí, cần thêm:
•	Gói học. 
•	Subscription. 
•	Đơn hàng. 
•	Thanh toán. 
•	Webhook. 
•	Idempotency. 
•	Hóa đơn. 
•	Mã giảm giá. 
•	Hoàn tiền. 
•	Gia hạn. 
•	Hủy gói. 
•	Grace period. 
•	Lịch sử giao dịch. 
•	Đối soát. 
•	Phân quyền nội dung theo gói. 
•	Xử lý thanh toán thành công nhưng callback lỗi. 
•	Không lưu thông tin thẻ trực tiếp. 
Đây nên là module riêng, không gắn logic thanh toán trực tiếp vào Course.
13. Chống gian lận học tập
Nếu có bài thi hoặc chứng chỉ, cần:
•	Giới hạn số lần thi. 
•	Random câu hỏi. 
•	Random đáp án. 
•	Question pool. 
•	Thời gian làm bài. 
•	Chống submit sau khi hết giờ. 
•	Ghi lại thay đổi tab nếu cần. 
•	Phát hiện tốc độ trả lời bất thường. 
•	Phát hiện dùng chung tài khoản. 
•	Lưu attempt bất biến. 
•	Snapshot đề thi tại thời điểm bắt đầu. 
•	Không làm thay đổi đề cũ khi admin sửa câu hỏi. 
14. Moderation và nội dung do người dùng tạo
Nếu có:
•	Bình luận. 
•	Chat AI. 
•	Lớp học. 
•	Bài viết giáo viên. 
•	File upload. 
•	Câu trả lời tự luận. 
Thì cần:
•	Report content. 
•	Moderation queue. 
•	Block user. 
•	Keyword filtering. 
•	Malware scan. 
•	Giới hạn upload. 
•	Chính sách nội dung. 
•	Nhật ký xử lý khiếu nại. 
•	Cơ chế kháng nghị. 
•	Xóa hoặc ẩn nội dung. 
15. AI Governance
Nếu triển khai AI Tutor hoặc chấm phát âm, cần bổ sung:
•	Prompt versioning. 
•	Model version. 
•	Input/output moderation. 
•	RAG source citation. 
•	Không cho AI tự xuất bản nội dung. 
•	AI-generated content phải qua review. 
•	Chống prompt injection. 
•	Giới hạn dữ liệu gửi sang nhà cung cấp AI. 
•	Loại bỏ dữ liệu cá nhân không cần thiết. 
•	Token quota. 
•	Cost limit. 
•	Timeout và fallback. 
•	Đánh giá hallucination. 
•	Bộ test tiếng Trung chuyên biệt. 
•	Lưu trạng thái đồng ý của người dùng. 
•	Cơ chế xóa dữ liệu hội thoại và ghi âm. 
16. SLO, cảnh báo và quản lý sự cố
Monitoring phải gắn với mục tiêu rõ ràng.
Ví dụ:
API availability ≥ 99,9%
P95 API latency < 500 ms
Tỷ lệ lỗi API < 1%
Audio start time < 1,5 giây
Lesson save success ≥ 99,9%
Cần có:
•	SLI. 
•	SLO. 
•	Error budget. 
•	Alert severity. 
•	Người chịu trách nhiệm. 
•	Escalation policy. 
•	Incident channel. 
•	Incident timeline. 
•	Postmortem. 
•	Corrective actions. 
•	Status page. 
Không nên cảnh báo mọi lỗi nhỏ vì sẽ gây alert fatigue.
17. Business continuity
Ngoài backup, cần phương án khi nhà cung cấp gặp sự cố:
•	Supabase không truy cập được. 
•	CDN lỗi. 
•	Email provider lỗi. 
•	AI provider lỗi. 
•	OAuth Google lỗi. 
•	DNS lỗi. 
•	Region lỗi. 
•	Tài khoản cloud bị khóa. 
•	Secret bị lộ. 
•	Nhân sự quản trị chính không sẵn sàng. 
Cần có:
•	Tài khoản khẩn cấp. 
•	Break-glass access. 
•	Danh sách liên hệ. 
•	Runbook. 
•	Bản sao cấu hình. 
•	Khả năng chuyển provider quan trọng. 
•	Kiểm thử disaster recovery. 
18. Quản lý chi phí
Nên bổ sung FinOps:
•	Chi phí trên mỗi người dùng hoạt động. 
•	Chi phí trên mỗi giờ học. 
•	Chi phí AI trên mỗi người dùng. 
•	Chi phí audio/video bandwidth. 
•	Chi phí lưu recording. 
•	Chi phí log. 
•	Budget alert. 
•	Quota theo gói. 
•	Storage lifecycle. 
•	Log retention. 
•	CDN cache hit. 
•	Báo cáo chi phí theo module. 
Không kiểm soát phần này có thể khiến hệ thống kỹ thuật chạy tốt nhưng không thể duy trì tài chính.
19. Quy trình vận hành nội bộ
Cần xác định ai chịu trách nhiệm:
•	Ai duyệt nội dung. 
•	Ai xuất bản. 
•	Ai xử lý tài khoản. 
•	Ai xử lý lỗi thanh toán. 
•	Ai phản hồi ticket. 
•	Ai trực sự cố. 
•	Ai phê duyệt migration. 
•	Ai được truy cập production. 
•	Ai được xem dữ liệu ghi âm. 
•	Ai có quyền phục hồi backup. 
Áp dụng nguyên tắc:
•	Least privilege. 
•	Separation of duties. 
•	Không dùng chung tài khoản admin. 
•	Có thời hạn cho quyền tạm thời. 
•	Audit toàn bộ hành động nhạy cảm. 
Đánh giá cuối cùng
Checklist hiện tại đã đủ để bắt đầu xây dựng production, nhưng chưa nên xem là đóng hoàn toàn. Nên bổ sung 8 nhóm ưu tiên cao nhất:
1.	Taxonomy và version HSK 
2.	Content provenance và kiểm tra chất lượng tự động 
3.	Learning-event tracking 
4.	Version hóa scoring, progress và SRS 
5.	Đồng bộ đa thiết bị/offline 
6.	SLO, incident management và runbook 
7.	AI governance nếu dùng AI 
8.	Quản lý chi phí và vận hành nội bộ 
Cấu trúc checklist cuối cùng nên gồm 10 khối
01. Product Requirements & Architecture
02. Frontend Application
03. Backend & API
04. Data, Storage & Search
05. Identity, Security & Privacy
06. Learning Engine & Content Governance
07. Infrastructure, DevOps & Deployment
08. Testing, Observability & Recovery
09. Operations, Support & Compliance
10. Analytics, Cost & Continuous Improvement
Đánh giá tổng quan
Checklist hiện tại đúng hướng và khá đầy đủ, có thể dùng làm nền tảng xây dựng hệ thống production. Tài liệu đã bao phủ sáu vùng quan trọng: frontend, backend, dữ liệu học, hạ tầng triển khai, giám sát–phục hồi và vận hành–pháp lý. 
Ngoài ra, tài liệu đã có những thành phần production quan trọng như:
•	API versioning, transaction, concurrency và chuẩn lỗi. 
•	Database indexing, connection pooling, backup và restore. 
•	Refresh-token rotation, MFA, session management. 
•	Chống XSS, CSRF, SSRF, SQL Injection và credential stuffing. 
•	Redis, CDN, queue, background jobs. 
•	Logging, metrics, tracing. 
•	Load balancing, autoscaling, circuit breaker và graceful degradation. 
•	Frontend caching, offline, accessibility, performance và monitoring. 
•	CMS, Learning Engine, SRS, quản trị nội dung và DevOps. 
Những nội dung này đã được mô tả khá rõ trong phần Backend và Infrastructure. 
Tuy nhiên, tài liệu hiện tại chủ yếu là danh sách chức năng cần có. Để đạt mục tiêu:
Mượt và nhanh
Bảo mật cao
Chống tấn công thực tế
Không mất dữ liệu học
Trải nghiệm tốt
Dễ mở rộng và vận hành
thì nên nâng cấp checklist thành bộ tiêu chuẩn kiến trúc có chỉ số đo, ngưỡng chấp nhận và bằng chứng kiểm thử.
1. Nâng cấp kiến trúc tổng thể
Kiến trúc phù hợp nhất ở giai đoạn đầu là:
Modular Monolith
+ Clean Architecture
+ Event-driven nội bộ
+ Background Workers
+ CDN và Redis
+ PostgreSQL
+ Object Storage
Không nên tách microservice quá sớm.
Cấu trúc đề xuất
Người dùng
   │
   ▼
DNS
   │
   ▼
CDN + WAF + DDoS Protection
   │
   ├── Static assets
   └── Media cache
   │
   ▼
Load Balancer / Reverse Proxy
   │
   ├── Next.js Public
   ├── Next.js Admin
   └── ASP.NET Core API
           │
           ├── Identity
           ├── Learning Paths
           ├── Dictionary
           ├── Lessons
           ├── Exercises
           ├── Progress
           ├── SRS
           ├── Media
           ├── Governance
           ├── Notifications
           └── Reports
                 │
                 ├── PostgreSQL
                 ├── Redis
                 ├── Object Storage
                 ├── Search
                 └── Message Queue
                        │
                        ├── Import Worker
                        ├── Media Worker
                        ├── Email Worker
                        ├── Analytics Worker
                        └── AI/Speech Worker
Nguyên tắc bắt buộc
•	Module không truy cập trực tiếp bảng của module khác một cách tùy tiện. 
•	Nghiệp vụ không đặt trong Controller. 
•	Frontend không quyết định chấm điểm, quyền truy cập hoặc hoàn thành bài học. 
•	Database không được truy cập trực tiếp từ frontend. 
•	AI, email, media và analytics lỗi không được làm sập chức năng học cơ bản. 
•	Hệ thống chính nên chạy được khi Redis tạm thời mất kết nối. 
•	Job phải có retry, idempotency và dead-letter queue. 
2. Nâng cấp để hệ thống mượt và nhanh
Checklist đã có lazy loading, code splitting, CDN, Redis và Core Web Vitals. Nhưng cần bổ sung các performance budget cụ thể.
2.1. Chỉ tiêu Frontend
Nên đặt mục tiêu:
Chỉ số	Mục tiêu
LCP	Dưới 2,5 giây
INP	Dưới 200 ms
CLS	Dưới 0,1
TTFB trang công khai	Dưới 800 ms
JavaScript ban đầu	Dưới 200–250 KB gzip
Chuyển bài học	Dưới 500 ms khi đã cache
Audio bắt đầu phát	Dưới 1–1,5 giây
Autosave câu trả lời	Dưới 1 giây
Canvas viết chữ	Ổn định gần 60 FPS
2.2. Nâng cấp Frontend
•	Dùng SSR/ISR cho trang từ điển, bài học công khai và landing page. 
•	Chỉ hydrate component thật sự tương tác. 
•	Dynamic import cho: 
o	Canvas viết chữ. 
o	Biểu đồ. 
o	Video player. 
o	Audio recorder. 
o	Admin editor. 
•	Prefetch có chọn lọc bài tiếp theo. 
•	Không prefetch toàn bộ khóa học. 
•	Dùng virtualized list cho: 
o	Danh sách từ vựng dài. 
o	Audit logs. 
o	Lịch sử học. 
o	Bảng quản trị. 
•	Tách server state khỏi global UI state. 
•	Debounce tìm kiếm và hủy request cũ bằng AbortController. 
•	Lưu nháp cục bộ trước, đồng bộ server sau. 
•	Không chặn giao diện trong lúc upload hoặc xử lý AI. 
•	Thiết kế skeleton tương ứng bố cục thật để tránh nhảy giao diện. 
•	Tự phát hiện kết nối chậm để giảm preload media. 
2.3. Nâng cấp Backend
Đặt mục tiêu latency:
API	P95 mục tiêu
Auth/session	Dưới 300 ms
Đọc bài học	Dưới 400 ms
Search từ điển	Dưới 300 ms
Lưu tiến độ	Dưới 300 ms
Nộp bài	Dưới 500 ms
Admin list	Dưới 700 ms
AI/speech	Chạy bất đồng bộ nếu vượt vài giây
Cần bổ sung:
•	Query projection thay vì lấy toàn bộ entity. 
•	AsNoTracking cho truy vấn đọc. 
•	Keyset pagination cho dữ liệu lớn. 
•	Composite index dựa trên truy vấn thật. 
•	Query timeout. 
•	Command timeout. 
•	Maximum page size. 
•	Response compression. 
•	ETag và conditional request cho nội dung công khai. 
•	Cache stampede protection. 
•	Request coalescing cho dữ liệu phổ biến. 
•	Read-through cache cho từ điển. 
•	Cache warming sau khi publish khóa học. 
•	Không cache dữ liệu cá nhân nhạy cảm ở CDN. 
•	Không gọi tuần tự nhiều dịch vụ khi có thể chạy song song an toàn. 
3. Nâng cấp bảo mật và chống tấn công
Checklist hiện đã có phần security tương đối tốt. Tuy nhiên, “chống hacker” không phải một chức năng đơn lẻ. Cần xây dựng nhiều lớp phòng thủ.
3.1. Lớp biên Internet
Bổ sung:
•	WAF trước ứng dụng. 
•	Managed DDoS protection. 
•	Bot management. 
•	IP reputation. 
•	Geo/risk-based rules khi phù hợp. 
•	Rate limiting ngay tại CDN/WAF. 
•	Chặn request có payload bất thường. 
•	Giới hạn request body. 
•	Giới hạn header size. 
•	Chặn HTTP method không sử dụng. 
•	TLS hiện đại và tự động gia hạn certificate. 
•	Origin chỉ nhận traffic từ load balancer/CDN nếu kiến trúc cho phép. 
Luồng bảo vệ:
Internet
→ DDoS Protection
→ CDN
→ WAF
→ Rate Limiter
→ Load Balancer
→ Application Security
→ Database RLS
3.2. Authentication an toàn hơn
•	Access token sống ngắn. 
•	Refresh token lưu bằng cookie HttpOnly, Secure, SameSite. 
•	Chỉ lưu hash refresh token ở database. 
•	Refresh-token rotation. 
•	Phát hiện reuse token. 
•	Thu hồi toàn bộ token family khi nghi ngờ bị đánh cắp. 
•	MFA bắt buộc cho Admin, Publisher và SuperAdmin. 
•	WebAuthn/passkey có thể bổ sung sau. 
•	Password hashing bằng Argon2id hoặc BCrypt cost phù hợp. 
•	Không tiết lộ email có tồn tại trong chức năng quên mật khẩu. 
•	Delay hoặc rate limit cho đăng nhập thất bại. 
•	Cảnh báo đăng nhập bất thường. 
•	Re-authentication trước: 
o	Đổi email. 
o	Đổi mật khẩu. 
o	Xóa tài khoản. 
o	Thay đổi quyền. 
o	Xuất dữ liệu nhạy cảm. 
3.3. Authorization nhiều lớp
Cần kiểm tra đồng thời:
Role
+ Permission
+ Ownership
+ Tenant scope
+ Content status
+ Resource relationship
Ví dụ:
•	Giáo viên chỉ được xem lớp mình quản lý. 
•	Học viên chỉ xem tiến độ của mình. 
•	ContentEditor không được tự duyệt nội dung mình tạo. 
•	Reviewer không tự xuất bản nếu không có quyền Publisher. 
•	SuperAdmin không được dùng làm tài khoản vận hành hằng ngày. 
Backend phải là nguồn quyết định quyền; tài liệu cũng đã xác định đúng nguyên tắc này. 
3.4. Bảo vệ API
Bổ sung:
•	Schema validation nghiêm ngặt. 
•	Giới hạn độ sâu object JSON. 
•	Giới hạn số phần tử mảng. 
•	Giới hạn độ dài chuỗi. 
•	Chống mass assignment. 
•	Allowlist field được cập nhật. 
•	Không bind trực tiếp request vào database entity. 
•	Idempotency key cho: 
o	Thanh toán. 
o	Nộp bài. 
o	Import. 
o	Tạo đơn hàng. 
o	Cập nhật tiến độ offline. 
•	Replay protection cho webhook. 
•	Chữ ký webhook. 
•	Timestamp tolerance. 
•	API key rotation cho tích hợp máy–máy. 
•	Không trả thông tin nội bộ trong lỗi production. 
3.5. Upload và media security
Ngoài MIME và magic number, cần:
•	Upload vào vùng cách ly. 
•	Tên file sinh ngẫu nhiên. 
•	Không dùng tên file người dùng làm path trực tiếp. 
•	Chặn SVG nguy hiểm hoặc sanitize SVG. 
•	Giải nén file trong sandbox. 
•	Giới hạn số file trong ZIP. 
•	Chống ZIP bomb. 
•	Quét malware. 
•	Re-encode ảnh thay vì giữ nguyên file không tin cậy. 
•	Signed URL thời hạn ngắn. 
•	Không để bucket chứa bản ghi âm ở chế độ public. 
•	Không cho file upload có quyền thực thi. 
•	Xóa metadata nhạy cảm khỏi ảnh nếu cần. 
3.6. Supply-chain security
Cần thêm vào CI/CD:
•	Dependency vulnerability scan. 
•	Container image scan. 
•	Secret scan. 
•	Static application security testing. 
•	License scan. 
•	Software Bill of Materials. 
•	Ký container image. 
•	Pin dependency version. 
•	Bảo vệ branch chính. 
•	Bắt buộc code review. 
•	Không deploy từ máy cá nhân trực tiếp. 
•	Production deployment sử dụng tài khoản service riêng. 
•	Quyền CI/CD theo least privilege. 
3.7. Security operations
Checklist nên có thêm:
•	Threat model cho từng module quan trọng. 
•	Security review trước release. 
•	Penetration testing định kỳ. 
•	Vulnerability disclosure process. 
•	Quy trình xử lý secret bị lộ. 
•	Quy trình khóa tài khoản nghi ngờ bị chiếm. 
•	Security incident runbook. 
•	Log chống chỉnh sửa. 
•	Retention riêng cho security logs. 
•	Cảnh báo: 
o	Login thất bại tăng đột biến. 
o	Token reuse. 
o	Thay đổi quyền quản trị. 
o	Export dữ liệu lớn. 
o	Request rate bất thường. 
o	Truy cập nhiều tài khoản từ cùng IP. 
o	Upload malware. 
o	Bypass authorization. 
4. Nâng cấp trải nghiệm người dùng
Tài liệu đã nhấn mạnh không được làm mất bài khi mạng ngắt, đây là yêu cầu rất đúng cho hệ thống học tập. 
Nên bổ sung một checklist UX riêng theo toàn bộ hành trình.
4.1. Onboarding
•	Chỉ hỏi thông tin cần thiết. 
•	Có thể bỏ qua bước không bắt buộc. 
•	Lưu tiến độ onboarding. 
•	Placement test ngắn và thích ứng. 
•	Giải thích rõ trình độ được xác định thế nào. 
•	Đề xuất lộ trình ngay sau onboarding. 
•	Cho phép thay đổi mục tiêu sau này. 
4.2. Trải nghiệm bài học
•	Tải trước audio của câu kế tiếp. 
•	Cho phép phát chậm. 
•	Lặp lại câu/từ. 
•	Hiển thị hoặc ẩn pinyin. 
•	Hiển thị hoặc ẩn nghĩa. 
•	Phím tắt cho người học trên máy tính. 
•	Autosave sau mỗi câu. 
•	Khôi phục đúng vị trí khi quay lại. 
•	Xác nhận trước khi rời bài chưa hoàn thành. 
•	Feedback ngay sau câu trả lời. 
•	Giải thích vì sao sai, không chỉ báo sai. 
•	Không lạm dụng animation. 
•	Không làm người học mất nhịp vì popup quảng cáo. 
4.3. Mất mạng và lỗi dịch vụ
Thay vì thông báo chung “Có lỗi xảy ra”, nên:
Không thể lưu lên máy chủ.
Bài làm đã được lưu tạm trên thiết bị.
Hệ thống sẽ tự đồng bộ khi có mạng.
Cần phân biệt:
•	Mất mạng. 
•	Session hết hạn. 
•	Server bận. 
•	Rate limit. 
•	File quá lớn. 
•	Microphone bị từ chối. 
•	Audio không tải được. 
•	AI đang tạm thời không khả dụng. 
4.4. Đa thiết bị
•	Tiến độ đồng bộ giữa điện thoại và máy tính. 
•	Phát hiện bài đang mở trên thiết bị khác. 
•	Không ghi đè âm thầm dữ liệu mới hơn. 
•	Có quy tắc conflict resolution. 
•	Hiển thị trạng thái “Đã đồng bộ”. 
•	Cho phép người dùng xem lần hoạt động gần nhất. 
4.5. Accessibility và đối tượng người học
Ngoài WCAG cơ bản:
•	Font tiếng Trung rõ ràng. 
•	Không dùng font làm biến dạng nét chữ. 
•	Pinyin không bị tách dòng sai. 
•	Có chế độ chữ lớn. 
•	Có transcript cho audio. 
•	Điều chỉnh tốc độ phát. 
•	Không bắt buộc nghe âm thanh để hiểu nội dung. 
•	Tone màu không làm khó người mù màu. 
•	Nút trên mobile đủ lớn. 
•	Canvas tương thích touch và stylus. 
5. Bổ sung kiến trúc dữ liệu học tập
Phần này quyết định sản phẩm có thật sự tốt hay không.
Checklist đã có Learning Engine, mastery, placement test, adaptive learning và SRS. Cần nâng cấp thêm:
5.1. Immutable learning attempts
Khi người dùng bắt đầu bài kiểm tra, cần lưu snapshot:
Attempt
Question snapshot
Answer snapshot
Scoring rule version
Content version
Started time
Submitted time
Admin sửa câu hỏi sau đó không được làm thay đổi kết quả lịch sử.
5.2. Event sourcing nhẹ cho tiến độ
Không nhất thiết áp dụng full Event Sourcing, nhưng nên lưu các sự kiện học quan trọng:
LessonStarted
QuestionAnswered
ExerciseSubmitted
LessonCompleted
ReviewCompleted
StreakUpdated
Từ đó có thể:
•	Tính lại tiến độ. 
•	Điều tra lỗi. 
•	Phân tích hành vi. 
•	Khôi phục khi logic thay đổi. 
•	Xử lý đồng bộ offline tốt hơn. 
5.3. Version hóa nội dung
Tiến độ phải gắn với:
•	Course version. 
•	Lesson version. 
•	Question version. 
•	Scoring version. 
•	SRS algorithm version. 
•	Taxonomy version. 
Điều này đã được đề cập trong tài liệu, nhưng cần biến thành yêu cầu database bắt buộc. 
6. Nâng cấp availability và recovery
Checklist đã có RPO, RTO, backup và restore test. Nên làm rõ:
Các mức dữ liệu
Loại dữ liệu	Mức quan trọng
Tài khoản và quyền	Critical
Tiến độ học	Critical
Thanh toán	Critical
Nội dung đã xuất bản	High
Recording	High/Medium
Analytics events	Medium
Cache	Có thể tái tạo
File tạm	Thấp
Mục tiêu gợi ý
•	Database production: point-in-time recovery. 
•	Backup tự động hằng ngày. 
•	Kiểm thử restore định kỳ. 
•	Tiến độ học không phụ thuộc duy nhất Redis. 
•	Cache mất phải tái tạo được. 
•	Job mất kết nối phải retry được. 
•	Deployment lỗi phải rollback nhanh. 
•	Migration lớn phải có phương án backward-compatible. 
•	AI hoặc email lỗi không ngăn người dùng học. 
•	Có maintenance mode nhưng vẫn bảo vệ dữ liệu đang nhập. 
7. Bổ sung kiểm thử production thực tế
Checklist đã có nhiều loại test frontend. Nên bổ sung ma trận test toàn hệ thống:
Security tests
•	Authorization bypass. 
•	IDOR/BOLA. 
•	Token theft/reuse. 
•	CSRF. 
•	XSS stored/reflected. 
•	SQL Injection. 
•	SSRF. 
•	Malicious upload. 
•	Rate-limit bypass. 
•	Privilege escalation. 
•	Session fixation. 
•	Dependency vulnerability. 
Performance tests
•	Load test. 
•	Stress test. 
•	Spike test. 
•	Soak test. 
•	Database benchmark. 
•	Cache failure test. 
•	Queue backlog test. 
•	Media concurrent playback. 
•	Search concurrency. 
•	Concurrent lesson submissions. 
Resilience tests
•	Tắt Redis. 
•	Tắt worker. 
•	Storage trả lỗi. 
•	Email provider timeout. 
•	AI provider timeout. 
•	Database read replica lỗi. 
•	CDN cache miss hàng loạt. 
•	Deploy khi có người dùng đang làm bài. 
•	Migration khi phiên bản frontend cũ vẫn còn hoạt động. 
8. Gate bắt buộc trước khi go-live
Không nên chỉ dùng checklist “đã làm/chưa làm”. Nên có cổng kiểm soát.
Gate 1 — Security
•	Không còn lỗ hổng Critical/High chưa xử lý. 
•	MFA đã bật cho admin. 
•	Secret không nằm trong source code. 
•	Authorization tests đã pass. 
•	Upload security đã pass. 
•	Backup đã mã hóa. 
•	Log không chứa token hoặc mật khẩu. 
Gate 2 — Performance
•	Core Web Vitals đạt yêu cầu. 
•	P95 API đạt mục tiêu. 
•	Load test đạt lượng người dùng dự kiến. 
•	Database không có slow query nghiêm trọng. 
•	Audio và media đạt thời gian khởi động mục tiêu. 
Gate 3 — Reliability
•	Restore database thành công. 
•	Rollback deployment thành công. 
•	Không mất bài khi mất mạng. 
•	Không submit trùng khi retry. 
•	Redis hoặc AI lỗi không làm sập luồng học chính. 
Gate 4 — UX
•	Các luồng chính đã qua E2E. 
•	Mobile và desktop hoạt động. 
•	Ghi âm hoạt động trên trình duyệt hỗ trợ. 
•	Error message dễ hiểu. 
•	Người dùng có thể quay lại đúng bài đang học. 
•	Không có dead end trong onboarding. 
Gate 5 — Operations
•	Dashboard và alert đã hoạt động. 
•	Có người chịu trách nhiệm từng loại sự cố. 
•	Có runbook. 
•	Có status page hoặc kênh thông báo sự cố. 
•	Có quy trình support theo trace ID. 
•	Có audit cho hành động quản trị. 
9. Cấu trúc checklist kiến trúc cuối cùng
Nên tái cấu trúc tài liệu thành 12 phân hệ:
01. Product Requirements & User Experience
02. Frontend Architecture
03. Backend & Domain Architecture
04. API Governance & Integration
05. Database, Storage & Search
06. Identity, Authorization & Security
07. Learning Engine & Content Governance
08. Performance, Cache, CDN & Scalability
09. DevOps, CI/CD & Release Management
10. Testing, Observability & Incident Response
11. Availability, Backup & Disaster Recovery
12. Operations, Compliance, Cost & Support
Mỗi đầu mục cần thêm các cột:
Trường	Ý nghĩa
Requirement	Nội dung phải thực hiện
Priority	P0, P1, P2
Owner	Người hoặc nhóm chịu trách nhiệm
Acceptance criteria	Điều kiện được xem là hoàn thành
Test evidence	Kết quả test, log, ảnh hoặc báo cáo
Monitoring	Cách giám sát sau khi chạy
Rollback	Phương án quay lại
Status	Chưa làm, đang làm, đã nghiệm thu
Kết luận
Kiến trúc trong tài liệu hiện tại đúng và đã đủ rộng để bắt đầu phát triển production. Tài liệu cũng đã tự xác định checklist ở mức khoảng 85–90% và đề xuất cấu trúc 10 khối tổng thể. 
Phần cần nâng cấp không phải chủ yếu là thêm nhiều công nghệ hơn, mà là biến từng mục thành tiêu chuẩn kiểm chứng được:
Có ngưỡng hiệu năng
Có threat model
Có security test
Có acceptance criteria
Có load test
Có restore test
Có monitoring và alert
Có owner chịu trách nhiệm
Có rollback
Có bằng chứng nghiệm thu
Ưu tiên cao nhất nên là:
1.	WAF, DDoS, bot protection và security monitoring. 
2.	Authorization nhiều lớp và chống IDOR/BOLA. 
3.	Performance budget cho frontend, API, database và media. 
4.	Autosave, offline queue và đồng bộ đa thiết bị. 
5.	Version hóa nội dung, attempt, scoring và Learning Engine. 
6.	Load, stress, security và failure testing. 
7.	Go-live gates và bằng chứng nghiệm thu. 
8.	Incident response, restore test và rollback thực tế.
1. Backend — đã tốt, cần bổ sung gì?
Backend đã có API versioning, validation, idempotency, transaction, concurrency, Swagger và kiểm soát tương thích API. Tài liệu cũng bao phủ đầy đủ các nghiệp vụ học tập như khóa học, bài học, từ vựng, bài tập, tiến độ, SRS, lớp học và AI Tutor. 
Nên bổ sung
1.1. Ranh giới module rõ ràng
Chốt kiến trúc Modular Monolith theo các bounded context:
Identity
User Profile
Taxonomy
Dictionary
Learning Content
Learning Paths
Exercises
Assessments
Progress
SRS
Media
Classroom
Notifications
Analytics
Governance
Billing
AI
Platform Operations
Mỗi module cần có:
•	Entity và aggregate riêng. 
•	Repository/interface riêng. 
•	Command/query riêng. 
•	Database schema hoặc ownership bảng rõ ràng. 
•	Không truy cập trực tiếp bảng của module khác. 
•	Giao tiếp thông qua application service hoặc domain event. 
1.2. Transactional Outbox
Đây là phần quan trọng chưa được nhấn mạnh đủ.
Ví dụ khi hoàn thành bài học:
1. Lưu LessonCompletion
2. Ghi Progress
3. Tạo SRS cards
4. Phát event
5. Gửi notification
Nếu database commit thành công nhưng message queue lỗi, dữ liệu có thể không đồng bộ. Nên dùng:
•	Transactional Outbox. 
•	Inbox/deduplication phía consumer. 
•	Event ID. 
•	Processing status. 
•	Retry. 
•	Dead-letter queue. 
1.3. API governance nâng cao
Bổ sung:
•	OpenAPI contract testing. 
•	Backward compatibility test. 
•	API deprecation policy. 
•	Client version header. 
•	Request body size limit. 
•	Maximum pagination size. 
•	Idempotency-key storage policy. 
•	ETag cho nội dung công khai. 
•	API changelog. 
•	Consumer-driven contract test nếu có nhiều client. 
1.4. Concurrency theo nghiệp vụ
Không chỉ dùng optimistic concurrency chung. Cần quy định riêng:
•	Một bài thi chỉ submit một lần. 
•	Một SRS card không review đồng thời trên hai thiết bị. 
•	Một nội dung không được hai reviewer cập nhật cùng lúc mà không cảnh báo. 
•	Không cấp chứng chỉ trùng. 
•	Không cộng streak hoặc reward nhiều lần. 
•	Không xử lý webhook thanh toán trùng. 
1.5. Graceful degradation
Hệ thống học cơ bản phải tiếp tục hoạt động khi:
•	AI lỗi. 
•	Email lỗi. 
•	Notification lỗi. 
•	Analytics lỗi. 
•	Redis lỗi. 
•	Search engine phụ lỗi. 
Cần xác định rõ chức năng nào:
Critical
Degraded but usable
Optional
2. Frontend — cần nâng cấp gì?
Frontend hiện đã có design system, routing, state, data layer, auth, validation, offline, error handling, performance, accessibility, SEO, testing và analytics. 
Nên bổ sung
2.1. Feature-based architecture
Không nên tổ chức chủ yếu theo loại file:
components/
hooks/
services/
pages/
Nên tổ chức theo nghiệp vụ:
features/
  auth/
  onboarding/
  dictionary/
  lesson/
  exercise/
  progress/
  srs/
  pronunciation/
  writing/
  classroom/
Bên trong mỗi feature:
components
hooks
schemas
services
types
tests
2.2. Backend for Frontend hoặc API aggregation
Trang bài học có thể cần:
•	Nội dung bài. 
•	Tiến độ. 
•	Media. 
•	Từ vựng. 
•	Quyền truy cập. 
•	SRS status. 
•	Cấu hình người dùng. 
Không nên để frontend gọi 8–10 API tuần tự. Có thể dùng:
•	Endpoint aggregate riêng cho lesson experience. 
•	Server Component gọi nhiều nguồn phía server. 
•	BFF nếu nhu cầu frontend ngày càng phức tạp. 
2.3. Offline-first rõ hơn
Tài liệu đã yêu cầu không làm mất bài khi mất mạng. Cần đặc tả:
•	Dữ liệu nào được lưu IndexedDB. 
•	Thời hạn lưu. 
•	Mã hóa dữ liệu nhạy cảm. 
•	Queue thao tác chưa đồng bộ. 
•	Thứ tự phát lại thao tác. 
•	Idempotency key. 
•	Chính sách conflict. 
•	Hiển thị “Đang lưu”, “Đã lưu cục bộ”, “Đã đồng bộ”. 
•	Cách xử lý khi tài khoản bị đăng xuất trước lúc đồng bộ. 
2.4. Performance budgets bắt buộc
Không chỉ ghi “tối ưu”. Nên đưa vào CI:
LCP ≤ 2,5 giây
INP ≤ 200 ms
CLS ≤ 0,1
Initial JS ≤ 250 KB gzip
Lesson navigation ≤ 500 ms khi cache
Audio start ≤ 1,5 giây
Canvas gần 60 FPS
Nếu vượt budget, pipeline phải cảnh báo hoặc chặn release.
2.5. Real User Monitoring
Synthetic test chưa đủ. Cần đo người dùng thật theo:
•	Quốc gia/khu vực. 
•	Loại mạng. 
•	Trình duyệt. 
•	Thiết bị. 
•	Phiên bản frontend. 
•	Trang và tính năng. 
•	Phân vị P50/P75/P95. 
2.6. Microphone và media UX
Đây là phần rất đặc thù:
•	Kiểm tra quyền microphone trước bài luyện nói. 
•	Có màn hình hướng dẫn nếu bị chặn quyền. 
•	Hiển thị mức âm lượng đầu vào. 
•	Cảnh báo môi trường quá ồn. 
•	Kiểm tra audio đầu ra. 
•	Có fallback khi trình duyệt không hỗ trợ codec. 
•	Không tự động upload bản ghi khi chưa có sự đồng ý. 
3. Database — phần cần nâng cấp đáng kể nhất
Tài liệu đã có key nội bộ, UUID công khai, constraint, index, concurrency, migration, pooling, slow-query monitoring, backup và restore. 
Tuy nhiên, để triển khai thực tế cần thêm một Database Architecture Specification riêng.
3.1. Data ownership
Mỗi bảng phải có module sở hữu:
Nhóm bảng	Module sở hữu
Users, Roles, Sessions	Identity
Words, Characters, Grammar	Dictionary
Courses, Lessons	Learning Content
Attempts, Answers	Assessment
Progress, Mastery	Progress
Cards, Reviews	SRS
Files, MediaAssets	Media
AuditLogs	Platform Operations
Không để nhiều module tự ý cập nhật cùng một bảng.
3.2. Immutable records
Các dữ liệu sau không nên sửa trực tiếp sau khi hoàn tất:
•	Examination attempt. 
•	Submitted answers. 
•	Payment transaction. 
•	Certificate issuance. 
•	Audit event. 
•	Security event. 
•	Published content revision. 
•	Scoring snapshot. 
Muốn thay đổi phải tạo version hoặc correction record mới.
3.3. Temporal và version data
Nên bổ sung:
ValidFrom
ValidTo
RevisionNumber
ContentVersion
AlgorithmVersion
PublishedAt
SupersededBy
Đặc biệt cho:
•	Course. 
•	Lesson. 
•	Question. 
•	Answer option. 
•	Scoring rule. 
•	HSK taxonomy. 
•	SRS algorithm. 
3.4. Partitioning
Khi hệ thống tăng trưởng, các bảng dễ rất lớn:
•	LearningEvents. 
•	AuditLogs. 
•	SecurityEvents. 
•	Notifications. 
•	QuestionAttempts. 
•	SRSReviews. 
•	AnalyticsEvents. 
Nên chuẩn bị partition theo:
•	Tháng/quý. 
•	Tenant nếu multi-tenant. 
•	Hash user nếu khối lượng rất lớn. 
Không cần partition ngay từ MVP, nhưng schema và khóa chính không nên chặn việc này.
3.5. Archival và retention
Phải có chính sách:
Dữ liệu	Chính sách
Refresh token hết hạn	Xóa hoặc archive sớm
Request log	Lưu ngắn hạn
Security log	Lưu lâu hơn
Audit log	Theo yêu cầu kiểm toán
Recording	Theo consent và gói dịch vụ
Learning events	Tổng hợp rồi archive
File tạm	Tự động xóa
Tài khoản đã xóa	Anonymize hoặc purge
3.6. Database security
Bổ sung:
•	Database không public trực tiếp. 
•	TLS cho kết nối. 
•	Tài khoản runtime không có quyền migration. 
•	Tài khoản migration riêng. 
•	Read-only account cho reporting. 
•	Rotate credential. 
•	Field-level encryption với dữ liệu nhạy cảm. 
•	Backup encryption. 
•	Audit truy cập database. 
•	RLS test tự động. 
•	Không cho admin dashboard chạy raw SQL tùy ý. 
3.7. Migration safety
Quy tắc expand–migrate–contract:
1. Thêm schema mới tương thích ngược
2. Deploy code đọc/ghi cả hai dạng
3. Backfill dữ liệu
4. Chuyển hoàn toàn sang schema mới
5. Xóa schema cũ ở release sau
Không nên deploy migration xóa/đổi tên cột cùng lúc với code mới.
4. Hạ tầng — cần bổ sung topology triển khai thật
Tài liệu có load balancer, health check, autoscaling, read replica, queue, circuit breaker và graceful degradation. 
Kiến trúc đề xuất
Internet
  ↓
DNS
  ↓
CDN + DDoS Protection + WAF
  ↓
Load Balancer / Reverse Proxy
  ↓
Next.js Public     Next.js Admin
  ↓                    ↓
        ASP.NET Core API
              ↓
   PostgreSQL Primary
   Redis
   Object Storage
   Queue
   Search
              ↓
         Workers
Cần bổ sung
4.1. Network segmentation
Tách:
•	Public subnet: load balancer. 
•	Private application subnet: frontend server/API. 
•	Private data subnet: database, Redis. 
•	Management path riêng. 
•	Không cho database nhận kết nối Internet. 
•	Egress allowlist nếu có thể. 
•	Private endpoint cho storage/database. 
4.2. Production access control
•	Không SSH trực tiếp bằng tài khoản chung. 
•	SSO cho cloud console. 
•	MFA bắt buộc. 
•	Just-in-time access. 
•	Bastion hoặc managed session. 
•	Break-glass account được giám sát. 
•	Ghi lại thao tác quản trị. 
•	IP allowlist cho công cụ nội bộ nhạy cảm. 
4.3. Autoscaling dựa trên đúng metric
Không chỉ CPU:
•	Request concurrency. 
•	P95 latency. 
•	Queue depth. 
•	Active connection. 
•	Worker job duration. 
•	Memory pressure. 
•	Database saturation. 
4.4. Capacity planning
Phải định nghĩa ít nhất ba kịch bản:
Giai đoạn 1: 100 người dùng đồng thời
Giai đoạn 2: 1.000 người dùng đồng thời
Giai đoạn 3: 10.000 người dùng đồng thời
Với mỗi mức cần ước lượng:
•	API instances. 
•	DB connections. 
•	Redis memory. 
•	Queue throughput. 
•	Storage. 
•	CDN bandwidth. 
•	Audio/video traffic. 
•	AI cost. 
4.5. Multi-region
Tài liệu xếp multi-region ở P2 là hợp lý. Không nên triển khai quá sớm trừ khi có yêu cầu kinh doanh.
Giai đoạn đầu ưu tiên:
•	Một region ổn định. 
•	Multi-zone nếu nhà cung cấp hỗ trợ. 
•	Backup ngoài vùng lỗi chính. 
•	CDN toàn cầu. 
•	Disaster recovery đã kiểm thử. 
5. Security — tốt nhưng cần chuyển thành chương trình AppSec
Tài liệu đã có SQL Injection, XSS, CSRF, SSRF, CSP, HSTS, secret management, dependency scanning và chống bot. 
Phần còn thiếu hoặc cần tăng cường
5.1. Threat modeling
Mỗi module nhạy cảm cần threat model:
•	Identity. 
•	Admin. 
•	Payment. 
•	Upload. 
•	AI Tutor. 
•	Recording. 
•	Classroom. 
•	Data export. 
•	Content publishing. 
Phân tích:
Asset
Actor
Trust boundary
Attack surface
Threat
Mitigation
Residual risk
5.2. Chống BOLA/IDOR
Đây là lỗi rất nguy hiểm với API.
Ví dụ kẻ tấn công đổi:
/users/{uuid}/progress
/classes/{uuid}
/recordings/{uuid}
/attempts/{uuid}
Backend phải luôn kiểm tra ownership và relationship, không chỉ kiểm tra token hợp lệ.
5.3. Admin security
•	Admin chạy trên domain/subdomain riêng. 
•	MFA bắt buộc. 
•	Session ngắn hơn người dùng thường. 
•	Re-authentication với hành động nguy hiểm. 
•	IP/risk policy nếu phù hợp. 
•	Không cho ContentEditor tự publish. 
•	Four-eyes approval cho thay đổi quyền cao. 
•	Cảnh báo khi export số lượng dữ liệu lớn. 
•	Không dùng SuperAdmin hằng ngày. 
5.4. Runtime security
•	WAF. 
•	Managed DDoS protection. 
•	Bot protection. 
•	Runtime/container read-only filesystem nếu phù hợp. 
•	Non-root container. 
•	Drop Linux capabilities. 
•	Network policy. 
•	Secret không bake vào image. 
•	Container image signing. 
•	Runtime anomaly detection nếu mức rủi ro yêu cầu. 
5.5. Security gates
Trước release:
•	Không còn Critical vulnerability. 
•	High vulnerability phải có quyết định xử lý rõ. 
•	SAST pass. 
•	Dependency scan pass. 
•	Container scan pass. 
•	Secret scan pass. 
•	Authorization tests pass. 
•	DAST hoặc API security test pass. 
•	Penetration test trước go-live lớn. 
Không thể bảo đảm “không bị hacker”, nhưng có thể giảm xác suất và giảm mức thiệt hại bằng phòng thủ nhiều lớp.
6. Hệ thống nội dung học — mạnh nhưng cần chuẩn hóa sâu hơn
Tài liệu đã có CMS workflow, version history, rollback, scheduled publishing, audit log và chuẩn hóa dữ liệu tiếng Trung. 
Nên bổ sung
6.1. Content schema validation
Mỗi loại nội dung có schema riêng:
•	Vocabulary. 
•	Character. 
•	Grammar point. 
•	Sentence. 
•	Dialogue. 
•	Listening exercise. 
•	Reading passage. 
•	Writing exercise. 
•	Pronunciation exercise. 
Ví dụ từ vựng không được publish nếu thiếu:
Simplified form
Pinyin
Vietnamese meaning
Part of speech
HSK mapping
Source/license
Review status
6.2. Content dependency graph
Kiểm tra:
•	Lesson prerequisite cycle. 
•	Khóa học tham chiếu bài đã archive. 
•	Exercise tham chiếu word version không hợp lệ. 
•	Media bị xóa nhưng còn sử dụng. 
•	HSK mapping không tồn tại. 
•	Nội dung public tham chiếu draft. 
6.3. Dual-review cho nội dung quan trọng
Đối với:
•	Câu hỏi thi. 
•	Đáp án. 
•	Phát âm. 
•	Bản dịch. 
•	Câu ví dụ nhạy cảm. 
•	Nội dung AI sinh. 
Nên yêu cầu người tạo khác người duyệt.
7. Observability — cần tập trung vào trải nghiệm học
Tài liệu đã có application log, security log, audit log, metrics, tracing và correlation ID. 
Ngoài metric kỹ thuật, cần metric nghiệp vụ:
Lesson load success rate
Lesson save success rate
Exercise submit success rate
Progress synchronization success
Audio playback success
Recording upload success
SRS scheduling correctness
Search zero-result rate
Content broken-link rate
Cảnh báo theo SLO
Ví dụ:
•	Lesson save success < 99,9%. 
•	P95 API > 500 ms trong 10 phút. 
•	Audio failure > 2%. 
•	Queue age > 5 phút. 
•	DB connection usage > 80%. 
•	Error rate > 1%. 
•	Refresh-token reuse phát hiện. 
•	Admin permission thay đổi bất thường. 
8. Testing — cần bổ sung tỷ lệ và ma trận trách nhiệm
Tài liệu đã liệt kê unit, component, integration, E2E, accessibility, visual regression, browser và offline test. 
Nên quy định rõ
Backend
•	Domain logic unit coverage cao. 
•	Integration test với PostgreSQL thật. 
•	Authorization test cho từng protected endpoint. 
•	Migration test từ phiên bản production gần nhất. 
•	Contract test. 
•	Queue retry/idempotency test. 
•	Concurrency test. 
Frontend
•	Component test cho interactive learning component. 
•	E2E cho luồng P0. 
•	Visual regression cho trang chính. 
•	Offline test. 
•	Microphone/audio compatibility test. 
•	Accessibility test tự động và thủ công. 
Hạ tầng
•	Load test. 
•	Stress test. 
•	Spike test. 
•	Soak test. 
•	Failover test. 
•	Restore test. 
•	Rollback test. 
•	Chaos/failure injection có kiểm soát. 
9. Những mục chưa cần triển khai ngay
Không nên vì checklist dài mà xây tất cả trước lần ra mắt đầu tiên.
Có thể trì hoãn:
•	Multi-region. 
•	Microservices. 
•	Kubernetes nếu tải chưa đủ lớn. 
•	Elasticsearch/OpenSearch riêng nếu PostgreSQL FTS đáp ứng. 
•	Read replica nếu chưa có báo cáo nặng. 
•	AI Tutor đầy đủ. 
•	Adaptive recommendation phức tạp. 
•	A/B testing platform riêng. 
•	Event sourcing toàn hệ thống. 
•	Data warehouse lớn. 
Tập trung trước vào:
Đúng nghiệp vụ
Không mất tiến độ
Nội dung chính xác
Auth an toàn
API ổn định
Giao diện nhanh
Backup khôi phục được
Quan sát được lỗi
Deploy và rollback được
Những bổ sung ưu tiên cao nhất
P0 — phải bổ sung hoặc đặc tả rõ trước production
1.	Transactional Outbox và xử lý event đáng tin cậy. 
2.	Database ownership và schema/module boundaries. 
3.	Immutable attempt và snapshot đề thi. 
4.	Chống BOLA/IDOR bằng authorization theo resource. 
5.	WAF, DDoS và bot protection. 
6.	Network segmentation, database private. 
7.	Offline sync protocol và conflict resolution. 
8.	Expand–migrate–contract cho database migration. 
9.	Performance budget và SLO có số đo. 
10.	Security, performance, restore và rollback release gates. 
P1 — cần để vận hành ổn định
1.	Content dependency graph. 
2.	Real User Monitoring. 
3.	Capacity planning. 
4.	Data retention và archival. 
5.	Database partition readiness. 
6.	Admin security nâng cao. 
7.	Supply-chain security và SBOM. 
8.	Incident drill và disaster-recovery drill. 
9.	Chi phí theo người dùng và theo giờ học. 
10.	UX research với người học thật. 
Kết luận cuối cùng
Checklist hiện tại đạt mức Production Architecture nâng cao, khoảng 8,7/10. Tài liệu đã tự đánh giá phạm vi khoảng 85–90% và xác định đúng các nhóm cần tiếp tục hoàn thiện. 
Kiến trúc không thiếu nhiều nhóm chức năng lớn. Phần còn thiếu chủ yếu là chiều sâu triển khai:
Từ “có cache” → cache gì, TTL bao lâu, invalidation thế nào
Từ “có bảo mật” → threat model và security test nào
Từ “có backup” → RPO/RTO và restore đã test chưa
Từ “có scaling” → scale theo metric nào
Từ “có offline” → conflict resolution ra sao
Từ “có database” → ownership, version và retention thế nào
Từ “có monitoring” → SLO và cảnh báo cụ thể nào
ĐIỀU KIỆN, RÀNG BUỘC VÀ QUYỀN THAO TÁC TRÊN HỆ THỐNG
1. Mục đích
Phần này quy định:
•	Điều kiện để người dùng được sử dụng từng chức năng.
•	Những thao tác người dùng được phép thực hiện.
•	Những thao tác bị giới hạn hoặc bị cấm.
•	Quy tắc nghiệp vụ hệ thống phải kiểm tra.
•	Ràng buộc về dữ liệu, bảo mật, tiến độ học và nội dung.
•	Trách nhiệm của từng nhóm người dùng.
Mọi quy tắc quan trọng phải được kiểm tra tại Backend. Frontend chỉ hỗ trợ hiển thị, hướng dẫn và hạn chế thao tác trên giao diện.
________________________________________
2. Các nhóm người dùng
Hệ thống gồm các nhóm người dùng chính:
2.1. Khách chưa đăng nhập — Guest
Khách chưa đăng nhập có thể:
•	Truy cập trang chủ.
•	Xem thông tin giới thiệu hệ thống.
•	Xem danh sách khóa học công khai.
•	Xem một phần nội dung học miễn phí.
•	Tra cứu từ điển công khai nếu hệ thống cho phép.
•	Xem bài viết, hướng dẫn và nội dung giới thiệu.
•	Đăng ký tài khoản.
•	Đăng nhập.
•	Yêu cầu đặt lại mật khẩu.
Khách chưa đăng nhập không được:
•	Lưu tiến độ học.
•	Tham gia bài kiểm tra có ghi nhận kết quả.
•	Sử dụng SRS cá nhân.
•	Xem nội dung yêu cầu đăng ký hoặc thanh toán.
•	Xem thông tin cá nhân của người dùng khác.
•	Truy cập trang quản trị.
•	Tải xuống nội dung riêng tư.
•	Sử dụng chức năng AI, ghi âm hoặc upload nếu chưa được cấp quyền.
________________________________________
2.2. Người học — Learner
Người học có thể:
•	Cập nhật hồ sơ cá nhân.
•	Chọn mục tiêu và lộ trình học.
•	Thực hiện bài kiểm tra đầu vào.
•	Đăng ký hoặc tham gia khóa học.
•	Học bài theo lộ trình.
•	Làm bài tập và bài kiểm tra.
•	Nghe audio, xem video và luyện phát âm.
•	Luyện viết chữ Hán.
•	Tra cứu từ vựng, ngữ pháp, bộ thủ và chữ Hán.
•	Lưu từ vựng yêu thích.
•	Ôn tập theo SRS.
•	Xem tiến độ, điểm số, streak và thành tích.
•	Nhận thông báo học tập.
•	Gửi phản hồi hoặc báo lỗi nội dung.
•	Yêu cầu xuất hoặc xóa dữ liệu cá nhân.
•	Quản lý các phiên đăng nhập của mình.
Người học không được:
•	Chỉnh sửa nội dung khóa học.
•	Thay đổi đáp án, điểm số hoặc tiến độ trực tiếp.
•	Truy cập dữ liệu học tập của người khác.
•	Thay đổi vai trò hoặc quyền của tài khoản.
•	Truy cập chức năng quản trị.
•	Can thiệp vào quy tắc chấm điểm.
•	Giả mạo kết quả học tập.
•	Chia sẻ tài khoản cho nhiều người sử dụng trái quy định.
•	Tải xuống hoặc sao chép nội dung có bản quyền ngoài phạm vi được phép.
________________________________________
2.3. Giáo viên — Teacher
Giáo viên có thể:
•	Tạo và quản lý lớp học được phân công.
•	Mời hoặc thêm học viên vào lớp.
•	Giao bài tập.
•	Theo dõi tiến độ học viên trong lớp.
•	Xem kết quả và nhận xét bài làm.
•	Chấm bài tự luận nếu được cấp quyền.
•	Gửi thông báo cho lớp.
•	Tạo tài liệu hỗ trợ giảng dạy.
•	Đề xuất chỉnh sửa nội dung.
•	Xem báo cáo thuộc lớp mình quản lý.
Giáo viên không được:
•	Xem dữ liệu học viên ngoài lớp được phân công.
•	Thay đổi điểm đã khóa nếu không có quyền.
•	Xem thông tin nhạy cảm không liên quan đến giảng dạy.
•	Xuất bản nội dung chính thức nếu không có quyền Publisher.
•	Thay đổi quyền hệ thống.
•	Tự thêm mình vào lớp không được phân công.
•	Truy cập dữ liệu thanh toán nếu không được cấp quyền.
________________________________________
2.4. Biên tập viên nội dung — ContentEditor
ContentEditor có thể:
•	Tạo nội dung nháp.
•	Chỉnh sửa từ vựng, chữ Hán, ngữ pháp, câu ví dụ và bài học.
•	Upload audio, hình ảnh, SVG và video.
•	Import dữ liệu từ file mẫu.
•	Gửi nội dung sang trạng thái chờ duyệt.
•	Xem lỗi kiểm tra dữ liệu.
•	Sửa nội dung bị Reviewer trả lại.
•	Xem lịch sử phiên bản nội dung mình được phép quản lý.
ContentEditor không được:
•	Tự duyệt nội dung do mình tạo.
•	Tự xuất bản nội dung nếu không có quyền Publisher.
•	Xóa vĩnh viễn nội dung đã xuất bản.
•	Bỏ qua kiểm tra nguồn và bản quyền.
•	Thay đổi lịch sử kiểm duyệt.
•	Sửa nội dung ngoài phạm vi được phân công.
________________________________________
2.5. Người kiểm duyệt — Reviewer
Reviewer có thể:
•	Xem nội dung đang chờ duyệt.
•	So sánh các phiên bản.
•	Kiểm tra tính chính xác học thuật.
•	Kiểm tra pinyin, nghĩa, âm Hán Việt, loại từ và ví dụ.
•	Kiểm tra nguồn và bản quyền.
•	Phê duyệt hoặc từ chối nội dung.
•	Ghi nhận lý do từ chối.
•	Yêu cầu ContentEditor chỉnh sửa.
•	Xem lịch sử thay đổi liên quan.
Reviewer không được:
•	Tự ý sửa nội dung và duyệt cùng một lúc nếu vi phạm quy trình phân tách trách nhiệm.
•	Xóa lịch sử duyệt.
•	Xuất bản nếu không có quyền Publisher.
•	Phê duyệt nội dung thiếu nguồn bắt buộc.
•	Phê duyệt nội dung có lỗi kiểm tra nghiêm trọng.
________________________________________
2.6. Người xuất bản — Publisher
Publisher có thể:
•	Xuất bản nội dung đã được phê duyệt.
•	Lên lịch xuất bản.
•	Tạm ẩn nội dung.
•	Thu hồi nội dung có lỗi.
•	Khôi phục phiên bản trước.
•	Quản lý trạng thái Published và Archived.
•	Xem báo cáo nội dung đã xuất bản.
Publisher không được:
•	Xuất bản nội dung chưa được duyệt.
•	Bỏ qua lỗi Critical.
•	Thay đổi lịch sử phiên bản.
•	Xóa vĩnh viễn nội dung đang được người học sử dụng.
•	Tự ý thay đổi quyền người dùng.
________________________________________
2.7. Chuyên viên phân tích — Analyst
Analyst có thể:
•	Xem báo cáo tổng hợp.
•	Xem số liệu học tập đã được phân quyền.
•	Phân tích tỷ lệ hoàn thành bài học.
•	Phân tích retention, streak, SRS và kết quả học tập.
•	Xuất báo cáo được cho phép.
•	Xem hiệu suất nội dung và câu hỏi.
Analyst không được:
•	Chỉnh sửa dữ liệu nghiệp vụ.
•	Xem dữ liệu cá nhân vượt quá phạm vi cần thiết.
•	Truy cập mật khẩu, token hoặc dữ liệu xác thực.
•	Thay đổi tiến độ và điểm số người học.
•	Xem bản ghi âm cá nhân nếu không có quyền riêng.
________________________________________
2.8. Quản trị viên — Admin
Admin có thể:
•	Quản lý người dùng.
•	Khóa hoặc mở khóa tài khoản.
•	Thu hồi session.
•	Phân quyền trong phạm vi được cấp.
•	Quản lý cấu hình hệ thống.
•	Xem audit log.
•	Quản lý import, job và lỗi vận hành.
•	Bật hoặc tắt feature flag.
•	Kích hoạt maintenance mode.
•	Xử lý ticket hỗ trợ.
•	Xem dashboard giám sát.
Admin không được:
•	Xem mật khẩu người dùng.
•	Xem refresh token dạng rõ.
•	Xóa audit log.
•	Sửa trực tiếp điểm số hoặc tiến độ nếu không có quy trình điều chỉnh.
•	Truy cập dữ liệu riêng tư không có lý do nghiệp vụ.
•	Tự nâng quyền mình lên SuperAdmin.
•	Thực hiện thao tác nguy hiểm mà không xác thực lại.
________________________________________
2.9. Quản trị viên cấp cao — SuperAdmin
SuperAdmin có thể:
•	Quản lý vai trò và quyền cấp cao.
•	Quản lý cấu hình bảo mật hệ thống.
•	Xử lý tình huống khẩn cấp.
•	Cấp hoặc thu hồi quyền quản trị.
•	Quản lý break-glass access.
•	Phê duyệt các thao tác có rủi ro cao.
•	Thực hiện hoặc phê duyệt quy trình phục hồi hệ thống.
Tài khoản SuperAdmin:
•	Không được dùng cho công việc hằng ngày.
•	Bắt buộc bật MFA.
•	Có thời gian session ngắn.
•	Phải xác thực lại trước thao tác nhạy cảm.
•	Mọi thao tác phải được ghi audit log.
•	Không được dùng chung giữa nhiều người.
•	Nên được giới hạn IP hoặc áp dụng chính sách truy cập theo rủi ro.
________________________________________
3. Điều kiện đăng ký tài khoản
Người dùng được đăng ký khi:
•	Cung cấp email hợp lệ.
•	Email chưa tồn tại trong hệ thống.
•	Đồng ý với điều khoản sử dụng và chính sách riêng tư.
•	Đáp ứng yêu cầu về độ tuổi nếu hệ thống quy định.
•	Hoàn thành CAPTCHA khi hệ thống yêu cầu.
•	Không bị phát hiện là hành vi tạo tài khoản hàng loạt.
•	Không sử dụng email nằm trong danh sách bị chặn.
Ràng buộc:
•	Mỗi email chỉ được liên kết với một tài khoản chính.
•	Email phải được chuẩn hóa trước khi lưu.
•	Mật khẩu phải đạt chính sách bảo mật.
•	Không lưu mật khẩu dạng rõ.
•	Hệ thống không được tiết lộ email đã tồn tại theo cách làm lộ thông tin người dùng.
•	Tài khoản chưa xác minh email có thể bị giới hạn chức năng.
•	Link xác minh phải có thời hạn và chỉ sử dụng một lần.
________________________________________
4. Điều kiện đăng nhập
Người dùng được đăng nhập khi:
•	Tài khoản tồn tại.
•	Tài khoản chưa bị khóa.
•	Thông tin đăng nhập hợp lệ.
•	Email đã được xác minh nếu hệ thống bắt buộc.
•	Không vượt quá số lần đăng nhập sai.
•	Không bị phát hiện có hành vi bất thường nghiêm trọng.
•	Hoàn thành MFA nếu tài khoản yêu cầu.
Ràng buộc:
•	Đăng nhập sai nhiều lần phải bị rate limit hoặc khóa tạm thời.
•	Không được thông báo rõ tài khoản hay mật khẩu sai riêng biệt.
•	Access token phải có thời gian sống ngắn.
•	Refresh token phải được rotation.
•	Refresh token cũ bị sử dụng lại phải được xem là dấu hiệu đánh cắp.
•	Session bị thu hồi không được tiếp tục sử dụng.
•	Thao tác nhạy cảm phải yêu cầu xác thực lại.
________________________________________
5. Điều kiện tham gia khóa học
Người học được tham gia khóa học khi:
•	Đã đăng nhập.
•	Tài khoản đang hoạt động.
•	Khóa học đang ở trạng thái Published.
•	Khóa học còn hiệu lực.
•	Người học đáp ứng điều kiện đầu vào nếu có.
•	Người học đã mua hoặc được cấp quyền với khóa học trả phí.
•	Người học không bị giới hạn khỏi khóa học.
•	Số lượng học viên chưa vượt giới hạn nếu áp dụng.
Ràng buộc:
•	Không được truy cập khóa học Draft, Review hoặc Archived.
•	Không được truy cập trực tiếp bằng URL nếu không có quyền.
•	Quyền học phải được kiểm tra tại Backend.
•	Nội dung trả phí không được phân phối bằng URL công khai cố định.
•	Nếu quyền học hết hạn, hệ thống phải áp dụng đúng chính sách gia hạn hoặc grace period.
________________________________________
6. Điều kiện mở bài học
Một bài học được mở khi:
•	Người học đã đăng ký khóa học.
•	Bài học đã được xuất bản.
•	Các bài học tiên quyết đã hoàn thành.
•	Người học đạt điểm tối thiểu nếu có.
•	Thời gian mở bài đã đến.
•	Người học thuộc đúng lớp hoặc nhóm được phân công.
•	Bài học không bị khóa bởi quản trị viên.
Ràng buộc:
•	Frontend chỉ hiển thị trạng thái khóa.
•	Backend phải kiểm tra lại khi người dùng gọi API.
•	Không được mở bài chỉ bằng cách thay URL.
•	Việc mở bài phải dựa trên phiên bản quy tắc hiện hành.
•	Nếu bài học đổi phiên bản, hệ thống phải xác định tiến độ cũ còn hợp lệ hay không.
________________________________________
7. Điều kiện hoàn thành bài học
Một bài học chỉ được xem là hoàn thành khi đáp ứng quy tắc đã cấu hình, ví dụ:
•	Xem đủ nội dung bắt buộc.
•	Hoàn thành các phần bài tập bắt buộc.
•	Đạt điểm tối thiểu.
•	Nộp bài thành công.
•	Không có phần bắt buộc đang bỏ dở.
•	Dữ liệu hoàn thành đã được lưu thành công.
Ràng buộc:
•	Quy tắc hoàn thành do Backend quyết định.
•	Không chỉ dựa vào thao tác hiển thị trên Frontend.
•	Việc hoàn thành phải có timestamp.
•	Phải ghi nhận phiên bản bài học và phiên bản thuật toán.
•	Không được cộng tiến độ hai lần vì gửi request trùng.
•	Cần sử dụng idempotency key với thao tác hoàn thành quan trọng.
•	Nếu người dùng offline, trạng thái phải được lưu cục bộ và đồng bộ sau.
•	Không được báo “đã hoàn thành” nếu server chưa xác nhận, trừ khi giao diện ghi rõ đang chờ đồng bộ.
________________________________________
8. Điều kiện làm bài tập
Người học được làm bài tập khi:
•	Đã được cấp quyền truy cập.
•	Bài tập đang mở.
•	Chưa vượt số lần làm cho phép.
•	Bài tập chưa hết hạn.
•	Đã hoàn thành điều kiện tiên quyết.
•	Không có attempt khác đang bị khóa nếu hệ thống chỉ cho phép một attempt.
Ràng buộc:
•	Câu trả lời tạm phải được autosave.
•	Không được mất toàn bộ bài làm khi mất mạng.
•	Không được submit trùng.
•	Không được thay đổi đáp án sau khi attempt đã khóa, trừ khi quy tắc cho phép.
•	Kết quả phải gắn với phiên bản câu hỏi và quy tắc chấm điểm.
•	Admin sửa câu hỏi sau đó không được làm thay đổi attempt cũ.
•	Thời gian làm bài phải được kiểm tra phía server.
•	Không tin hoàn toàn vào đồng hồ phía client.
________________________________________
9. Điều kiện làm bài kiểm tra
Người học được bắt đầu bài kiểm tra khi:
•	Đã đăng nhập.
•	Có quyền tham gia.
•	Bài kiểm tra đang trong thời gian mở.
•	Chưa vượt số lần thi.
•	Đáp ứng điều kiện học tập.
•	Không có attempt đang hoạt động nếu quy tắc chỉ cho phép một attempt.
•	Hệ thống đã tạo snapshot đề thi thành công.
Khi bắt đầu, hệ thống phải lưu:
•	Attempt ID.
•	Phiên bản đề.
•	Danh sách câu hỏi.
•	Thứ tự câu hỏi.
•	Thứ tự đáp án nếu có random.
•	Thời điểm bắt đầu.
•	Thời điểm hết hạn.
•	Phiên bản quy tắc chấm điểm.
Ràng buộc:
•	Không được submit sau thời gian cho phép.
•	Server là nguồn xác định thời gian.
•	Không được thay đổi snapshot đề sau khi bắt đầu.
•	Mỗi attempt chỉ được chấm chính thức một lần.
•	Request submit lặp lại phải trả về cùng kết quả, không tạo thêm kết quả.
•	Nếu mất mạng, câu trả lời phải được lưu cục bộ trong phạm vi cho phép.
•	Hệ thống phải ghi nhận trường hợp gián đoạn bất thường.
•	Kết quả đã khóa không được sửa trực tiếp.
________________________________________
10. Điều kiện chấm điểm
Điểm số phải được tính khi:
•	Attempt hợp lệ.
•	Câu trả lời đã được lưu.
•	Attempt đã submit hoặc hết thời gian.
•	Quy tắc chấm điểm tồn tại.
•	Phiên bản nội dung và thuật toán xác định được.
Ràng buộc:
•	Chấm điểm chính thức phải thực hiện tại Backend.
•	Frontend chỉ được hiển thị điểm tạm nếu ghi rõ.
•	Không dùng dữ liệu do client tự khai báo làm kết quả cuối cùng.
•	Mỗi kết quả phải lưu scoring version.
•	Bài tự luận cần trạng thái chờ chấm nếu chưa thể tự động.
•	Nếu chấm lại, phải lưu lịch sử điều chỉnh.
•	Không ghi đè âm thầm kết quả cũ.
•	Mọi chỉnh sửa thủ công phải có người thực hiện, lý do và audit log.
________________________________________
11. Điều kiện lưu tiến độ
Tiến độ được ghi nhận khi:
•	Người học thực hiện hành động hợp lệ.
•	Người học có quyền với nội dung.
•	Event chưa được xử lý trước đó.
•	Phiên bản dữ liệu hợp lệ.
•	Request không bị trùng.
Ràng buộc:
•	Không chỉ lưu một phần trăm tiến độ tổng.
•	Phải theo dõi riêng từ vựng, ngữ pháp, nghe, nói, đọc, viết, chữ Hán, pinyin và thanh điệu.
•	Tiến độ phải gắn với nội dung và phiên bản thuật toán.
•	Không được giảm hoặc tăng tiến độ bất thường vì request lặp lại.
•	Redis không được là nơi lưu duy nhất của tiến độ.
•	Dữ liệu tiến độ quan trọng phải được lưu trong database.
•	Có cơ chế tính lại tiến độ từ learning events khi cần.
•	Mọi điều chỉnh thủ công phải được ghi audit.
________________________________________
12. Điều kiện sử dụng SRS
Một từ hoặc nội dung được đưa vào SRS khi:
•	Người học đã gặp nội dung trong bài học.
•	Người học chủ động lưu từ.
•	Hệ thống xác định nội dung cần ôn.
•	Nội dung còn hiệu lực.
•	Thuật toán SRS được cấu hình.
Ràng buộc:
•	Mỗi lượt review phải được lưu riêng.
•	Không được ghi đè lịch sử review.
•	Phải lưu phiên bản thuật toán SRS.
•	Review trùng do nhiều thiết bị phải được phát hiện.
•	Ngày ôn tiếp theo phải do Backend tính.
•	Người dùng có thể trì hoãn trong giới hạn cho phép.
•	Xóa từ khỏi SRS không nhất thiết xóa lịch sử học.
•	Khi thuật toán đổi phiên bản, phải có chính sách chuyển đổi dữ liệu.
________________________________________
13. Điều kiện ghi âm và luyện phát âm
Người dùng được ghi âm khi:
•	Đã đăng nhập nếu chức năng yêu cầu.
•	Đã cấp quyền microphone.
•	Trình duyệt và thiết bị hỗ trợ.
•	Đồng ý với chính sách xử lý bản ghi âm.
•	Không vượt quá dung lượng hoặc thời lượng.
•	Không vượt quota.
•	Kết nối đáp ứng yêu cầu upload hoặc có cơ chế lưu tạm.
Ràng buộc:
•	Bản ghi âm mặc định phải là dữ liệu riêng tư.
•	Không lưu bản ghi lâu hơn thời hạn đã công bố.
•	Không sử dụng bản ghi cho AI nếu chưa có sự đồng ý.
•	Không công khai URL bản ghi.
•	Phải dùng signed URL có thời hạn.
•	Phải kiểm tra MIME, magic number và dung lượng.
•	File phải được quét an toàn.
•	Người dùng phải có quyền xóa bản ghi nếu chính sách cho phép.
•	Không gửi dữ liệu không cần thiết sang nhà cung cấp AI.
________________________________________
14. Điều kiện sử dụng AI Tutor
Người dùng được sử dụng AI khi:
•	Đã đăng nhập nếu chức năng yêu cầu.
•	Tài khoản có quyền sử dụng AI.
•	Chưa vượt quota.
•	Tính năng AI đang hoạt động.
•	Người dùng đã đồng ý với điều khoản xử lý dữ liệu AI nếu cần.
•	Nội dung đầu vào không vi phạm chính sách.
Ràng buộc:
•	AI không được tự động xuất bản nội dung học.
•	Nội dung AI tạo ra phải được xem là gợi ý.
•	Nội dung dùng chính thức phải qua kiểm duyệt.
•	Không gửi mật khẩu, token hoặc dữ liệu nhạy cảm vào AI.
•	Phải có giới hạn token và chi phí.
•	Phải có timeout và fallback.
•	AI lỗi không được làm gián đoạn chức năng học cơ bản.
•	Phải lưu model version và prompt version nếu kết quả ảnh hưởng đến học tập.
•	Kết quả chấm phát âm phải ghi rõ mức độ tham khảo nếu chưa được kiểm chứng đầy đủ.
________________________________________
15. Điều kiện upload file
Người dùng được upload khi:
•	Có quyền upload.
•	File thuộc loại cho phép.
•	Kích thước nằm trong giới hạn.
•	Số lượng file không vượt giới hạn.
•	Tài khoản không bị hạn chế.
•	Mục đích upload hợp lệ.
Ràng buộc:
•	Không dùng trực tiếp tên file người dùng làm đường dẫn lưu.
•	Tên file phải được sinh lại.
•	Phải kiểm tra MIME và magic number.
•	File phải được lưu vào vùng cách ly trước.
•	Phải quét malware.
•	SVG phải được sanitize hoặc chuyển đổi.
•	ZIP phải được kiểm tra chống ZIP bomb.
•	Không cho file có quyền thực thi.
•	Không cho upload file bị cấm.
•	File lỗi phải bị từ chối và ghi log.
•	File tạm phải được tự động xóa theo thời hạn.
________________________________________
16. Điều kiện thanh toán và sử dụng nội dung trả phí
Người dùng được truy cập nội dung trả phí khi:
•	Giao dịch đã được xác nhận hợp lệ.
•	Gói học còn hiệu lực.
•	Tài khoản không bị đình chỉ.
•	Quyền truy cập đã được cấp.
•	Không có tranh chấp hoặc hoàn tiền làm mất quyền truy cập.
Ràng buộc:
•	Không tin trạng thái thanh toán do Frontend gửi.
•	Webhook phải được kiểm tra chữ ký.
•	Webhook phải chống replay.
•	Thanh toán phải có idempotency.
•	Không lưu thông tin thẻ trực tiếp.
•	Thanh toán thành công nhưng webhook lỗi phải có cơ chế đối soát.
•	Hoàn tiền phải cập nhật quyền theo chính sách.
•	Gia hạn, hủy gói và grace period phải được quy định rõ.
•	Giao dịch không được xóa khỏi lịch sử.
________________________________________
17. Điều kiện chỉnh sửa nội dung
Nội dung được chỉnh sửa khi:
•	Người dùng có quyền.
•	Nội dung ở trạng thái cho phép chỉnh sửa.
•	Không bị khóa bởi quy trình duyệt.
•	Không có xung đột phiên bản chưa xử lý.
Ràng buộc:
•	Mỗi lần sửa phải tạo version hoặc lịch sử thay đổi.
•	Không chỉnh sửa trực tiếp bản Published đang phục vụ người học nếu làm thay đổi kết quả lịch sử.
•	Thay đổi quan trọng phải tạo revision mới.
•	Nội dung đã xuất bản phải đi lại quy trình duyệt khi thay đổi.
•	Phải kiểm tra optimistic concurrency.
•	Nếu có người khác đã sửa trước, hệ thống phải cảnh báo.
•	Không được ghi đè âm thầm dữ liệu mới hơn.
________________________________________
18. Điều kiện duyệt nội dung
Nội dung được duyệt khi:
•	Đang ở trạng thái Review.
•	Người duyệt có quyền.
•	Người duyệt không vi phạm quy tắc phân tách trách nhiệm.
•	Nội dung đạt kiểm tra dữ liệu bắt buộc.
•	Có nguồn và giấy phép nếu cần.
•	Không có lỗi Critical.
•	Các media liên quan tồn tại và hợp lệ.
Ràng buộc:
•	Phê duyệt hoặc từ chối phải có timestamp.
•	Từ chối phải có lý do.
•	Không xóa lịch sử duyệt.
•	Nội dung AI tạo phải được đánh dấu.
•	Nội dung quan trọng có thể yêu cầu hai người duyệt.
•	Người tạo không được tự duyệt nếu quy trình cấm.
________________________________________
19. Điều kiện xuất bản nội dung
Nội dung được xuất bản khi:
•	Đã được phê duyệt.
•	Không còn lỗi Critical.
•	Các dependency hợp lệ.
•	Media đầy đủ.
•	Nguồn và bản quyền hợp lệ.
•	Taxonomy mapping hợp lệ.
•	Người thực hiện có quyền Publisher.
•	Thời gian xuất bản phù hợp.
Ràng buộc:
•	Không xuất bản nội dung Draft.
•	Không xuất bản nội dung đã hết giấy phép.
•	Không cho phép tham chiếu nội dung bị xóa hoặc Archived.
•	Phải lưu version Published.
•	Phải hỗ trợ rollback.
•	Phải ghi audit log.
•	Việc publish phải làm mới hoặc vô hiệu cache đúng cách.
•	Không được để người dùng thấy dữ liệu nửa cũ, nửa mới.
________________________________________
20. Ràng buộc dữ liệu
20.1. Ràng buộc chung
•	Mọi thực thể công khai sử dụng UUID.
•	Khóa nội bộ không được lộ ra API nếu không cần.
•	Foreign key phải hợp lệ.
•	Dữ liệu bắt buộc không được null.
•	Giá trị duy nhất phải có unique constraint.
•	Trạng thái phải thuộc danh sách hợp lệ.
•	Dữ liệu Unicode phải được chuẩn hóa.
•	Timestamp phải thống nhất múi giờ lưu trữ.
•	Dữ liệu nhạy cảm phải được mã hóa phù hợp.
•	Không lưu dữ liệu thừa không có mục đích.
20.2. Ràng buộc dữ liệu tiếng Trung
Một mục từ chỉ được xuất bản khi có tối thiểu:
•	Chữ giản thể hoặc dạng chính.
•	Pinyin chuẩn.
•	Nghĩa tiếng Việt.
•	Loại từ.
•	Nguồn dữ liệu.
•	Trạng thái duyệt.
•	Mapping cấp độ nếu áp dụng.
Nếu có dữ liệu nâng cao cần kiểm tra:
•	Giản thể và phồn thể.
•	Pinyin có dấu.
•	Pinyin dạng số.
•	Âm Hán Việt.
•	Bộ thủ.
•	Số nét.
•	Thứ tự nét.
•	Câu ví dụ.
•	Audio.
•	HSK version.
•	CEFR hoặc chuẩn tương đương.
20.3. Ràng buộc quan hệ nội dung
•	Một bài học phải thuộc chương hoặc cấu trúc hợp lệ.
•	Một chương phải thuộc khóa học.
•	Không được tạo vòng lặp prerequisite.
•	Không được tham chiếu media không tồn tại.
•	Không được tham chiếu nội dung Draft trong bài Published.
•	Không xóa nội dung đang được tham chiếu.
•	Việc archive phải kiểm tra ảnh hưởng tới khóa học và tiến độ.
________________________________________
21. Ràng buộc bảo mật
•	Mọi request phải được kiểm tra xác thực và phân quyền nếu cần.
•	Không tin dữ liệu từ Frontend.
•	Không lưu mật khẩu dạng rõ.
•	Không ghi token, mật khẩu hoặc secret vào log.
•	Không cho phép truy cập database trực tiếp từ Internet.
•	Không dùng tài khoản database quyền cao cho runtime.
•	Không dùng chung tài khoản Admin.
•	Không cho phép bypass quyền bằng cách thay UUID.
•	Không trả stack trace ở môi trường production.
•	Không để file riêng tư trong public bucket.
•	Không để secret trong source code.
•	Mọi thao tác quản trị nhạy cảm phải có audit log.
•	MFA bắt buộc với tài khoản quản trị.
•	Hành động nhạy cảm phải yêu cầu xác thực lại.
•	Hệ thống phải có rate limiting.
•	Hệ thống phải chống bot, credential stuffing và tạo tài khoản hàng loạt.
•	Hệ thống phải sử dụng HTTPS.
•	CORS phải theo allowlist.
•	Security headers phải được cấu hình.
•	Upload phải được kiểm tra an toàn.
•	Dependency và container phải được quét lỗ hổng.
________________________________________
22. Ràng buộc hiệu năng
Hệ thống cần đáp ứng các mục tiêu tối thiểu:
•	Trang chính tải nhanh trên mạng phổ biến.
•	Chuyển bài học không bị giật.
•	Audio bắt đầu phát trong thời gian chấp nhận được.
•	Canvas viết chữ không bị trễ rõ rệt.
•	Autosave không làm khóa giao diện.
•	Không tải toàn bộ khóa học cùng lúc.
•	API danh sách phải phân trang.
•	Search phải debounce.
•	Request cũ phải được hủy khi không còn cần.
•	Media phải được phân phối qua CDN.
•	Truy vấn đọc phổ biến phải được tối ưu.
•	Truy vấn chậm phải được giám sát.
•	Cache phải có TTL và chiến lược invalidation.
•	Không cache dữ liệu cá nhân nhạy cảm tại CDN.
•	Công việc nặng phải chạy background job.
Mục tiêu tham khảo:
•	LCP dưới 2,5 giây.
•	INP dưới 200 ms.
•	CLS dưới 0,1.
•	P95 API thông thường dưới 500 ms.
•	Audio bắt đầu phát dưới 1,5 giây trong điều kiện mạng phù hợp.
•	Tỷ lệ lưu bài học thành công tối thiểu 99,9%.
________________________________________
23. Ràng buộc đồng bộ và offline
•	Người dùng không được mất toàn bộ bài làm khi mất mạng.
•	Dữ liệu tạm phải được lưu cục bộ.
•	Mỗi thao tác offline phải có client-generated ID.
•	Request đồng bộ phải có idempotency key.
•	Dữ liệu phải có version.
•	Không ghi đè dữ liệu mới hơn mà không cảnh báo.
•	Hệ thống phải có quy tắc xử lý conflict.
•	Phải hiển thị trạng thái lưu và đồng bộ.
•	Queue offline phải được gửi lại khi có mạng.
•	Thao tác đã xử lý không được thực hiện lại.
•	Dữ liệu nhạy cảm lưu offline phải được hạn chế hoặc mã hóa.
•	Khi đăng xuất, dữ liệu offline phải được xử lý theo chính sách.
•	Khi hai thiết bị cùng cập nhật, Backend phải quyết định kết quả cuối cùng.
________________________________________
24. Ràng buộc logging và audit
Hệ thống phải ghi nhận:
•	Đăng nhập và đăng xuất.
•	Đăng nhập thất bại.
•	Thu hồi session.
•	Thay đổi quyền.
•	Khóa hoặc mở tài khoản.
•	Tạo, sửa, duyệt và xuất bản nội dung.
•	Điều chỉnh điểm số.
•	Xuất dữ liệu.
•	Xóa dữ liệu.
•	Thao tác thanh toán.
•	Thao tác khôi phục.
•	Thay đổi cấu hình hệ thống.
•	Kích hoạt break-glass access.
Audit log phải có:
•	Người thực hiện.
•	Hành động.
•	Đối tượng.
•	Thời gian.
•	Kết quả.
•	Địa chỉ hoặc thông tin phiên phù hợp.
•	Dữ liệu trước và sau nếu được phép.
•	Lý do với thao tác nhạy cảm.
•	Trace ID.
Audit log không được cho người dùng thông thường chỉnh sửa hoặc xóa.
________________________________________
25. Ràng buộc hệ thống khi có lỗi
•	AI lỗi không được làm sập bài học.
•	Email lỗi không được làm mất tài khoản vừa tạo.
•	Notification lỗi không được rollback tiến độ học.
•	Redis lỗi không được làm mất dữ liệu chính.
•	Queue lỗi phải có retry.
•	Job lỗi nhiều lần phải đưa vào dead-letter queue.
•	Database lỗi phải trả thông báo an toàn.
•	Không hiển thị thông tin kỹ thuật nội bộ.
•	Frontend phải có fallback UI.
•	Người dùng phải biết dữ liệu đã lưu hay chưa.
•	Hệ thống phải hỗ trợ retry có kiểm soát.
•	Không retry tự động với thao tác không idempotent.
•	Mỗi lỗi quan trọng phải có trace ID để hỗ trợ.
________________________________________
26. Ràng buộc quyền riêng tư
•	Chỉ thu thập dữ liệu cần thiết.
•	Người dùng phải biết dữ liệu nào được thu thập.
•	Phải có chính sách lưu trữ dữ liệu.
•	Phải có cơ chế xuất dữ liệu cá nhân.
•	Phải có cơ chế yêu cầu xóa tài khoản.
•	Bản ghi âm phải có chính sách riêng.
•	Dữ liệu không được dùng cho AI nếu chưa có cơ sở hợp lệ hoặc sự đồng ý.
•	Không chia sẻ dữ liệu cho bên thứ ba ngoài phạm vi công bố.
•	Dữ liệu trẻ em phải có chính sách phù hợp nếu hệ thống phục vụ người chưa thành niên.
•	Log và analytics phải hạn chế dữ liệu định danh không cần thiết.
•	Xóa tài khoản phải xử lý cả database, storage và dữ liệu liên quan theo chính sách.
________________________________________
27. Hành vi bị cấm
Người dùng không được:
•	Tấn công, dò quét hoặc khai thác lỗ hổng.
•	Truy cập trái phép tài khoản người khác.
•	Chia sẻ hoặc mua bán tài khoản trái quy định.
•	Cố tình vượt rate limit.
•	Tự động scraping nội dung.
•	Tải lên malware hoặc file nguy hiểm.
•	Giả mạo giáo viên hoặc quản trị viên.
•	Thao túng điểm số, streak hoặc chứng chỉ.
•	Lạm dụng AI, audio hoặc tài nguyên tính toán.
•	Sao chép hoặc phát tán nội dung có bản quyền trái phép.
•	Gửi spam.
•	Quấy rối người dùng khác.
•	Đăng nội dung vi phạm pháp luật hoặc chính sách.
•	Can thiệp vào hoạt động bình thường của hệ thống.
Hệ thống có quyền:
•	Giới hạn chức năng.
•	Yêu cầu CAPTCHA.
•	Thu hồi session.
•	Khóa tạm thời.
•	Khóa vĩnh viễn.
•	Xóa nội dung vi phạm.
•	Lưu bằng chứng phục vụ điều tra.
•	Thông báo cho quản trị viên hoặc cơ quan có thẩm quyền khi cần thiết.
________________________________________
28. Điều kiện nghiệm thu nghiệp vụ
Một chức năng chỉ được xem là hoàn thành khi:
•	Có yêu cầu nghiệp vụ rõ ràng.
•	Có điều kiện trước và sau.
•	Có phân quyền.
•	Có validation.
•	Có xử lý lỗi.
•	Có audit nếu là thao tác nhạy cảm.
•	Có test case.
•	Có kiểm thử quyền truy cập.
•	Có kiểm thử dữ liệu không hợp lệ.
•	Có kiểm thử request trùng.
•	Có kiểm thử mất mạng nếu liên quan.
•	Có log và metric cần thiết.
•	Có tiêu chí hiệu năng.
•	Có hướng xử lý rollback hoặc recovery.
•	Được nghiệm thu trên môi trường staging.
________________________________________
29. Ma trận thao tác tổng quát
Chức năng	Guest	Learner	Teacher	Editor	Reviewer	Publisher	Admin
Xem nội dung công khai	Có	Có	Có	Có	Có	Có	Có
Học và lưu tiến độ	Không	Có	Có	Theo quyền	Theo quyền	Theo quyền	Theo quyền
Làm bài tập	Không	Có	Có	Theo quyền	Theo quyền	Theo quyền	Theo quyền
Xem tiến độ cá nhân	Không	Có	Có	Không	Không	Không	Theo quyền
Xem tiến độ lớp	Không	Không	Có	Không	Không	Không	Theo quyền
Tạo nội dung	Không	Không	Theo quyền	Có	Theo quyền	Theo quyền	Có
Duyệt nội dung	Không	Không	Không	Không	Có	Theo quyền	Có
Xuất bản nội dung	Không	Không	Không	Không	Không	Có	Có
Quản lý người dùng	Không	Không	Không	Không	Không	Không	Có
Thay đổi quyền	Không	Không	Không	Không	Không	Không	Theo quyền cao
Xem audit log	Không	Không	Không	Hạn chế	Hạn chế	Hạn chế	Có
Quản lý hệ thống	Không	Không	Không	Không	Không	Không	Có
Lưu ý: “Có” trong ma trận không thay thế kiểm tra chi tiết theo Permission, Ownership, phạm vi dữ liệu và trạng thái tài nguyên.
________________________________________
30. Nguyên tắc kiểm tra quyền cuối cùng
Mỗi thao tác phải được kiểm tra theo công thức:
Đã xác thực
+ Có permission
+ Đúng vai trò
+ Đúng chủ sở hữu hoặc phạm vi quản lý
+ Tài nguyên đang ở trạng thái hợp lệ
+ Đáp ứng điều kiện nghiệp vụ
+ Không vượt quota hoặc giới hạn
+ Request hợp lệ và không trùng
Ví dụ người dùng có quyền content.edit vẫn không được sửa nội dung nếu:
•	Nội dung thuộc đơn vị khác.
•	Nội dung đang bị khóa duyệt.
•	Nội dung đã Archived.
•	Người dùng chỉ có quyền với một nhóm nội dung khác.
•	Phiên bản dữ liệu đã cũ.
•	Tài khoản đang bị hạn chế.
________________________________________
31. Kết luận
Hệ thống phải bảo đảm ba nguyên tắc:
1.	Người dùng chỉ được thực hiện đúng chức năng được cấp quyền.
2.	Mọi điều kiện nghiệp vụ quan trọng phải được Backend kiểm tra.
3.	Mọi thao tác nhạy cảm phải có validation, authorization, audit và khả năng truy vết.
Frontend không được xem là lớp bảo mật. Việc ẩn nút hoặc ẩn màn hình chỉ giúp cải thiện trải nghiệm; Backend vẫn phải từ chối mọi request không hợp lệ.
1. Phần API còn thiếu
Tài liệu đã có các nguyên tắc API chung nhưng chưa có API Catalog hoàn chỉnh.
1.1. Danh mục endpoint
Cần bổ sung bảng cho từng API:
Thuộc tính	Nội dung
API ID	Ví dụ AUTH-001
Method	GET, POST, PUT, PATCH, DELETE
Path	/api/v1/auth/login
Actor	Guest, Learner, Admin
Permission	Quyền bắt buộc
Request schema	Cấu trúc request
Response schema	Cấu trúc response
Error codes	400, 401, 403, 409, 422...
Idempotency	Có hay không
Rate limit	Giới hạn riêng
Transaction	Phạm vi transaction
Audit	Có ghi audit hay không
Cache	Có cache hay không
SLO	Mục tiêu latency
Test case	Bộ test tương ứng
1.2. Thiếu nhóm API hệ thống
Nên bổ sung rõ:
/api/v1/health/live
/api/v1/health/ready
/api/v1/version
/api/v1/config/public
/api/v1/auth/sessions
/api/v1/auth/sessions/{uuid}/revoke
/api/v1/users/me/data-export
/api/v1/users/me/deletion-request
/api/v1/sync/batch
/api/v1/sync/status
/api/v1/uploads/init
/api/v1/uploads/complete
/api/v1/webhooks/{provider}
1.3. API governance
Cần thêm:
•	Quy tắc đặt tên endpoint. 
•	Quy tắc dùng PUT và PATCH. 
•	Chuẩn filter và sort. 
•	Cursor pagination và keyset pagination. 
•	Maximum page size. 
•	API deprecation policy. 
•	Thời gian hỗ trợ API cũ. 
•	OpenAPI breaking-change check. 
•	Contract testing. 
•	Client-version header. 
•	ETag và If-None-Match. 
•	Retry-After. 
•	Request body và header size limits. 
•	API changelog. 
Tài liệu có đề cập compatibility matrix và client version, nhưng cần gom thành một chương API Governance độc lập. 
1.4. Chuẩn lỗi nghiệp vụ
Không chỉ dùng HTTP status. Cần mã lỗi ổn định:
{
  "type": "https://errors.example.com/lesson-locked",
  "title": "Bài học chưa được mở",
  "status": 403,
  "code": "LEARNING.LESSON_LOCKED",
  "traceId": "...",
  "errors": []
}
Nên lập danh mục:
•	AUTH.INVALID_CREDENTIALS 
•	AUTH.SESSION_REVOKED 
•	CONTENT.VERSION_CONFLICT 
•	LEARNING.LESSON_LOCKED 
•	ASSESSMENT.ATTEMPT_EXPIRED 
•	SYNC.CONFLICT 
•	PAYMENT.ALREADY_PROCESSED 
2. Backend còn thiếu
2.1. Module ownership
Tài liệu cần có bảng module sở hữu dữ liệu:
Identity          → Users, Roles, Sessions
Dictionary        → Words, Characters, Grammar
Learning Content  → Courses, Chapters, Lessons
Assessment        → Questions, Attempts, Answers
Progress          → Progress, Mastery, Streak
SRS               → Cards, Reviews, Schedules
Media             → Assets, Uploads, Recordings
Governance        → Reviews, Revisions, Sources
Không module nào được tự ý sửa bảng của module khác.
2.2. Aggregate và transaction boundary
Cần xác định aggregate root cho:
•	User. 
•	Course. 
•	Lesson. 
•	ContentRevision. 
•	AssessmentAttempt. 
•	Enrollment. 
•	SRSCard. 
•	PaymentOrder. 
Mỗi command phải ghi rõ:
•	Dữ liệu được thay đổi. 
•	Transaction boundary. 
•	Concurrency token. 
•	Domain events phát sinh. 
•	Điều kiện rollback. 
2.3. Transactional Outbox và Inbox
Đây là phần còn thiếu quan trọng nhất.
Khi lưu tiến độ rồi phát event tạo SRS, gửi thông báo hoặc analytics, cần đảm bảo database và message queue không lệch nhau.
Bổ sung:
•	Outbox table. 
•	Outbox processor. 
•	Event ID. 
•	Consumer inbox. 
•	Duplicate detection. 
•	Retry/backoff. 
•	Dead-letter handling. 
•	Event schema version. 
•	Event retention. 
Tài liệu hiện đã liệt kê Transactional Outbox là ưu tiên P0 nhưng chưa đặc tả cách triển khai. 
2.4. Job governance
Ngoài retry và dead-letter queue, cần quy định:
•	Job uniqueness. 
•	Job priority. 
•	Maximum retry. 
•	Retryable và non-retryable errors. 
•	Job lease/lock. 
•	Poison message. 
•	Job payload version. 
•	Cancellation semantics. 
•	Manual replay. 
•	Batch size. 
•	Worker concurrency. 
•	Queue isolation. 
Nên tách queue:
critical
default
email
media
analytics
ai
imports
2.5. Feature flags
Cần thêm:
•	Flag owner. 
•	Ngày hết hạn. 
•	Môi trường áp dụng. 
•	Phần trăm rollout. 
•	Nhóm người dùng. 
•	Audit thay đổi flag. 
•	Fallback khi dịch vụ flag lỗi. 
•	Quy trình xóa flag cũ. 
2.6. Configuration governance
Phân loại:
•	Static configuration. 
•	Runtime configuration. 
•	Secret. 
•	Feature flag. 
•	Business rule. 
•	Tenant configuration. 
Không nên lưu mọi thứ chung trong một bảng Settings.
3. Database còn thiếu
3.1. ERD và data dictionary
Tài liệu chưa có:
•	ERD hoàn chỉnh. 
•	Danh sách bảng. 
•	Mô tả từng cột. 
•	Kiểu dữ liệu. 
•	Nullable. 
•	Default value. 
•	Constraint. 
•	Index. 
•	Dữ liệu nhạy cảm. 
•	Owner module. 
•	Retention. 
•	Audit requirement. 
Đây là tài liệu bắt buộc trước khi xây database lớn.
3.2. Quy ước dữ liệu
Cần chốt:
•	Quy tắc đặt tên bảng và cột. 
•	Dùng snake_case hay PascalCase. 
•	timestamp with time zone. 
•	Quy ước lưu UTC. 
•	Collation. 
•	Unicode normalization. 
•	Decimal precision. 
•	Money representation. 
•	JSONB được dùng khi nào. 
•	Enum trong database hay lookup table. 
•	UUID generation. 
•	Soft-delete policy. 
3.3. Data classification
Phân loại từng trường:
Public
Internal
Confidential
Restricted
Ví dụ:
Dữ liệu	Mức
Tên hiển thị	Internal
Email	Confidential
Mật khẩu hash	Restricted
Refresh-token hash	Restricted
Recording	Restricted
Learning analytics	Confidential
Nội dung công khai	Public
Từ đó xác định:
•	Mã hóa. 
•	Quyền xem. 
•	Log redaction. 
•	Retention. 
•	Backup. 
•	Export. 
3.4. Encryption
Cần ghi rõ:
•	Encryption at rest. 
•	Encryption in transit. 
•	Backup encryption. 
•	Field-level encryption. 
•	Key rotation. 
•	Key ownership. 
•	Key recovery. 
•	Không lưu encryption key cùng database. 
3.5. Partitioning và archival
Những bảng lớn cần thiết kế sẵn khả năng partition:
•	LearningEvents. 
•	AuditLogs. 
•	SecurityEvents. 
•	Attempts. 
•	Answers. 
•	Notifications. 
•	SRSReviews. 
•	AIUsage. 
•	RequestLogs. 
Nên quy định:
•	Partition theo tháng/quý. 
•	Khi nào tạo partition. 
•	Khi nào archive. 
•	Khi nào xóa. 
•	Cách truy vấn dữ liệu archive. 
3.6. Database migration
Tài liệu đã có expand–migrate–contract nhưng cần thêm checklist chi tiết:
•	Migration có backward compatibility. 
•	Không đổi tên/xóa cột ngay. 
•	Có dry-run. 
•	Có backup trước migration nguy hiểm. 
•	Có thời gian lock ước tính. 
•	Có kế hoạch backfill. 
•	Backfill có resume. 
•	Có verification query. 
•	Có rollback hoặc forward-fix. 
•	Migration runtime dùng tài khoản riêng. 
Database security và migration safety đã được tài liệu đề cập nhưng chưa chuyển thành biểu mẫu nghiệm thu. 
3.7. Data quality
Ngoài content QA, cần database data-quality rules:
•	Orphan records. 
•	Duplicate UUID. 
•	Invalid state transition. 
•	Attempt thiếu snapshot. 
•	Progress không khớp learning event. 
•	SRS due date bất thường. 
•	Published content không có revision. 
•	Payment không có reconciliation status. 
•	File database record không có object storage tương ứng. 
•	Object storage file không có database record. 
4. Frontend còn thiếu
4.1. Kiến trúc thư mục và dependency rule
Cần quy định:
app/
features/
shared/
entities/
services/
config/
tests/
Và ràng buộc:
•	Feature không import ngược từ app. 
•	Shared không chứa business logic. 
•	Component không gọi API trực tiếp. 
•	Không đặt toàn bộ state trong global store. 
•	Không lưu token nhạy cảm trong LocalStorage. 
4.2. Design token
Design system cần thêm:
•	Color tokens. 
•	Spacing tokens. 
•	Typography tokens. 
•	Border radius. 
•	Shadow. 
•	Motion duration. 
•	Breakpoints. 
•	Z-index scale. 
•	Component states. 
•	Dark/light theme contract. 
4.3. Internationalization
Vì là hệ thống học tiếng Trung, nên bổ sung:
•	UI language. 
•	Content language. 
•	Locale. 
•	Time zone. 
•	Number/date formatting. 
•	Simplified/traditional preference. 
•	Pinyin display preference. 
•	Vietnamese meaning preference. 
•	Fallback language. 
•	Translation-key versioning. 
4.4. Frontend security
Nên có chương riêng:
•	CSP nonce. 
•	Trusted Types nếu phù hợp. 
•	Không render HTML thô. 
•	URL sanitization. 
•	Safe redirect allowlist. 
•	Không để secret trong frontend environment. 
•	Source-map access policy. 
•	Không log PII trên trình duyệt. 
•	CSRF strategy. 
•	Clickjacking protection. 
•	Dependency scanning. 
•	Third-party script governance. 
4.5. Service Worker
Tài liệu đề cập Service Worker nhưng chưa quy định:
•	Asset cache strategy. 
•	API cache strategy. 
•	Không cache response nhạy cảm. 
•	Update lifecycle. 
•	Skip waiting policy. 
•	Force reload. 
•	Cache version. 
•	Cache cleanup. 
•	Offline fallback. 
•	Background sync. 
•	Xử lý client cũ. 
4.6. Analytics governance
Cần thêm:
•	Event naming convention. 
•	Event schema. 
•	Required fields. 
•	Consent. 
•	PII restrictions. 
•	Event deduplication. 
•	Session definition. 
•	Bot filtering. 
•	Data retention. 
•	Event version. 
•	Kiểm thử analytics event. 
4.7. UX measurement
Tài liệu đã có UX khá đầy đủ, nhưng còn thiếu:
•	Task completion rate. 
•	Time to first lesson. 
•	Onboarding abandonment. 
•	Lesson abandonment. 
•	Search success rate. 
•	Error recovery rate. 
•	Recording completion rate. 
•	User satisfaction. 
•	Accessibility feedback. 
•	UX test với người dùng thật. 
5. Hạ tầng còn thiếu
5.1. Deployment topology theo từng môi trường
Cần vẽ riêng cho:
Local
Development
Testing
Staging
Production
Disaster Recovery
Mỗi môi trường ghi rõ:
•	Domain. 
•	Network. 
•	Compute. 
•	Database. 
•	Redis. 
•	Storage. 
•	Queue. 
•	Monitoring. 
•	Secrets. 
•	Data source. 
•	Quyền truy cập. 
5.2. Network architecture
Tài liệu đã có network segmentation, private database và bastion, nhưng cần sơ đồ chi tiết. 
Cần bổ sung:
•	Ingress rules. 
•	Egress rules. 
•	Security groups. 
•	Firewall matrix. 
•	Private endpoints. 
•	NAT gateway. 
•	DNS resolution. 
•	Admin access path. 
•	Database access path. 
•	Worker access path. 
•	Vendor API outbound path. 
5.3. DNS và certificate
Thiếu checklist:
•	DNS provider. 
•	TTL policy. 
•	DNSSEC nếu phù hợp. 
•	Certificate issuer. 
•	Auto-renewal. 
•	Certificate expiry alert. 
•	CAA record. 
•	Subdomain inventory. 
•	Domain takeover prevention. 
•	Staging domain không được index. 
5.4. Container runtime hardening
Bổ sung:
•	Chạy non-root. 
•	Read-only filesystem. 
•	Drop Linux capabilities. 
•	Resource requests/limits. 
•	Health check. 
•	Graceful shutdown. 
•	Không chứa secret trong image. 
•	Minimal base image. 
•	Image signing. 
•	SBOM. 
•	Image provenance. 
•	Không dùng tag latest. 
5.5. Capacity planning
Tài liệu đã gợi ý các mức 100, 1.000 và 10.000 người dùng đồng thời. Cần chuyển thành bảng:
Tải	API instance	DB connection	Worker	Redis	Bandwidth
100 concurrent	TBD	TBD	TBD	TBD	TBD
1.000 concurrent	TBD	TBD	TBD	TBD	TBD
10.000 concurrent	TBD	TBD	TBD	TBD	TBD
5.6. Infrastructure as Code governance
Thiếu:
•	State backend. 
•	State locking. 
•	State encryption. 
•	Module version. 
•	Plan review. 
•	Drift detection. 
•	Policy as Code. 
•	Environment parameterization. 
•	Không sửa production ngoài IaC. 
•	Import tài nguyên tạo thủ công. 
•	Rollback infrastructure. 
6. Security còn thiếu
6.1. Security requirement matrix
Mỗi module cần một bảng:
Module	Asset	Threat	Control	Test
Identity	Session	Token theft	Rotation	Reuse test
Upload	File	Malware	Scan	Malicious upload
Admin	Permission	Escalation	MFA + RBAC	Privilege test
6.2. OWASP API Security
Tài liệu cần đối chiếu rõ:
•	BOLA/IDOR. 
•	Broken authentication. 
•	Broken object property authorization. 
•	Unrestricted resource consumption. 
•	Broken function-level authorization. 
•	SSRF. 
•	Security misconfiguration. 
•	Improper inventory management. 
•	Unsafe consumption of APIs. 
Chống BOLA/IDOR đã được đề cập, nhưng nên biến thành test bắt buộc cho mọi endpoint dùng UUID. 
6.3. Secret lifecycle
Không chỉ lưu secret trong vault. Cần:
•	Ai được tạo. 
•	Ai được đọc. 
•	Cách rotate. 
•	Chu kỳ rotate. 
•	Expiration alert. 
•	Emergency revoke. 
•	Secret inventory. 
•	Không dùng chung giữa môi trường. 
•	Audit truy cập. 
•	Quy trình khi secret bị lộ. 
6.4. Vulnerability management
Bổ sung:
•	Severity classification. 
•	SLA xử lý: 
o	Critical. 
o	High. 
o	Medium. 
o	Low. 
•	Exception approval. 
•	Risk acceptance expiry. 
•	Retest. 
•	Patch window. 
•	Emergency patch. 
•	Asset owner. 
6.5. Data Loss Prevention
Nên có:
•	Giới hạn export. 
•	Watermark báo cáo nhạy cảm. 
•	Cảnh báo export hàng loạt. 
•	Mask dữ liệu. 
•	Download signed URL. 
•	Audit download. 
•	Hạn sử dụng file export. 
•	Không gửi file chứa dữ liệu nhạy cảm qua email thường. 
7. Testing còn thiếu
Tài liệu đã có load, stress, spike, soak, security và resilience test. 
Nhưng cần thêm:
7.1. Test strategy
•	Test pyramid. 
•	Ownership. 
•	Môi trường test. 
•	Test data. 
•	Coverage target. 
•	Khi nào chạy. 
•	Điều kiện pass/fail. 
•	Flaky-test policy. 
•	Test evidence retention. 
7.2. Backend tests
•	Domain invariant tests. 
•	Authorization matrix tests. 
•	Database constraint tests. 
•	Migration tests. 
•	Outbox/inbox tests. 
•	Idempotency tests. 
•	Concurrent update tests. 
•	Retry tests. 
•	Contract tests. 
•	Webhook replay tests. 
7.3. Data tests
•	Schema validation. 
•	Referential integrity. 
•	Duplicate detection. 
•	Content completeness. 
•	Reconciliation. 
•	Data drift. 
•	Backup consistency. 
•	Restore verification. 
7.4. Release gates
Tài liệu đã có các gate Security, Performance, Reliability, UX và Operations. Nên thêm:
•	Database migration gate. 
•	Privacy/compliance gate. 
•	Content-quality gate. 
•	Analytics correctness gate. 
•	Cost gate. 
•	Rollback rehearsal gate. 
8. Observability còn thiếu
8.1. Log schema
Cần chuẩn hóa:
timestamp
level
service
environment
version
requestId
traceId
userIdHash
eventName
errorCode
duration
result
8.2. Log redaction
Quy định không log:
•	Mật khẩu. 
•	Access token. 
•	Refresh token. 
•	Cookie. 
•	Authorization header. 
•	Recording URL. 
•	Full payment information. 
•	Nội dung nhạy cảm do người dùng nhập. 
8.3. Dashboard cụ thể
Cần danh mục dashboard:
•	API overview. 
•	Authentication. 
•	Database. 
•	Redis. 
•	Queue. 
•	Storage. 
•	Media. 
•	Learning progress. 
•	SRS. 
•	Content health. 
•	Payments. 
•	AI usage. 
•	Security. 
•	Cost. 
8.4. Alert routing
Mỗi alert cần:
•	Severity. 
•	Threshold. 
•	Duration. 
•	Owner. 
•	Notification channel. 
•	Runbook. 
•	Auto-remediation. 
•	Escalation. 
•	Suppression rule. 
9. Backup và disaster recovery còn thiếu
Tài liệu đã đề cập RPO, RTO, restore test và business continuity. 
Cần thêm ma trận cụ thể:
Thành phần	RPO	RTO	Backup	Restore test
PostgreSQL	TBD	TBD	PITR	Hàng quý
Object Storage	TBD	TBD	Versioning/replication	Hàng quý
Redis	Không phải nguồn chính	TBD	Tùy nhu cầu	Failure test
Secrets	TBD	TBD	Secure export	Định kỳ
IaC state	TBD	TBD	Versioned	Định kỳ
Bổ sung:
•	Backup immutability. 
•	Backup ngoài tài khoản/region chính. 
•	Quyền restore. 
•	Restore approval. 
•	Restore audit. 
•	Data reconciliation sau restore. 
•	DR communication plan. 
•	DR drill report. 
10. Vận hành còn thiếu
10.1. Runbook cụ thể
Cần runbook cho:
•	API lỗi diện rộng. 
•	Database đầy connection. 
•	Slow query. 
•	Redis down. 
•	Queue backlog. 
•	Worker chết. 
•	Storage lỗi. 
•	CDN lỗi. 
•	DNS lỗi. 
•	Token bị lộ. 
•	Tài khoản admin bị chiếm. 
•	Payment mismatch. 
•	Nội dung sai đã publish. 
•	Người dùng mất tiến độ. 
•	AI tăng chi phí bất thường. 
10.2. Support operations
Bổ sung:
•	Ticket severity. 
•	SLA phản hồi. 
•	SLA xử lý. 
•	Escalation. 
•	Mẫu yêu cầu thông tin. 
•	Không yêu cầu người dùng gửi mật khẩu. 
•	Identity verification trước hỗ trợ tài khoản. 
•	Quy trình điều chỉnh tiến độ. 
•	Quy trình bồi hoàn. 
•	Audit thao tác support. 
10.3. Change management
•	Change request. 
•	Risk level. 
•	Reviewer. 
•	Maintenance window. 
•	Communication. 
•	Rollback plan. 
•	Post-deployment verification. 
•	Emergency change process. 
•	Audit. 
11. Nội dung tài liệu cần chỉnh cấu trúc
Tài liệu hiện có nhiều lần:
•	Đánh giá lại cùng một nội dung. 
•	Lặp “cần bổ sung”. 
•	Trộn checklist, giải thích kiến trúc và quy định người dùng. 
•	Một số bảng bị lỗi định dạng khi chuyển sang DOCX. 
•	Một số tiêu đề không tách trang hoặc không đánh số đồng nhất. 
Nên chia thành các tài liệu:
01_Product_and_Business_Requirements
02_System_Architecture
03_Backend_and_API_Specification
04_Database_and_Data_Architecture
05_Frontend_Architecture
06_Infrastructure_and_DevOps
07_Security_and_Privacy
08_Test_Strategy
09_Operations_and_Incident_Response
10_Content_and_Learning_Engine
11_User_Roles_Rules_and_Permissions
12_Production_Readiness_Checklist
Kết luận ưu tiên
P0 — cần bổ sung trước khi xem là hoàn thiện checklist
1.	API Catalog và chuẩn mã lỗi. 
2.	ERD, data dictionary và database ownership. 
3.	Transactional Outbox/Inbox. 
4.	Module boundaries và transaction boundaries. 
5.	Offline synchronization protocol. 
6.	Security requirement matrix và BOLA tests. 
7.	Network topology và firewall matrix. 
8.	Migration runbook theo expand–migrate–contract. 
9.	Backup/DR matrix có RPO và RTO cụ thể. 
10.	Release gates có bằng chứng nghiệm thu. 
11.	Log schema và dữ liệu bắt buộc phải che. 
12.	Owner, acceptance criteria, test evidence cho từng checklist item. 
P1 — cần cho vận hành ổn định
1.	Partitioning và archival strategy. 
2.	Analytics event specification. 
3.	Capacity model. 
4.	Feature-flag governance. 
5.	Secret lifecycle. 
6.	Vulnerability management SLA. 
7.	Support SLA. 
8.	Change-management workflow. 
9.	Data-quality reconciliation. 
10.	UX metrics và Real User Monitoring.
I. Product Specification (Thiếu gần như hoàn toàn)
Hiện tại có nhắc đến PRD/BRD nhưng chưa có nội dung thực tế. 
Nên bổ sung:
•	Product Vision 
•	Product Goal 
•	Product KPI 
•	Business Objective 
•	Personas 
•	User Segments 
•	User Journey 
•	User Story 
•	Functional Requirement 
•	Non-functional Requirement 
•	Success Metrics 
•	Release Roadmap 
•	Product Constraints 
________________________________________
II. Business Requirement (Thiếu)
Ví dụ:
Module Course
Hiện mới ghi
CRUD Course
Nhưng BRD phải viết
Business Objective

Actor

Pre-condition

Main Flow

Alternative Flow

Exception Flow

Post-condition

Acceptance Criteria
cho từng nghiệp vụ.
________________________________________
III. Business Rule Specification (Thiếu rất nhiều)
Đây là phần lớn nhất.
Ví dụ Lesson
Hiện mới có
Unlock Lesson
Nhưng cần
BR-LESSON-001

Điều kiện mở

BR-LESSON-002

Điều kiện học lại

BR-LESSON-003

Điều kiện Complete

BR-LESSON-004

Điều kiện Mastery

BR-LESSON-005

Điều kiện Streak

BR-LESSON-006

Offline

BR-LESSON-007

Conflict Resolution

BR-LESSON-008

Rollback
Tương tự cho
•	User 
•	Course 
•	Chapter 
•	Lesson 
•	Dictionary 
•	Character 
•	Grammar 
•	Exercise 
•	Exam 
•	AI 
•	Search 
•	Import 
•	Export 
•	Notification 
________________________________________
IV. Database Specification
Checklist mới nói
Thiết kế bảng

Index

FK

Backup
Nhưng chưa có
Table Specification
Ví dụ
Table

tbl_words

Description

Purpose

Columns

Datatype

Nullable

Default

Validation

Constraint

FK

Index

Audit

Soft Delete

Retention

API Usage

Business Rule
Khoảng
180–250 bảng.
________________________________________
V. Database Dictionary
Thiếu hoàn toàn.
Ví dụ
WordId

UUID

Primary Key

Generated

Not Null

Description

...

Meaning

nvarchar(500)

...

...
________________________________________
VI. Database ERD Detail
Hiện mới có ý tưởng.
Thiếu
Aggregate

Ownership

Cascade

Delete Behavior

Versioning

History

Soft Delete

Concurrency
________________________________________
VII. API Specification
Đây là phần thiếu lớn nhất.
Hiện chỉ mô tả
Login

Register

...
Cần
API ID

Name

Method

URL

Permission

Authentication

Request

Validation

Business Rule

Database

Transaction

Audit

Response

Retry

Rate Limit

Error

Test Cases
________________________________________
VIII. DTO Specification
Thiếu hoàn toàn.
Ví dụ
LoginRequest

LoginResponse

CreateLessonRequest

CreateLessonResponse

...

Validation
________________________________________
IX. Error Code Catalog
Thiếu.
Ví dụ
AUTH-001

AUTH-002

COURSE-001

LESSON-001

...
________________________________________
X. Permission Matrix
Hiện chỉ có Role.
Thiếu
Resource

Action

Role

Permission

Policy

Scope
Ví dụ
Resource	Action	Learner	Teacher	Admin
________________________________________
XI. UI Specification
Checklist mới ghi
Có màn hình

Đăng nhập

Lesson

Dictionary
Thiếu
Screen ID

Purpose

Components

State

Validation

Permission

API Mapping

Navigation

Responsive

Acceptance
________________________________________
XII. Design System
Thiếu
•	Color Token 
•	Typography 
•	Icon Guideline 
•	Grid 
•	Elevation 
•	Animation 
•	Motion 
•	Spacing 
•	Responsive Rule 
________________________________________
XIII. Learning Engine Specification
Checklist có Learning Engine.
Nhưng chưa có
Progress Formula

Mastery Formula

Placement Formula

Recommendation Formula

SRS Formula

Certificate Formula

Daily Goal Formula
________________________________________
XIV. CMS Workflow
Hiện chỉ có
Draft

Review

Published
Thiếu
Workflow Diagram

Permission

Transition

Rollback

Reject

Archive
________________________________________
XV. Content Governance
Thiếu
Editorial Guideline

Review Checklist

Publishing Checklist

Content Quality Score

Content SLA
________________________________________
XVI. Testing Specification
Checklist có loại test.
Nhưng chưa có
Test Strategy

Unit

Integration

Contract

Performance

Security

UAT

Regression

Smoke

Sanity
________________________________________
XVII. Test Case Catalog
Thiếu hoàn toàn.
Ví dụ
TC-LOGIN-001

TC-LOGIN-002

...

TC-LESSON-001

...
________________________________________
XVIII. Runbook
Mới nhắc đến Incident.
Thiếu
Redis Down

Postgres Down

Supabase Down

CDN Down

DNS Down

Rollback

Restore

Key Rotation

Secret Leak
________________________________________
XIX. SLO / SLA
Có nhắc sơ bộ.
Thiếu
Availability

Latency

Error Budget

Escalation

Owner

Severity

Response Time

Resolution Time
________________________________________
XX. Project Management
Thiếu
•	Epic Breakdown 
•	Sprint Plan 
•	Milestone 
•	Release Plan 
•	Dependency Map 
•	Resource Plan 
•	Risk Register 
•	Decision Log (ADR) 
•	Change Request Process 
________________________________________
XXI. Master Task Plan
Hiện chưa có.
Theo quy mô hệ thống này, mình đề xuất khoảng:
•	250–300 Epic 
•	700–900 Feature 
•	2.500–4.000 Task 
•	Mỗi Task liên kết với: 
o	PRD 
o	BRD 
o	BRS 
o	Database 
o	API 
o	UI 
o	Test Case 
o	Owner 
o	Sprint 
o	Status

