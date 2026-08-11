import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import type { PagedResult } from "@/lib/api/api-result";

import type {
  AdminCourseDetail,
  AdminCourseListItem,
  AdminCourseQuery,
  CourseWorkflowRequest,
  CreateCourseRequest,
  RejectCourseRequest,
  UpdateCourseRequest,
} from "../types/course.types";
import type {
  CourseValidationResult,
  ReorderChaptersRequest,
} from "../types/curriculum.types";

function buildQuery(query: AdminCourseQuery): string {
  const params = new URLSearchParams();
  if (query.search) params.set("search", query.search);
  if (query.hskLevelId !== undefined) params.set("hskLevelId", String(query.hskLevelId));
  if (query.status !== undefined) params.set("status", String(query.status));
  if (query.isActive !== undefined) params.set("isActive", String(query.isActive));
  if (query.isFeatured !== undefined) params.set("isFeatured", String(query.isFeatured));
  if (query.includeDeleted !== undefined) params.set("includeDeleted", String(query.includeDeleted));
  if (query.sortBy) params.set("sortBy", query.sortBy);
  if (query.sortDescending !== undefined) params.set("sortDescending", String(query.sortDescending));
  params.set("page", String(query.page ?? 1));
  params.set("pageSize", String(query.pageSize ?? 20));
  return params.toString();
}

export const courseApi = {
  list(query: AdminCourseQuery = {}) {
    return apiClient<PagedResult<AdminCourseListItem>>(
      `${API_ENDPOINTS.COURSE.ROOT}?${buildQuery(query)}`,
    );
  },
  getById(id: number) {
    return apiClient<AdminCourseDetail>(API_ENDPOINTS.COURSE.DETAIL(id));
  },
  create(request: CreateCourseRequest) {
    return apiClient<AdminCourseDetail>(API_ENDPOINTS.COURSE.ROOT, { method: "POST", body: request });
  },
  update(id: number, request: UpdateCourseRequest) {
    return apiClient<AdminCourseDetail>(API_ENDPOINTS.COURSE.DETAIL(id), { method: "PUT", body: request });
  },
  validate(id: number) {
    return apiClient<CourseValidationResult>(API_ENDPOINTS.COURSE.VALIDATE(id), { method: "POST" });
  },
  reorderChapters(id: number, body: ReorderChaptersRequest) {
    return apiClient<void>(API_ENDPOINTS.COURSE.CHAPTER_REORDER(id), { method: "PUT", body });
  },
  submitReview(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(API_ENDPOINTS.COURSE.SUBMIT_REVIEW(id), { method: "POST", body: request });
  },
  approve(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(API_ENDPOINTS.COURSE.APPROVE(id), { method: "POST", body: request });
  },
  reject(id: number, request: RejectCourseRequest) {
    return apiClient<AdminCourseDetail>(API_ENDPOINTS.COURSE.REJECT(id), { method: "POST", body: request });
  },
  publish(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(API_ENDPOINTS.COURSE.PUBLISH(id), { method: "POST", body: request });
  },
  archive(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(API_ENDPOINTS.COURSE.ARCHIVE(id), { method: "POST", body: request });
  },
  restore(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(API_ENDPOINTS.COURSE.RESTORE(id), { method: "POST", body: request });
  },
  delete(id: number, request: CourseWorkflowRequest) {
    return apiClient<void>(API_ENDPOINTS.COURSE.DETAIL(id), { method: "DELETE", body: request });
  },
  restoreDeleted(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(API_ENDPOINTS.COURSE.RESTORE_DELETED(id), {
      method: "POST",
      body: request,
    });
  },
};
