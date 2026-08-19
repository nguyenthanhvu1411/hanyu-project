export enum QuizAttemptStatus {
  InProgress = 0,
  Submitted = 1,
  Expired = 2,
  Abandoned = 3,
}

export interface AdminQuizAttemptQuery {
  userId?: string;
  quizId?: number;
  status?: QuizAttemptStatus;
  isPassed?: boolean;
  from?: string;
  to?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminQuizAttempt {
  id: number;
  userId: string;
  userDisplayName: string;
  userEmail: string;
  quizId: number;
  quizTitleVi: string;
  attemptNumber: number;
  status: QuizAttemptStatus;
  score: number | null;
  maxScore: number | null;
  percentage: number | null;
  isPassed: boolean | null;
  correctAnswers: number;
  wrongAnswers: number;
  unansweredQuestions: number;
  startedAt: string;
  submittedAt: string | null;
  expiresAt: string | null;
  durationSeconds: number | null;
}

export interface AdminQuizAttemptAnswer {
  id: number;
  questionPrompt: string;
  questionPinyin: string | null;
  answerText: string | null;
  isCorrect: boolean | null;
  earnedPoints: number | null;
  responseTimeMs: number | null;
  answeredAt: string | null;
}

export interface AdminQuizAttemptDetail {
  attempt: AdminQuizAttempt;
  answers: AdminQuizAttemptAnswer[];
}

export interface AdminQuizAttemptStatistics {
  totalAttempts: number;
  inProgressAttempts: number;
  submittedAttempts: number;
  passedAttempts: number;
  failedAttempts: number;
  averagePercentage: number;
  passRatePercent: number;
  attemptsToday: number;
}

export const QUIZ_ATTEMPT_STATUS_LABELS: Record<QuizAttemptStatus, string> = {
  [QuizAttemptStatus.InProgress]: "Đang làm",
  [QuizAttemptStatus.Submitted]: "Đã nộp",
  [QuizAttemptStatus.Expired]: "Hết hạn",
  [QuizAttemptStatus.Abandoned]: "Đã bỏ",
};