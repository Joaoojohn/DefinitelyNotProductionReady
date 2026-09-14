using DefinitelyNotProductionReady.Domain.UserDomain;

namespace DefinitelyNotProductionReady.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesUser()
    {
        var birthDate = AdultBirthDate();

        var user = CreateUser(birthDate: birthDate);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("alice", user.NickName);
        Assert.Equal("Alice Doe", user.FullName);
        Assert.Equal("hashed-password", user.PasswordHash);
        Assert.Equal("alice@example.com", user.Email);
        Assert.Equal("11999999999", user.PhoneNumber);
        Assert.Equal(birthDate, user.BirthDate);
        Assert.True(user.CreationDate <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenNicknameIsMissing_Throws(string? nickName)
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateUser(nickName: nickName!));

        Assert.Equal("nickName", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenFullNameIsMissing_Throws(string? fullName)
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateUser(fullName: fullName!));

        Assert.Equal("fullName", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenPasswordHashIsMissing_Throws(string? passwordHash)
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateUser(passwordHash: passwordHash!));

        Assert.Equal("passwordHash", exception.ParamName);
    }

    [Theory]
    [InlineData("alice")]
    [InlineData("alice@")]
    [InlineData("alice.example.com")]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenEmailIsInvalid_Throws(string email)
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateUser(email: email));

        Assert.Equal("email", exception.ParamName);
    }

    [Theory]
    [InlineData("119999999")]
    [InlineData("1199999999a")]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenPhoneNumberIsInvalid_Throws(string phoneNumber)
    {
        var exception = Assert.Throws<ArgumentException>(() => CreateUser(phoneNumber: phoneNumber));

        Assert.Equal("phoneNumber", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenUserIsUnder18_Throws()
    {
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18).AddDays(1);

        var exception = Assert.Throws<ArgumentException>(() => CreateUser(birthDate: birthDate));

        Assert.Equal("birthDate", exception.ParamName);
    }

    [Fact]
    public void Constructor_WhenUserIsExactly18_Succeeds()
    {
        var birthDate = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-18);

        var user = CreateUser(birthDate: birthDate);

        Assert.Equal(birthDate, user.BirthDate);
    }

    private static User CreateUser(
        string nickName = "alice",
        string fullName = "Alice Doe",
        string passwordHash = "hashed-password",
        string email = "alice@example.com",
        string phoneNumber = "11999999999",
        DateOnly? birthDate = null)
        => new(nickName, fullName, passwordHash, email, phoneNumber, birthDate ?? AdultBirthDate());

    private static DateOnly AdultBirthDate()
        => DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-20);
}
