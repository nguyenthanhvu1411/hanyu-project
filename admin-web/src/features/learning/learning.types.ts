export type SortDirection = "asc" | "desc";

export interface HskLevelListQuery {
  page?: number;
  pageSize?: number;
  q?: string;
  isActive?: boolean;
  sortBy?: string;
  sortDirection?: SortDirection;
}

export enum LearningGoalStatus {
  Active = 0,
  Completed = 1,
  Paused = 2,
  Cancelled = 3,
}

export enum LearningActivityType {
  LessonStarted = 0,
  LessonCompleted = 1,
  VocabularyLearned = 10,
  VocabularyReviewed = 11,
  FlashcardStarted = 20,
  FlashcardCompleted = 21,
  QuizStarted = 30,
  QuizCompleted = 31,
  AiTutor = 40,
  Other = 99,
}

export interface AdminLearningGoal {
  id: number;
  userId: string;
  targetHskLevel: number;
  targetDate: string | null;
  dailyGoalMinutes: number;
  dailyVocabularyGoal: number | null;
  weeklyLessonGoal: number | null;
  status: LearningGoalStatus;
  startedAt: string;
  completedAt: string | null;
  pausedAt: string | null;
  createdAt: string;
  updatedAt: string;
  userDisplayName?: string | null;
  userEmail?: string | null;
}

export interface AdminLearningGoalQuery {
  userId?: string;
  status?: LearningGoalStatus;
  targetHskLevel?: number;
  page?: number;
  pageSize?: number;
}

export interface CreateLearningGoalRequest {
  userId: string;
  targetHskLevel: number;
  targetDate?: string | null;
  dailyGoalMinutes: number;
  dailyVocabularyGoal?: number | null;
  weeklyLessonGoal?: number | null;
}

export interface UpdateLearningGoalRequest {
  targetHskLevel: number;
  targetDate?: string | null;
  dailyGoalMinutes: number;
  dailyVocabularyGoal?: number | null;
  weeklyLessonGoal?: number | null;
  status: LearningGoalStatus;
}

export interface AdminLearningActivity {
  id: number;
  userId: string;
  activityType: LearningActivityType;
  lessonId: number | null;
  vocabularyId: number | null;
  quizAttemptId: number | null;
  flashcardSessionId: number | null;
  durationSeconds: number;
  xpEarned: number;
  isCompleted: boolean;
  metadataJson: string | null;
  startedAt: string;
  completedAt: string | null;
  userDisplayName?: string | null;
  userEmail?: string | null;
  lessonTitleVi?: string | null;
  vocabularySimplified?: string | null;
  vocabularyPinyin?: string | null;
}

export interface AdminLearningActivityQuery {
  userId?: string;
  activityType?: LearningActivityType;
  isCompleted?: boolean;
  from?: string;
  to?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateLearningActivityRequest {
  userId: string;
  activityType: LearningActivityType;
  lessonId?: number | null;
  vocabularyId?: number | null;
  quizAttemptId?: number | null;
  flashcardSessionId?: number | null;
  durationSeconds: number;
  xpEarned: number;
  isCompleted: boolean;
  metadataJson?: string | null;
}

export type UpdateLearningActivityRequest = Omit<CreateLearningActivityRequest, "userId">;

export interface AdminLearningSummary {
  userId: string;
  totalLearningSeconds: number;
  totalLessonsCompleted: number;
  totalVocabularyLearned: number;
  totalVocabularyMastered: number;
  totalReviews: number;
  totalQuizAttempts: number;
  totalQuizPassed: number;
  totalXp: number;
  currentHskLevel: number;
  overallMasteryPercent: number;
  lastLearningAt: string | null;
  updatedAt: string;
  userDisplayName?: string | null;
  userEmail?: string | null;
}

export interface AdminLearningSummaryQuery {
  userId?: string;
  currentHskLevel?: number;
  page?: number;
  pageSize?: number;
}

export const LEARNING_GOAL_STATUS_LABELS: Record<LearningGoalStatus, string> = {
  [LearningGoalStatus.Active]: "Đang hoạt động",
  [LearningGoalStatus.Completed]: "Hoàn thành",
  [LearningGoalStatus.Paused]: "Tạm dừng",
  [LearningGoalStatus.Cancelled]: "Đã hủy",
};

export const LEARNING_ACTIVITY_TYPE_LABELS: Record<LearningActivityType, string> = {
  [LearningActivityType.LessonStarted]: "Bắt đầu bài giảng",
  [LearningActivityType.LessonCompleted]: "Hoàn thành bài giảng",
  [LearningActivityType.VocabularyLearned]: "Học từ vựng",
  [LearningActivityType.VocabularyReviewed]: "Ôn từ vựng",
  [LearningActivityType.FlashcardStarted]: "Bắt đầu flashcard",
  [LearningActivityType.FlashcardCompleted]: "Hoàn thành flashcard",
  [LearningActivityType.QuizStarted]: "Bắt đầu quiz",
  [LearningActivityType.QuizCompleted]: "Hoàn thành quiz",
  [LearningActivityType.AiTutor]: "AI Tutor",
  [LearningActivityType.Other]: "Khác",
};