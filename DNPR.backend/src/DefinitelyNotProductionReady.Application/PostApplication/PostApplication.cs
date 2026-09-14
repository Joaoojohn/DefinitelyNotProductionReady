using DefinitelyNotProductionReady.Application.PostApplication.PostRequestDTO;
using DefinitelyNotProductionReady.Application.UserApplication;
using DefinitelyNotProductionReady.Domain.PostDomain;

namespace DefinitelyNotProductionReady.Application.PostApplication;

public class PostApplication(IPostRepository postRepository, IUserRepository userRepository)
{
    public async Task<PostResponseDTO> Create(CreatePostRequestDTO request)
    {
        if (await userRepository.GetByIdAsync(request.UserId) is null)
            throw new KeyNotFoundException("User not found.");

        var post = new Post(request.UserId, request.Content, request.Type);
        await postRepository.CreateAsync(post);
        return ToResponse(post);
    }

    public async Task<PostResponseDTO?> GetById(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        return post is null ? null : ToResponse(post);
    }

    public async Task<IReadOnlyList<PostResponseDTO>?> GetByUserId(Guid userId)
    {
        if (await userRepository.GetByIdAsync(userId) is null)
            return null;

        var posts = await postRepository.GetByUserIdAsync(userId);
        return posts.Select(ToResponse).ToList();
    }

    private static PostResponseDTO ToResponse(Post post) => new(
        post.Id,
        post.UserId,
        post.Content,
        post.Type,
        post.CreatedAt,
        post.UpdatedAt);
}
