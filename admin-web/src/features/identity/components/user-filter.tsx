"use client";

import {
  SearchInput,
} from "@/components/common/search-input";

import {
  Select,
} from "@/components/ui/select";

import {
  USER_STATUS_OPTIONS,
} from "../identity.constants";

interface UserFilterProps {
  search: string;

  status: string;

  onSearchChange: (
    value: string,
  ) => void;

  onStatusChange: (
    value: string,
  ) => void;
}

export function UserFilter({
  search,
  status,
  onSearchChange,
  onStatusChange,
}: UserFilterProps) {
  return (
    <div
      className="
        flex
        flex-col
        gap-2
        sm:flex-row
        sm:items-center
      "
    >
      <SearchInput
        value={search}
        onChange={
          onSearchChange
        }
        placeholder="Tìm email hoặc tên người dùng..."
        className="sm:w-[320px]"
      />

      <div className="w-full sm:w-[180px]">
        <Select
          value={status}
          onValueChange={
            onStatusChange
          }
          clearable
          placeholder="Tất cả trạng thái"
          options={
            USER_STATUS_OPTIONS.map(
              (item) => ({
                ...item,
              }),
            )
          }
        />
      </div>
    </div>
  );
}
