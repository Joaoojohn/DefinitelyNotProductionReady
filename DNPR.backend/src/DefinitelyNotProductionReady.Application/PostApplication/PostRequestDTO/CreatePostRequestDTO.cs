using DefinitelyNotProductionReady.Domain.PostDomain;

namespace DefinitelyNotProductionReady.Application.PostApplication.PostRequestDTO;

public record CreatePostRequestDTO(
    Guid UserId,
    string Content,
    PostType Type);
