import type { ContentStatus } from "@/types/enums";

export interface AdminCourseChapterDto {
  id: number;
  publicId: string;
  titleVi: string;
  descriptionVi?: string;
  sortOrder: number;
  isActive: boolean;
  lessonCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface AdminCourseDetailDto {
  id: number;
  publicId: string;
  code: string;
  slug: string;
  titleVi: string;
  shortDescriptionVi?: string;
  descriptionVi?: string;
  
  hskLevelId?: number;
  hskCode?: string;
  hskNameVi?: string;
  
  coverImageUrl?: string;
  sortOrder: number;
  estimatedMinutes?: number;
  
  status: ContentStatus;
  isActive: boolean;
  isFeatured: boolean;
  
  chapters: AdminCourseChapterDto[];
  // prerequisites left out for brevity unless needed
}
