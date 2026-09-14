namespace DefinitelyNotProductionReady.Domain.UserDomain;

public class User
{
    public User(string nickName, string fullName, string passwordHash, string email, string phoneNumber, DateOnly birthDate)
    {
        if (string.IsNullOrWhiteSpace(nickName))
            throw new ArgumentException("Nickname is required.", nameof(nickName));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email.", nameof(email));

        if (!IsValidPhoneNumber(phoneNumber))
            throw new ArgumentException("Invalid phone number.", nameof(phoneNumber));

        if (!IsAdult(birthDate))
            throw new ArgumentException("User must be at least 18 years old.", nameof(birthDate));

        Id = Guid.NewGuid();
        NickName = nickName;
        FullName = fullName;
        PasswordHash = passwordHash;
        Email = email;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
        CreationDate = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string NickName { get; private set; }
    public string FullName { get; private set; }
    public string PasswordHash { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }

    public DateOnly BirthDate { get; private set; }
    public DateTime CreationDate { get; private set; }

    private static bool IsAdult(DateOnly birthDate)
        => birthDate.AddYears(18) <= DateOnly.FromDateTime(DateTime.UtcNow);

    private static bool IsValidEmail(string email)
        => !string.IsNullOrWhiteSpace(email)
           && email.Contains('@')
           && email.Contains('.');

    private static bool IsValidPhoneNumber(string phoneNumber)
        => !string.IsNullOrWhiteSpace(phoneNumber)
           && phoneNumber.Length >= 10
           && phoneNumber.All(char.IsDigit);
}