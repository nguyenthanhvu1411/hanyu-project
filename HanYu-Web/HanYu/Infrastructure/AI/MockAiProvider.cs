using HanYu.Application.Interfaces.AI;

namespace HanYu.Infrastructure.AI;

public class MockAiProvider : IAiProvider
{
    public async Task<AiProviderResult> GenerateAsync(
        AiProviderRequest request,
        CancellationToken cancellationToken = default)
    {
        // Giả lập delay của API AI
        await Task.Delay(500, cancellationToken);

        string responseContent = $"[Mock] Phản hồi chung từ AI cho: {request.Messages.LastOrDefault()?.Content}";

        // Trả về kết quả giả với số token ngẫu nhiên
        return new AiProviderResult(
            Provider: "Mock",
            Model: "mock-model",
            Content: responseContent,
            InputTokens: 50,
            OutputTokens: 100,
            EstimatedCostUsd: 0.001m,
            LatencyMs: 500);
    }
}
