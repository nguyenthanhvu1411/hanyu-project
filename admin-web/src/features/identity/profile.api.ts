import { apiClient } from "@/lib/api/api-client";

export interface UserProfile {
  publicId: string;
  userName: string;
  email: string;
  emailConfirmed: boolean;
  displayName: string;
  avatarUrl: string | null;
  currentHskLevel: number;
  dailyGoalMinutes: number;
  timezone: string;
  uiLanguage: string;
  onboardingCompleted: boolean;
  onboardingCompletedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateUserProfileRequest {
  displayName: string;
  avatarUrl: string | null;
  currentHskLevel: number;
  dailyGoalMinutes: number;
  timezone: string;
  uiLanguage: string;
}

export const profileApi = {
  get() {
    return apiClient<UserProfile>("/profile");
  },
  update(request: UpdateUserProfileRequest) {
    return apiClient<UserProfile>("/profile", { method: "PUT", body: request });
  },
};