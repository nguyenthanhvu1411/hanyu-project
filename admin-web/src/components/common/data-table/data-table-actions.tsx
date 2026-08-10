"use client";

import {
  Eye,
  MoreHorizontal,
  Pencil,
  RotateCcw,
  Trash2,
} from "lucide-react";

import {
  Button,
} from "@/components/ui/button";

import {
  useEffect,
  useRef,
  useState,
} from "react";

interface DataTableActionsProps {
  onView?: () => void;
  onEdit?: () => void;
  onDelete?: () => void;
  onRestore?: () => void;
  customActions?: React.ReactNode;
}

export function DataTableActions({
  onView,
  onEdit,
  onDelete,
  onRestore,
  customActions,
}: DataTableActionsProps) {
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClick(event: MouseEvent) {
      if (ref.current && !ref.current.contains(event.target as Node)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClick);
    return () => document.removeEventListener("mousedown", handleClick);
  }, []);

  return (
    <div ref={ref} className="relative inline-block">
      <button
        type="button"
        onClick={() => setOpen((value) => !value)}
        className="flex h-8 w-8 items-center justify-center rounded-[6px] text-[#777] transition hover:bg-[#f2f2f2]"
      >
        <MoreHorizontal size={17} />
      </button>

      {open && (
        <div className="absolute right-0 top-[35px] z-20 w-[160px] rounded-[8px] border border-[#e7e2db] bg-white p-1 shadow-lg">
          {onView && (
            <ActionButton icon={<Eye size={14} />} onClick={() => { setOpen(false); onView(); }}>
              Xem chi tiết
            </ActionButton>
          )}

          {onEdit && (
            <ActionButton icon={<Pencil size={14} />} onClick={() => { setOpen(false); onEdit(); }}>
              Chỉnh sửa
            </ActionButton>
          )}

          {customActions && (
            <div onClick={() => setOpen(false)}>
              {customActions}
            </div>
          )}

          {onRestore && (
            <ActionButton icon={<RotateCcw size={14} />} onClick={() => { setOpen(false); onRestore(); }}>
              Khôi phục
            </ActionButton>
          )}

          {onDelete && (
            <>
              <div className="my-1 h-px bg-[#eee]" />
              <ActionButton icon={<Trash2 size={14} />} onClick={() => { setOpen(false); onDelete(); }} danger>
                Xóa
              </ActionButton>
            </>
          )}
        </div>
      )}
    </div>
  );
}

export interface ActionButtonProps {
  icon: React.ReactNode;
  children: React.ReactNode;
  onClick: () => void;
  danger?: boolean;
}

export function ActionButton({
  icon,
  children,
  onClick,
  danger = false,
}: ActionButtonProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={`flex h-8 w-full items-center gap-2 rounded-[6px] px-2 text-[11px] transition ${
        danger ? "text-[#e2372f] hover:bg-[#fff0ee]" : "text-[#555] hover:bg-[#f7f7f7]"
      }`}
    >
      {icon}
      {children}
    </button>
  );
}



