using System.Collections.Concurrent;
using DefinitelyNotProductionReady.Application.PostApplication;
using DefinitelyNotProductionReady.Domain.PostDomain;

namespace DefinitelyNotProductionReady.Infrastructure.Persistence;

public sealed class InMemoryPostRepository : IPostRepository
{
    private readonly ConcurrentDictionary<Guid, Post> _posts = new();

    public Task<Post?> GetByIdAsync(Guid id)
    {
        _posts.TryGetValue(id, out var post);
        return Task.FromResult(post);
    }

    public Task<IReadOnlyList<Post>> GetByUserIdAsync(Guid userId)
    {
        IReadOnlyList<Post> posts = _posts.Values
            .Where(post => post.UserId == userId)
            .OrderByDescending(post => post.CreatedAt)
            .ToList();

        return Task.FromResult(posts);
    }

    public Task CreateAsync(Post post)
    {
        if (!_posts.TryAdd(post.Id, post))
            throw new InvalidOperationException($"Post '{post.Id}' already exists.");

        return Task.CompletedTask;
    }
}
