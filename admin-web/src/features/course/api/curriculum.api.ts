import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";

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

export const curriculumApi = {
  chapters(courseId: number, includeDeleted = false) {
    return apiClient<CourseChapter[]>(
      `${API_ENDPOINTS.COURSE.CHAPTERS(courseId)}?includeDeleted=${includeDeleted}`,
    );
  },
  chapter(courseId: number, chapterId: number) {
    return apiClient<CourseChapter>(API_ENDPOINTS.COURSE.CHAPTER(courseId, chapterId));
  },
  createChapter(courseId: number, body: CreateChapterRequest) {
    return apiClient<CourseChapter>(API_ENDPOINTS.COURSE.CHAPTERS(courseId), {
      method: "POST",
      body,
    });
  },
  updateChapter(courseId: number, chapterId: number, body: UpdateChapterRequest) {
    return apiClient<CourseChapter>(API_ENDPOINTS.COURSE.CHAPTER(courseId, chapterId), {
      method: "PUT",
      body,
    });
  },
  deleteChapter(courseId: number, chapterId: number, body: EntityWorkflowRequest) {
    return apiClient<void>(API_ENDPOINTS.COURSE.CHAPTER(courseId, chapterId), {
      method: "DELETE",
      body,
    });
  },
  restoreChapter(courseId: number, chapterId: number, body: EntityWorkflowRequest) {
    return apiClient<CourseChapter>(API_ENDPOINTS.COURSE.CHAPTER_RESTORE(courseId, chapterId), {
      method: "POST",
      body,
    });
  },
  reorderChapters(courseId: number, body: ReorderChaptersRequest) {
    return apiClient<void>(API_ENDPOINTS.COURSE.CHAPTER_REORDER(courseId), {
      method: "PUT",
      body,
    });
  },
  lessons(courseId: number, chapterId: number) {
    return apiClient<CourseChapterLesson[]>(
      API_ENDPOINTS.COURSE.CHAPTER_LESSONS(courseId, chapterId),
    );
  },
  assignLesson(courseId: number, chapterId: number, body: AssignLessonRequest) {
    return apiClient<CourseChapterLesson>(
      API_ENDPOINTS.COURSE.CHAPTER_LESSON_ASSIGN(courseId, chapterId),
      { method: "POST", body },
    );
  },
  moveLesson(
    courseId: number,
    sourceChapterId: number,
    lessonId: number,
    body: MoveLessonRequest,
  ) {
    return apiClient<CourseChapterLesson>(
      API_ENDPOINTS.COURSE.CHAPTER_LESSON_MOVE(courseId, sourceChapterId, lessonId),
      { method: "POST", body },
    );
  },
  removeLesson(courseId: number, chapterId: number, lessonId: number) {
    return apiClient<void>(
      API_ENDPOINTS.COURSE.CHAPTER_LESSON(courseId, chapterId, lessonId),
      { method: "DELETE" },
    );
  },
  reorderLessons(courseId: number, chapterId: number, body: ReorderLessonsRequest) {
    return apiClient<void>(
      API_ENDPOINTS.COURSE.CHAPTER_LESSON_REORDER(courseId, chapterId),
      { method: "PUT", body },
    );
  },
};
