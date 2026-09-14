using DefinitelyNotProductionReady.Application.Security;
using DefinitelyNotProductionReady.Application.UserApplication;
using DefinitelyNotProductionReady.Application.UserApplication.UserRequestDTO;
using DefinitelyNotProductionReady.Domain.UserDomain;
using UserApp = DefinitelyNotProductionReady.Application.UserApplication.UserApplication;

namespace DefinitelyNotProductionReady.UnitTests.Application;

public class UserApplicationTests
{
    [Fact]
    public async Task Create_HashesPasswordAndDoesNotExposeIt()
    {
        var repository = new FakeUserRepository();
        var application = new UserApp(repository, new FakePasswordHasher());

        var created = await application.Create(ValidRequest());

        var stored = await repository.GetByIdAsync(created.Id);
        Assert.NotNull(stored);
        Assert.Equal("hashed:secret123", stored.PasswordHash);
        Assert.DoesNotContain("secret123", created.ToString());
        Assert.Equal("alice", created.NickName);
        Assert.Equal("alice@example.com", created.Email);
    }

    [Fact]
    public async Task Create_WhenPasswordIsMissing_Throws()
    {
        var application = new UserApp(new FakeUserRepository(), new FakePasswordHasher());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            application.Create(ValidRequest() with { Password = " " }));
    }

    [Fact]
    public async Task Create_WhenEmailAlreadyExists_Throws()
    {
        var application = new UserApp(new FakeUserRepository(), new FakePasswordHasher());
        await application.Create(ValidRequest());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            application.Create(ValidRequest() with { NickName = "bob" }));

        Assert.Equal("Email is already in use.", exception.Message);
    }

    [Fact]
    public async Task Create_WhenNicknameAlreadyExists_Throws()
    {
        var application = new UserApp(new FakeUserRepository(), new FakePasswordHasher());
        await application.Create(ValidRequest());

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            application.Create(ValidRequest() with { Email = "bob@example.com" }));

        Assert.Equal("Nickname is already in use.", exception.Message);
    }

    [Fact]
    public async Task GetById_WhenUserExists_ReturnsUserWithoutPassword()
    {
        var repository = new FakeUserRepository();
        var application = new UserApp(repository, new FakePasswordHasher());
        var created = await application.Create(ValidRequest());

        var found = await application.GetById(created.Id);

        Assert.NotNull(found);
        Assert.Equal(created.Id, found.Id);
        Assert.Equal("alice", found.NickName);
    }

    [Fact]
    public async Task GetById_WhenUserDoesNotExist_ReturnsNull()
    {
        var application = new UserApp(new FakeUserRepository(), new FakePasswordHasher());

        var found = await application.GetById(Guid.NewGuid());

        Assert.Null(found);
    }

    private static CreateUserRequestDTO ValidRequest() => new(
        "alice",
        "Alice Doe",
        "secret123",
        "alice@example.com",
        "11999999999",
        DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-20));

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => $"hashed:{password}";
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = [];

        public Task<User?> GetByIdAsync(Guid id)
            => Task.FromResult(_users.FirstOrDefault(user => user.Id == id));

        public Task<User?> GetByEmailAsync(string email)
            => Task.FromResult(_users.FirstOrDefault(user =>
                string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase)));

        public Task<User?> GetByNickNameAsync(string nickName)
            => Task.FromResult(_users.FirstOrDefault(user =>
                string.Equals(user.NickName, nickName, StringComparison.OrdinalIgnoreCase)));

        public Task CreateAsync(User user)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }
    }
}
