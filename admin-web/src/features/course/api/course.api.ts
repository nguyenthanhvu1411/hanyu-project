import { apiClient } from "@/lib/api/api-client";
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

const BASE = "/api/v1/admin/courses";

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
    return apiClient<PagedResult<AdminCourseListItem>>(`${BASE}?${buildQuery(query)}`);
  },

  getById(id: number) {
    return apiClient<AdminCourseDetail>(`${BASE}/${id}`);
  },

  create(request: CreateCourseRequest) {
    return apiClient<AdminCourseDetail>(BASE, { method: "POST", body: request });
  },

  update(id: number, request: UpdateCourseRequest) {
    return apiClient<AdminCourseDetail>(`${BASE}/${id}`, { method: "PUT", body: request });
  },

  validate(id: number) {
    return apiClient<CourseValidationResult>(`${BASE}/${id}/validate`, { method: "POST" });
  },

  reorderChapters(id: number, body: ReorderChaptersRequest) {
    return apiClient<void>(`${BASE}/${id}/chapters/order`, { method: "PUT", body });
  },

  submitReview(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(`${BASE}/${id}/submit-review`, { method: "POST", body: request });
  },

  approve(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(`${BASE}/${id}/approve`, { method: "POST", body: request });
  },

  reject(id: number, request: RejectCourseRequest) {
    return apiClient<AdminCourseDetail>(`${BASE}/${id}/reject`, { method: "POST", body: request });
  },

  publish(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(`${BASE}/${id}/publish`, { method: "POST", body: request });
  },

  archive(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(`${BASE}/${id}/archive`, { method: "POST", body: request });
  },

  restore(id: number, request: CourseWorkflowRequest) {
    return apiClient<AdminCourseDetail>(`${BASE}/${id}/restore`, { method: "POST", body: request });
  },

  delete(id: number, request: CourseWorkflowRequest) {
    return apiClient<void>(`${BASE}/${id}`, { method: "DELETE", body: request });
  },
};
