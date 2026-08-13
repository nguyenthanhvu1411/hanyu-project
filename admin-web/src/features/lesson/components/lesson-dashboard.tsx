"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { getContentStatusLabel } from "@/lib/constants/content-status";
import { lessonApi } from "../api/lesson.api";
import type { AdminLessonDetail } from "../types/lesson.types";
import { LessonValidationPanel } from "./lesson-validation-panel";

export function LessonDashboard({ lessonId }: { lessonId: number }) {
  const [lesson, setLesson] = useState<AdminLessonDetail | null>(null);
  const [loading, setLoading] = useState(true);
  useEffect(() => { let active = true; void lessonApi.getById(lessonId).then(x => { if (active) setLesson(x); }).catch(e => toast.error(e instanceof Error ? e.message : "Không thể tải bài giảng.")).finally(() => { if (active) setLoading(false); }); return () => { active = false; }; }, [lessonId]);
  if (loading) return <Skeleton className="h-[420px] rounded-[11px]" />;
  if (!lesson) return <Card><CardContent className="p-6">Không tìm thấy bài giảng.</CardContent></Card>;
  const metrics = [["Sections", lesson.sectionCount], ["Từ vựng", lesson.vocabularyCount], ["Media", lesson.assetCount], ["Tiên quyết", lesson.prerequisiteCount]] as const;
  return <div className="space-y-5">
    <Card><CardHeader><div className="flex flex-wrap items-center gap-2"><CardTitle className="text-[18px]">{lesson.titleVi}</CardTitle><Badge variant="info">{getContentStatusLabel(lesson.status)}</Badge>{lesson.hskCode && <Badge>{lesson.hskCode}</Badge>}</div><p className="mt-2 text-[13px] text-[#666]">{lesson.shortDescriptionVi || "Chưa có mô tả ngắn."}</p><p className="mt-2 text-[11px] text-[#999]">{lesson.slug} · v{lesson.version} · {lesson.publicId}</p></CardHeader><CardContent className="flex flex-wrap gap-2 border-t border-[#eee9e2] pt-4"><Link href={`/bai-giang/${lessonId}/chinh-sua`}><Button size="sm">Metadata & Workflow</Button></Link><Link href={`/bai-giang/${lessonId}/noi-dung`}><Button variant="outline" size="sm">Content Editor</Button></Link><Link href={`/bai-giang/${lessonId}/noi-dung/quan-ly`}><Button variant="outline" size="sm">Vocabulary & Tiên quyết</Button></Link></CardContent></Card>
    <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">{metrics.map(([label,value]) => <Card key={label}><CardContent className="p-4"><div className="text-[20px] font-semibold">{value}</div><div className="text-[12px] text-[#777]">{label}</div></CardContent></Card>)}</div>
    <div className="grid gap-4 xl:grid-cols-2"><Card><CardHeader><CardTitle>Phân loại</CardTitle></CardHeader><CardContent className="text-[13px] leading-7 text-[#666]">Khóa học: {lesson.courseTitleVi || "—"}<br/>Chương: {lesson.courseChapterTitleVi || "—"}<br/>HSK: {lesson.hskNameVi || lesson.hskCode || "—"}<br/>Chủ đề: {lesson.topicNameVi || "—"}</CardContent></Card><Card><CardHeader><CardTitle>Thiết lập</CardTitle></CardHeader><CardContent className="text-[13px] leading-7 text-[#666]">Thời lượng: {lesson.estimatedMinutes} phút<br/>Độ khó: {lesson.difficulty}/5<br/>Thứ tự: {lesson.sortOrder}<br/>Published: {lesson.publishedAt ? new Date(lesson.publishedAt).toLocaleString("vi-VN") : "Chưa xuất bản"}</CardContent></Card></div>
    <LessonValidationPanel lessonId={lessonId}/>
  </div>;
}
