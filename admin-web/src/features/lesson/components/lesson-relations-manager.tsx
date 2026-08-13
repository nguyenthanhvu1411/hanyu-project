"use client";

import { LessonContentManager } from "./lesson-content-manager";

export function LessonRelationsManager({ lessonId }: { lessonId: number }) {
  return <LessonContentManager lessonId={lessonId} />;
}
