using System.Threading;
using System.Threading.Tasks;
using CVerify.API.Application.DTOs;

namespace CVerify.API.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> LoginWithGoogleAsync(GoogleLoginRequest request);
    Task LogoutAsync();
    Task<AuthResponse?> RefreshTokenAsync();
    Task<UserProfileResponse?> GetMeAsync();

    // New recovery, verification, and registration contracts
    Task<RegisterResponse> RegisterAsync(RegisterRequest request, string userAgent, string ipAddress, CancellationToken cancellationToken = default);
    Task<AuthResponse?> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);
    Task<bool> ResendVerificationEmailAsync(ResendVerificationRequest request, CancellationToken cancellationToken = default);
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse?> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteMeAsync();
}


