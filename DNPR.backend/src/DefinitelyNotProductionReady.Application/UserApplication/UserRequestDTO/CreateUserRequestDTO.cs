namespace DefinitelyNotProductionReady.Application.UserApplication.UserRequestDTO
{
    public record CreateUserRequestDTO(
      string NickName,
      string FullName,
      string Password,
      string Email,
      string PhoneNumber,
      DateOnly BirthDate);
}
