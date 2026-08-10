"use client";

import {
  Eye,
  MoreHorizontal,
  Pencil,
  RotateCcw,
  Trash2,
} from "lucide-react";
import { createPortal } from "react-dom";
import {
  useCallback,
  useEffect,
  useLayoutEffect,
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

interface MenuPosition {
  top: number;
  left: number;
}

const MENU_WIDTH = 160;
const MENU_GAP = 6;
const VIEWPORT_PADDING = 8;

export function DataTableActions({
  onView,
  onEdit,
  onDelete,
  onRestore,
  customActions,
}: DataTableActionsProps) {
  const [open, setOpen] = useState(false);
  const [position, setPosition] = useState<MenuPosition>({ top: 0, left: 0 });
  const triggerRef = useRef<HTMLButtonElement>(null);
  const menuRef = useRef<HTMLDivElement>(null);

  const updatePosition = useCallback(() => {
    const trigger = triggerRef.current;
    if (!trigger) return;

    const triggerRect = trigger.getBoundingClientRect();
    const menuHeight = menuRef.current?.offsetHeight ?? 160;

    const spaceBelow = window.innerHeight - triggerRect.bottom;
    const shouldOpenUp =
      spaceBelow < menuHeight + MENU_GAP + VIEWPORT_PADDING &&
      triggerRect.top >= menuHeight + MENU_GAP + VIEWPORT_PADDING;

    const desiredTop = shouldOpenUp
      ? triggerRect.top - menuHeight - MENU_GAP
      : triggerRect.bottom + MENU_GAP;

    const desiredLeft = triggerRect.right - MENU_WIDTH;

    setPosition({
      top: Math.max(
        VIEWPORT_PADDING,
        Math.min(desiredTop, window.innerHeight - menuHeight - VIEWPORT_PADDING),
      ),
      left: Math.max(
        VIEWPORT_PADDING,
        Math.min(desiredLeft, window.innerWidth - MENU_WIDTH - VIEWPORT_PADDING),
      ),
    });
  }, []);

  useLayoutEffect(() => {
    if (!open) return;

    updatePosition();
    const frame = window.requestAnimationFrame(updatePosition);

    return () => window.cancelAnimationFrame(frame);
  }, [open, updatePosition]);

  useEffect(() => {
    if (!open) return;

    function handlePointerDown(event: MouseEvent) {
      const target = event.target as Node;

      if (
        triggerRef.current?.contains(target) ||
        menuRef.current?.contains(target)
      ) {
        return;
      }

      setOpen(false);
    }

    function handleViewportChange() {
      updatePosition();
    }

    document.addEventListener("mousedown", handlePointerDown);
    window.addEventListener("resize", handleViewportChange);
    window.addEventListener("scroll", handleViewportChange, true);

    return () => {
      document.removeEventListener("mousedown", handlePointerDown);
      window.removeEventListener("resize", handleViewportChange);
      window.removeEventListener("scroll", handleViewportChange, true);
    };
  }, [open, updatePosition]);

  const menu = open ? (
    <div
      ref={menuRef}
      role="menu"
      className="fixed z-[1000] w-[160px] rounded-[8px] border border-[#e7e2db] bg-white p-1 shadow-lg"
      style={{
        top: position.top,
        left: position.left,
      }}
    >
      {onView && (
        <ActionButton
          icon={<Eye size={14} />}
          onClick={() => {
            setOpen(false);
            onView();
          }}
        >
          Xem chi tiết
        </ActionButton>
      )}

      {onEdit && (
        <ActionButton
          icon={<Pencil size={14} />}
          onClick={() => {
            setOpen(false);
            onEdit();
          }}
        >
          Chỉnh sửa
        </ActionButton>
      )}

      {customActions && (
        <div onClick={() => setOpen(false)}>
          {customActions}
        </div>
      )}

      {onRestore && (
        <ActionButton
          icon={<RotateCcw size={14} />}
          onClick={() => {
            setOpen(false);
            onRestore();
          }}
        >
          Khôi phục
        </ActionButton>
      )}

      {onDelete && (
        <>
          <div className="my-1 h-px bg-[#eee]" />
          <ActionButton
            icon={<Trash2 size={14} />}
            onClick={() => {
              setOpen(false);
              onDelete();
            }}
            danger
          >
            Xóa
          </ActionButton>
        </>
      )}
    </div>
  ) : null;

  return (
    <>
      <button
        ref={triggerRef}
        type="button"
        aria-haspopup="menu"
        aria-expanded={open}
        onClick={() => setOpen((value) => !value)}
        className="inline-flex h-8 w-8 items-center justify-center rounded-[6px] text-[#777] transition hover:bg-[#f2f2f2]"
      >
        <MoreHorizontal size={17} />
      </button>

      {menu && createPortal(menu, document.body)}
    </>
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
      role="menuitem"
      onClick={onClick}
      className={`flex h-8 w-full items-center gap-2 rounded-[6px] px-2 text-[11px] transition ${
        danger
          ? "text-[#e2372f] hover:bg-[#fff0ee]"
          : "text-[#555] hover:bg-[#f7f7f7]"
      }`}
    >
      {icon}
      {children}
    </button>
  );
}
