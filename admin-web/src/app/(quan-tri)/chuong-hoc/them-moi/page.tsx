import { redirect } from "next/navigation";

export default async function ChapterLegacyCreatePage(props: {
  searchParams: Promise<{ courseId?: string }>;
}) {
  const { courseId } = await props.searchParams;
  const resolvedCourseId = Number(courseId);

  if (Number.isSafeInteger(resolvedCourseId) && resolvedCourseId > 0) {
    redirect(`/khoa-hoc/${resolvedCourseId}/noi-dung?mode=create-chapter`);
  }

  redirect("/chuong-hoc");
}
