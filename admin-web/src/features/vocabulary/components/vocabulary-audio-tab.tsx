"use client";

import { Volume2 } from "lucide-react";
import { useCallback, useEffect, useState } from "react";

import { Alert } from "@/components/ui/alert";
import { Card, CardContent } from "@/components/ui/card";
import { apiClient } from "@/lib/api/api-client";
import { API_ENDPOINTS } from "@/lib/api/api-endpoints";
import { AudioAssetPicker } from "./audio-asset-picker";

interface VocabularyDto {
  id: number;
  audioAssetId: number | null;
  version: number;
}

export function VocabularyAudioTab({ vocabularyId }: { vocabularyId: number }) {
  const [vocabulary, setVocabulary] = useState<VocabularyDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setVocabulary(await apiClient<VocabularyDto>(API_ENDPOINTS.VOCABULARY.DETAIL(vocabularyId)));
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Không thể tải thông tin audio từ vựng.");
    } finally {
      setLoading(false);
    }
  }, [vocabularyId]);

  useEffect(() => { void load(); }, [load]);

  async function attachAudio(audioAssetId: number | null) {
    if (!vocabulary) return;
    const updated = await apiClient<VocabularyDto>(API_ENDPOINTS.VOCABULARY.AUDIO(vocabularyId), {
      method: "PUT",
      body: { audioAssetId, version: vocabulary.version },
    });
    setVocabulary(updated);
  }

  if (loading) {
    return (
      <Card>
        <CardContent className="flex items-center gap-2 p-6 text-[13px] text-[#777]">
          <Volume2 size={16} /> Đang tải audio...
        </CardContent>
      </Card>
    );
  }

  if (!vocabulary) {
    return <Alert variant="danger" title="Không thể tải Audio">{error ?? "Không tìm thấy Vocabulary."}</Alert>;
  }

  return (
    <div className="space-y-3">
      {error && <Alert variant="danger">{error}</Alert>}
      <AudioAssetPicker
        value={vocabulary.audioAssetId}
        onChange={attachAudio}
        kind={0}
        title="Audio phát âm từ vựng"
        description="Chọn AudioAsset phát âm có sẵn hoặc upload trực tiếp lên Backblaze B2. Đổi audio dùng command riêng có kiểm tra Version, không ghi đè các trường Vocabulary khác."
      />
    </div>
  );
}
