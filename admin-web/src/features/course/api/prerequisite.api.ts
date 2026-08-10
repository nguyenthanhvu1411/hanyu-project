import { apiClient } from "@/lib/api/api-client";

import type {
  CoursePrerequisite,
  CreatePrerequisiteRequest,
  EntityWorkflowRequest,
  UpdatePrerequisiteRequest,
} from "../types/curriculum.types";

function base(
  courseId: number,
) {
  return `/api/v1/admin/courses/${courseId}/prerequisites`;
}

export const prerequisiteApi = {
  list(
    courseId: number,
    includeDeleted = false,
  ) {
    return apiClient<
      CoursePrerequisite[]
    >(
      `${base(
        courseId,
      )}?includeDeleted=${includeDeleted}`,
    );
  },

  create(
    courseId: number,
    body: CreatePrerequisiteRequest,
  ) {
    return apiClient<CoursePrerequisite>(
      base(courseId),
      {
        method: "POST",
        body,
      },
    );
  },

  update(
    courseId: number,
    prerequisiteId: number,
    body: UpdatePrerequisiteRequest,
  ) {
    return apiClient<CoursePrerequisite>(
      `${base(
        courseId,
      )}/${prerequisiteId}`,
      {
        method: "PUT",
        body,
      },
    );
  },

  delete(
    courseId: number,
    prerequisiteId: number,
    body: EntityWorkflowRequest,
  ) {
    return apiClient<void>(
      `${base(
        courseId,
      )}/${prerequisiteId}`,
      {
        method: "DELETE",
        body,
      },
    );
  },

  restore(
    courseId: number,
    prerequisiteId: number,
    body: EntityWorkflowRequest,
  ) {
    return apiClient<CoursePrerequisite>(
      `${base(
        courseId,
      )}/${prerequisiteId}/restore`,
      {
        method: "POST",
        body,
      },
    );
  },
};
