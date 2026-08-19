export interface AdminReviewDashboard {
  totalVocabularyStates: number;
  dueReviews: number;
  overdueReviews: number;
  learningVocabulary: number;
  knownVocabulary: number;
  masteredVocabulary: number;
  favoriteVocabulary: number;
  reviewsToday: number;
  correctReviewsToday: number;
  wrongReviewsToday: number;
  accuracyToday: number;
  activeFlashcardSessions: number;
  completedFlashcardSessionsToday: number;
  abandonedFlashcardSessionsToday: number;
}

export interface AdminVocabularyState {
  userId: string;
  vocabularyId: number;
  vocabularyPublicId: string;
  simplified: string;
  traditional: string | null;
  pinyin: string;
  primaryMeaningVi: string;
  hskLevelId: number;
  learningState: number;
  isFavorite: boolean;
  masteryScore: number;
  correctCount: number;
  wrongCount: number;
  consecutiveCorrect: number;
  distinctCorrectDays: number;
  lastCorrectAt: string | null;
  lastReviewedAt: string | null;
  nextReviewAt: string | null;
  currentIntervalMinutes: number | null;
  firstLearnedAt: string | null;
  masteredAt: string | null;
  updatedAt: string;
}

export interface AdminVocabularyStateQuery {
  userId?: string;
  vocabularyId?: number;
  q?: string;
  hskLevelId?: number;
  topicId?: number;
  learningState?: number;
  isFavorite?: boolean;
  isDue?: boolean;
  isOverdue?: boolean;
  minMastery?: number;
  maxMastery?: number;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminFlashcardSession {
  id: number;
  publicId: string;
  userId: string;
  mode: number;
  sourceType: number;
  sourceId: number | null;
  status: number;
  currentIndex: number;
  totalItems: number;
  correctItems: number;
  wrongItems: number;
  accuracyPercent: number;
  startedAt: string;
  completedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface AdminFlashcardSessionItem {
  id: number;
  publicId: string;
  vocabularyId: number;
  vocabularyPublicId: string;
  simplified: string;
  traditional: string | null;
  pinyin: string;
  primaryMeaningVi: string;
  sortOrder: number;
  isAnswered: boolean;
  rating: number | null;
  wasCorrect: boolean | null;
  responseTimeMs: number | null;
  answeredAt: string | null;
}

export interface AdminFlashcardSessionDetail extends Omit<AdminFlashcardSession, "createdAt" | "updatedAt"> {
  items: AdminFlashcardSessionItem[];
}

export interface AdminFlashcardSessionQuery {
  userId?: string;
  mode?: number;
  sourceType?: number;
  status?: number;
  sourceId?: number;
  from?: string;
  to?: string;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminReviewEvent {
  id: number;
  publicId: string;
  userId: string;
  vocabularyId: number;
  vocabularyPublicId: string;
  simplified: string;
  pinyin: string;
  primaryMeaningVi: string;
  flashcardSessionId: number | null;
  flashcardSessionPublicId: string | null;
  rating: number;
  wasCorrect: boolean;
  responseTimeMs: number | null;
  masteryBefore: number;
  masteryAfter: number;
  intervalBeforeMinutes: number | null;
  intervalAfterMinutes: number;
  reviewedAt: string;
}

export interface AdminReviewEventQuery {
  userId?: string;
  vocabularyId?: number;
  flashcardSessionId?: number;
  rating?: number;
  wasCorrect?: boolean;
  from?: string;
  to?: string;
  minMasteryAfter?: number;
  maxMasteryAfter?: number;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminUserReviewSummary {
  userId: string;
  totalVocabulary: number;
  learningVocabulary: number;
  knownVocabulary: number;
  masteredVocabulary: number;
  dueVocabulary: number;
  overdueVocabulary: number;
  favoriteVocabulary: number;
  totalReviews: number;
  correctReviews: number;
  wrongReviews: number;
  overallAccuracy: number;
  lastReviewedAt: string | null;
  activeFlashcardSessions: number;
}
