const XLSX = require("xlsx");
const fs = require("fs");
const path = require("path");

const dir = path.join(process.cwd(), "public", "file-mau");
if (!fs.existsSync(dir)) {
  fs.mkdirSync(dir, { recursive: true });
}

const wb = XLSX.utils.book_new();
const ws = XLSX.utils.aoa_to_sheet([
  ["Tài khoản", "Email", "Họ và tên", "Vai trò"],
  ["nguyenvana", "vana@hanyu.vn", "Nguyễn Văn A", "Học viên"]
]);

XLSX.utils.book_append_sheet(wb, ws, "NguoiDung");
XLSX.writeFile(wb, path.join(dir, "mau-import-nguoi-dung.xlsx"));
console.log("Created sample file");
