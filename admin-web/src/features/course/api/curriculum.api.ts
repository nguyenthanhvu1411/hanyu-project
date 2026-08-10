import { apiClient } from "@/lib/api/api-client";

import type {
  AssignLessonRequest,
  CourseChapter,
  CourseChapterLesson,
  CreateChapterRequest,
  EntityWorkflowRequest,
  MoveLessonRequest,
  ReorderChaptersRequest,
  ReorderLessonsRequest,
  UpdateChapterRequest,
} from "../types/curriculum.types";

function chapterBase(courseId: number) {
  return `/api/v1/admin/courses/${courseId}/chapters`;
}

function lessonBase(courseId: number, chapterId: number) {
  return `${chapterBase(courseId)}/${chapterId}/lessons`;
}

export const curriculumApi = {
  chapters(courseId: number, includeDeleted = false) {
    return apiClient<CourseChapter[]>(
      `${chapterBase(courseId)}?includeDeleted=${includeDeleted}`,
    );
  },

  chapter(courseId: number, chapterId: number) {
    return apiClient<CourseChapter>(`${chapterBase(courseId)}/${chapterId}`);
  },

  createChapter(courseId: number, body: CreateChapterRequest) {
    return apiClient<CourseChapter>(chapterBase(courseId), {
      method: "POST",
      body,
    });
  },

  updateChapter(
    courseId: number,
    chapterId: number,
    body: UpdateChapterRequest,
  ) {
    return apiClient<CourseChapter>(`${chapterBase(courseId)}/${chapterId}`, {
      method: "PUT",
      body,
    });
  },

  deleteChapter(
    courseId: number,
    chapterId: number,
    body: EntityWorkflowRequest,
  ) {
    return apiClient<void>(`${chapterBase(courseId)}/${chapterId}`, {
      method: "DELETE",
      body,
    });
  },

  restoreChapter(
    courseId: number,
    chapterId: number,
    body: EntityWorkflowRequest,
  ) {
    return apiClient<CourseChapter>(
      `${chapterBase(courseId)}/${chapterId}/restore`,
      {
        method: "POST",
        body,
      },
    );
  },

  reorderChapters(courseId: number, body: ReorderChaptersRequest) {
    return apiClient<void>(`/api/v1/admin/courses/${courseId}/chapters/order`, {
      method: "PUT",
      body,
    });
  },

  lessons(courseId: number, chapterId: number) {
    return apiClient<CourseChapterLesson[]>(lessonBase(courseId, chapterId));
  },

  assignLesson(
    courseId: number,
    chapterId: number,
    body: AssignLessonRequest,
  ) {
    return apiClient<CourseChapterLesson>(
      `${lessonBase(courseId, chapterId)}/assign`,
      {
        method: "POST",
        body,
      },
    );
  },

  moveLesson(
    courseId: number,
    sourceChapterId: number,
    lessonId: number,
    body: MoveLessonRequest,
  ) {
    return apiClient<CourseChapterLesson>(
      `${lessonBase(courseId, sourceChapterId)}/${lessonId}/move`,
      {
        method: "POST",
        body,
      },
    );
  },

  removeLesson(courseId: number, chapterId: number, lessonId: number) {
    return apiClient<void>(
      `${lessonBase(courseId, chapterId)}/${lessonId}`,
      {
        method: "DELETE",
      },
    );
  },

  reorderLessons(
    courseId: number,
    chapterId: number,
    body: ReorderLessonsRequest,
  ) {
    return apiClient<void>(`${lessonBase(courseId, chapterId)}/reorder`, {
      method: "PUT",
      body,
    });
  },
};
