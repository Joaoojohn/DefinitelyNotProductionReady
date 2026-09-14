using DefinitelyNotProductionReady.Domain.PostDomain;

namespace DefinitelyNotProductionReady.Application.PostApplication;

public record PostResponseDTO(
    Guid Id,
    Guid UserId,
    string Content,
    PostType Type,
    DateTime CreatedAt,
    DateTime UpdatedAt);
