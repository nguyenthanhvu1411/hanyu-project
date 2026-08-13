using HanYu.Application.Common.Models;
using HanYu.Domain.Entities.Vocabulary;
using HanYu.Domain.Enums;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HanYu.API.Common;

public static class VocabularyAudioGuard
{
    public static async Task<Result> ValidateAsync(
        HanYuDbContext db,
        long? audioAssetId,
        AudioAssetKind expectedKind,
        CancellationToken cancellationToken = default)
    {
        if (!audioAssetId.HasValue)
        {
            return Result.Success();
        }

        var audio = await db.Set<AudioAsset>()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.Id == audioAssetId.Value,
                cancellationToken);

        if (audio is null)
        {
            return Result.Failure(
                Error.Validation(
                    "AudioAsset.NotFound",
                    "AudioAsset không tồn tại."));
        }

        if (audio.Status == ContentStatus.Archived)
        {
            return Result.Failure(
                Error.Conflict(
                    "AudioAsset.Archived",
                    "AudioAsset đã Archived."));
        }

        if (audio.Kind != expectedKind)
        {
            return Result.Failure(
                Error.Validation(
                    "AudioAsset.InvalidKind",
                    expectedKind == AudioAssetKind.Vocabulary
                        ? "Vocabulary chỉ được gắn AudioAsset loại Vocabulary."
                        : "VocabularyExample chỉ được gắn AudioAsset loại ExampleSentence."));
        }

        return Result.Success();
    }
}
