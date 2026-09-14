using DefinitelyNotProductionReady.Domain.PostDomain;

namespace DefinitelyNotProductionReady.Application.PostApplication;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Post>> GetByUserIdAsync(Guid userId);
    Task CreateAsync(Post post);
}
