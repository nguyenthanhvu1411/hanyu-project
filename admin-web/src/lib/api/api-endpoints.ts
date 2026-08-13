export const API_ENDPOINTS = {
  ADMIN: {
    USERS: "/admin/users",
    USER: (id: string) => `/admin/users/${id}`,
    USER_RESTORE: (id: string) => `/admin/users/${id}/restore`,
    USER_LOCK: (id: string) => `/admin/users/${id}/lock`,
    USER_UNLOCK: (id: string) => `/admin/users/${id}/unlock`,
    USER_ROLES: (id: string) => `/admin/users/${id}/roles`,
    USER_SESSIONS: (id: string) => `/admin/users/${id}/sessions`,
    USER_RESET_PASSWORD: (id: string) => `/admin/users/${id}/reset-password`,
    ROLES: "/admin/roles",
    ROLE: (id: string) => `/admin/roles/${id}`,
    ROLE_RESTORE: (id: string) => `/admin/roles/${id}/restore`,
    PERMISSIONS: "/admin/permissions",
    PERMISSION: (id: string) => `/admin/permissions/${id}`,
    PERMISSION_RESTORE: (id: string) => `/admin/permissions/${id}/restore`,
    SESSIONS: "/admin/sessions",
    SESSION: (id: string) => `/admin/sessions/${id}`,
    SESSION_RESTORE: (id: string) => `/admin/sessions/${id}/restore`,
    UPLOAD_IMAGE: "/admin/uploads/images",
    UPLOAD_AUDIO: "/admin/uploads/audio",
    UPLOAD_VIDEO: "/admin/uploads/videos",
    UPLOAD_DOCUMENT: "/admin/uploads/documents",
  },

  MEDIA: {
    READ_URL: (objectKey: string) => `/media/read-url?objectKey=${encodeURIComponent(objectKey)}`,
  },

  LEARNING: {
    HSK_LEVELS: "/admin/hsk-levels",
    HSK_LEVEL: (id: number) => `/admin/hsk-levels/${id}`,
    HSK_LEVEL_RESTORE: (id: number) => `/admin/hsk-levels/${id}/restore`,
    HSK_LEVEL_ACTIVATE: (id: number) => `/admin/hsk-levels/${id}/activate`,
    HSK_LEVEL_DEACTIVATE: (id: number) => `/admin/hsk-levels/${id}/deactivate`,
  },

  VOCABULARY: {
    ROOT: "/admin/vocabularies",
    DETAIL: (id: number) => `/admin/vocabularies/${id}`,
    AUDIO: (id: number) => `/admin/vocabularies/${id}/audio`,
    VALIDATE: (id: number, forPublish = false) => `/admin/vocabularies/${id}/validate${forPublish ? "?forPublish=true" : ""}`,
    SUBMIT_REVIEW: (id: number) => `/admin/vocabularies/${id}/submit-review`,
    APPROVE: (id: number) => `/admin/vocabularies/${id}/approve`,
    PUBLISH: (id: number) => `/admin/vocabularies/${id}/publish`,
    ARCHIVE: (id: number) => `/admin/vocabularies/${id}/archive`,
    RESTORE: (id: number) => `/admin/vocabularies/${id}/restore`,
    TOPICS: "/admin/vocabulary-topics",
    TOPIC_SLUG_AVAILABILITY: (slug: string, excludeId?: number) => {
      const params = new URLSearchParams({ slug });
      if (excludeId !== undefined) params.set("excludeId", String(excludeId));
      return `/admin/vocabulary-topics/slug-availability?${params.toString()}`;
    },
    PARTS_OF_SPEECH: "/admin/parts-of-speech",
    PART_OF_SPEECH: (id: number) => `/admin/parts-of-speech/${id}`,
    MEANINGS: (id: number) => `/admin/vocabularies/${id}/meanings`,
    MEANING: (id: number, meaningId: number) => `/admin/vocabularies/${id}/meanings/${meaningId}`,
    EXAMPLES: (id: number) => `/admin/vocabularies/${id}/examples`,
    EXAMPLE: (id: number, exampleId: number) => `/admin/vocabularies/${id}/examples/${exampleId}`,
    EXAMPLE_AUDIO: (id: number, exampleId: number) => `/admin/vocabularies/${id}/examples/${exampleId}/audio`,
    EXAMPLE_SUBMIT_REVIEW: (id: number, exampleId: number) => `/admin/vocabularies/${id}/examples/${exampleId}/submit-review`,
    EXAMPLE_APPROVE: (id: number, exampleId: number) => `/admin/vocabularies/${id}/examples/${exampleId}/approve`,
    EXAMPLE_PUBLISH: (id: number, exampleId: number) => `/admin/vocabularies/${id}/examples/${exampleId}/publish`,
    EXAMPLE_ARCHIVE: (id: number, exampleId: number) => `/admin/vocabularies/${id}/examples/${exampleId}/archive`,
    EXAMPLE_RESTORE: (id: number, exampleId: number) => `/admin/vocabularies/${id}/examples/${exampleId}/restore`,
    RELATIONS: (id: number) => `/admin/vocabularies/${id}/relations`,
    RELATION: (id: number, relationId: number) => `/admin/vocabularies/${id}/relations/${relationId}`,
    AUDIO_ASSETS: "/admin/audio-assets",
    AUDIO_ASSET: (id: number) => `/admin/audio-assets/${id}`,
    AUDIO_ASSET_PUBLISH: (id: number) => `/admin/audio-assets/${id}/publish`,
    AUDIO_ASSET_ARCHIVE: (id: number) => `/admin/audio-assets/${id}/archive`,
  },

  COURSE: {
    ROOT: "/admin/courses",
    DETAIL: (courseId: number) => `/admin/courses/${courseId}`,
    SLUG_AVAILABILITY: (slug: string, excludeId?: number) => {
      const params = new URLSearchParams({ slug });
      if (excludeId !== undefined) params.set("excludeId", String(excludeId));
      return `/admin/courses/slug-availability?${params.toString()}`;
    },
    VALIDATE: (courseId: number) => `/admin/courses/${courseId}/validate`,
    SUBMIT_REVIEW: (courseId: number) => `/admin/courses/${courseId}/submit-review`,
    APPROVE: (courseId: number) => `/admin/courses/${courseId}/approve`,
    REJECT: (courseId: number) => `/admin/courses/${courseId}/reject`,
    PUBLISH: (courseId: number) => `/admin/courses/${courseId}/publish`,
    ARCHIVE: (courseId: number) => `/admin/courses/${courseId}/archive`,
    RESTORE: (courseId: number) => `/admin/courses/${courseId}/restore`,
    RESTORE_DELETED: (courseId: number) => `/admin/courses/${courseId}/restore-deleted`,

    CHAPTERS: (courseId: number) => `/admin/courses/${courseId}/chapters`,
    CHAPTER: (courseId: number, chapterId: number) => `/admin/courses/${courseId}/chapters/${chapterId}`,
    CHAPTER_RESTORE: (courseId: number, chapterId: number) => `/admin/courses/${courseId}/chapters/${chapterId}/restore`,
    CHAPTER_REORDER: (courseId: number) => `/admin/courses/${courseId}/chapters/order`,

    CHAPTER_LESSONS: (courseId: number, chapterId: number) => `/admin/courses/${courseId}/chapters/${chapterId}/lessons`,
    CHAPTER_LESSON_ASSIGN: (courseId: number, chapterId: number) => `/admin/courses/${courseId}/chapters/${chapterId}/lessons/assign`,
    CHAPTER_LESSON: (courseId: number, chapterId: number, lessonId: number) => `/admin/courses/${courseId}/chapters/${chapterId}/lessons/${lessonId}`,
    CHAPTER_LESSON_MOVE: (courseId: number, chapterId: number, lessonId: number) => `/admin/courses/${courseId}/chapters/${chapterId}/lessons/${lessonId}/move`,
    CHAPTER_LESSON_REORDER: (courseId: number, chapterId: number) => `/admin/courses/${courseId}/chapters/${chapterId}/lessons/reorder`,

    PREREQUISITES: (courseId: number) => `/admin/courses/${courseId}/prerequisites`,
    PREREQUISITE: (courseId: number, prerequisiteId: number) => `/admin/courses/${courseId}/prerequisites/${prerequisiteId}`,
    PREREQUISITE_RESTORE: (courseId: number, prerequisiteId: number) => `/admin/courses/${courseId}/prerequisites/${prerequisiteId}/restore`,
  },

  LESSON: {
    ROOT: "/admin/lessons",
    DETAIL: (lessonId: number) => `/admin/lessons/${lessonId}`,
    SLUG_AVAILABILITY: (slug: string, excludeId?: number) => {
      const params = new URLSearchParams({ slug });
      if (excludeId !== undefined) params.set("excludeId", String(excludeId));
      return `/admin/lessons/slug-availability?${params.toString()}`;
    },
    VALIDATE: (lessonId: number) => `/admin/lessons/${lessonId}/validate`,
    SUBMIT_REVIEW: (lessonId: number) => `/admin/lessons/${lessonId}/submit-review`,
    APPROVE: (lessonId: number) => `/admin/lessons/${lessonId}/approve`,
    PUBLISH: (lessonId: number) => `/admin/lessons/${lessonId}/publish`,
    ARCHIVE: (lessonId: number) => `/admin/lessons/${lessonId}/archive`,
    RESTORE: (lessonId: number) => `/admin/lessons/${lessonId}/restore`,
    RESTORE_DELETED: (lessonId: number) => `/admin/lessons/${lessonId}/restore-deleted`,
    SECTIONS: (lessonId: number) => `/admin/lessons/${lessonId}/sections`,
    SECTION: (lessonId: number, sectionId: number) => `/admin/lessons/${lessonId}/sections/${sectionId}`,
    VOCABULARY: (lessonId: number) => `/admin/lessons/${lessonId}/vocabulary`,
    VOCABULARY_ITEM: (lessonId: number, vocabularyId: number) => `/admin/lessons/${lessonId}/vocabulary/${vocabularyId}`,
    ASSETS: (lessonId: number) => `/admin/lessons/${lessonId}/assets`,
    ASSET: (lessonId: number, assetId: number) => `/admin/lessons/${lessonId}/assets/${assetId}`,
    PREREQUISITES: (lessonId: number) => `/admin/lessons/${lessonId}/prerequisites`,
    PREREQUISITE: (lessonId: number, requiredLessonId: number) => `/admin/lessons/${lessonId}/prerequisites/${requiredLessonId}`,
  },
} as const;
