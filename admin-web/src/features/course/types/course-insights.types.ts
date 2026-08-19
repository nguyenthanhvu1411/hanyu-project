export interface CourseHistoryItem {
  id?: number | null;
  action: string;
  label: string;
  userId?: string | null;
  userDisplayName?: string | null;
  oldValuesJson?: string | null;
  newValuesJson?: string | null;
  changedPropertiesJson?: string | null;
  ipAddress?: string | null;
  correlationId?: string | null;
  occurredAt: string;
}

export interface CourseStatistics {
  courseId: number;
  totalChapters: number;
  activeChapters: number;
  totalLessons: number;
  totalStudents: number;
  studentsInProgress: number;
  studentsCompleted: number;
  averageCompletionPercent: number;
  estimatedMinutes?: number | null;
}

export type CourseStudentStatus = "in_progress" | "completed";

export interface CourseStudent {
  userId: string;
  email: string;
  displayName: string;
  startedLessons: number;
  completedLessons: number;
  totalLessons: number;
  completionPercent: number;
  status: CourseStudentStatus;
  startedAt?: string | null;
  lastAccessedAt?: string | null;
  completedAt?: string | null;
}

export interface CourseStudentsQuery {
  search?: string;
  status?: CourseStudentStatus;
  page?: number;
  pageSize?: number;
}
