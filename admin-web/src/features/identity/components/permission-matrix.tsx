"use client";

import {
  useMemo,
} from "react";

import {
  Checkbox,
} from "@/components/ui/checkbox";

import type {
  AdminPermissionDto,
} from "@/dto/identity/admin-permission.dto";

interface PermissionMatrixProps {
  permissions:
    AdminPermissionDto[];

  value: string[];

  onChange: (
    ids: string[],
  ) => void;

  disabled?: boolean;
}

export function PermissionMatrix({
  permissions,
  value,
  onChange,
  disabled = false,
}: PermissionMatrixProps) {
  const groups =
    useMemo(() => {
      const map =
        new Map<
          string,
          AdminPermissionDto[]
        >();

      permissions.forEach(
        (
          permission,
        ) => {
          const key =
            permission.resource ||
            "other";

          const current =
            map.get(
              key,
            ) ?? [];

          current.push(
            permission,
          );

          map.set(
            key,
            current,
          );
        },
      );

      return Array.from(
        map.entries(),
      );
    }, [
      permissions,
    ]);

  function toggle(
    id: string,
  ) {
    if (
      value.includes(
        id,
      )
    ) {
      onChange(
        value.filter(
          (item) =>
            item !== id,
        ),
      );
    } else {
      onChange([
        ...value,
        id,
      ]);
    }
  }

  function toggleGroup(
    permissions:
      AdminPermissionDto[],
  ) {
    const ids =
      permissions.map(
        (item) =>
          item.id,
      );

    const all =
      ids.every(
        (id) =>
          value.includes(
            id,
          ),
      );

    if (all) {
      onChange(
        value.filter(
          (id) =>
            !ids.includes(
              id,
            ),
        ),
      );
    } else {
      onChange(
        Array.from(
          new Set([
            ...value,
            ...ids,
          ]),
        ),
      );
    }
  }

  if (permissions.length === 0) {
    return (
      <div
        className="
          flex
          min-h-[160px]
          items-center
          justify-center
          rounded-[9px]
          border
          border-dashed
          border-[#ddd8d0]
          bg-[#faf9f7]
          px-4
          text-center
        "
      >
        <div>
          <div
            className="
              text-[11px]
              font-medium
              text-[#666]
            "
          >
            Chưa có quyền hạn
          </div>

          <div
            className="
              mt-1
              text-[9px]
              text-[#999]
            "
          >
            Không có dữ liệu quyền để hiển thị.
          </div>
        </div>
      </div>
    );
  }

  return (
    <div
      className="
        overflow-hidden
        rounded-[10px]
        border
        border-[#e8e3dc]
      "
    >
      {groups.map(
        ([
          resource,
          items,
        ]) => {
          const all =
            items.every(
              (permission) =>
                value.includes(
                  permission.id,
                ),
            );

          return (
            <div
              key={
                resource
              }
              className="
                border-b
                border-[#eee9e2]
                last:border-0
              "
            >
              <div
                className="
                  flex
                  items-center
                  gap-3
                  bg-[#faf9f7]
                  px-4
                  py-3
                "
              >
                <Checkbox
                  checked={
                    all
                  }
                  onCheckedChange={() =>
                    toggleGroup(
                      items,
                    )
                  }
                />

                <span
                  className="
                    text-[12px]
                    font-semibold
                    text-[#444]
                  "
                >
                  {resource}
                </span>
              </div>

              <div
                className="
                  grid
                  gap-3
                  p-4
                  md:grid-cols-2
                  xl:grid-cols-3
                "
              >
                {items.map(
                  (
                    permission,
                  ) => (
                    <label
                      key={
                        permission.id
                      }
                      className="
                        flex
                        items-start
                        gap-2
                      "
                    >
                      <Checkbox
                        checked={
                          value.includes(
                            permission.id,
                          )
                        }
                        onCheckedChange={() =>
                          toggle(
                            permission.id,
                          )
                        }
                      />

                      <span>
                        <span
                          className="
                            block
                            text-[11px]
                            font-medium
                            text-[#444]
                          "
                        >
                          {
                            permission.action
                          }
                        </span>

                        <span
                          className="
                            mt-[2px]
                            block
                            text-[9px]
                            text-[#999]
                          "
                        >
                          {
                            permission.code
                          }
                        </span>
                      </span>
                    </label>
                  ),
                )}
              </div>
            </div>
          );
        },
      )}
    </div>
  );
}
