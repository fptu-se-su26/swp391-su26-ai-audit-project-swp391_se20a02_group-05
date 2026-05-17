using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TripGenie.API.Application.DTOs;

public record LoginRequest(
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    string Email,

    [Required]
    string Password
);

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; init; } = null!;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^()_\-+=\[\]{}|\\:;""'<>,.?/~`])[A-Za-z\d@$!%*?&#^()_\-+=\[\]{}|\\:;""'<>,.?/~`]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string Password { get; init; } = null!;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; init; } = null!;

    [Required]
    [MaxLength(255)]
    public string FullName { get; init; } = null!;

    public RegisterRequest() { }

    public RegisterRequest(string Email, string Password, string ConfirmPassword, string FullName)
    {
        this.Email = Email;
        this.Password = Password;
        this.ConfirmPassword = ConfirmPassword;
        this.FullName = FullName;
    }
}

public record VerifyEmailRequest(
    [Required]
    string Token
);

public record ResendVerificationRequest(
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    string Email
);

public record ForgotPasswordRequest(
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    string Email
);

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; init; } = null!;

    [Required]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^()_\-+=\[\]{}|\\:;""'<>,.?/~`])[A-Za-z\d@$!%*?&#^()_\-+=\[\]{}|\\:;""'<>,.?/~`]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
    public string Password { get; init; } = null!;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; init; } = null!;

    public ResetPasswordRequest() { }

    public ResetPasswordRequest(string Token, string Password, string ConfirmPassword)
    {
        this.Token = Token;
        this.Password = Password;
        this.ConfirmPassword = ConfirmPassword;
    }
}

public record AuthResponse(Guid Id, string Email, IEnumerable<string> Roles, IEnumerable<string> Permissions);

public record UserProfileResponse(Guid Id, string Email, IEnumerable<string> Roles, IEnumerable<string> Permissions);
