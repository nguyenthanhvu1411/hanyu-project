import { redirect } from "next/navigation";

export default async function ChapterLegacyEditPage(props: {
  params: Promise<{ id: string }>;
  searchParams: Promise<{ courseId?: string }>;
}) {
  const [{ courseId }, { id }] = await Promise.all([props.searchParams, props.params]);
  const resolvedCourseId = Number(courseId);

  if (Number.isSafeInteger(resolvedCourseId) && resolvedCourseId > 0) {
    redirect(`/khoa-hoc/${resolvedCourseId}/noi-dung?chapterId=${encodeURIComponent(id)}&mode=edit`);
  }

  redirect("/chuong-hoc");
}
