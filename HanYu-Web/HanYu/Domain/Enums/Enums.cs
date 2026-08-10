namespace HanYu.Domain.Enums;

public enum VocabularyRelationType { Related = 0, Confusable = 1, Synonym = 2, Antonym = 3 }
public enum AudioAssetKind { Vocabulary = 0, ExampleSentence = 1, Lesson = 2 }
public enum ContentStatus { Draft = 0, Review = 1, Approved = 2, Published = 3, Archived = 4 }

public enum UserSessionStatus { Active = 0, Revoked = 1, Expired = 2 }
public enum UserSecurityEventType
{
    LoginSucceeded = 0, LoginFailed = 1,
    EmailVerified = 10, EmailChanged = 11,
    PasswordChanged = 20, PasswordReset = 21,
    SessionCreated = 30, SessionRevoked = 31, AllSessionsRevoked = 32, RefreshTokenReuseDetected = 33,
    AccountLocked = 40, AccountUnlocked = 41, AccountDeleted = 42, AccountRestored = 43,
    TwoFactorEnabled = 50, TwoFactorDisabled = 51
}
public enum UserConsentType { TermsOfService = 0, PrivacyPolicy = 1, ProductAnalytics = 2, AiProcessing = 3 }
public enum DataExportStatus { Pending = 0, Processing = 1, Completed = 2, Failed = 3, Expired = 4 }

public enum LessonProgressStatus { NotStarted = 0, InProgress = 1, Completed = 2 }
public enum LessonSectionType { Introduction = 0, Vocabulary = 1, Explanation = 2, Example = 3, Grammar = 4, Note = 5, Practice = 6, Summary = 7 }
public enum LessonAssetType { Image = 0, Audio = 1, Document = 2 }

public enum LearningState { NotStarted = 0, Learning = 1, Known = 2, Mastered = 3 }
public enum ReviewRating { Again = 0, Hard = 1, Good = 2, Easy = 3 }
public enum FlashcardMode { HanziToMeaning = 0, MeaningToHanzi = 1, PinyinToHanzi = 2 }
public enum FlashcardSourceType { Lesson = 0, Topic = 1, ReviewQueue = 2, Custom = 3 }
public enum FlashcardSessionStatus { Active = 0, Completed = 1, Abandoned = 2 }

public enum QuizType { Lesson = 0, Vocabulary = 1, Review = 2, Placement = 3, Custom = 4 }
public enum QuizQuestionType { MeaningChoice = 0, PinyinChoice = 1, HanziChoice = 2, FillBlank = 3, Matching = 4, TrueFalse = 5, SentenceOrder = 6, MultipleChoice = 7 }
public enum QuizAttemptStatus { InProgress = 0, Submitted = 1, Expired = 2, Abandoned = 3 }
public enum QuizShuffleMode { None = 0, QuestionsOnly = 1, OptionsOnly = 2, QuestionsAndOptions = 3 }
public enum QuizFeedbackMode { AfterEachAnswer = 0, AfterSubmit = 1, None = 2 }

public enum AiFeatureType { VocabularyExplanation = 0, QuizExplanation = 1, GrammarExplanation = 2, SentenceExplanation = 3, AiTutor = 4, ExampleGeneration = 5, ContentAssist = 6 }
public enum AiRequestStatus { Pending = 0, Completed = 1, Failed = 2, Cancelled = 3 }
public enum AiFeedbackRating { Negative = -1, Neutral = 0, Positive = 1 }
public enum AiConversationStatus { Active = 0, Archived = 1 }
public enum AiMessageRole { System = 0, User = 1, Assistant = 2 }

public enum NotificationType { General = 0, LearningReminder = 10, ReviewDue = 11, StreakWarning = 12, StreakAchievement = 13, LessonCompleted = 20, QuizResult = 21, AccountSecurity = 30, System = 40 }
public enum NotificationChannel { InApp = 0, Email = 1, Push = 2 }
public enum NotificationDeliveryStatus { Pending = 0, Processing = 1, Sent = 2, Delivered = 3, Failed = 4, Cancelled = 5 }

public enum ContentEntityType
{
    Vocabulary = 0,
    VocabularyExample = 1,
    
    Lesson = 2,
    LessonSection = 3,
    
    QuizQuestion = 4,
    AudioAsset = 5,
    
    Course = 6,
    CourseChapter = 7
}
public enum ContentReportReason { IncorrectContent = 0, IncorrectTranslation = 1, IncorrectPinyin = 2, AudioProblem = 3, Typo = 4, OutdatedContent = 5, Other = 99 }
public enum ContentReportStatus { Open = 0, InReview = 1, Resolved = 2, Rejected = 3 }
public enum ContentImportType { Vocabulary = 0, VocabularyExample = 1, Lesson = 2, Quiz = 3 }
public enum ContentImportStatus { Pending = 0, Processing = 1, Completed = 2, CompletedWithErrors = 3, Failed = 4 }

public enum LearningGoalStatus { Active = 0, Completed = 1, Paused = 2, Cancelled = 3 }
public enum LearningActivityType { LessonStarted = 0, LessonCompleted = 1, VocabularyLearned = 10, VocabularyReviewed = 11, FlashcardStarted = 20, FlashcardCompleted = 21, QuizStarted = 30, QuizCompleted = 31, AiTutor = 40, Other = 99 }
public enum CourseStatus{ Draft = 0, Published = 1, Archived = 2 }