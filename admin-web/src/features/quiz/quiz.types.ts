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

export enum QuizQuestionType {
  MeaningChoice = 0,
  PinyinChoice = 1,
  HanziChoice = 2,
  FillBlank = 3,
  Matching = 4,
  TrueFalse = 5,
  SentenceOrder = 6,
  MultipleChoice = 7,
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

export interface AdminQuizQuestion {
  id: number;
  publicId: string;
  quizId: number;
  vocabularyId: number | null;
  vocabularyPublicId: string | null;
  questionType: QuizQuestionType;
  prompt: string;
  promptPinyin: string | null;
  correctAnswerText: string | null;
  explanationVi: string | null;
  hintVi: string | null;
  points: number;
  sortOrder: number;
  timeLimitSeconds: number | null;
  isRequired: boolean;
  status: ContentStatus;
  createdAt: string;
  updatedAt: string;
}

export interface QuizQuestionRequest {
  questionType: QuizQuestionType;
  prompt: string;
  promptPinyin?: string | null;
  correctAnswerText?: string | null;
  explanationVi?: string | null;
  hintVi?: string | null;
  points: number;
  sortOrder: number;
  timeLimitSeconds?: number | null;
  isRequired: boolean;
  vocabularyId?: number | null;
}

export interface AdminQuestionBank {
  id: number;
  publicId: string;
  code: string;
  nameVi: string;
  descriptionVi: string | null;
  hskLevelId: number | null;
  isActive: boolean;
  questionCount: number;
}

export interface QuestionBankRequest {
  code: string;
  nameVi: string;
  descriptionVi?: string | null;
  hskLevelId?: number | null;
}

export const QUIZ_TYPE_LABELS: Record<QuizType, string> = {
  [QuizType.Lesson]: "Theo bài giảng",
  [QuizType.Vocabulary]: "Từ vựng",
  [QuizType.Review]: "Ôn tập",
  [QuizType.Placement]: "Xếp lớp",
  [QuizType.Custom]: "Tùy chỉnh",
};

export const QUIZ_QUESTION_TYPE_LABELS: Record<QuizQuestionType, string> = {
  [QuizQuestionType.MeaningChoice]: "Chọn nghĩa",
  [QuizQuestionType.PinyinChoice]: "Chọn Pinyin",
  [QuizQuestionType.HanziChoice]: "Chọn Hán tự",
  [QuizQuestionType.FillBlank]: "Điền vào chỗ trống",
  [QuizQuestionType.Matching]: "Ghép cặp",
  [QuizQuestionType.TrueFalse]: "Đúng / Sai",
  [QuizQuestionType.SentenceOrder]: "Sắp xếp câu",
  [QuizQuestionType.MultipleChoice]: "Trắc nghiệm",
};

export const QUIZ_STATUS_LABELS: Record<ContentStatus, string> = {
  [ContentStatus.Draft]: "Bản nháp",
  [ContentStatus.Review]: "Chờ duyệt",
  [ContentStatus.Approved]: "Đã duyệt",
  [ContentStatus.Published]: "Đã xuất bản",
  [ContentStatus.Archived]: "Đã lưu trữ",
};
