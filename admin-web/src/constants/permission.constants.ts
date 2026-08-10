function crud(resource: string) {
  return {
    READ: `${resource}.read`,
    CREATE: `${resource}.create`,
    UPDATE: `${resource}.update`,
    DELETE: `${resource}.delete`,
  };
}

export const PERMISSIONS = {
  USERS: {
    READ: "users.read",
    CREATE: "users.create",
    UPDATE: "users.update",
    DELETE: "users.delete",
    RESTORE: "users.restore",

    LOCK: "users.lock",
    UNLOCK: "users.unlock",

    MANAGE_ROLES: "users.roles.manage",

    IMPORT: "users.import",
    EXPORT: "users.export",
  },

  ROLES: {
    READ: "roles.read",
    CREATE: "roles.create",
    UPDATE: "roles.update",
    DELETE: "roles.delete",
    RESTORE: "roles.restore",

    MANAGE_PERMISSIONS: "roles.permissions.manage",
  },

  PERMISSIONS: {
    READ: "permissions.read",
    CREATE: "permissions.create",
    UPDATE: "permissions.update",
    DELETE: "permissions.delete",
    RESTORE: "permissions.restore",
  },

  SESSIONS: {
    READ: "sessions.read",
    REVOKE: "sessions.revoke",
    REVOKE_ALL: "sessions.revoke-all",
  },
  HSK_LEVELS: {
    ...crud("hsk-levels"),
    ACTIVATE: "hsk-levels.activate",
    DEACTIVATE: "hsk-levels.deactivate",
  },

  COURSES: {
    ...crud("courses"),
    RESTORE: "courses.restore",
    SUBMIT_REVIEW: "courses.submit-review",
    REVIEW: "courses.review",
    APPROVE: "courses.approve",
    REJECT: "courses.reject",
    PUBLISH: "courses.publish",
    UNPUBLISH: "courses.unpublish",
    ARCHIVE: "courses.archive",
    ROLLBACK: "courses.rollback",
    IMPORT: "courses.import",
    EXPORT: "courses.export",
  },

  CHAPTERS: {
    ...crud("chapters"),
    RESTORE: "chapters.restore",
    REORDER: "chapters.reorder",
    PUBLISH: "chapters.publish",
    IMPORT: "chapters.import",
    EXPORT: "chapters.export",
  },

  LESSONS: {
    ...crud("lessons"),
    RESTORE: "lessons.restore",
    REORDER: "lessons.reorder",
    PUBLISH: "lessons.publish",
    IMPORT: "lessons.import",
    EXPORT: "lessons.export",
  },

  VOCABULARY: {
    ...crud("vocabulary"),
    RESTORE: "vocabulary.restore",
    REVIEW: "vocabulary.review",
    APPROVE: "vocabulary.approve",
    REJECT: "vocabulary.reject",
    PUBLISH: "vocabulary.publish",
    IMPORT: "vocabulary.import",
    EXPORT: "vocabulary.export",
  },

  QUESTION_BANK: {
    ...crud("question-bank"),
    RESTORE: "question-bank.restore",
    REVIEW: "question-bank.review",
    APPROVE: "question-bank.approve",
    REJECT: "question-bank.reject",
    IMPORT: "question-bank.import",
    EXPORT: "question-bank.export",
  },

  QUIZZES: {
    ...crud("quizzes"),
    RESTORE: "quizzes.restore",
    PUBLISH: "quizzes.publish",
    UNPUBLISH: "quizzes.unpublish",
    IMPORT: "quizzes.import",
    EXPORT: "quizzes.export",
  },

  MEDIA: {
    READ: "media.read",
    UPLOAD: "media.upload",
    DELETE: "media.delete",
    RESTORE: "media.restore",
  },

  NOTIFICATIONS: {
    ...crud("notifications"),
    SEND: "notifications.send",
    BROADCAST: "notifications.broadcast",
  },

  AUDIT: {
    READ: "audit-logs.read",
    EXPORT: "audit-logs.export",
  },

  SYSTEM: {
    READ: "system-settings.read",
    UPDATE: "system-settings.update",
  },

  REPORTS: {
    READ: "reports.read",
    EXPORT: "reports.export",
  },
} as const;
