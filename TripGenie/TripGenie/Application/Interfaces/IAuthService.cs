using System.Threading;
using System.Threading.Tasks;
using TripGenie.API.Application.DTOs;

namespace TripGenie.API.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task LogoutAsync();
    Task<AuthResponse?> RefreshTokenAsync();
    Task<UserProfileResponse?> GetMeAsync();

    // New recovery, verification, and registration contracts
    Task<bool> RegisterAsync(RegisterRequest request, string userAgent, string ipAddress, CancellationToken cancellationToken = default);
    Task<bool> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);
    Task<bool> ResendVerificationEmailAsync(ResendVerificationRequest request, CancellationToken cancellationToken = default);
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteMeAsync();
}


