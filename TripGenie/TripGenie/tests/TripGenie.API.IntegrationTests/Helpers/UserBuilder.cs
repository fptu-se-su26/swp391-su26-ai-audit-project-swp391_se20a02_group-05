using System;
using TripGenie.API.Core.Entities;

namespace TripGenie.API.IntegrationTests.Helpers;

public class UserBuilder
{
    private string _email = "test@example.com";
    private string _password = "Password123!";
    private string _fullName = "Test User";
    private UserStatus _status = UserStatus.EMAIL_VERIFY_PENDING;
    private Guid _roleId = Guid.Empty;

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public UserBuilder WithFullName(string fullName)
    {
        _fullName = fullName;
        return this;
    }

    public UserBuilder WithStatus(UserStatus status)
    {
        _status = status;
        return this;
    }

    public UserBuilder WithRole(Guid roleId)
    {
        _roleId = roleId;
        return this;
    }

    public User Build()
    {
        return new User
        {
            Email = _email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(_password),
            FullName = _fullName,
            Status = _status,
            RoleId = _roleId,
            CreatedAt = DateTimeOffset.UtcNow.UtcDateTime,
            UpdatedAt = DateTimeOffset.UtcNow.UtcDateTime
        };
    }
}
