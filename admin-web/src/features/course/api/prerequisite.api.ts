import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";

import type {
  CoursePrerequisite,
  CreatePrerequisiteRequest,
  EntityWorkflowRequest,
  UpdatePrerequisiteRequest,
} from "../types/curriculum.types";

export const prerequisiteApi = {
  list(courseId: number, includeDeleted = false) {
    return apiClient<CoursePrerequisite[]>(
      `${API_ENDPOINTS.COURSE.PREREQUISITES(courseId)}?includeDeleted=${includeDeleted}`,
    );
  },
  create(courseId: number, body: CreatePrerequisiteRequest) {
    return apiClient<CoursePrerequisite>(API_ENDPOINTS.COURSE.PREREQUISITES(courseId), {
      method: "POST",
      body,
    });
  },
  update(courseId: number, prerequisiteId: number, body: UpdatePrerequisiteRequest) {
    return apiClient<CoursePrerequisite>(API_ENDPOINTS.COURSE.PREREQUISITE(courseId, prerequisiteId), {
      method: "PUT",
      body,
    });
  },
  delete(courseId: number, prerequisiteId: number, body: EntityWorkflowRequest) {
    return apiClient<void>(API_ENDPOINTS.COURSE.PREREQUISITE(courseId, prerequisiteId), {
      method: "DELETE",
      body,
    });
  },
  restore(courseId: number, prerequisiteId: number, body: EntityWorkflowRequest) {
    return apiClient<CoursePrerequisite>(API_ENDPOINTS.COURSE.PREREQUISITE_RESTORE(courseId, prerequisiteId), {
      method: "POST",
      body,
    });
  },
};
