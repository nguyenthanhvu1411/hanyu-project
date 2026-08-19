import { apiClient } from "@/lib/api/api-client";
import type {
  AdminVocabularyExample,
  AdminVocabularyMeaning,
  AdminVocabularyRelation,
  VocabularyExampleRequest,
  VocabularyMeaningRequest,
  VocabularyRelationRequest,
} from "./vocabulary.types";

function root(vocabularyId: number) {
  return `/admin/vocabularies/${vocabularyId}`;
}

export const vocabularyApi = {
  meanings: {
    list(vocabularyId: number) {
      return apiClient<AdminVocabularyMeaning[]>(`${root(vocabularyId)}/meanings`);
    },
    create(vocabularyId: number, request: VocabularyMeaningRequest) {
      return apiClient<AdminVocabularyMeaning>(`${root(vocabularyId)}/meanings`, { method: "POST", body: request });
    },
    update(vocabularyId: number, meaningId: number, request: VocabularyMeaningRequest) {
      return apiClient<AdminVocabularyMeaning>(`${root(vocabularyId)}/meanings/${meaningId}`, { method: "PUT", body: request });
    },
    remove(vocabularyId: number, meaningId: number) {
      return apiClient<void>(`${root(vocabularyId)}/meanings/${meaningId}`, { method: "DELETE" });
    },
  },

  examples: {
    list(vocabularyId: number) {
      return apiClient<AdminVocabularyExample[]>(`${root(vocabularyId)}/examples`);
    },
    create(vocabularyId: number, request: VocabularyExampleRequest) {
      return apiClient<AdminVocabularyExample>(`${root(vocabularyId)}/examples`, { method: "POST", body: request });
    },
    update(vocabularyId: number, exampleId: number, request: VocabularyExampleRequest) {
      return apiClient<AdminVocabularyExample>(`${root(vocabularyId)}/examples/${exampleId}`, { method: "PUT", body: request });
    },
    remove(vocabularyId: number, exampleId: number) {
      return apiClient<void>(`${root(vocabularyId)}/examples/${exampleId}`, { method: "DELETE" });
    },
    submitReview(vocabularyId: number, exampleId: number) {
      return apiClient<void>(`${root(vocabularyId)}/examples/${exampleId}/submit-review`, { method: "POST" });
    },
    approve(vocabularyId: number, exampleId: number) {
      return apiClient<void>(`${root(vocabularyId)}/examples/${exampleId}/approve`, { method: "POST" });
    },
    publish(vocabularyId: number, exampleId: number) {
      return apiClient<void>(`${root(vocabularyId)}/examples/${exampleId}/publish`, { method: "POST" });
    },
    archive(vocabularyId: number, exampleId: number) {
      return apiClient<void>(`${root(vocabularyId)}/examples/${exampleId}/archive`, { method: "POST" });
    },
    restore(vocabularyId: number, exampleId: number) {
      return apiClient<void>(`${root(vocabularyId)}/examples/${exampleId}/restore`, { method: "POST" });
    },
  },

  relations: {
    list(vocabularyId: number) {
      return apiClient<AdminVocabularyRelation[]>(`${root(vocabularyId)}/relations`);
    },
    create(vocabularyId: number, request: VocabularyRelationRequest) {
      return apiClient<AdminVocabularyRelation>(`${root(vocabularyId)}/relations`, { method: "POST", body: request });
    },
    update(vocabularyId: number, relationId: number, request: VocabularyRelationRequest) {
      return apiClient<AdminVocabularyRelation>(`${root(vocabularyId)}/relations/${relationId}`, { method: "PUT", body: request });
    },
    remove(vocabularyId: number, relationId: number) {
      return apiClient<void>(`${root(vocabularyId)}/relations/${relationId}`, { method: "DELETE" });
    },
  },
};
