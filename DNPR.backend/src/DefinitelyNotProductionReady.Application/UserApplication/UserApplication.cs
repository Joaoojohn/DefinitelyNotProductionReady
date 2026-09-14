using DefinitelyNotProductionReady.Application.Security;
using DefinitelyNotProductionReady.Application.UserApplication.UserRequestDTO;
using DefinitelyNotProductionReady.Domain.UserDomain;

namespace DefinitelyNotProductionReady.Application.UserApplication;

public class UserApplication(IUserRepository userRepository, IPasswordHasher passwordHasher)
{
    public async Task<UserResponseDTO> Create(CreateUserRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required.", nameof(request));

        if (await userRepository.GetByEmailAsync(request.Email) is not null)
            throw new InvalidOperationException("Email is already in use.");

        if (await userRepository.GetByNickNameAsync(request.NickName) is not null)
            throw new InvalidOperationException("Nickname is already in use.");

        var user = new User(
            request.NickName,
            request.FullName,
            passwordHasher.Hash(request.Password),
            request.Email,
            request.PhoneNumber,
            request.BirthDate);

        await userRepository.CreateAsync(user);
        return ToResponse(user);
    }

    public async Task<UserResponseDTO?> GetById(Guid id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return user is null ? null : ToResponse(user);
    }

    private static UserResponseDTO ToResponse(User user) => new(
        user.Id,
        user.NickName,
        user.FullName,
        user.Email,
        user.PhoneNumber,
        user.BirthDate,
        user.CreationDate);
}
