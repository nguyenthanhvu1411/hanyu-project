import { apiClient } from "@/lib/api/api-client";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AdminQuestionBank,
  AdminQuiz,
  AdminQuizQuery,
  AdminQuizQuestion,
  CreateQuizRequest,
  QuestionBankRequest,
  QuizQuestionRequest,
  UpdateQuizRequest,
} from "./quiz.types";

const ROOT = "/admin/quizzes";
const QUESTION_BANK_ROOT = "/admin/question-banks";

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

  getById(id: number) {
    return apiClient<AdminQuiz>(`${ROOT}/${id}`);
  },

  create(request: CreateQuizRequest) {
    return apiClient<AdminQuiz>(ROOT, { method: "POST", body: request });
  },

  update(id: number, request: UpdateQuizRequest) {
    return apiClient<AdminQuiz>(`${ROOT}/${id}`, { method: "PUT", body: request });
  },

  submitReview(id: number) {
    return apiClient<void>(`${ROOT}/${id}/submit-review`, { method: "POST" });
  },

  approve(id: number) {
    return apiClient<void>(`${ROOT}/${id}/approve`, { method: "POST" });
  },

  publish(id: number) {
    return apiClient<void>(`${ROOT}/${id}/publish`, { method: "POST" });
  },

  archive(id: number) {
    return apiClient<void>(`${ROOT}/${id}/archive`, { method: "POST" });
  },

  restore(id: number) {
    return apiClient<void>(`${ROOT}/${id}/restore`, { method: "POST" });
  },

  delete(id: number) {
    return apiClient<void>(`${ROOT}/${id}`, { method: "DELETE" });
  },

  listQuestions(quizId: number) {
    return apiClient<AdminQuizQuestion[]>(`${ROOT}/${quizId}/questions`);
  },

  createQuestion(quizId: number, request: QuizQuestionRequest) {
    return apiClient<AdminQuizQuestion>(`${ROOT}/${quizId}/questions`, { method: "POST", body: request });
  },

  updateQuestion(quizId: number, questionId: number, request: QuizQuestionRequest) {
    return apiClient<AdminQuizQuestion>(`${ROOT}/${quizId}/questions/${questionId}`, { method: "PUT", body: request });
  },

  deleteQuestion(quizId: number, questionId: number) {
    return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}`, { method: "DELETE" });
  },

  submitQuestionReview(quizId: number, questionId: number) {
    return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/submit-review`, { method: "POST" });
  },

  approveQuestion(quizId: number, questionId: number) {
    return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/approve`, { method: "POST" });
  },

  publishQuestion(quizId: number, questionId: number) {
    return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/publish`, { method: "POST" });
  },

  archiveQuestion(quizId: number, questionId: number) {
    return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/archive`, { method: "POST" });
  },

  restoreQuestion(quizId: number, questionId: number) {
    return apiClient<void>(`${ROOT}/${quizId}/questions/${questionId}/restore`, { method: "POST" });
  },

  listQuestionBanks() {
    return apiClient<AdminQuestionBank[]>(QUESTION_BANK_ROOT);
  },

  createQuestionBank(request: QuestionBankRequest) {
    return apiClient<AdminQuestionBank>(QUESTION_BANK_ROOT, { method: "POST", body: request });
  },

  updateQuestionBank(id: number, request: QuestionBankRequest) {
    return apiClient<AdminQuestionBank>(`${QUESTION_BANK_ROOT}/${id}`, { method: "PUT", body: request });
  },

  activateQuestionBank(id: number) {
    return apiClient<void>(`${QUESTION_BANK_ROOT}/${id}/activate`, { method: "POST" });
  },

  deactivateQuestionBank(id: number) {
    return apiClient<void>(`${QUESTION_BANK_ROOT}/${id}/deactivate`, { method: "POST" });
  },

  addQuestionToBank(id: number, questionId: number) {
    return apiClient<void>(`${QUESTION_BANK_ROOT}/${id}/questions`, { method: "POST", body: { questionId } });
  },

  removeQuestionFromBank(id: number, questionId: number) {
    return apiClient<void>(`${QUESTION_BANK_ROOT}/${id}/questions/${questionId}`, { method: "DELETE" });
  },
};
