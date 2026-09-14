using DefinitelyNotProductionReady.Domain.UserDomain;

namespace DefinitelyNotProductionReady.Application.UserApplication;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByNickNameAsync(string nickName);
    Task CreateAsync(User user);
}
