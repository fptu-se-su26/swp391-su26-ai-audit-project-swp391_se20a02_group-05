using Microsoft.EntityFrameworkCore;
using TripGenie.API.Core.Entities;
using TripGenie.API.Application.DTOs;
using System.Security.Claims;
using TripGenie.API.Application.Interfaces;
using TripGenie.API.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;

namespace TripGenie.API.Application.Services;

/// <summary>
/// Orchestrates authentication flows including login, token rotation, and logout.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ICacheService _cacheService;
    private readonly IAccountService _accountService;
    private readonly IIdentityRepository _identityRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        ApplicationDbContext context,
        ITokenService tokenService,
        ICacheService cacheService,
        IAccountService accountService,
        IIdentityRepository identityRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _tokenService = tokenService;
        _cacheService = cacheService;
        _accountService = accountService;
        _identityRepository = identityRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Authenticates a user and issues JWT and Refresh tokens in HttpOnly cookies.
    /// Handles account lockout and failed attempt tracking.
    /// </summary>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null) return null;

        if (_accountService.IsAccountDisabled(user))
        {
            throw new UnauthorizedAccessException("Account is disabled.");
        }

        if (_accountService.IsAccountLocked(user))
        {
            throw new UnauthorizedAccessException($"Account is temporarily locked until {user.LockUntil}");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            await _accountService.HandleFailedLoginAsync(user);
            return null;
        }

        await _accountService.ResetFailedAttemptsAsync(user);

        var roles = await _identityRepository.GetUserRolesAsync(user.Id);
        var permissions = await _identityRepository.GetUserPermissionsAsync(user.Id);

        await CacheUserAuthDataAsync(user.Id, roles, permissions);

        var jwt = _tokenService.GenerateJwtToken(user, roles, permissions);
        var refreshTokenStr = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user.Id, refreshTokenStr);

        _tokenService.SetTokenInsideCookie("access_token", jwt);
        _tokenService.SetTokenInsideCookie("refresh_token", refreshTokenStr);

        return new AuthResponse(user.Id, user.Email, roles, permissions);
    }

    /// <summary>
    /// Revokes the current refresh token and removes authentication cookies.
    /// </summary>
    public async Task LogoutAsync()
    {
        var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (storedToken != null)
            {
                storedToken.RevokedAt = DateTimeOffset.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        _tokenService.RemoveTokenFromCookie("access_token");
        _tokenService.RemoveTokenFromCookie("refresh_token");
    }

    /// <summary>
    /// Validates the refresh token cookie and issues a new set of tokens (Token Rotation).
    /// Prevents replay attacks by revoking the old token.
    /// </summary>
    public async Task<AuthResponse?> RefreshTokenAsync()
    {
        var refreshTokenStr = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshTokenStr)) return null;

        var storedToken = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == refreshTokenStr);

        if (storedToken == null || !storedToken.IsActive) return null;

        var user = storedToken.User;
        var roles = await _identityRepository.GetUserRolesAsync(user.Id);
        var permissions = await _identityRepository.GetUserPermissionsAsync(user.Id);

        // Rotate Refresh Token
        var newRefreshTokenStr = _tokenService.GenerateRefreshToken();
        storedToken.RevokedAt = DateTimeOffset.UtcNow;
        storedToken.ReplacedByToken = newRefreshTokenStr;

        await SaveRefreshTokenAsync(user.Id, newRefreshTokenStr);

        var jwt = _tokenService.GenerateJwtToken(user, roles, permissions);

        _tokenService.SetTokenInsideCookie("access_token", jwt);
        _tokenService.SetTokenInsideCookie("refresh_token", newRefreshTokenStr);

        return new AuthResponse(user.Id, user.Email, roles, permissions);
    }

    public async Task<UserProfileResponse?> GetMeAsync()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return null;

        var userId = Guid.Parse(userIdClaim.Value);

        var roles = (await _cacheService.GetSetAsync($"auth:user:{userId}:roles")).ToList();
        var permissions = (await _cacheService.GetSetAsync($"auth:user:{userId}:permissions")).ToList();

        if (!roles.Any())
        {
            roles = (await _identityRepository.GetUserRolesAsync(userId)).ToList();
            permissions = (await _identityRepository.GetUserPermissionsAsync(userId)).ToList();
            await CacheUserAuthDataAsync(userId, roles, permissions);
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new UserProfileResponse(user.Id, user.Email, roles, permissions);
    }

    private async Task CacheUserAuthDataAsync(Guid userId, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var rolesKey = $"auth:user:{userId}:roles";
        var permsKey = $"auth:user:{userId}:permissions";

        await _cacheService.RemoveAsync(rolesKey);
        await _cacheService.RemoveAsync(permsKey);

        foreach (var role in roles) await _cacheService.AddToSetAsync(rolesKey, role);
        foreach (var perm in permissions) await _cacheService.AddToSetAsync(permsKey, perm);
    }

    private async Task SaveRefreshTokenAsync(Guid userId, string tokenStr)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = tokenStr,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
        };
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }
}
