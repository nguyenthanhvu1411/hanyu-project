namespace HanYu.Domain.Constants;

public static class Permissions
{
    public static class Users
    {
        public const string Read = "users.read";
        public const string Create = "users.create";
        public const string Update = "users.update";
        public const string Delete = "users.delete";
        public const string Restore = "users.restore";

        public const string Lock = "users.lock";
        public const string Unlock = "users.unlock";

        public const string ManageRoles = "users.roles.manage";

        public const string Import = "users.import";
        public const string Export = "users.export";
    }

    public static class Roles
    {
        public const string Read = "roles.read";
        public const string Create = "roles.create";
        public const string Update = "roles.update";
        public const string Delete = "roles.delete";
        public const string Restore = "roles.restore";

        public const string ManagePermissions =
            "roles.permissions.manage";
    }

    public static class PermissionCatalog
    {
        public const string Read = "permissions.read";
        public const string Create = "permissions.create";
        public const string Update = "permissions.update";
        public const string Delete = "permissions.delete";
        public const string Restore = "permissions.restore";
    }

    public static class Sessions
    {
        public const string Read = "sessions.read";
        public const string Revoke = "sessions.revoke";
        public const string RevokeAll = "sessions.revoke-all";
        public const string Delete = "sessions.delete";
    }

    public static class HskLevels
    {
        public const string Read = "hsk-levels.read";
        public const string Create = "hsk-levels.create";
        public const string Update = "hsk-levels.update";
        public const string Delete = "hsk-levels.delete";
        public const string Restore = "hsk-levels.restore";
    }

    public static class Courses
    {
        public const string Read = "courses.read";
        public const string Create = "courses.create";
        public const string Update = "courses.update";
        public const string Delete = "courses.delete";
        public const string Restore = "courses.restore";

        public const string SubmitReview = "courses.submit-review";
        public const string Review = "courses.review";
        public const string Approve = "courses.approve";
        public const string Reject = "courses.reject";

        public const string Publish = "courses.publish";
        public const string Unpublish = "courses.unpublish";
        public const string Archive = "courses.archive";
        public const string Rollback = "courses.rollback";

        public const string Import = "courses.import";
        public const string Export = "courses.export";
    }

    public static class Chapters
    {
        public const string Read = "chapters.read";
        public const string Create = "chapters.create";
        public const string Update = "chapters.update";
        public const string Delete = "chapters.delete";
        public const string Restore = "chapters.restore";

        public const string Reorder = "chapters.reorder";

        public const string SubmitReview = "chapters.submit-review";
        public const string Review = "chapters.review";
        public const string Approve = "chapters.approve";
        public const string Reject = "chapters.reject";
        public const string Publish = "chapters.publish";
        public const string Rollback = "chapters.rollback";

        public const string Import = "chapters.import";
        public const string Export = "chapters.export";
    }

    public static class Lessons
    {
        public const string Read = "lessons.read";
        public const string Create = "lessons.create";
        public const string Update = "lessons.update";
        public const string Delete = "lessons.delete";
        public const string Restore = "lessons.restore";

        public const string Reorder = "lessons.reorder";

        public const string SubmitReview = "lessons.submit-review";
        public const string Review = "lessons.review";
        public const string Approve = "lessons.approve";
        public const string Reject = "lessons.reject";

        public const string Publish = "lessons.publish";
        public const string Unpublish = "lessons.unpublish";
        public const string Archive = "lessons.archive";
        public const string Rollback = "lessons.rollback";

        public const string Import = "lessons.import";
        public const string Export = "lessons.export";
    }

    public static class Vocabulary
    {
        public const string Read = "vocabulary.read";
        public const string Create = "vocabulary.create";
        public const string Update = "vocabulary.update";
        public const string Delete = "vocabulary.delete";
        public const string Restore = "vocabulary.restore";

        public const string SubmitReview = "vocabulary.submit-review";
        public const string Review = "vocabulary.review";
        public const string Approve = "vocabulary.approve";
        public const string Reject = "vocabulary.reject";
        public const string Publish = "vocabulary.publish";

        public const string Import = "vocabulary.import";
        public const string Export = "vocabulary.export";
    }

    public static class VocabularyMeanings
    {
        public const string Read = "vocabulary-meanings.read";
        public const string Create = "vocabulary-meanings.create";
        public const string Update = "vocabulary-meanings.update";
        public const string Delete = "vocabulary-meanings.delete";
    }

    public static class VocabularyExamples
    {
        public const string Read = "vocabulary-examples.read";
        public const string Create = "vocabulary-examples.create";
        public const string Update = "vocabulary-examples.update";
        public const string Delete = "vocabulary-examples.delete";
    }

    public static class VocabularyRelations
    {
        public const string Read = "vocabulary-relations.read";
        public const string Create = "vocabulary-relations.create";
        public const string Update = "vocabulary-relations.update";
        public const string Delete = "vocabulary-relations.delete";
    }

    public static class VocabularyTopics
    {
        public const string Read = "vocabulary-topics.read";
        public const string Create = "vocabulary-topics.create";
        public const string Update = "vocabulary-topics.update";
        public const string Delete = "vocabulary-topics.delete";
        public const string Restore = "vocabulary-topics.restore";
    }

    public static class PartsOfSpeech
    {
        public const string Read = "parts-of-speech.read";
        public const string Create = "parts-of-speech.create";
        public const string Update = "parts-of-speech.update";
        public const string Delete = "parts-of-speech.delete";
        public const string Restore = "parts-of-speech.restore";
    }

    public static class QuestionBank
    {
        public const string Read = "question-bank.read";
        public const string Create = "question-bank.create";
        public const string Update = "question-bank.update";
        public const string Delete = "question-bank.delete";
        public const string Restore = "question-bank.restore";

        public const string Review = "question-bank.review";
        public const string Approve = "question-bank.approve";
        public const string Reject = "question-bank.reject";

        public const string Import = "question-bank.import";
        public const string Export = "question-bank.export";
    }

    public static class Quizzes
    {
        public const string Read = "quizzes.read";
        public const string Create = "quizzes.create";
        public const string Update = "quizzes.update";
        public const string Delete = "quizzes.delete";
        public const string Restore = "quizzes.restore";

        public const string Publish = "quizzes.publish";
        public const string Unpublish = "quizzes.unpublish";

        public const string Import = "quizzes.import";
        public const string Export = "quizzes.export";
    }

    public static class QuizResults
    {
        public const string Read = "quiz-results.read";
        public const string Delete = "quiz-results.delete";
        public const string Export = "quiz-results.export";
    }

    public static class LearningGoals
    {
        public const string Read = "learning-goals.read";
        public const string Create = "learning-goals.create";
        public const string Update = "learning-goals.update";
        public const string Delete = "learning-goals.delete";
    }

    public static class LearningActivities
    {
        public const string Read = "learning-activities.read";
        public const string Delete = "learning-activities.delete";
        public const string Export = "learning-activities.export";
    }

    public static class LearningProgress
    {
        public const string Read = "learning-progress.read";
        public const string Update = "learning-progress.update";
        public const string Reset = "learning-progress.reset";
        public const string Export = "learning-progress.export";
    }

    public static class Media
    {
        public const string Read = "media.read";
        public const string Upload = "media.upload";
        public const string Delete = "media.delete";
        public const string Restore = "media.restore";
        public const string Quarantine = "media.quarantine";
    }

    public static class Notifications
    {
        public const string Read = "notifications.read";
        public const string Create = "notifications.create";
        public const string Update = "notifications.update";
        public const string Delete = "notifications.delete";

        public const string Send = "notifications.send";
        public const string Broadcast = "notifications.broadcast";
    }

    public static class EmailTemplates
    {
        public const string Read = "email-templates.read";
        public const string Create = "email-templates.create";
        public const string Update = "email-templates.update";
        public const string Delete = "email-templates.delete";

        public const string Preview = "email-templates.preview";
        public const string SendTest = "email-templates.send-test";
    }

    public static class AuditLogs
    {
        public const string Read = "audit-logs.read";
        public const string Export = "audit-logs.export";
    }

    public static class ReviewQueue
    {
        public const string Read = "review-queue.read";
        public const string Review = "review-queue.review";
        public const string Approve = "review-queue.approve";
        public const string Reject = "review-queue.reject";
    }

    public static class SystemSettings
    {
        public const string Read = "system-settings.read";
        public const string Update = "system-settings.update";
    }

    public static class FeatureFlags
    {
        public const string Read = "feature-flags.read";
        public const string Create = "feature-flags.create";
        public const string Update = "feature-flags.update";
        public const string Delete = "feature-flags.delete";
    }

    public static class Reports
    {
        public const string Read = "reports.read";
        public const string Export = "reports.export";
    }

    public static class ImportJobs
    {
        public const string Read = "import-jobs.read";
        public const string Create = "import-jobs.create";
        public const string Cancel = "import-jobs.cancel";
        public const string Delete = "import-jobs.delete";
    }

    public static class ExportJobs
    {
        public const string Read = "export-jobs.read";
        public const string Create = "export-jobs.create";
        public const string Cancel = "export-jobs.cancel";
        public const string Download = "export-jobs.download";
    }

    public static class Comments
    {
        public const string Read = "comments.read";
        public const string Update = "comments.update";
        public const string Delete = "comments.delete";
        public const string Moderate = "comments.moderate";
    }

    public static class Reviews
    {
        public const string Read = "reviews.read";
        public const string Update = "reviews.update";
        public const string Delete = "reviews.delete";
        public const string Moderate = "reviews.moderate";
    }

    public static class Gamification
    {
        public const string Read = "gamification.read";
        public const string Create = "gamification.create";
        public const string Update = "gamification.update";
        public const string Delete = "gamification.delete";
    }
}