using DefinitelyNotProductionReady.Application.PostApplication;
using DefinitelyNotProductionReady.Application.PostApplication.PostRequestDTO;
using DefinitelyNotProductionReady.Application.UserApplication;
using DefinitelyNotProductionReady.Domain.PostDomain;
using DefinitelyNotProductionReady.Domain.UserDomain;
using PostApp = DefinitelyNotProductionReady.Application.PostApplication.PostApplication;

namespace DefinitelyNotProductionReady.UnitTests.Application;

public class PostApplicationTests
{
    [Fact]
    public async Task Create_WhenUserExists_CreatesPost()
    {
        var user = CreateUser();
        var users = new FakeUserRepository(user);
        var posts = new FakePostRepository();
        var application = new PostApp(posts, users);

        var created = await application.Create(new CreatePostRequestDTO(user.Id, "Hello", PostType.Text));

        Assert.Equal(user.Id, created.UserId);
        Assert.Equal("Hello", created.Content);
        Assert.Equal(PostType.Text, created.Type);
        Assert.NotNull(await posts.GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task Create_WhenUserDoesNotExist_Throws()
    {
        var application = new PostApp(new FakePostRepository(), new FakeUserRepository());

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            application.Create(new CreatePostRequestDTO(Guid.NewGuid(), "Hello", PostType.Text)));
    }

    [Fact]
    public async Task GetById_WhenPostDoesNotExist_ReturnsNull()
    {
        var application = new PostApp(new FakePostRepository(), new FakeUserRepository());

        Assert.Null(await application.GetById(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByUserId_WhenUserDoesNotExist_ReturnsNull()
    {
        var application = new PostApp(new FakePostRepository(), new FakeUserRepository());

        Assert.Null(await application.GetByUserId(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByUserId_WhenUserHasNoPosts_ReturnsEmptyList()
    {
        var user = CreateUser();
        var application = new PostApp(new FakePostRepository(), new FakeUserRepository(user));

        var posts = await application.GetByUserId(user.Id);

        Assert.NotNull(posts);
        Assert.Empty(posts);
    }

    private static User CreateUser() => new(
        "alice",
        "Alice Doe",
        "hashed",
        "alice@example.com",
        "11999999999",
        DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-20));

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public FakeUserRepository(params User[] users) => _users = [.. users];

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

    private sealed class FakePostRepository : IPostRepository
    {
        private readonly List<Post> _posts = [];

        public Task<Post?> GetByIdAsync(Guid id)
            => Task.FromResult(_posts.FirstOrDefault(post => post.Id == id));

        public Task<IReadOnlyList<Post>> GetByUserIdAsync(Guid userId)
            => Task.FromResult<IReadOnlyList<Post>>(_posts.Where(post => post.UserId == userId).ToList());

        public Task CreateAsync(Post post)
        {
            _posts.Add(post);
            return Task.CompletedTask;
        }
    }
}
