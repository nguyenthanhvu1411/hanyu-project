import * as React from "react";

import {
  cn,
} from "@/lib/utils/cn";

export function Table({
  className,
  ...props
}: React.HTMLAttributes<HTMLTableElement>) {
  return (
    <div className="relative w-full overflow-visible">
      <table
        className={cn(
          "w-full",
          "caption-bottom",
          "text-[13px]",
          className,
        )}
        {...props}
      />
    </div>
  );
}

export function TableHeader({
  className,
  ...props
}: React.HTMLAttributes<HTMLTableSectionElement>) {
  return (
    <thead
      className={cn(
        "[&_tr]:border-b",
        className,
      )}
      {...props}
    />
  );
}

export function TableBody({
  className,
  ...props
}: React.HTMLAttributes<HTMLTableSectionElement>) {
  return (
    <tbody
      className={cn(
        "[&_tr:last-child]:border-0",
        className,
      )}
      {...props}
    />
  );
}

export function TableRow({
  className,
  ...props
}: React.HTMLAttributes<HTMLTableRowElement>) {
  return (
    <tr
      className={cn(
        "border-b",
        "border-[#eee9e2]",
        "transition-colors",
        "hover:bg-[#fffaf8]",
        className,
      )}
      {...props}
    />
  );
}

export function TableHead({
  className,
  ...props
}: React.ThHTMLAttributes<HTMLTableCellElement>) {
  return (
    <th
      className={cn(
        "h-[45px]",
        "px-3",
        "text-left",
        "align-middle",
        "text-[12px]",
        "font-semibold",
        "text-[#666]",
        className,
      )}
      {...props}
    />
  );
}

export function TableCell({
  className,
  ...props
}: React.TdHTMLAttributes<HTMLTableCellElement>) {
  return (
    <td
      className={cn(
        "px-3",
        "py-[11px]",
        "align-middle",
        "text-[13px]",
        "text-[#3f3f3f]",
        className,
      )}
      {...props}
    />
  );
}
