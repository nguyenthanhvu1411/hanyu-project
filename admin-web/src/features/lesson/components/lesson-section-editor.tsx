"use client";

import { LessonSectionStudio } from "./lesson-section-studio";

export function LessonSectionEditor({ lessonId }: { lessonId: number }) {
  return <LessonSectionStudio lessonId={lessonId} />;
}
