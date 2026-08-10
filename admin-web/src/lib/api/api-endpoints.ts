export const API_ENDPOINTS = {
  ADMIN: {
    USERS: "/admin/users",
    USER: (id: string) => `/admin/users/${id}`,
    USER_RESTORE: (id: string) => `/admin/users/${id}/restore`,
    USER_LOCK: (id: string) => `/admin/users/${id}/lock`,
    USER_UNLOCK: (id: string) => `/admin/users/${id}/unlock`,
    USER_ROLES: (id: string) => `/admin/users/${id}/roles`,
    USER_SESSIONS: (id: string) => `/admin/users/${id}/sessions`,

    ROLES: "/admin/roles",
    ROLE: (id: string) => `/admin/roles/${id}`,
    ROLE_RESTORE: (id: string) => `/admin/roles/${id}/restore`,

    PERMISSIONS: "/admin/permissions",
    PERMISSION: (id: string) => `/admin/permissions/${id}`,
    PERMISSION_RESTORE: (id: string) => `/admin/permissions/${id}/restore`,

    SESSIONS: "/admin/sessions",
    SESSION: (id: string) => `/admin/sessions/${id}`,
    SESSION_RESTORE: (id: string) => `/admin/sessions/${id}/restore`,
  },

  LEARNING: {
    HSK_LEVELS: "/admin/hsk-levels",
    HSK_LEVEL: (id: number) => `/admin/hsk-levels/${id}`,
    HSK_LEVEL_ACTIVATE: (id: number) => `/admin/hsk-levels/${id}/activate`,
    HSK_LEVEL_DEACTIVATE: (id: number) => `/admin/hsk-levels/${id}/deactivate`,
  },

  COURSE: {
    ROOT: "/admin/courses",
    DETAIL: (courseId: number) => `/admin/courses/${courseId}`,
    VALIDATE: (courseId: number) => `/admin/courses/${courseId}/validate`,
    SUBMIT_REVIEW: (courseId: number) => `/admin/courses/${courseId}/submit-review`,
    APPROVE: (courseId: number) => `/admin/courses/${courseId}/approve`,
    REJECT: (courseId: number) => `/admin/courses/${courseId}/reject`,
    PUBLISH: (courseId: number) => `/admin/courses/${courseId}/publish`,
    ARCHIVE: (courseId: number) => `/admin/courses/${courseId}/archive`,
    RESTORE: (courseId: number) => `/admin/courses/${courseId}/restore`,
    RESTORE_DELETED: (courseId: number) => `/admin/courses/${courseId}/restore-deleted`,

    CHAPTERS: (courseId: number) => `/admin/courses/${courseId}/chapters`,
    CHAPTER: (courseId: number, chapterId: number) =>
      `/admin/courses/${courseId}/chapters/${chapterId}`,
    CHAPTER_RESTORE: (courseId: number, chapterId: number) =>
      `/admin/courses/${courseId}/chapters/${chapterId}/restore`,
    CHAPTER_REORDER: (courseId: number) =>
      `/admin/courses/${courseId}/chapters/order`,

    CHAPTER_LESSONS: (courseId: number, chapterId: number) =>
      `/admin/courses/${courseId}/chapters/${chapterId}/lessons`,
    CHAPTER_LESSON_ASSIGN: (courseId: number, chapterId: number) =>
      `/admin/courses/${courseId}/chapters/${chapterId}/lessons/assign`,
    CHAPTER_LESSON: (courseId: number, chapterId: number, lessonId: number) =>
      `/admin/courses/${courseId}/chapters/${chapterId}/lessons/${lessonId}`,
    CHAPTER_LESSON_MOVE: (courseId: number, chapterId: number, lessonId: number) =>
      `/admin/courses/${courseId}/chapters/${chapterId}/lessons/${lessonId}/move`,
    CHAPTER_LESSON_REORDER: (courseId: number, chapterId: number) =>
      `/admin/courses/${courseId}/chapters/${chapterId}/lessons/reorder`,
  },

  LESSON: {
    ROOT: "/admin/lessons",
    DETAIL: (lessonId: number) => `/admin/lessons/${lessonId}`,
    VALIDATE: (lessonId: number) => `/admin/lessons/${lessonId}/validate`,
    SUBMIT_REVIEW: (lessonId: number) => `/admin/lessons/${lessonId}/submit-review`,
    APPROVE: (lessonId: number) => `/admin/lessons/${lessonId}/approve`,
    PUBLISH: (lessonId: number) => `/admin/lessons/${lessonId}/publish`,
    ARCHIVE: (lessonId: number) => `/admin/lessons/${lessonId}/archive`,
    RESTORE: (lessonId: number) => `/admin/lessons/${lessonId}/restore`,
    RESTORE_DELETED: (lessonId: number) => `/admin/lessons/${lessonId}/restore-deleted`,

    SECTIONS: (lessonId: number) => `/admin/lessons/${lessonId}/sections`,
    SECTION: (lessonId: number, sectionId: number) =>
      `/admin/lessons/${lessonId}/sections/${sectionId}`,

    VOCABULARY: (lessonId: number) => `/admin/lessons/${lessonId}/vocabulary`,
    VOCABULARY_ITEM: (lessonId: number, vocabularyId: number) =>
      `/admin/lessons/${lessonId}/vocabulary/${vocabularyId}`,

    ASSETS: (lessonId: number) => `/admin/lessons/${lessonId}/assets`,
    ASSET: (lessonId: number, assetId: number) =>
      `/admin/lessons/${lessonId}/assets/${assetId}`,

    PREREQUISITES: (lessonId: number) => `/admin/lessons/${lessonId}/prerequisites`,
    PREREQUISITE: (lessonId: number, requiredLessonId: number) =>
      `/admin/lessons/${lessonId}/prerequisites/${requiredLessonId}`,
  },
} as const;
