import {
  AlertTriangle,
  CheckCircle2,
  FileSpreadsheet,
  Rows3,
} from "lucide-react";

interface ExcelImportSummaryProps {
  totalRows: number;

  validRows: number;

  invalidRows: number;

  columns: number;
}

export function ExcelImportSummary({
  totalRows,
  validRows,
  invalidRows,
  columns,
}: ExcelImportSummaryProps) {
  return (
    <div
      className="
        grid
        grid-cols-2
        gap-2
        lg:grid-cols-4
      "
    >
      <SummaryCard
        icon={
          <Rows3
            size={17}
          />
        }
        label="Tổng dữ liệu"
        value={totalRows}
      />

      <SummaryCard
        icon={
          <FileSpreadsheet
            size={17}
          />
        }
        label="Số cột"
        value={columns}
      />

      <SummaryCard
        icon={
          <CheckCircle2
            size={17}
          />
        }
        label="Hợp lệ"
        value={validRows}
        type="success"
      />

      <SummaryCard
        icon={
          <AlertTriangle
            size={17}
          />
        }
        label="Có lỗi"
        value={invalidRows}
        type="danger"
      />
    </div>
  );
}

interface SummaryCardProps {
  icon: React.ReactNode;

  label: string;

  value: number;

  type?:
    | "normal"
    | "success"
    | "danger";
}

function SummaryCard({
  icon,
  label,
  value,
  type = "normal",
}: SummaryCardProps) {
  const styles = {
    normal:
      "bg-[#faf9f7] text-[#666]",

    success:
      "bg-[#edf8f2] text-[#16975b]",

    danger:
      "bg-[#fff0ee] text-[#ef241c]",
  };

  return (
    <div
      className={`
        rounded-[9px]
        border
        border-[#ebe6df]
        p-3
        ${styles[type]}
      `}
    >
      <div className="flex items-center gap-2">
        {icon}

        <span
          className="
            text-[10px]
            font-medium
          "
        >
          {label}
        </span>
      </div>

      <div
        className="
          mt-2
          text-[19px]
          font-semibold
        "
      >
        {value}
      </div>
    </div>
  );
}
