namespace HanYu.Application.Interfaces.AI;

public interface IAiProvider
{
    Task<AiProviderResult> GenerateAsync(
        AiProviderRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record AiProviderRequest(
    string SystemPrompt,
    IReadOnlyCollection<AiProviderMessage> Messages);

public sealed record AiProviderMessage(
    string Role,
    string Content);

public sealed record AiProviderResult(
    string Provider,
    string Model,
    string Content,
    int InputTokens,
    int OutputTokens,
    decimal? EstimatedCostUsd,
    int LatencyMs);
