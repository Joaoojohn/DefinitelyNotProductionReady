using System.Collections.Concurrent;
using DefinitelyNotProductionReady.Application.UserApplication;
using DefinitelyNotProductionReady.Domain.UserDomain;

namespace DefinitelyNotProductionReady.Infrastructure.Persistence;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public Task<User?> GetByIdAsync(Guid id)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var user = _users.Values.FirstOrDefault(candidate =>
            string.Equals(candidate.Email, email, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task<User?> GetByNickNameAsync(string nickName)
    {
        var user = _users.Values.FirstOrDefault(candidate =>
            string.Equals(candidate.NickName, nickName, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task CreateAsync(User user)
    {
        if (!_users.TryAdd(user.Id, user))
            throw new InvalidOperationException($"User '{user.Id}' already exists.");

        return Task.CompletedTask;
    }
}
