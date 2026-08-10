import { ReactNode } from "react";
import { CourseDetailLayout } from "@/features/course/components/curriculum/course-detail-layout";

export default async function CourseDetailLayoutWrapper(props: { children: ReactNode }) {
  return <CourseDetailLayout>{props.children}</CourseDetailLayout>;
}
