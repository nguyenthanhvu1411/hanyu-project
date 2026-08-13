"use client";
import { useParams } from "next/navigation";
import { PageContainer } from "@/components/layout/page-container";
import { PageHeader } from "@/components/layout/page-header";
import { PERMISSIONS } from "@/constants/permission.constants";
import { LessonDashboard } from "@/features/lesson/components/lesson-dashboard";
import { PermissionGuard } from "@/security/permission-guard";

export default function LessonDetailPage() {
  const params = useParams<{ id: string }>();
  const lessonId = Number(params.id);
  return <PermissionGuard permission={PERMISSIONS.LESSONS.READ}><PageContainer><PageHeader title="Chi tiết bài giảng" description="Tổng quan nội dung, liên kết, trạng thái và độ sẵn sàng của bài giảng."/><LessonDashboard lessonId={lessonId}/></PageContainer></PermissionGuard>;
}
