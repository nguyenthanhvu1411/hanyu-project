import { CourseCurriculumTab } from "@/features/khoa-hoc/components/curriculum/course-curriculum-tab";

export default async function CourseCurriculumPage(props: { params: Promise<{ id: string }> }) {
  const params = await props.params;
  const courseId = Number(params.id);
  
  return <CourseCurriculumTab courseId={courseId} />;
}
