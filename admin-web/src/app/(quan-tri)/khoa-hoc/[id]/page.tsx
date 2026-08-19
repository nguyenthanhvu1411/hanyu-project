import { redirect } from "next/navigation";

interface Props {
  params: Promise<{
    id: string;
  }>;
}

export default async function CourseDetailPage({ params }: Props) {
  const { id } = await params;
  const courseId = Number(id);

  if (!Number.isInteger(courseId) || courseId <= 0) {
    redirect("/khoa-hoc");
  }

  redirect(`/khoa-hoc/${courseId}/tong-quan`);
}
