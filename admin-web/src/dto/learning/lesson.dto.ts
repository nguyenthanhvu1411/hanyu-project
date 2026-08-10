import type { ContentStatus } from "@/types/enums";

export interface AdminLessonListItemDto {
  id: number;
  publicId: string;
  
  courseId?: number;
  courseTitleVi?: string;
  
  chapterId?: number;
  chapterTitleVi?: string;
  
  hskLevelId: number;
  hskCode?: string;
  
  topicId?: number;
  
  slug: string;
  titleVi: string;
  shortDescriptionVi?: string;
  
  sortOrder: number;
  estimatedMinutes: number;
  difficulty: number;
  
  isFeatured: boolean;
  
  status: ContentStatus;
  version: number;
  
  publishedAt?: string;
  createdAt: string;
  updatedAt: string;
}
