namespace DefinitelyNotProductionReady.Domain.PostDomain;

public class Post
{
    public Post(Guid userId, string content, PostType type)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is required.", nameof(userId));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        if (!Enum.IsDefined(type))
            throw new ArgumentException("Invalid post type.", nameof(type));

        var now = DateTime.UtcNow;
        Id = Guid.NewGuid();
        UserId = userId;
        Content = content;
        Type = type;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; }
    public PostType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
}
