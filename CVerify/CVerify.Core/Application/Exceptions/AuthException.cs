using System;

namespace CVerify.API.Application.Exceptions;

public class AuthException : Exception
{
    public string Code { get; }

    public AuthException(string code, string message) : base(message)
    {
        Code = code;
    }

    public AuthException(string code, string message, Exception innerException) : base(message, innerException)
    {
        Code = code;
    }
}

public static class AuthErrorCodes
{
    public const string EmailAlreadyExists = "AUTH_EMAIL_ALREADY_EXISTS";
    public const string InvalidToken = "AUTH_INVALID_TOKEN";
    public const string ExpiredToken = "AUTH_EXPIRED_TOKEN";
    public const string TokenAlreadyConsumed = "AUTH_TOKEN_ALREADY_CONSUMED";
    public const string PasswordTooWeak = "AUTH_PASSWORD_TOO_WEAK";
    public const string PasswordsDoNotMatch = "AUTH_PASSWORDS_DO_NOT_MATCH";
    public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
    public const string LockedOut = "AUTH_LOCKED_OUT";
    public const string CooldownActive = "AUTH_COOLDOWN_ACTIVE";
    public const string Unauthorized = "AUTH_UNAUTHORIZED";
    public const string UntrustedRedirect = "AUTH_UNTRUSTED_REDIRECT";
    public const string SuspiciousActivity = "AUTH_SUSPICIOUS_ACTIVITY";
    public const string MaxAttemptsReached = "AUTH_MAX_ATTEMPTS_REACHED";
}
