using System.Text.Json;
using HanYu.Domain.Entities.AI;
using HanYu.Domain.Entities.Analytics;
using HanYu.Domain.Entities.Gamification;
using HanYu.Domain.Entities.Identity;
using HanYu.Domain.Entities.Learning;
using HanYu.Domain.Entities.Lesson;
using HanYu.Domain.Entities.Notification;
using HanYu.Domain.Entities.Operations;
using HanYu.Domain.Entities.Quiz;
using HanYu.Domain.Entities.Review;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Configurations.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HanYu.Domain.Entities.Course;
using CourseEntity = HanYu.Domain.Entities.Course.Course;
using LessonEntity = HanYu.Domain.Entities.Lesson.Lesson;
using QuizEntity = HanYu.Domain.Entities.Quiz.Quiz;
using VocabularyEntity = HanYu.Domain.Entities.Vocabulary.Vocabulary;

namespace HanYu.Infrastructure.Persistence;

public class HanYuDbContext : IdentityDbContext<User, Role, Guid>, HanYu.Application.Interfaces.Persistence.IHanYuDbContext
{
    public HanYuDbContext(DbContextOptions<HanYuDbContext> options)
        : base(options)
    {
    }

    // ==========================================
    // 1. Identity Module
    // ==========================================
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<UserBlockedSession> UserBlockedSessions => Set<UserBlockedSession>();
    public DbSet<UserSecurityEvent> UserSecurityEvents => Set<UserSecurityEvent>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // ==========================================
    // 2. Shared taxonomy
    // ==========================================
    // Topic is shared by Lesson/Vocabulary. The CLR namespace remains under
    // Vocabulary for migration compatibility, but it is no longer vocabulary-only.
    public DbSet<Topic> Topics => Set<Topic>();

    // ==========================================
    // 3. Vocabulary Module
    // ==========================================
    public DbSet<HskLevel> HskLevels => Set<HskLevel>();
    public DbSet<PartOfSpeech> PartsOfSpeech => Set<PartOfSpeech>();
    public DbSet<VocabularyEntity> Vocabularies => Set<VocabularyEntity>();
    public DbSet<VocabularyExample> VocabularyExamples => Set<VocabularyExample>();
    public DbSet<VocabularyMeaning> VocabularyMeanings => Set<VocabularyMeaning>();
    public DbSet<VocabularyRelation> VocabularyRelations => Set<VocabularyRelation>();
    public DbSet<AudioAsset> AudioAssets => Set<AudioAsset>();
    public DbSet<UserVocabularyNote> UserVocabularyNotes => Set<UserVocabularyNote>();

    // ==========================================
    // 4. Quiz Module
    // ==========================================
    public DbSet<QuizEntity> Quizzes => Set<QuizEntity>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizQuestionOption> QuizQuestionOptions => Set<QuizQuestionOption>();
    public DbSet<QuizMatchingPair> QuizMatchingPairs => Set<QuizMatchingPair>();
    public DbSet<QuizTag> QuizTags => Set<QuizTag>();
    public DbSet<QuizQuestionTag> QuizQuestionTags => Set<QuizQuestionTag>();
    public DbSet<QuizQuestionBank> QuizQuestionBanks => Set<QuizQuestionBank>();
    public DbSet<QuizQuestionBankItem> QuizQuestionBankItems => Set<QuizQuestionBankItem>();

    // ==========================================
    // 5. Review Module
    // ==========================================
    public DbSet<FlashcardSession> FlashcardSessions => Set<FlashcardSession>();
    public DbSet<FlashcardSessionItem> FlashcardSessionItems => Set<FlashcardSessionItem>();
    public DbSet<ReviewEvent> ReviewEvents => Set<ReviewEvent>();
    public DbSet<UserVocabularyState> UserVocabularyStates => Set<UserVocabularyState>();

    // ==========================================
    // 6. Lesson Module
    // ==========================================
    public DbSet<LessonEntity> Lessons => Set<LessonEntity>();
    public DbSet<LessonSection> LessonSections => Set<LessonSection>();
    public DbSet<LessonPrerequisite> LessonPrerequisites => Set<LessonPrerequisite>();
    public DbSet<LessonVocabulary> LessonVocabularies => Set<LessonVocabulary>();
    public DbSet<UserLessonProgress> UserLessonProgresses => Set<UserLessonProgress>();
    public DbSet<UserLessonSectionProgress> UserLessonSectionProgresses => Set<UserLessonSectionProgress>();

    // ==========================================
    // 7. Learning Module
    // ==========================================
    public DbSet<LearningActivity> LearningActivities => Set<LearningActivity>();
    public DbSet<UserLessonBookmark> UserLessonBookmarks => Set<UserLessonBookmark>();
    public DbSet<UserLearningGoal> UserLearningGoals => Set<UserLearningGoal>();
    public DbSet<UserLearningSummary> UserLearningSummaries => Set<UserLearningSummary>();

    // ==========================================
    // 8. Gamification & Analytics Module
    // ==========================================
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    public DbSet<UserStreak> UserStreaks => Set<UserStreak>();

    // ==========================================
    // 9. Notification Module
    // ==========================================
    public DbSet<InAppNotification> InAppNotifications => Set<InAppNotification>();
    public DbSet<NotificationDelivery> NotificationDeliveries => Set<NotificationDelivery>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    // ==========================================
    // 10. Operations Module
    // ==========================================
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ProductEvent> ProductEvents => Set<ProductEvent>();

    // ==========================================
    // 11. AI Module
    // ==========================================
    public DbSet<AiConversation> AiConversations => Set<AiConversation>();
    public DbSet<AiRequest> AiRequests => Set<AiRequest>();

    // ==========================================
    // 12. Course Module
    // ==========================================
    public DbSet<CourseEntity> Courses => Set<CourseEntity>();
    public DbSet<CourseChapter> CourseChapters => Set<CourseChapter>();
    public DbSet<CoursePrerequisite> CoursePrerequisites => Set<CoursePrerequisite>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await ValidateLessonClassificationAsync(cancellationToken);
        AddCourseAuditEntries();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Final persistence boundary for Lesson classification invariants.
    /// This intentionally lives below controllers/services so every write path is protected.
    /// </summary>
    private async Task ValidateLessonClassificationAsync(CancellationToken cancellationToken)
    {
        var lessonEntries = ChangeTracker
            .Entries<LessonEntity>()
            .Where(entry =>
                entry.State is EntityState.Added or EntityState.Modified &&
                !entry.Entity.IsDeleted)
            .ToArray();

        if (lessonEntries.Length == 0)
        {
            return;
        }

        var chapterIds = lessonEntries
            .Select(entry => entry.Entity.CourseChapterId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToArray();

        var chapters = chapterIds.Length == 0
            ? new Dictionary<long, CourseChapter>()
            : await CourseChapters
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Include(chapter => chapter.Course)
                .Where(chapter => chapterIds.Contains(chapter.Id))
                .ToDictionaryAsync(chapter => chapter.Id, cancellationToken);

        var topicIds = lessonEntries
            .Where(entry =>
                entry.Entity.TopicId.HasValue &&
                entry.Entity.Status is ContentStatus.Review or ContentStatus.Approved or ContentStatus.Published)
            .Select(entry => entry.Entity.TopicId!.Value)
            .Distinct()
            .ToArray();

        var publishedTopicIds = topicIds.Length == 0
            ? new HashSet<long>()
            : (await Topics
                .AsNoTracking()
                .Where(topic =>
                    topicIds.Contains(topic.Id) &&
                    topic.Status == ContentStatus.Published &&
                    topic.DeletedAt == null)
                .Select(topic => topic.Id)
                .ToArrayAsync(cancellationToken))
                .ToHashSet();

        foreach (var entry in lessonEntries)
        {
            var lesson = entry.Entity;

            if (lesson.HskLevelId <= 0)
            {
                throw new InvalidOperationException(
                    "Lesson phải có HSK Level hợp lệ.");
            }

            if (lesson.CourseChapterId.HasValue)
            {
                if (!chapters.TryGetValue(lesson.CourseChapterId.Value, out var chapter))
                {
                    throw new InvalidOperationException(
                        "Chapter được gán cho Lesson không tồn tại.");
                }

                if (chapter.IsDeleted || !chapter.IsActive)
                {
                    throw new InvalidOperationException(
                        "Không thể gán Lesson vào Chapter đã xóa hoặc đang tạm ngưng.");
                }

                if (chapter.Course.IsDeleted)
                {
                    throw new InvalidOperationException(
                        "Không thể gán Lesson vào Course đã bị xóa.");
                }

                if (chapter.Course.HskLevelId.HasValue &&
                    chapter.Course.HskLevelId.Value != lesson.HskLevelId)
                {
                    throw new InvalidOperationException(
                        $"HSK của Lesson phải trùng với HSK của Course. Course dùng HSK #{chapter.Course.HskLevelId.Value}, Lesson đang dùng HSK #{lesson.HskLevelId}.");
                }
            }

            if (lesson.TopicId.HasValue &&
                lesson.Status is ContentStatus.Review or ContentStatus.Approved or ContentStatus.Published &&
                !publishedTopicIds.Contains(lesson.TopicId.Value))
            {
                throw new InvalidOperationException(
                    "Chủ đề của Lesson phải ở trạng thái Published trước khi Lesson được gửi duyệt hoặc xuất bản.");
            }
        }
    }

    private void AddCourseAuditEntries()
    {
        var entries = ChangeTracker
            .Entries<CourseEntity>()
            .Where(entry => entry.State == EntityState.Modified && entry.Entity.Id > 0)
            .ToArray();

        foreach (var entry in entries)
        {
            var changed = entry.Properties
                .Where(property => property.IsModified)
                .ToArray();

            if (changed.Length == 0)
            {
                continue;
            }

            var oldValues = changed.ToDictionary(
                property => property.Metadata.Name,
                property => property.OriginalValue);
            var newValues = changed.ToDictionary(
                property => property.Metadata.Name,
                property => property.CurrentValue);
            var changedProperties = changed
                .Select(property => property.Metadata.Name)
                .OrderBy(name => name)
                .ToArray();

            var action = ResolveCourseAuditAction(entry.Entity, changedProperties);

            AuditLogs.Add(new AuditLog(
                entry.Entity.UpdatedById,
                action,
                "Course",
                entry.Entity.Id.ToString(),
                entry.Entity.PublicId.ToString(),
                JsonSerializer.Serialize(oldValues),
                JsonSerializer.Serialize(newValues),
                JsonSerializer.Serialize(changedProperties)));
        }
    }

    private static string ResolveCourseAuditAction(
        CourseEntity course,
        IReadOnlyCollection<string> changedProperties)
    {
        if (changedProperties.Contains(nameof(CourseEntity.DeletedAt)))
        {
            return course.DeletedAt.HasValue ? "deleted" : "restored";
        }

        if (changedProperties.Contains(nameof(CourseEntity.Status)))
        {
            return course.Status switch
            {
                ContentStatus.Review => "submitted-review",
                ContentStatus.Approved => "approved",
                ContentStatus.Published => "published",
                ContentStatus.Archived => "archived",
                _ => "updated"
            };
        }

        return "updated";
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Map DateTimeOffset -> timestamptz in PostgreSQL
        configurationBuilder.Properties<DateTimeOffset>().HaveColumnType("timestamptz");
        configurationBuilder.Properties<DateTimeOffset?>().HaveColumnType("timestamptz");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Scan và apply toàn bộ IEntityTypeConfiguration trong project Infrastructure
        builder.ApplyHanYuConfigurations();

        // Cập nhật tên các bảng Identity mặc định thành naming convention của hệ thống (snake_case)
        builder.ConfigureIdentityTableNames();
    }
}
