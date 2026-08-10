"use client";

import { create } from "zustand";

interface SidebarState {
  collapsed: boolean;
  mobileOpen: boolean;

  toggleCollapsed: () => void;

  setCollapsed: (
    collapsed: boolean,
  ) => void;

  openMobile: () => void;

  closeMobile: () => void;

  toggleMobile: () => void;
}

export const useSidebarStore =
  create<SidebarState>(
    (set) => ({
      collapsed: false,
      mobileOpen: false,

      toggleCollapsed: () =>
        set((state) => ({
          collapsed:
            !state.collapsed,
        })),

      setCollapsed: (
        collapsed,
      ) =>
        set({
          collapsed,
        }),

      openMobile: () =>
        set({
          mobileOpen: true,
        }),

      closeMobile: () =>
        set({
          mobileOpen: false,
        }),

      toggleMobile: () =>
        set((state) => ({
          mobileOpen:
            !state.mobileOpen,
        })),
    }),
  );
