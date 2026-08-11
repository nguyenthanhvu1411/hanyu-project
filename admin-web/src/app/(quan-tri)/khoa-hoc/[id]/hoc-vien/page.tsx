"use client";

import { Users } from "lucide-react";

import { EmptyState } from "@/components/common/empty-state";
import { FormSection } from "@/components/forms/form-section";

export default function CourseStudentsPage() {
  return (
    <FormSection
      title="Học viên khóa học"
      description="Khu vực này chỉ hiển thị dữ liệu thật khi backend có Course Enrollment/Progress theo Course."
      icon={<Users size={18} />}
    >
      <EmptyState
        title="Chưa có API học viên theo khóa học"
        description="Backend hiện chưa expose quan hệ Course Enrollment nên admin-web không tạo dữ liệu giả. Khi module Enrollment được bổ sung, trang này sẽ hiển thị học viên, tiến độ, ngày bắt đầu và trạng thái hoàn thành."
      />
    </FormSection>
  );
}
