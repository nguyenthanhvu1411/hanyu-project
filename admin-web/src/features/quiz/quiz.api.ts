import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AdminQuestionBank,
  AdminQuiz,
  AdminQuizMatchingPair,
  AdminQuizQuery,
  AdminQuizQuestion,
  AdminQuizQuestionOption,
  AdminQuizTag,
  CreateQuizRequest,
  QuestionBankRequest,
  QuizMatchingPairRequest,
  QuizQuestionOptionRequest,
  QuizQuestionRequest,
  QuizTagRequest,
  UpdateQuizRequest,
} from "./quiz.types";

const ROOT = "/admin/quizzes";
const QUESTION_BANK_ROOT = "/admin/question-banks";
const TAG_ROOT = "/admin/quiz-tags";

function buildQuery(query: AdminQuizQuery) {
  const params = new URLSearchParams();
  Object.entries(query)
    .filter(([, value]) => value !== undefined && value !== null && value !== "")
    .forEach(([key, value]) => params.set(key, String(value)));
  return params.toString();
}

export const quizApi = {
  list(query: AdminQuizQuery = {}) {
    const queryString = buildQuery(query);
    return apiClient<PagedResult<AdminQuiz>>(queryString ? `${ROOT}?${queryString}` : ROOT);
  },
  getById(id: number) { return apiClient<AdminQuiz>(`${ROOT}/${id}`); },
  create(request: CreateQuizRequest) { return apiClient<AdminQuiz>(ROOT, { method: "POST", body: request }); },
  update(id: number, request: UpdateQuizRequest) { return apiClient<AdminQuiz>(`${ROOT}/${id}`, { method: "PUT", body: request }); },
  submitReview(id: number) { return apiClient<void>(`${ROOT}/${id}/submit-review`, { method: "POST" }); },
  approve(id: number) { return apiClient<void>(`${ROOT}/${id}/approve`, { method: "POST" }); },
  publish(id: number) { return apiClient<void>(`${ROOT}/${id}/publish`, { method: "POST" }); },
  archive(id: number) { return apiClient<void>(`${ROOT}/${id}/archive`, { method: "POST" }); },
  restore(id: number) { return apiClient<void>(`${ROOT}/${id}/restore`, { method: "POST" }); },
  delete(id: number) { return apiClient<void>(`${ROOT}/${id}`, { method: "DELETE" }); },

  listQuestions(quizId: number) { return apiClient<AdminQuizQuestion[]>(`${ROOT}/${quizId}/questions`); },
  createQuestion(quizId: number, request: QuizQuestionRequest) { return apiClient<AdminQuizQuestion>(`${ROOT}/${quizId}/questions`, { method: "POST", body: request }); },
  updateQuestion(quizId: number, questionId: number, request: QuizQuestionRequest) { return apiClient<AdminQuizQuestion>(`${ROOT}/${quizId}/questions/${questionId}`, { method: "PUT", body: request }); },
  deleteQuestion(quizId: number, questionId: number) { return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}`, { method: "DELETE" }); },
  submitQuestionReview(quizId: number, questionId: number) { return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/submit-review`, { method: "POST" }); },
  approveQuestion(quizId: number, questionId: number) { return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/approve`, { method: "POST" }); },
  publishQuestion(quizId: number, questionId: number) { return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/publish`, { method: "POST" }); },
  archiveQuestion(quizId: number, questionId: number) { return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/archive`, { method: "POST" }); },
  restoreQuestion(quizId: number, questionId: number) { return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/restore`, { method: "POST" }); },

  listQuestionOptions(quizId: number, questionId: number) { return apiClient<AdminQuizQuestionOption[]>(`${ROOT}/${quizId}/questions/${questionId}/options`); },
  createQuestionOption(quizId: number, questionId: number, request: QuizQuestionOptionRequest) { return apiClient<AdminQuizQuestionOption>(`${ROOT}/${quizId}/questions/${questionId}/options`, { method: "POST", body: request }); },
  updateQuestionOption(quizId: number, questionId: number, optionId: number, request: QuizQuestionOptionRequest) { return apiClient<AdminQuizQuestionOption>(`${ROOT}/${quizId}/questions/${questionId}/options/${optionId}`, { method: "PUT", body: request }); },
  deleteQuestionOption(quizId: number, questionId: number, optionId: number) { return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/options/${optionId}`, { method: "DELETE" }); },

  listMatchingPairs(quizId: number, questionId: number) { return apiClient<AdminQuizMatchingPair[]>(`${ROOT}/${quizId}/questions/${questionId}/matching-pairs`); },
  createMatchingPair(quizId: number, questionId: number, request: QuizMatchingPairRequest) { return apiClient<AdminQuizMatchingPair>(`${ROOT}/${quizId}/questions/${questionId}/matching-pairs`, { method: "POST", body: request }); },
  updateMatchingPair(quizId: number, questionId: number, pairId: number, request: QuizMatchingPairRequest) { return apiClient<AdminQuizMatchingPair>(`${ROOT}/${quizId}/questions/${questionId}/matching-pairs/${pairId}`, { method: "PUT", body: request }); },
  deleteMatchingPair(quizId: number, questionId: number, pairId: number) { return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/matching-pairs/${pairId}`, { method: "DELETE" }); },

  listTags() { return apiClient<AdminQuizTag[]>(TAG_ROOT); },
  createTag(request: QuizTagRequest) { return apiClient<AdminQuizTag>(TAG_ROOT, { method: "POST", body: request }); },
  updateTag(id: number, request: QuizTagRequest) { return apiClient<AdminQuizTag>(`${TAG_ROOT}/${id}`, { method: "PUT", body: request }); },
  activateTag(id: number) { return apiClient<void>(`${TAG_ROOT}/${id}/activate`, { method: "POST" }); },
  deactivateTag(id: number) { return apiClient<void>(`${TAG_ROOT}/${id}/deactivate`, { method: "POST" }); },
  deleteTag(id: number) { return apiClient<void>(`${TAG_ROOT}/${id}`, { method: "DELETE" }); },
  listQuestionTags(quizId: number, questionId: number) { return apiClient<AdminQuizTag[]>(`${ROOT}/${quizId}/questions/${questionId}/tags`); },
  attachTag(quizId: number, questionId: number, tagId: number) { return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/tags/${tagId}`, { method: "POST" }); },
  detachTag(quizId: number, questionId: number, tagId: number) { return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/tags/${tagId}`, { method: "DELETE" }); },

  listQuestionBanks() { return apiClient<AdminQuestionBank[]>(QUESTION_BANK_ROOT); },
  createQuestionBank(request: QuestionBankRequest) { return apiClient<AdminQuestionBank>(QUESTION_BANK_ROOT, { method: "POST", body: request }); },
  updateQuestionBank(id: number, request: QuestionBankRequest) { return apiClient<AdminQuestionBank>(`${QUESTION_BANK_ROOT}/${id}`, { method: "PUT", body: request }); },
  activateQuestionBank(id: number) { return apiClient<void>(`${QUESTION_BANK_ROOT}/${id}/activate`, { method: "POST" }); },
  deactivateQuestionBank(id: number) { return apiClient<void>(`${QUESTION_BANK_ROOT}/${id}/deactivate`, { method: "POST" }); },
  listQuestionBankQuestions(id: number) { return apiClient<AdminQuizQuestion[]>(`${QUESTION_BANK_ROOT}/${id}/questions`); },
  addQuestionToBank(id: number, questionId: number) { return apiClient<void>(`${QUESTION_BANK_ROOT}/${id}/questions`, { method: "POST", body: { questionId } }); },
  removeQuestionFromBank(id: number, questionId: number) { return apiClient<void>(`${QUESTION_BANK_ROOT}/${id}/questions/${questionId}`, { method: "DELETE" }); },
};
