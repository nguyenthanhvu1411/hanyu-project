import type { HskLevelListQuery } from "./learning.types";

export const learningKeys = {
  all: ["learning"] as const,

  hskLevels: () => [...learningKeys.all, "hsk-levels"] as const,

  hskLevelList: (query: HskLevelListQuery) =>
    [...learningKeys.hskLevels(), "list", query] as const,
};
