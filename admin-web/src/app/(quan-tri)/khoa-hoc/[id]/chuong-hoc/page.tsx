import { redirect } from "next/navigation";

export default async function CourseChaptersPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  redirect(`/khoa-hoc/${id}/noi-dung`);
}
