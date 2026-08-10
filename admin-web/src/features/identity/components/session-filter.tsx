"use client";

import {
  SearchInput,
} from "@/components/common/search-input";

import {
  Select,
} from "@/components/ui/select";

interface SessionFilterProps {
  search: string;

  active: string;

  onSearchChange: (
    value: string,
  ) => void;

  onActiveChange: (
    value: string,
  ) => void;
}

export function SessionFilter({
  search,
  active,
  onSearchChange,
  onActiveChange,
}: SessionFilterProps) {
  return (
    <div
      className="
        flex
        flex-col
        gap-2
        md:flex-row
        md:items-center
      "
    >
      <SearchInput
        value={search}
        onChange={onSearchChange}
        placeholder="Tìm email, tên người dùng, IP..."
        className="md:w-[320px]"
      />

      <div className="w-full md:w-[180px]">
        <Select
          value={active}
          onValueChange={onActiveChange}
          clearable
          placeholder="Tất cả trạng thái"
          options={[
            {
              label:
                "Đang hoạt động",

              value:
                "true",
            },
            {
              label:
                "Đã thu hồi",

              value:
                "false",
            },
          ]}
        />
      </div>
    </div>
  );
}
