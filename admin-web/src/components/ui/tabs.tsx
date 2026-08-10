"use client";

import {
  createContext,
  useContext,
} from "react";

import { cn } from "@/lib/utils/cn";

interface TabsContextValue {
  value: string;

  onValueChange: (
    value: string,
  ) => void;
}

const TabsContext =
  createContext<TabsContextValue | null>(
    null,
  );

interface TabsProps {
  value: string;

  onValueChange: (
    value: string,
  ) => void;

  children: React.ReactNode;

  className?: string;
}

export function Tabs({
  value,
  onValueChange,
  children,
  className,
}: TabsProps) {
  return (
    <TabsContext.Provider
      value={{
        value,
        onValueChange,
      }}
    >
      <div
        className={
          className
        }
      >
        {children}
      </div>
    </TabsContext.Provider>
  );
}

export function TabsList({
  children,
  className,
}: {
  children: React.ReactNode;
  className?: string;
}) {
  return (
    <div
      role="tablist"
      className={cn(
        "flex",
        "items-center",
        "gap-1",
        "overflow-x-auto",
        "border-b",
        "border-[#e9e4dc]",
        className,
      )}
    >
      {children}
    </div>
  );
}

export function TabsTrigger({
  value,
  children,
  className,
}: {
  value: string;
  children: React.ReactNode;
  className?: string;
}) {
  const context =
    useContext(
      TabsContext,
    );

  if (!context) {
    throw new Error(
      "TabsTrigger phải nằm trong Tabs.",
    );
  }

  const active =
    context.value ===
    value;

  return (
    <button
      type="button"
      role="tab"
      aria-selected={
        active
      }
      onClick={() =>
        context.onValueChange(
          value,
        )
      }
      className={cn(
        "relative",
        "h-[42px]",
        "shrink-0",
        "px-3",
        "text-[12px]",
        "font-medium",
        "transition",

        active
          ? "text-[#ef241c]"
          : "text-[#777] hover:text-[#333]",

        className,
      )}
    >
      {children}

      {active && (
        <span
          className="
            absolute
            bottom-0
            left-2
            right-2
            h-[2px]
            rounded-full
            bg-[#ef241c]
          "
        />
      )}
    </button>
  );
}

export function TabsContent({
  value,
  children,
  className,
}: {
  value: string;
  children: React.ReactNode;
  className?: string;
}) {
  const context =
    useContext(
      TabsContext,
    );

  if (!context) {
    throw new Error(
      "TabsContent phải nằm trong Tabs.",
    );
  }

  if (
    context.value !==
    value
  ) {
    return null;
  }

  return (
    <div
      role="tabpanel"
      className={cn(
        "pt-4",
        className,
      )}
    >
      {children}
    </div>
  );
}
