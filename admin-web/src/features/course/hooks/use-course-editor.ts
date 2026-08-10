"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { courseApi } from "../api/course.api";
import { curriculumApi } from "../api/curriculum.api";
import { prerequisiteApi } from "../api/prerequisite.api";
import type { AdminCourseDetail } from "../types/course.types";
import type {
  CourseChapter,
  CourseChapterLesson,
  CoursePrerequisite,
  CourseValidationResult,
  CreateChapterRequest,
  CreatePrerequisiteRequest,
  MoveLessonRequest,
  UpdateChapterRequest,
} from "../types/curriculum.types";

export type CourseEditorTab = "overview" | "curriculum" | "prerequisites";
interface ChapterLessonsState { [chapterId: number]: CourseChapterLesson[]; }

export function useCourseEditor(courseId: number) {
  const [course, setCourse] = useState<AdminCourseDetail | null>(null);
  const [chapters, setChapters] = useState<CourseChapter[]>([]);
  const [chapterLessons, setChapterLessons] = useState<ChapterLessonsState>({});
  const [prerequisites, setPrerequisites] = useState<CoursePrerequisite[]>([]);
  const [validation, setValidation] = useState<CourseValidationResult | null>(null);
  const [tab, setTab] = useState<CourseEditorTab>("curriculum");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const canEdit = course?.status === 0 && !course.deletedAt;

  const loadChapterLessons = useCallback(async (chapterId: number) => {
    const lessons = await curriculumApi.lessons(courseId, chapterId);
    setChapterLessons((current) => ({ ...current, [chapterId]: lessons }));
    return lessons;
  }, [courseId]);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const [courseResult, chapterResult, prerequisiteResult] = await Promise.all([
        courseApi.getById(courseId),
        curriculumApi.chapters(courseId),
        prerequisiteApi.list(courseId),
      ]);
      setCourse(courseResult);
      const ordered = [...chapterResult].sort((a, b) => a.sortOrder - b.sortOrder);
      setChapters(ordered);
      setPrerequisites(prerequisiteResult);
      const lessonResults = await Promise.all(ordered.map(async (chapter) => ({
        chapterId: chapter.id,
        lessons: await curriculumApi.lessons(courseId, chapter.id),
      })));
      setChapterLessons(Object.fromEntries(lessonResults.map(({ chapterId, lessons }) => [chapterId, lessons])));
    } catch (e) {
      setError(e instanceof Error ? e.message : "Không thể tải khóa học.");
    } finally { setLoading(false); }
  }, [courseId]);

  useEffect(() => { void load(); }, [load]);

  const refreshCourse = useCallback(async () => {
    const result = await courseApi.getById(courseId);
    setCourse(result);
    return result;
  }, [courseId]);

  async function run<T>(action: () => Promise<T>): Promise<T | undefined> {
    try { setSaving(true); setError(null); return await action(); }
    catch (e) { setError(e instanceof Error ? e.message : "Thao tác thất bại."); return undefined; }
    finally { setSaving(false); }
  }

  async function createChapter(body: CreateChapterRequest) {
    return run(async () => {
      const created = await curriculumApi.createChapter(courseId, body);
      setChapters((current) => [...current, created].sort((a, b) => a.sortOrder - b.sortOrder));
      setChapterLessons((current) => ({ ...current, [created.id]: [] }));
      await refreshCourse();
      return created;
    });
  }

  async function updateChapter(chapterId: number, body: UpdateChapterRequest) {
    return run(async () => {
      const updated = await curriculumApi.updateChapter(courseId, chapterId, body);
      setChapters((current) => current.map((item) => item.id === chapterId ? updated : item).sort((a, b) => a.sortOrder - b.sortOrder));
      return updated;
    });
  }

  async function deleteChapter(chapter: CourseChapter) {
    return run(async () => {
      await curriculumApi.deleteChapter(courseId, chapter.id, { concurrencyToken: chapter.concurrencyToken });
      setChapters((current) => current.filter((x) => x.id !== chapter.id));
      setChapterLessons((current) => { const next = { ...current }; delete next[chapter.id]; return next; });
      await refreshCourse();
    });
  }

  async function assignLesson(chapterId: number, lessonId: number) {
    return run(async () => {
      const current = chapterLessons[chapterId] ?? [];
      const sortOrder = current.length === 0 ? 0 : Math.max(...current.map((x) => x.sortOrder)) + 1;
      await curriculumApi.assignLesson(courseId, chapterId, { lessonId, sortOrder });
      await loadChapterLessons(chapterId);
      await refreshCourse();
    });
  }

  async function removeLesson(chapterId: number, lessonId: number) {
    return run(async () => { await curriculumApi.removeLesson(courseId, chapterId, lessonId); await loadChapterLessons(chapterId); await refreshCourse(); });
  }

  async function moveLesson(sourceChapterId: number, lessonId: number, request: MoveLessonRequest) {
    return run(async () => {
      await curriculumApi.moveLesson(courseId, sourceChapterId, lessonId, request);
      await Promise.all([loadChapterLessons(sourceChapterId), loadChapterLessons(request.targetChapterId)]);
      await refreshCourse();
    });
  }

  async function reorderLessons(chapterId: number, items: CourseChapterLesson[]) {
    return run(async () => {
      await curriculumApi.reorderLessons(courseId, chapterId, { items: items.map((item, index) => ({ lessonId: item.id, sortOrder: index })) });
      await loadChapterLessons(chapterId);
    });
  }

  async function moveLessonUp(chapterId: number, lessonId: number) {
    const items = [...(chapterLessons[chapterId] ?? [])].sort((a, b) => a.sortOrder - b.sortOrder);
    const index = items.findIndex((x) => x.id === lessonId);
    if (index <= 0) return;
    [items[index - 1], items[index]] = [items[index], items[index - 1]];
    return reorderLessons(chapterId, items);
  }

  async function moveLessonDown(chapterId: number, lessonId: number) {
    const items = [...(chapterLessons[chapterId] ?? [])].sort((a, b) => a.sortOrder - b.sortOrder);
    const index = items.findIndex((x) => x.id === lessonId);
    if (index < 0 || index >= items.length - 1) return;
    [items[index], items[index + 1]] = [items[index + 1], items[index]];
    return reorderLessons(chapterId, items);
  }

  async function validateCourse() { return run(async () => { const result = await courseApi.validate(courseId); setValidation(result); return result; }); }
  async function createPrerequisite(body: CreatePrerequisiteRequest) { return run(async () => { const created = await prerequisiteApi.create(courseId, body); setPrerequisites((current) => [...current, created]); return created; }); }
  async function deletePrerequisite(item: CoursePrerequisite) { return run(async () => { await prerequisiteApi.delete(courseId, item.id, { concurrencyToken: item.concurrencyToken }); setPrerequisites((current) => current.filter((x) => x.id !== item.id)); }); }

  const lessonCount = useMemo(() => Object.values(chapterLessons).reduce((sum, items) => sum + items.length, 0), [chapterLessons]);

  return {
    course, chapters, chapterLessons, prerequisites, validation, tab, setTab, loading, saving, error,
    canEdit, lessonCount, reload: load, refreshCourse, createChapter, updateChapter, deleteChapter,
    assignLesson, removeLesson, moveLesson, moveLessonUp, moveLessonDown, validateCourse,
    createPrerequisite, deletePrerequisite,
  };
}

export type CourseEditorController = ReturnType<typeof useCourseEditor>;
