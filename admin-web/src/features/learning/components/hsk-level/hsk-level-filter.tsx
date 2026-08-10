"use client";

import { SearchInput } from "@/components/common/search-input";
import { Select } from "@/components/ui/select";
import { HSK_LEVEL_STATUS_OPTIONS } from "../../learning.constants";

interface HskLevelFilterProps {
  search: string;
  status: string;
  onSearchChange: (value: string) => void;
  onStatusChange: (value: string) => void;
}

export function HskLevelFilter({
  search,
  status,
  onSearchChange,
  onStatusChange,
}: HskLevelFilterProps) {
  return (
    <div className="flex flex-col gap-2 lg:flex-row lg:items-center">
      <SearchInput
        value={search}
        onChange={onSearchChange}
        placeholder="Tìm mã hoặc tên cấp độ..."
        className="w-full lg:w-[320px]"
      />

      <div className="w-full lg:w-[190px]">
        <Select
          value={status}
          onValueChange={onStatusChange}
          clearable
          placeholder="Tất cả trạng thái"
          options={HSK_LEVEL_STATUS_OPTIONS.map((item) => ({
            label: item.label,
            value: item.value,
          }))}
        />
      </div>
    </div>
  );
}
