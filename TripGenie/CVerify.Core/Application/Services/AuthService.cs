using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CVerify.API.Application.DTOs;
using CVerify.API.Application.Exceptions;
using CVerify.API.Application.Interfaces;
using CVerify.API.Core.Entities;
using CVerify.API.Infrastructure.Configuration;
using CVerify.API.Infrastructure.Diagnostics;
using CVerify.API.Infrastructure.Persistence;
using CVerify.API.Infrastructure.Security;
using Google.Apis.Auth;

namespace CVerify.API.Application.Services;

/// <summary>
/// Orchestrates advanced authentication flows including register, login, verification, and password recovery.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ICacheService _cacheService;
    private readonly IAccountService _accountService;
    private readonly IIdentityRepository _identityRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly EnvConfiguration _envConfig;
    private readonly ILogger<AuthService> _logger;
    private readonly AuthMetrics _metrics;
    private readonly TimeProvider _timeProvider;

    public AuthService(
        ApplicationDbContext context,
        ITokenService tokenService,
        ICacheService cacheService,
        IAccountService accountService,
        IIdentityRepository identityRepository,
        IHttpContextAccessor httpContextAccessor,
        EnvConfiguration envConfig,
        ILogger<AuthService> logger,
        AuthMetrics metrics,
        TimeProvider timeProvider)
    {
        _context = context;
        _tokenService = tokenService;
        _cacheService = cacheService;
        _accountService = accountService;
        _identityRepository = identityRepository;
        _httpContextAccessor = httpContextAccessor;
        _envConfig = envConfig;
        _logger = logger;
        _metrics = metrics;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Authenticates a user and issues JWT and Refresh tokens in HttpOnly cookies.
    /// Handles account lockout and failed attempt tracking.
    /// </summary>
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().Normalize(NormalizationForm.FormC).ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user == null)
        {
            _metrics.RecordLoginFailed();
            await LogAuditEventAsync(null, "USER_LOGIN_FAILED_EMAIL", $"Unknown email login attempt for {normalizedEmail}.");
            return null;
        }

        if (_accountService.IsAccountDisabled(user))
        {
            _metrics.RecordLoginFailed();
            await LogAuditEventAsync(user.Id, "USER_LOGIN_FAILED_DISABLED", $"Disabled user account login attempt for {user.Email}.");
            throw new UnauthorizedAccessException("Account is disabled.");
        }

        if (_accountService.IsAccountLocked(user))
        {
            _metrics.RecordLoginFailed();
            await LogAuditEventAsync(user.Id, "USER_LOGIN_FAILED_LOCKED", $"Locked user account login attempt for {user.Email}.");
            throw new UnauthorizedAccessException($"Account is temporarily locked until {user.LockUntil}");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            _metrics.RecordLoginFailed();
            await _accountService.HandleFailedLoginAsync(user);
            await LogAuditEventAsync(user.Id, "USER_LOGIN_FAILED_CREDENTIALS", $"Invalid password login attempt for {user.Email}.");
            return null;
        }

        await _accountService.ResetFailedAttemptsAsync(user);

        var roles = await _identityRepository.GetUserRolesAsync(user.Id);
        var permissions = await _identityRepository.GetUserPermissionsAsync(user.Id);

        if (user.Status == UserStatus.EMAIL_VERIFY_PENDING)
        {
            await LogAuditEventAsync(user.Id, "USER_LOGIN_UNVERIFIED", $"User {user.Email} attempted to login but email is unverified.");
            return new AuthResponse(user.Id, user.Email, user.FullName, user.AvatarUrl, roles, permissions, false, "EMAIL_VERIFY_PENDING", "VERIFY_EMAIL");
        }

        await CacheUserAuthDataAsync(user.Id, roles, permissions);

        var jwt = _tokenService.GenerateJwtToken(user, roles, permissions);
        var refreshTokenStr = _tokenService.GenerateRefreshToken();

        var sessionId = Guid.NewGuid();
        var rememberMe = request.RememberMe;
        await SaveRefreshTokenAsync(user.Id, refreshTokenStr, sessionId, rememberMe);

        var refreshExpiry = rememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(24);
        _tokenService.SetTokenInsideCookie("access_token", jwt, DateTime.UtcNow.AddMinutes(15));
        _tokenService.SetTokenInsideCookie("refresh_token", refreshTokenStr, refreshExpiry);

        _metrics.RecordLoginSuccess();
        await LogAuditEventAsync(user.Id, "USER_LOGIN_SUCCESS", $"User {user.Email} logged in successfully.");
        return new AuthResponse(user.Id, user.Email, user.FullName, user.AvatarUrl, roles, permissions, true, "ACTIVE", "DASHBOARD");
    }

    /// <summary>
    /// Authenticates a Google user using their ID Token.
    /// Performs auto-registration for new users and status promotion/verification on successful token validation.
    /// </summary>
    public async Task<AuthResponse?> LoginWithGoogleAsync(GoogleLoginRequest request)
    {
        try
        {
            // Hardened Clock Tolerance settings (5 minutes safety buffer) to handle potential clock skews.
            // In a production environment, server instances must run a localized Network Time Protocol (NTP)
            // daemon (such as chronyd or ntpd) to synchronize internal system clocks with standard global NTP servers
            // (e.g. pool.ntp.org), guaranteeing clock skew is minimized under 1 second.
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _envConfig.Auth.GoogleClientId },
                IssuedAtClockTolerance = TimeSpan.FromMinutes(5),
                ExpirationTimeClockTolerance = TimeSpan.FromMinutes(5)
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
            if (payload == null)
            {
                _metrics.RecordLoginFailed();
                throw new UnauthorizedAccessException("Google authentication failed.");
            }

            var email = payload.Email.Trim().Normalize(NormalizationForm.FormC).ToLowerInvariant();
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "USER");
                if (userRole == null)
                {
                    throw new InvalidOperationException("Default 'USER' role not found in the database.");
                }

                user = new User
                {
                    Email = email,
                    FullName = payload.Name ?? "Google User",
                    AvatarUrl = payload.Picture,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N")),
                    Status = UserStatus.ACTIVE,
                    EmailVerifiedAt = _timeProvider.GetUtcNow().UtcDateTime,
                    CreatedAt = _timeProvider.GetUtcNow().UtcDateTime,
                    UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime,
                    Roles = new List<Role> { userRole }
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await LogAuditEventAsync(user.Id, "USER_GOOGLE_REGISTER", $"New user {user.Email} registered via Google OAuth.");
            }
            else
            {
                if (_accountService.IsAccountDisabled(user))
                {
                    _metrics.RecordLoginFailed();
                    await LogAuditEventAsync(user.Id, "USER_LOGIN_FAILED_DISABLED", $"Disabled user account Google login attempt for {user.Email}.");
                    throw new UnauthorizedAccessException("Account is disabled.");
                }

                if (!string.IsNullOrEmpty(payload.Picture) && user.AvatarUrl != payload.Picture)
                {
                    user.AvatarUrl = payload.Picture;
                }
                if (!string.IsNullOrEmpty(payload.Name) && user.FullName != payload.Name && user.FullName == "Google User")
                {
                    user.FullName = payload.Name;
                }

                if (user.Status == UserStatus.EMAIL_VERIFY_PENDING)
                {
                    user.TransitionTo(UserStatus.ACTIVE);
                    user.EmailVerifiedAt = _timeProvider.GetUtcNow().UtcDateTime;
                }

                user.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;
                await _context.SaveChangesAsync();
            }

            var roles = await _identityRepository.GetUserRolesAsync(user.Id);
            var permissions = await _identityRepository.GetUserPermissionsAsync(user.Id);

            await CacheUserAuthDataAsync(user.Id, roles, permissions);

            var jwt = _tokenService.GenerateJwtToken(user, roles, permissions);
            var refreshTokenStr = _tokenService.GenerateRefreshToken();

            var sessionId = Guid.NewGuid();
            var rememberMe = true; // default to true for Google OAuth / Remembered sessions
            await SaveRefreshTokenAsync(user.Id, refreshTokenStr, sessionId, rememberMe);

            var refreshExpiry = DateTime.UtcNow.AddDays(7);
            _tokenService.SetTokenInsideCookie("access_token", jwt, DateTime.UtcNow.AddMinutes(15));
            _tokenService.SetTokenInsideCookie("refresh_token", refreshTokenStr, refreshExpiry);

            _metrics.RecordLoginSuccess();
            await LogAuditEventAsync(user.Id, "USER_GOOGLE_LOGIN_SUCCESS", $"User {user.Email} logged in successfully via Google OAuth.");

            return new AuthResponse(user.Id, user.Email, user.FullName, user.AvatarUrl, roles, permissions, true, "ACTIVE", "DASHBOARD");
        }
        catch (InvalidJwtException ex)
        {
            _metrics.RecordLoginFailed();
            _logger.LogWarning("Invalid Google ID Token provided: {Message}", ex.Message);
            throw new UnauthorizedAccessException("Invalid Google ID Token.");
        }
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
                storedToken.RevokedAt = _timeProvider.GetUtcNow();
                await _context.SaveChangesAsync();
            }
        }

        _tokenService.RemoveTokenFromCookie("access_token");
        _tokenService.RemoveTokenFromCookie("refresh_token");
    }

    /// <summary>
    /// Validates the refresh token cookie and issues a new set of tokens (Token Rotation).
    /// Prevents replay attacks by revoking the old token.
    /// Incorporates a 10-second concurrency grace window for multi-tab support and isolation bounds.
    /// </summary>
    public async Task<AuthResponse?> RefreshTokenAsync()
    {
        var refreshTokenStr = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshTokenStr)) return null;

        // Redis Distributed Lock to avoid concurrent rotation race conditions
        var lockKey = $"lock:token:rotate:{refreshTokenStr}";
        var lockValue = Guid.NewGuid().ToString("N");
        var acquired = await _cacheService.AcquireLockAsync(lockKey, lockValue, TimeSpan.FromSeconds(10));
        var maskedToken = refreshTokenStr.Length > 8 ? $"{refreshTokenStr[..4]}...{refreshTokenStr[^4..]}" : "***MASKED***";
        if (!acquired)
        {
            _logger.LogWarning("Token rotation request rejected due to lock contention: {Token}", maskedToken);
            throw new AuthException(AuthErrorCodes.InvalidToken, "Concurrent token rotation detected.");
        }

        try
        {
            var storedToken = await _context.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == refreshTokenStr);

            var validationResult = ValidateRefreshToken(storedToken);

            if (validationResult == RefreshTokenValidationResult.Invalid || validationResult == RefreshTokenValidationResult.Expired)
            {
                return null;
            }

            if (validationResult == RefreshTokenValidationResult.Reused)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var currentIp = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var currentUa = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";

                // Staged Revocation / Compromise Isolation: Revoke all tokens belonging to the compromised Session ID
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await RevokeSessionChainAsync(storedToken!.SessionId);
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                // Structured security log
                _logger.LogWarning("SECURITY ALERT: [Event=TOKEN_REUSE_DETECTED] [SessionId={SessionId}] [UserId={UserId}] [IpAddress={IpAddress}] [UserAgent={UserAgent}]",
                    storedToken.SessionId, storedToken.UserId, currentIp, currentUa);

                await LogAuditEventAsync(storedToken.UserId, "TOKEN_THEFT_DETECTED",
                    $"Refresh token reuse/theft detected for token {maskedToken}. Session {storedToken.SessionId} isolated and revoked. IP: {currentIp}, UA: {currentUa}");

                throw new AuthException(AuthErrorCodes.InvalidToken, "Token reuse detected.");
            }

            if (validationResult == RefreshTokenValidationResult.WithinGracePeriod)
            {
                // Safe concurrency: Return the active replacement token that was already generated
                var activeReplacement = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.Id == storedToken!.ReplacedByTokenId);

                if (activeReplacement != null)
                {
                    _logger.LogInformation("Safe concurrent refresh race handled. SessionId: {SessionId}.", storedToken!.SessionId);

                    var user = storedToken.User;
                    var roles = await _identityRepository.GetUserRolesAsync(user.Id);
                    var permissions = await _identityRepository.GetUserPermissionsAsync(user.Id);

                    var jwt = _tokenService.GenerateJwtToken(user, roles, permissions);

                    // Re-set cookies for the active rotated token
                    var refreshExpiry = activeReplacement.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(24);
                    _tokenService.SetTokenInsideCookie("access_token", jwt, DateTime.UtcNow.AddMinutes(15));
                    _tokenService.SetTokenInsideCookie("refresh_token", activeReplacement.Token, refreshExpiry);

                    return new AuthResponse(user.Id, user.Email, user.FullName, user.AvatarUrl, roles, permissions,
                        user.Status == UserStatus.ACTIVE, user.Status.ToString(),
                        user.Status == UserStatus.EMAIL_VERIFY_PENDING ? "VERIFY_EMAIL" : "DASHBOARD");
                }
            }

            // Normal path: validationResult == RefreshTokenValidationResult.Valid
            var oldUser = storedToken!.User;
            var oldRoles = await _identityRepository.GetUserRolesAsync(oldUser.Id);
            var oldPermissions = await _identityRepository.GetUserPermissionsAsync(oldUser.Id);

            // Compare User-Agents for hijack warnings
            var currentUserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            if (!string.IsNullOrWhiteSpace(storedToken.UserAgent) &&
                !string.Equals(storedToken.UserAgent, currentUserAgent, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("SECURITY WARNING: User-Agent changed during token refresh for Session {SessionId}. Original: '{OriginalUA}', Current: '{CurrentUA}'",
                    storedToken.SessionId, storedToken.UserAgent, currentUserAgent);
            }

            var newRefreshTokenStr = _tokenService.GenerateRefreshToken();

            using var rotationTx = await _context.Database.BeginTransactionAsync();
            RefreshToken newRefreshToken;
            try
            {
                newRefreshToken = await RotateRefreshTokenAsync(storedToken, newRefreshTokenStr);
                await rotationTx.CommitAsync();
            }
            catch
            {
                await rotationTx.RollbackAsync();
                throw;
            }

            var newJwt = _tokenService.GenerateJwtToken(oldUser, oldRoles, oldPermissions);

            var newRefreshExpiry = newRefreshToken.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(24);
            _tokenService.SetTokenInsideCookie("access_token", newJwt, DateTime.UtcNow.AddMinutes(15));
            _tokenService.SetTokenInsideCookie("refresh_token", newRefreshTokenStr, newRefreshExpiry);

            await LogAuditEventAsync(oldUser.Id, "TOKEN_ROTATED", $"Token rotated successfully. New token issued for Session {storedToken.SessionId}.");

            return new AuthResponse(oldUser.Id, oldUser.Email, oldUser.FullName, oldUser.AvatarUrl, oldRoles, oldPermissions,
                oldUser.Status == UserStatus.ACTIVE, oldUser.Status.ToString(),
                oldUser.Status == UserStatus.EMAIL_VERIFY_PENDING ? "VERIFY_EMAIL" : "DASHBOARD");
        }
        finally
        {
            await _cacheService.ReleaseLockAsync(lockKey, lockValue);
        }
    }

    /// <summary>
    /// Validates a refresh token. If it is already revoked, checks if it is within
    /// the 10-second multi-tab safe concurrency grace period to avoid false positive security logouts.
    /// </summary>
    private RefreshTokenValidationResult ValidateRefreshToken(RefreshToken? storedToken)
    {
        if (storedToken == null)
        {
            return RefreshTokenValidationResult.Invalid;
        }

        var now = _timeProvider.GetUtcNow();

        if (storedToken.IsRevoked)
        {
            // Concurrent safe refresh window of 10 seconds for multi-tab scenarios
            if (storedToken.RevokedAt.HasValue && storedToken.RevokedAt.Value.AddSeconds(10) > now)
            {
                return RefreshTokenValidationResult.WithinGracePeriod;
            }

            return RefreshTokenValidationResult.Reused;
        }

        if (storedToken.IsExpired)
        {
            return RefreshTokenValidationResult.Expired;
        }

        return RefreshTokenValidationResult.Valid;
    }

    private enum RefreshTokenValidationResult
    {
        Invalid,
        Expired,
        Valid,
        Reused,
        WithinGracePeriod
    }

    private async Task<RefreshToken> RotateRefreshTokenAsync(RefreshToken oldToken, string newRefreshTokenStr)
    {
        var now = _timeProvider.GetUtcNow();
        var expiration = oldToken.RememberMe ? TimeSpan.FromDays(7) : TimeSpan.FromHours(24);

        // Revoke the old token and tie it to the new one
        oldToken.RevokedAt = now;
        oldToken.ReplacedByToken = newRefreshTokenStr;

        // Generate the new token sharing the same sessionId and rememberMe
        var httpContext = _httpContextAccessor.HttpContext;
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();

        var newRefreshToken = new RefreshToken
        {
            UserId = oldToken.UserId,
            Token = newRefreshTokenStr,
            SessionId = oldToken.SessionId,
            RememberMe = oldToken.RememberMe,
            ExpiresAt = now.Add(expiration),
            UserAgent = string.IsNullOrWhiteSpace(userAgent) ? null : (userAgent.Length > 500 ? userAgent[..500] : userAgent),
            IpAddress = string.IsNullOrWhiteSpace(ipAddress) ? null : (ipAddress.Length > 45 ? ipAddress[..45] : ipAddress)
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        // Update the reference of the old token's ReplacedByTokenId
        oldToken.ReplacedByTokenId = newRefreshToken.Id;
        await _context.SaveChangesAsync();

        return newRefreshToken;
    }

    private async Task RevokeSessionChainAsync(Guid sessionId)
    {
        var now = _timeProvider.GetUtcNow();
        var activeTokens = await _context.RefreshTokens
            .Where(t => t.SessionId == sessionId && t.RevokedAt == null)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.RevokedAt = now;
        }

        await _context.SaveChangesAsync();
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

        return new UserProfileResponse(user.Id, user.Email, user.FullName, user.AvatarUrl, roles, permissions, user.Status == UserStatus.ACTIVE, user.Status.ToString(), user.Status == UserStatus.EMAIL_VERIFY_PENDING ? "VERIFY_EMAIL" : "DASHBOARD");
    }

    /// <summary>
    /// Registers a new user inside an EF Core transactional consistency boundary.
    /// Normalizes emails, handles idempotency retries, enforces password constraints,
    /// and schedules email verification via Outbox pattern.
    /// </summary>
    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, string userAgent, string ipAddress, CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        _logger.LogInformation("[CorrelationID: {CorrelationId}] Handling user registration request for {Email}.", correlationId, request.Email);

        var normalizedEmail = request.Email.Trim().Normalize(NormalizationForm.FormC).ToLowerInvariant();

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (existingUser != null)
        {
            // Idempotency: If pending verification, rotate verification token and send a new link
            if (existingUser.Status == UserStatus.EMAIL_VERIFY_PENDING)
            {
                _logger.LogWarning("[CorrelationID: {CorrelationId}] User registration is duplicate but pending email verification. Rotating verification token.", correlationId);
                var resendRequest = new ResendVerificationRequest(request.Email);
                await ResendVerificationEmailAsync(resendRequest, cancellationToken);
                return RegisterResponseFactory.Create(UserStatus.EMAIL_VERIFY_PENDING);
            }

            _logger.LogWarning("[CorrelationID: {CorrelationId}] User registration is duplicate and status is {Status}. Throwing DuplicateEmailException.", correlationId, existingUser.Status);
            throw new DuplicateEmailException("This email is already in use.");
        }

        var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "USER", cancellationToken);
        if (userRole == null)
        {
            throw new InvalidOperationException("Default 'USER' role not found in the database.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Transaction consistency boundary
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var newUser = new User
            {
                Email = normalizedEmail,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                Status = UserStatus.EMAIL_VERIFY_PENDING,
                CreatedAt = _timeProvider.GetUtcNow().UtcDateTime,
                UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime,
                Roles = new List<Role> { userRole }
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(cancellationToken);

            // Generate secure URL-safe verification token
            var plainToken = EmailTokenGenerator.GenerateSecureToken();

            // Hash the token for database storage
            using var sha256 = SHA256.Create();
            var hashedToken = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(plainToken))).ToLowerInvariant();

            var verificationToken = new VerificationToken
            {
                UserId = newUser.Id,
                TokenHash = hashedToken,
                ExpiresAt = _timeProvider.GetUtcNow().AddMinutes(5),
                CreatedAt = _timeProvider.GetUtcNow()
            };

            _context.VerificationTokens.Add(verificationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Outbox Pattern enrollment: insert verification email envelope into outbox inside transaction
            var verificationLink = _envConfig.Auth.VerifyEmailUrlFormat.Replace("{token}", plainToken);
            
            // Validate the redirect domain
            var uri = new Uri(verificationLink);
            var trusted = _envConfig.Auth.TrustedDomains.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (!trusted.Any(t => uri.Host.Equals(t, StringComparison.OrdinalIgnoreCase)))
            {
                throw new AuthException(AuthErrorCodes.UntrustedRedirect, $"Verification link uses untrusted domain '{uri.Host}'.");
            }

            var payloadObj = new
            {
                Email = newUser.Email,
                FullName = newUser.FullName,
                Link = verificationLink,
                CorrelationId = correlationId
            };

            var outboxMessage = new OutboxMessage
            {
                Type = "EmailVerification",
                Payload = System.Text.Json.JsonSerializer.Serialize(payloadObj),
                CreatedAt = _timeProvider.GetUtcNow()
            };

            _context.OutboxMessages.Add(outboxMessage);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            await LogAuditEventAsync(newUser.Id, "USER_REGISTERED", $"User account {newUser.Email} registered successfully.");

            _logger.LogInformation("[CorrelationID: {CorrelationId}] User {UserId} registered successfully, outbox message enqueued.", correlationId, newUser.Id);
            _metrics.RecordRegistration();
            return RegisterResponseFactory.Success();
        }
        catch (DuplicateEmailException ex)
        {
            _logger.LogWarning(ex, "[CorrelationID: {CorrelationId}] Database-level unique constraint violation caught during concurrent registration insert. Falling back to duplicate registration logic.", correlationId);
            await transaction.RollbackAsync(cancellationToken);

            // Fetch the existing user that just got created in the concurrent transaction
            var concurrentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
            if (concurrentUser != null)
            {
                if (concurrentUser.Status == UserStatus.EMAIL_VERIFY_PENDING)
                {
                    var resendRequest = new ResendVerificationRequest(request.Email);
                    await ResendVerificationEmailAsync(resendRequest, cancellationToken);
                }
                return RegisterResponseFactory.Create(concurrentUser.Status);
            }
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CorrelationID: {CorrelationId}] Failed to complete user registration transaction. Rolling back.", correlationId);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Confirms email ownership and activates pending accounts.
    /// </summary>
    public async Task<AuthResponse?> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        _logger.LogInformation("[CorrelationID: {CorrelationId}] Processing email verification request.", correlationId);

        using var sha256 = SHA256.Create();
        var hashedToken = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Token))).ToLowerInvariant();

        var tokenEntity = await _context.VerificationTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hashedToken, cancellationToken);

        if (tokenEntity == null)
        {
            _logger.LogWarning("[CorrelationID: {CorrelationId}] Verification failed: invalid token hash.", correlationId);
            throw new AuthException(AuthErrorCodes.InvalidToken, "The verification token is invalid.");
        }

        if (tokenEntity.IsConsumed)
        {
            _logger.LogWarning("[CorrelationID: {CorrelationId}] Verification failed: token was already consumed.", correlationId);
            throw new AuthException(AuthErrorCodes.TokenAlreadyConsumed, "This verification token has already been used.");
        }

        if (tokenEntity.IsExpired)
        {
            _logger.LogWarning("[CorrelationID: {CorrelationId}] Verification failed: token is expired.", correlationId);
            throw new AuthException(AuthErrorCodes.ExpiredToken, "This verification token has expired.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            tokenEntity.ConsumedAt = _timeProvider.GetUtcNow();
            
            var user = tokenEntity.User;
            user.TransitionTo(UserStatus.ACTIVE);
            user.EmailVerifiedAt = _timeProvider.GetUtcNow().UtcDateTime;
            user.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;

            await _context.SaveChangesAsync(cancellationToken);

            // Queue onboarding welcome email via Outbox Pattern
            var payloadObj = new
            {
                Email = user.Email,
                FullName = user.FullName,
                CorrelationId = correlationId
            };

            var outboxMessage = new OutboxMessage
            {
                Type = "WelcomeNotice",
                Payload = System.Text.Json.JsonSerializer.Serialize(payloadObj),
                CreatedAt = _timeProvider.GetUtcNow()
            };

            _context.OutboxMessages.Add(outboxMessage);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            await LogAuditEventAsync(user.Id, "USER_EMAIL_VERIFIED", $"User email {user.Email} successfully verified.");

            var roles = await _identityRepository.GetUserRolesAsync(user.Id);
            var permissions = await _identityRepository.GetUserPermissionsAsync(user.Id);

            await CacheUserAuthDataAsync(user.Id, roles, permissions);

            var jwt = _tokenService.GenerateJwtToken(user, roles, permissions);
            var refreshTokenStr = _tokenService.GenerateRefreshToken();

            var sessionId = Guid.NewGuid();
            var rememberMe = false; // default to false
            await SaveRefreshTokenAsync(user.Id, refreshTokenStr, sessionId, rememberMe);

            var refreshExpiry = DateTime.UtcNow.AddHours(24);
            _tokenService.SetTokenInsideCookie("access_token", jwt, DateTime.UtcNow.AddMinutes(15));
            _tokenService.SetTokenInsideCookie("refresh_token", refreshTokenStr, refreshExpiry);

            _logger.LogInformation("[CorrelationID: {CorrelationId}] Email successfully verified for user {UserId} and auto-logged in.", correlationId, user.Id);
            _metrics.RecordVerification();
            
            return new AuthResponse(user.Id, user.Email, user.FullName, user.AvatarUrl, roles, permissions, true, "ACTIVE", "DASHBOARD");
        }

        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "[CorrelationID: {CorrelationId}] Concurrency conflict detected during verification.", correlationId);
            await transaction.RollbackAsync(cancellationToken);
            throw new AuthException(AuthErrorCodes.InvalidToken, "A concurrency conflict occurred. Please retry your request.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CorrelationID: {CorrelationId}] Verification transaction failed. Rolling back.", correlationId);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Resends a verification email, applying strict cooldown rules and rotated token keys.
    /// </summary>
    public async Task<bool> ResendVerificationEmailAsync(ResendVerificationRequest request, CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        _logger.LogInformation("[CorrelationID: {CorrelationId}] Processing resend verification request for {Email}.", correlationId, request.Email);

        var normalizedEmail = request.Email.Trim().Normalize(NormalizationForm.FormC).ToLowerInvariant();

        // Check if cooldown in Redis is active
        var cooldownKey = $"cooldown:verify-email:{normalizedEmail}";
        var isCooldown = await _cacheService.GetAsync<string>(cooldownKey);
        if (isCooldown != null)
        {
            _logger.LogWarning("[CorrelationID: {CorrelationId}] Cooldown active for {Email}.", correlationId, normalizedEmail);
            throw new AuthException(AuthErrorCodes.CooldownActive, "Please wait before requesting another verification email.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        
        // Strict Account Enumeration Prevention: if user does not exist, return generic success
        if (user == null)
        {
            _logger.LogInformation("[CorrelationID: {CorrelationId}] Email does not exist. Returning safe generic success response.", correlationId);
            return true;
        }

        // If user is already active, return success silently (enumeration prevention)
        if (user.Status == UserStatus.ACTIVE)
        {
            _logger.LogInformation("[CorrelationID: {CorrelationId}] User is already verified/active. Returning safe generic success response.", correlationId);
            return true;
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Token Rotation: Invalidate all existing pending verification tokens for this user
            var oldTokens = await _context.VerificationTokens
                .Where(vt => vt.UserId == user.Id && vt.ConsumedAt == null && vt.ExpiresAt > _timeProvider.GetUtcNow())
                .ToListAsync(cancellationToken);

            foreach (var ot in oldTokens)
            {
                ot.ConsumedAt = _timeProvider.GetUtcNow();
            }

            // Generate secure URL-safe verification token
            var plainToken = EmailTokenGenerator.GenerateSecureToken();

            // Hash the token for database storage
            using var sha256 = SHA256.Create();
            var hashedToken = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(plainToken))).ToLowerInvariant();

            var verificationToken = new VerificationToken
            {
                UserId = user.Id,
                TokenHash = hashedToken,
                ExpiresAt = _timeProvider.GetUtcNow().AddMinutes(5),
                CreatedAt = _timeProvider.GetUtcNow()
            };

            _context.VerificationTokens.Add(verificationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Outbox Pattern enrollment
            var verificationLink = _envConfig.Auth.VerifyEmailUrlFormat.Replace("{token}", plainToken);

            // Validate redirect domain
            var uri = new Uri(verificationLink);
            var trusted = _envConfig.Auth.TrustedDomains.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (!trusted.Any(t => uri.Host.Equals(t, StringComparison.OrdinalIgnoreCase)))
            {
                throw new AuthException(AuthErrorCodes.UntrustedRedirect, $"Verification link uses untrusted domain '{uri.Host}'.");
            }

            var payloadObj = new
            {
                Email = user.Email,
                FullName = user.FullName,
                Link = verificationLink,
                CorrelationId = correlationId
            };

            var outboxMessage = new OutboxMessage
            {
                Type = "EmailVerification",
                Payload = System.Text.Json.JsonSerializer.Serialize(payloadObj),
                CreatedAt = _timeProvider.GetUtcNow()
            };

            _context.OutboxMessages.Add(outboxMessage);
            await _context.SaveChangesAsync(cancellationToken);

            // Set 1-minute rate limiting cooldown in Redis
            await _cacheService.SetAsync(cooldownKey, "active", TimeSpan.FromMinutes(1));

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("[CorrelationID: {CorrelationId}] Verification email successfully re-enqueued for user {UserId}.", correlationId, user.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CorrelationID: {CorrelationId}] Resend verification transaction failed. Rolling back.", correlationId);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Creates a password recovery token and triggers transactional outbox dispatching.
    /// Enforces enumeration prevention.
    /// </summary>
    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        _logger.LogInformation("[CorrelationID: {CorrelationId}] Processing forgot password request for {Email}.", correlationId, request.Email);

        var normalizedEmail = request.Email.Trim().Normalize(NormalizationForm.FormC).ToLowerInvariant();

        // Enforce cooldown to prevent spamming
        var cooldownKey = $"cooldown:forgot-password:{normalizedEmail}";
        var isCooldown = await _cacheService.GetAsync<string>(cooldownKey);
        if (isCooldown != null)
        {
            _logger.LogWarning("[CorrelationID: {CorrelationId}] Forgot password cooldown active for {Email}.", correlationId, normalizedEmail);
            throw new AuthException(AuthErrorCodes.CooldownActive, "Please wait before requesting another recovery email.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

        // Strict Account Enumeration Prevention: always return generic success
        if (user == null)
        {
            _logger.LogInformation("[CorrelationID: {CorrelationId}] Forgot password request for non-existent email. Returning safe generic success.", correlationId);
            return true;
        }

        if (user.Status != UserStatus.ACTIVE && user.Status != UserStatus.EMAIL_VERIFY_PENDING)
        {
            _logger.LogWarning("[CorrelationID: {CorrelationId}] User status is inactive/suspended/banned/deleted. Returning safe generic success.", correlationId);
            return true;
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Token Rotation: Invalidate all existing password reset tokens for this user
            var oldTokens = await _context.ResetPasswordTokens
                .Where(rt => rt.UserId == user.Id && rt.ConsumedAt == null && rt.ExpiresAt > _timeProvider.GetUtcNow())
                .ToListAsync(cancellationToken);

            foreach (var ot in oldTokens)
            {
                ot.ConsumedAt = _timeProvider.GetUtcNow();
            }

            // Generate secure URL-safe password reset token
            var plainToken = EmailTokenGenerator.GenerateSecureToken();

            // Hash the token for database storage
            using var sha256 = SHA256.Create();
            var hashedToken = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(plainToken))).ToLowerInvariant();

            var resetToken = new ResetPasswordToken
            {
                UserId = user.Id,
                TokenHash = hashedToken,
                ExpiresAt = _timeProvider.GetUtcNow().AddMinutes(5),
                CreatedAt = _timeProvider.GetUtcNow()
            };

            _context.ResetPasswordTokens.Add(resetToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Outbox Pattern enrollment
            var resetLink = _envConfig.Auth.ResetPasswordUrlFormat.Replace("{token}", plainToken);

            // Validate redirect domain
            var uri = new Uri(resetLink);
            var trusted = _envConfig.Auth.TrustedDomains.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (!trusted.Any(t => uri.Host.Equals(t, StringComparison.OrdinalIgnoreCase)))
            {
                throw new AuthException(AuthErrorCodes.UntrustedRedirect, $"Password reset link uses untrusted domain '{uri.Host}'.");
            }

            var payloadObj = new
            {
                Email = user.Email,
                FullName = user.FullName,
                Link = resetLink,
                CorrelationId = correlationId
            };

            var outboxMessage = new OutboxMessage
            {
                Type = "PasswordReset",
                Payload = System.Text.Json.JsonSerializer.Serialize(payloadObj),
                CreatedAt = _timeProvider.GetUtcNow()
            };

            _context.OutboxMessages.Add(outboxMessage);
            await _context.SaveChangesAsync(cancellationToken);

            // Set 1-minute rate limiting cooldown in Redis
            await _cacheService.SetAsync(cooldownKey, "active", TimeSpan.FromMinutes(1));

            await transaction.CommitAsync(cancellationToken);

            await LogAuditEventAsync(user.Id, "USER_PASSWORD_RECOVERY_REQUESTED", $"Password recovery token requested for {user.Email}.");

            _logger.LogInformation("[CorrelationID: {CorrelationId}] Password reset token generated and outbox message enqueued for user {UserId}.", correlationId, user.Id);
            return true;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "[CorrelationID: {CorrelationId}] Forgot password transaction failed. Rolling back.", correlationId);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Resets the user's password, consumes the token, and globally revokes all active refresh sessions.
    /// </summary>
    public async Task<AuthResponse?> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        _logger.LogInformation("[CorrelationID: {CorrelationId}] Processing password reset request.", correlationId);

        using var sha256 = SHA256.Create();
        var hashedToken = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Token))).ToLowerInvariant();

        var tokenEntity = await _context.ResetPasswordTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hashedToken, cancellationToken);

        if (tokenEntity == null)
        {
            _logger.LogWarning("[CorrelationID: {CorrelationId}] Reset failed: invalid token.", correlationId);
            throw new AuthException(AuthErrorCodes.InvalidToken, "The password reset token is invalid.");
        }

        if (tokenEntity.IsConsumed)
        {
            _logger.LogWarning("[CorrelationID: {CorrelationId}] Reset failed: token already consumed.", correlationId);
            throw new AuthException(AuthErrorCodes.TokenAlreadyConsumed, "This password reset token has already been used.");
        }

        if (tokenEntity.IsExpired)
        {
            _logger.LogWarning("[CorrelationID: {CorrelationId}] Reset failed: token expired.", correlationId);
            throw new AuthException(AuthErrorCodes.ExpiredToken, "This password reset token has expired.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            tokenEntity.ConsumedAt = _timeProvider.GetUtcNow();

            var user = tokenEntity.User;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;

            // Session Revocation Strategy: Revoke all active refresh sessions to force global logout across all devices
            var activeSessions = await _context.RefreshTokens
                .Where(t => t.UserId == user.Id && t.RevokedAt == null && t.ExpiresAt > _timeProvider.GetUtcNow())
                .ToListAsync(cancellationToken);

            foreach (var session in activeSessions)
            {
                session.RevokedAt = _timeProvider.GetUtcNow();
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await LogAuditEventAsync(user.Id, "USER_PASSWORD_RESET_SUCCESS", $"Password reset successfully. Sessions globally revoked for {user.Email}.");

            var roles = await _identityRepository.GetUserRolesAsync(user.Id);
            var permissions = await _identityRepository.GetUserPermissionsAsync(user.Id);

            await CacheUserAuthDataAsync(user.Id, roles, permissions);

            var jwt = _tokenService.GenerateJwtToken(user, roles, permissions);
            var refreshTokenStr = _tokenService.GenerateRefreshToken();

            var sessionId = Guid.NewGuid();
            var rememberMe = false; // default to false
            await SaveRefreshTokenAsync(user.Id, refreshTokenStr, sessionId, rememberMe);

            var refreshExpiry = DateTime.UtcNow.AddHours(24);
            _tokenService.SetTokenInsideCookie("access_token", jwt, DateTime.UtcNow.AddMinutes(15));
            _tokenService.SetTokenInsideCookie("refresh_token", refreshTokenStr, refreshExpiry);

            _logger.LogInformation("[CorrelationID: {CorrelationId}] Password reset successfully and user {UserId} auto-logged in.", correlationId, user.Id);
            _metrics.RecordPasswordReset();
            
            return new AuthResponse(user.Id, user.Email, user.FullName, user.AvatarUrl, roles, permissions, true, "ACTIVE", "DASHBOARD");
        }

        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "[CorrelationID: {CorrelationId}] Concurrency conflict detected during password reset.", correlationId);
            await transaction.RollbackAsync(cancellationToken);
            throw new AuthException(AuthErrorCodes.InvalidToken, "A concurrency conflict occurred. Please retry your request.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[CorrelationID: {CorrelationId}] Password reset transaction failed. Rolling back.", correlationId);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
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

    private async Task SaveRefreshTokenAsync(Guid userId, string tokenStr, Guid sessionId, bool rememberMe, Guid? replacedByTokenId = null)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();

        var expiration = rememberMe ? TimeSpan.FromDays(7) : TimeSpan.FromHours(24);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = tokenStr,
            SessionId = sessionId,
            RememberMe = rememberMe,
            ReplacedByTokenId = replacedByTokenId,
            ExpiresAt = _timeProvider.GetUtcNow().Add(expiration),
            UserAgent = string.IsNullOrWhiteSpace(userAgent) ? null : (userAgent.Length > 500 ? userAgent[..500] : userAgent),
            IpAddress = string.IsNullOrWhiteSpace(ipAddress) ? null : (ipAddress.Length > 45 ? ipAddress[..45] : ipAddress)
        };
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteMeAsync()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return false;

        var userId = Guid.Parse(userIdClaim.Value);
        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.DeletedAt != null) return false;

        // Perform soft deletion
        user.DeletedAt = _timeProvider.GetUtcNow();
        user.Status = UserStatus.DELETED;

        // Revoke all refresh tokens
        var refreshTokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId)
            .ToListAsync();
        foreach (var token in refreshTokens)
        {
            token.RevokedAt = _timeProvider.GetUtcNow();
        }

        // Revoke verification and reset password tokens
        var verificationTokens = await _context.VerificationTokens
            .Where(t => t.UserId == userId && t.ConsumedAt == null)
            .ToListAsync();
        foreach (var vt in verificationTokens)
        {
            vt.ConsumedAt = _timeProvider.GetUtcNow();
        }

        var resetTokens = await _context.ResetPasswordTokens
            .Where(t => t.UserId == userId && t.ConsumedAt == null)
            .ToListAsync();
        foreach (var rt in resetTokens)
        {
            rt.ConsumedAt = _timeProvider.GetUtcNow();
        }

        // Clean up cache
        await _cacheService.RemoveAsync($"auth:user:{userId}:roles");
        await _cacheService.RemoveAsync($"auth:user:{userId}:permissions");

        // Structured security audit log
        _logger.LogWarning("SECURITY AUDIT: User account {UserId} ({Email}) was soft-deleted.", userId, user.Email);
        await LogAuditEventAsync(userId, "USER_DELETED", $"User account {user.Email} was soft-deleted.");

        await _context.SaveChangesAsync();

        // Remove auth cookies from HTTP context
        _tokenService.RemoveTokenFromCookie("access_token");
        _tokenService.RemoveTokenFromCookie("refresh_token");

        return true;
    }

    private async Task LogAuditEventAsync(Guid? userId, string eventType, string description)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();

        var auditLog = new AuditLog
        {
            UserId = userId,
            EventType = eventType,
            Description = description,
            UserAgent = string.IsNullOrWhiteSpace(userAgent) ? null : (userAgent.Length > 500 ? userAgent[..500] : userAgent),
            IpAddress = string.IsNullOrWhiteSpace(ipAddress) ? null : (ipAddress.Length > 45 ? ipAddress[..45] : ipAddress),
            CreatedAt = _timeProvider.GetUtcNow()
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}

