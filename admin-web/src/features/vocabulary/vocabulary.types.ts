export enum VocabularyRelationType {
  Related = 0,
  Confusable = 1,
  Synonym = 2,
  Antonym = 3,
}

export enum VocabularyContentStatus {
  Draft = 0,
  Review = 1,
  Approved = 2,
  Published = 3,
  Archived = 4,
}

export interface AdminVocabularyMeaning {
  id: number;
  vocabularyId: number;
  meaningVi: string;
  senseOrder: number;
  usageNoteVi: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface VocabularyMeaningRequest {
  meaningVi: string;
  senseOrder: number;
  usageNoteVi?: string | null;
}

export interface AdminVocabularyExample {
  id: number;
  vocabularyId: number;
  audioAssetId: number | null;
  sentenceZh: string;
  sentencePinyin: string;
  sentenceVi: string;
  difficulty: number;
  status: VocabularyContentStatus;
  sourceNote: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface VocabularyExampleRequest {
  sentenceZh: string;
  sentencePinyin: string;
  sentenceVi: string;
  difficulty: number;
  audioAssetId?: number | null;
  sourceNote?: string | null;
}

export interface AdminVocabularyRelation {
  id: number;
  vocabularyId: number;
  relatedVocabularyId: number;
  relatedSimplified: string;
  relatedPinyin: string;
  relatedMeaningVi: string;
  relationType: VocabularyRelationType;
  noteVi: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface VocabularyRelationRequest {
  relatedVocabularyId: number;
  relationType: VocabularyRelationType;
  noteVi?: string | null;
}

export const VOCABULARY_RELATION_LABELS: Record<VocabularyRelationType, string> = {
  [VocabularyRelationType.Related]: "Liên quan",
  [VocabularyRelationType.Confusable]: "Dễ nhầm",
  [VocabularyRelationType.Synonym]: "Đồng nghĩa",
  [VocabularyRelationType.Antonym]: "Trái nghĩa",
};

export const VOCABULARY_CONTENT_STATUS_LABELS: Record<VocabularyContentStatus, string> = {
  [VocabularyContentStatus.Draft]: "Bản nháp",
  [VocabularyContentStatus.Review]: "Chờ duyệt",
  [VocabularyContentStatus.Approved]: "Đã duyệt",
  [VocabularyContentStatus.Published]: "Đã xuất bản",
  [VocabularyContentStatus.Archived]: "Lưu trữ",
};
