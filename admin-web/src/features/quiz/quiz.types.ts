export enum ContentStatus {
  Draft = 0,
  Review = 1,
  Approved = 2,
  Published = 3,
  Archived = 4,
}

export enum QuizType {
  Lesson = 0,
  Vocabulary = 1,
  Review = 2,
  Placement = 3,
  Custom = 4,
}

export enum QuizShuffleMode {
  None = 0,
  QuestionsOnly = 1,
  OptionsOnly = 2,
  QuestionsAndOptions = 3,
}

export enum QuizFeedbackMode {
  AfterEachAnswer = 0,
  AfterSubmit = 1,
  None = 2,
}

export interface AdminQuiz {
  id: number;
  publicId: string;
  lessonId: number | null;
  lessonPublicId: string | null;
  lessonTitleVi: string | null;
  titleVi: string;
  descriptionVi: string | null;
  quizType: QuizType;
  passingScore: number;
  timeLimitSeconds: number | null;
  maxAttempts: number;
  shuffleMode: QuizShuffleMode;
  feedbackMode: QuizFeedbackMode;
  allowRetry: boolean;
  showCorrectAnswer: boolean;
  showExplanation: boolean;
  status: ContentStatus;
  version: number;
  publishedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface AdminQuizQuery {
  q?: string;
  lessonId?: number;
  quizType?: QuizType;
  status?: ContentStatus;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateQuizRequest {
  titleVi: string;
  descriptionVi?: string | null;
  quizType: QuizType;
  passingScore: number;
  timeLimitSeconds?: number | null;
  maxAttempts: number;
  lessonId?: number | null;
  shuffleMode: QuizShuffleMode;
  feedbackMode: QuizFeedbackMode;
  allowRetry: boolean;
  showCorrectAnswer: boolean;
  showExplanation: boolean;
}

export interface UpdateQuizRequest extends CreateQuizRequest {
  version: number;
}

export const QUIZ_TYPE_LABELS: Record<QuizType, string> = {
  [QuizType.Lesson]: "Theo bài giảng",
  [QuizType.Vocabulary]: "Từ vựng",
  [QuizType.Review]: "Ôn tập",
  [QuizType.Placement]: "Xếp lớp",
  [QuizType.Custom]: "Tùy chỉnh",
};

export const QUIZ_STATUS_LABELS: Record<ContentStatus, string> = {
  [ContentStatus.Draft]: "Bản nháp",
  [ContentStatus.Review]: "Chờ duyệt",
  [ContentStatus.Approved]: "Đã duyệt",
  [ContentStatus.Published]: "Đã xuất bản",
  [ContentStatus.Archived]: "Đã lưu trữ",
};
