using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DefinitelyNotProductionReady.Application.PostApplication;
using DefinitelyNotProductionReady.Application.UserApplication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DefinitelyNotProductionReady.IntegrationTests;

public class UserPostVerticalSliceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _client;

    public UserPostVerticalSliceTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Testing"))
            .CreateClient();
    }

    [Fact]
    public async Task CreateAndGetUser_ReturnsCreatedUserWithoutPassword()
    {
        var suffix = UniqueSuffix();
        var createResponse = await _client.PostAsJsonAsync("/users", NewUser(suffix));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<UserResponseDTO>(JsonOptions);
        Assert.NotNull(created);
        Assert.Equal($"user{suffix}", created.NickName);
        Assert.Equal($"user{suffix}@example.com", created.Email);
        Assert.DoesNotContain("password", await createResponse.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);

        var getResponse = await _client.GetAsync($"/users/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<UserResponseDTO>(JsonOptions);
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
    }

    [Fact]
    public async Task GetUser_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/users/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WhenUnderage_ReturnsBadRequest()
    {
        var suffix = UniqueSuffix();
        var request = new
        {
            nickName = $"user{suffix}",
            fullName = "Test User",
            password = "secret123",
            email = $"user{suffix}@example.com",
            phoneNumber = "11999999999",
            birthDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-10).ToString("yyyy-MM-dd")
        };

        var response = await _client.PostAsJsonAsync("/users", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAndGetPost_AndListUserPosts()
    {
        var suffix = UniqueSuffix();
        var userResponse = await _client.PostAsJsonAsync("/users", NewUser(suffix));
        var user = await userResponse.Content.ReadFromJsonAsync<UserResponseDTO>(JsonOptions);
        Assert.NotNull(user);

        var createPostResponse = await _client.PostAsJsonAsync("/posts", new
        {
            userId = user.Id,
            content = "Hello, playground.",
            type = "Text"
        });

        Assert.Equal(HttpStatusCode.Created, createPostResponse.StatusCode);
        var createdPost = await createPostResponse.Content.ReadFromJsonAsync<PostResponseDTO>(JsonOptions);
        Assert.NotNull(createdPost);
        Assert.Equal(user.Id, createdPost.UserId);
        Assert.Equal("Hello, playground.", createdPost.Content);

        var getPostResponse = await _client.GetAsync($"/posts/{createdPost.Id}");
        Assert.Equal(HttpStatusCode.OK, getPostResponse.StatusCode);

        var listResponse = await _client.GetAsync($"/users/{user.Id}/posts");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var posts = await listResponse.Content.ReadFromJsonAsync<List<PostResponseDTO>>(JsonOptions);
        Assert.NotNull(posts);
        Assert.Single(posts);
        Assert.Equal(createdPost.Id, posts[0].Id);
    }

    [Fact]
    public async Task CreatePost_WhenUserMissing_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync("/posts", new
        {
            userId = Guid.NewGuid(),
            content = "Hello",
            type = "Text"
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetUserPosts_WhenUserMissing_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/users/{Guid.NewGuid()}/posts");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static object NewUser(string suffix) => new
    {
        nickName = $"user{suffix}",
        fullName = "Test User",
        password = "secret123",
        email = $"user{suffix}@example.com",
        phoneNumber = "11999999999",
        birthDate = "1990-05-20"
    };

    private static string UniqueSuffix() => Guid.NewGuid().ToString("N")[..8];
}
