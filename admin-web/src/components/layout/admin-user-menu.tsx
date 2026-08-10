"use client";

import {
  ChevronDown,
  LogOut,
  Settings,
  ShieldCheck,
  UserRound,
} from "lucide-react";

import Link from "next/link";

import {
  useEffect,
  useRef,
  useState,
} from "react";

import { ROUTES } from "@/constants/route.constants";
import { useAuth } from "@/features/identity/auth/hooks/use-auth";

export function AdminUserMenu() {
  const { user, logout } = useAuth();
  const [open, setOpen] = useState(false);

  const ref =
    useRef<HTMLDivElement>(
      null,
    );

  useEffect(() => {
    function handleClick(
      event: MouseEvent,
    ) {
      if (
        ref.current &&
        !ref.current.contains(
          event.target as Node,
        )
      ) {
        setOpen(false);
      }
    }

    document.addEventListener(
      "mousedown",
      handleClick,
    );

    return () =>
      document.removeEventListener(
        "mousedown",
        handleClick,
      );
  }, []);

  return (
    <div
      ref={ref}
      className="relative"
    >
      <button
        type="button"
        onClick={() =>
          setOpen(
            (value) =>
              !value,
          )
        }
        className="
          flex
          items-center
          gap-2
          rounded-[9px]
          px-2
          py-[5px]
          transition
          hover:bg-[#faf8f5]
        "
      >
        <div
          className="
            flex h-8 w-8
            items-center
            justify-center
            rounded-full
            bg-[#fff0ee]
            text-[#ef241c]
          "
        >
          <UserRound
            size={17}
          />
        </div>

        <div className="hidden min-w-0 text-left sm:block">
          <div
            className="
              max-w-[150px]
              truncate
              text-[12px]
              font-semibold
              text-[#323232]
            "
          >
            {user?.displayName || "Quản trị viên"}
          </div>

          <div
            className="
              max-w-[150px]
              truncate
              text-[10px]
              text-[#8c8c8c]
            "
          >
            {user?.email || "Administrator"}
          </div>
        </div>

        <ChevronDown
          size={14}
          className="
            hidden
            text-[#8a8a8a]
            sm:block
          "
        />
      </button>

      {open && (
        <div
          className="
            absolute
            right-0
            top-[48px]
            w-[230px]
            overflow-hidden
            rounded-[11px]
            border
            border-[#e8e3dc]
            bg-white
            p-2
            shadow-[0_18px_45px_rgba(0,0,0,0.10)]
          "
        >
          <div
            className="
              border-b
              border-[#eee9e2]
              px-2
              pb-3
              pt-1
            "
          >
            <div
              className="
                text-[13px]
                font-semibold
              "
            >
              {user?.displayName || "Quản trị viên"}
            </div>

            <div
              className="
                mt-[2px]
                text-[11px]
                text-[#8c8c8c]
              "
            >
              {user?.email || "admin@hanyu.vn"}
            </div>
          </div>

          <MenuLink
            href={
              ROUTES.HO_SO
            }
            icon={
              <UserRound
                size={16}
              />
            }
          >
            Hồ sơ cá nhân
          </MenuLink>

          <MenuLink
            href={`${ROUTES.HO_SO}/bao-mat`}
            icon={
              <ShieldCheck
                size={16}
              />
            }
          >
            Bảo mật
          </MenuLink>

          <MenuLink
            href={
              ROUTES.CAU_HINH_HE_THONG
            }
            icon={
              <Settings
                size={16}
              />
            }
          >
            Cấu hình
          </MenuLink>

          <div className="my-1 h-px bg-[#eee9e2]" />

          <button
            type="button"
            onClick={() => void logout()}
            className="
              flex h-9
              w-full
              items-center
              gap-2
              rounded-[7px]
              px-2
              text-[12px]
              text-[#ef241c]
              transition
              hover:bg-[#fff0ee]
            "
          >
            <LogOut
              size={16}
            />

            Đăng xuất
          </button>
        </div>
      )}
    </div>
  );
}

interface MenuLinkProps {
  href: string;
  icon: React.ReactNode;
  children: React.ReactNode;
}

function MenuLink({
  href,
  icon,
  children,
}: MenuLinkProps) {
  return (
    <Link
      href={href}
      className="
        mt-1
        flex h-9
        items-center
        gap-2
        rounded-[7px]
        px-2
        text-[12px]
        text-[#555]
        transition
        hover:bg-[#faf8f5]
        hover:text-[#ef241c]
      "
    >
      {icon}

      {children}
    </Link>
  );
}
