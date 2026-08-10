using System.Text.Json;

namespace HanYu.IntegrationTests.Common;

public static class TestHttpClientExtensions
{
    public static async Task<JsonDocument>
        ReadJsonAsync(
            this HttpResponseMessage response)
    {
        var body =
            await response.Content
                .ReadAsStringAsync();

        return JsonDocument.Parse(body);
    }

    public static Guid GetGuid(
        this JsonElement element,
        string propertyName)
    {
        if (element.TryGetProperty(
                propertyName,
                out var value))
        {
            return value.GetGuid();
        }

        if (element.TryGetProperty(
                "data",
                out var data) &&
            data.TryGetProperty(
                propertyName,
                out value))
        {
            return value.GetGuid();
        }

        throw new InvalidOperationException(
            $"Property '{propertyName}' không tồn tại.");
    }

    public static long GetLong(
        this JsonElement element,
        string propertyName)
    {
        if (element.TryGetProperty(
                propertyName,
                out var value))
        {
            return value.GetInt64();
        }

        if (element.TryGetProperty(
                "data",
                out var data) &&
            data.TryGetProperty(
                propertyName,
                out value))
        {
            return value.GetInt64();
        }

        throw new InvalidOperationException(
            $"Property '{propertyName}' không tồn tại.");
    }
}