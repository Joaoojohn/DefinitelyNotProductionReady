using DefinitelyNotProductionReady.Domain.PostDomain;

namespace DefinitelyNotProductionReady.UnitTests.Domain;

public class PostTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesPost()
    {
        var userId = Guid.NewGuid();

        var post = new Post(userId, "Hello, playground.", PostType.Text);

        Assert.NotEqual(Guid.Empty, post.Id);
        Assert.Equal(userId, post.UserId);
        Assert.Equal("Hello, playground.", post.Content);
        Assert.Equal(PostType.Text, post.Type);
        Assert.Equal(post.CreatedAt, post.UpdatedAt);
    }

    [Fact]
    public void Constructor_WhenUserIdIsEmpty_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Post(Guid.Empty, "Hello", PostType.Text));

        Assert.Equal("userId", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenContentIsMissing_Throws(string? content)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Post(Guid.NewGuid(), content!, PostType.Image));

        Assert.Equal("content", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenTypeIsUndefined_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Post(Guid.NewGuid(), "Hello", (PostType)999));

        Assert.Equal("type", exception.ParamName);
    }
}
