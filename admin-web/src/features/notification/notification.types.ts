export enum NotificationType {
  General = 0,
  LearningReminder = 10,
  ReviewDue = 11,
  StreakWarning = 12,
  StreakAchievement = 13,
  LessonCompleted = 20,
  QuizResult = 21,
  AccountSecurity = 30,
  System = 40,
}

export interface AdminNotification {
  id: number;
  publicId: string;
  userId: string;
  type: NotificationType;
  title: string;
  message: string;
  actionUrl: string | null;
  metadataJson: string | null;
  isRead: boolean;
  isExpired: boolean;
  createdAt: string;
  readAt: string | null;
  expiresAt: string | null;
}

export interface AdminNotificationQuery {
  userId?: string;
  type?: NotificationType;
  isRead?: boolean;
  isExpired?: boolean;
  from?: string;
  to?: string;
  sort?: string;
  page?: number;
  pageSize?: number;
}

export interface SendNotificationRequest {
  userId: string;
  type: NotificationType;
  title: string;
  message: string;
  actionUrl?: string | null;
  metadataJson?: string | null;
  expiresAt?: string | null;
}

export interface BroadcastNotificationRequest {
  type: NotificationType;
  title: string;
  message: string;
  actionUrl?: string | null;
  metadataJson?: string | null;
  expiresAt?: string | null;
  userIds?: string[] | null;
}

export const NOTIFICATION_TYPE_LABELS: Record<NotificationType, string> = {
  [NotificationType.General]: "Chung",
  [NotificationType.LearningReminder]: "Nhắc học",
  [NotificationType.ReviewDue]: "Đến hạn ôn tập",
  [NotificationType.StreakWarning]: "Cảnh báo streak",
  [NotificationType.StreakAchievement]: "Thành tích streak",
  [NotificationType.LessonCompleted]: "Hoàn thành bài học",
  [NotificationType.QuizResult]: "Kết quả kiểm tra",
  [NotificationType.AccountSecurity]: "Bảo mật tài khoản",
  [NotificationType.System]: "Hệ thống",
};
