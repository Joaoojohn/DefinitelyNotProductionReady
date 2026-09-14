namespace DefinitelyNotProductionReady.Application.UserApplication;

public record UserResponseDTO(
    Guid Id,
    string NickName,
    string FullName,
    string Email,
    string PhoneNumber,
    DateOnly BirthDate,
    DateTime CreationDate);
