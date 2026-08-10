"use client";

import {
  CalendarDays,
  Hash,
} from "lucide-react";

import {
  Drawer,
} from "@/components/ui/drawer";

interface ViewDetailDrawerProps {
  open: boolean;

  onOpenChange: (
    open: boolean,
  ) => void;

  title: string;

  description?: string;

  children: React.ReactNode;

  actions?: React.ReactNode;

  metadata?: {
    id?: string | number;

    createdAt?: string;

    updatedAt?: string;
  };
}

export function ViewDetailDrawer({
  open,
  onOpenChange,
  title,
  description,
  children,
  actions,
  metadata,
}: ViewDetailDrawerProps) {
  return (
    <Drawer
      open={open}
      onOpenChange={
        onOpenChange
      }
      width="lg"
      title={title}
      description={
        description
      }
      footer={
        actions ? (
          <div
            className="
              flex
              justify-end
              gap-2
            "
          >
            {actions}
          </div>
        ) : undefined
      }
    >
      {metadata && (
        <div
          className="
            mb-5
            flex
            flex-wrap
            gap-2
          "
        >
          {metadata.id !==
            undefined && (
            <MetadataItem
              icon={
                <Hash
                  size={12}
                />
              }
            >
              ID:{" "}
              {
                metadata.id
              }
            </MetadataItem>
          )}

          {metadata.createdAt && (
            <MetadataItem
              icon={
                <CalendarDays
                  size={12}
                />
              }
            >
              Tạo:{" "}
              {
                metadata.createdAt
              }
            </MetadataItem>
          )}

          {metadata.updatedAt && (
            <MetadataItem
              icon={
                <CalendarDays
                  size={12}
                />
              }
            >
              Cập nhật:{" "}
              {
                metadata.updatedAt
              }
            </MetadataItem>
          )}
        </div>
      )}

      <div className="space-y-5">
        {children}
      </div>
    </Drawer>
  );
}

function MetadataItem({
  icon,
  children,
}: {
  icon: React.ReactNode;
  children: React.ReactNode;
}) {
  return (
    <div
      className="
        inline-flex
        items-center
        gap-1
        rounded-full
        bg-[#f5f4f1]
        px-2
        py-1
        text-[9px]
        text-[#777]
      "
    >
      {icon}

      {children}
    </div>
  );
}
