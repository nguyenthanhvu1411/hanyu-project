export interface AdminAiDashboard {
  requestsToday: number;
  completedToday: number;
  failedToday: number;
  cancelledToday: number;
  inputTokensToday: number;
  outputTokensToday: number;
  totalTokensToday: number;
  estimatedCostUsdToday: number;
  averageLatencyMs: number;
}

export interface AdminAiRequest {
  id: number;
  publicId: string;
  userId: string | null;
  conversationId: number | null;
  vocabularyId: number | null;
  lessonId: number | null;
  quizAttemptAnswerId: number | null;
  featureType: number;
  provider: string;
  model: string;
  requestHash: string | null;
  promptVersion: string | null;
  inputTokens: number;
  outputTokens: number;
  totalTokens: number;
  estimatedCostUsd: number | null;
  latencyMs: number | null;
  status: number;
  errorCode: string | null;
  errorMessage: string | null;
  requestedAt: string;
  completedAt: string | null;
}

export interface AdminAiConversation {
  id: number;
  publicId: string;
  userId: string;
  title: string | null;
  status: number;
  messageCount: number;
  lastMessageAt: string;
  createdAt: string;
  updatedAt: string;
}

export interface AdminAiFeedback {
  id: number;
  publicId: string;
  userId: string;
  aiRequestId: number;
  rating: number;
  comment: string | null;
  issueType: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface AdminAiCacheEntry {
  id: number;
  publicId: string;
  featureType: number;
  cacheKey: string;
  model: string;
  promptVersion: string;
  hitCount: number;
  lastAccessedAt: string | null;
  expiresAt: string | null;
  isExpired: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface AdminAnalyticsDashboard {
  activeUsersToday: number;
  learningSecondsToday: number;
  lessonsCompletedToday: number;
  vocabularyReviewedToday: number;
  quizAttemptsToday: number;
  quizPassedToday: number;
  aiInteractionsToday: number;
  xpEarnedToday: number;
}

export interface AdminDailyLearningStat {
  userId: string;
  date: string;
  learningSeconds: number;
  lessonsStarted: number;
  lessonsCompleted: number;
  vocabularyReviewed: number;
  vocabularyLearned: number;
  correctReviews: number;
  wrongReviews: number;
  quizAttempts: number;
  quizPassed: number;
  aiInteractions: number;
  xpEarned: number;
  updatedAt: string;
}

export interface UserAnalyticsSummary {
  totalLearningSeconds: number;
  lessonsCompleted: number;
  vocabularyReviewed: number;
  vocabularyLearned: number;
  quizAttempts: number;
  quizPassed: number;
  aiInteractions: number;
  xpEarned: number;
  reviewAccuracy: number;
  currentStreak: number;
  longestStreak: number;
  totalActiveDays: number;
  lastLearningDate: string | null;
}
