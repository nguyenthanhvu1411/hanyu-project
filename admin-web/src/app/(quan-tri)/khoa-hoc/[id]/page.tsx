import { CourseEditor } from "@/features/course/components/editor/course-editor";

interface Props {
  params: Promise<{
    id: string;
  }>;
}

export default async function CourseDetailPage({ params }: Props) {
  const { id } = await params;
  const courseId = Number(id);

  if (!Number.isInteger(courseId) || courseId <= 0) {
    return <div className="p-6">Khóa học không hợp lệ.</div>;
  }

  return <CourseEditor courseId={courseId} />;
}
