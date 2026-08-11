using HanYu.Application.Interfaces.Storage;
using System.Collections.Concurrent;

namespace HanYu.Infrastructure.Storage;

public sealed class InMemoryPublicFileStorage : IPublicFileStorage
{
    private readonly ConcurrentDictionary<string, byte[]> _objects = new();

    public async Task<PublicFileUploadResult> UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (content.CanSeek)
            content.Position = 0;

        await using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);
        _objects[objectKey] = buffer.ToArray();

        return new PublicFileUploadResult(
            objectKey,
            await GetReadUrlAsync(objectKey, cancellationToken));
    }

    public Task<string> GetReadUrlAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult($"https://storage.test/{objectKey.TrimStart('/')}");
    }

    public Task DeleteAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        _objects.TryRemove(objectKey, out _);
        return Task.CompletedTask;
    }
}
