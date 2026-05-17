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
using TripGenie.API.Application.DTOs;
using TripGenie.API.Application.Exceptions;
using TripGenie.API.Application.Interfaces;
using TripGenie.API.Core.Entities;
using TripGenie.API.Infrastructure.Configuration;
using TripGenie.API.Infrastructure.Diagnostics;
using TripGenie.API.Infrastructure.Persistence;
using TripGenie.API.Infrastructure.Security;

namespace TripGenie.API.Application.Services;

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

        await CacheUserAuthDataAsync(user.Id, roles, permissions);

        var jwt = _tokenService.GenerateJwtToken(user, roles, permissions);
        var refreshTokenStr = _tokenService.GenerateRefreshToken();

        await SaveRefreshTokenAsync(user.Id, refreshTokenStr);

        _tokenService.SetTokenInsideCookie("access_token", jwt);
        _tokenService.SetTokenInsideCookie("refresh_token", refreshTokenStr);

        _metrics.RecordLoginSuccess();
        await LogAuditEventAsync(user.Id, "USER_LOGIN_SUCCESS", $"User {user.Email} logged in successfully.");
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
    /// </summary>
    public async Task<AuthResponse?> RefreshTokenAsync()
    {
        var refreshTokenStr = _httpContextAccessor.HttpContext?.Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshTokenStr)) return null;

        // Redis Distributed Lock to avoid concurrent rotation race conditions
        var lockKey = $"lock:token:rotate:{refreshTokenStr}";
        var lockValue = Guid.NewGuid().ToString("N");
        var acquired = await _cacheService.AcquireLockAsync(lockKey, lockValue, TimeSpan.FromSeconds(10));
        if (!acquired)
        {
            _logger.LogWarning("Token rotation request rejected due to lock contention: {Token}", refreshTokenStr);
            throw new AuthException(AuthErrorCodes.InvalidToken, "Concurrent token rotation detected.");
        }

        try
        {
            var storedToken = await _context.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == refreshTokenStr);

            if (storedToken != null && storedToken.IsRevoked)
            {
                // Theft detection! Revoke all tokens for this user
                var userTokens = await _context.RefreshTokens
                    .Where(t => t.UserId == storedToken.UserId)
                    .ToListAsync();
                foreach (var token in userTokens)
                {
                    token.RevokedAt = _timeProvider.GetUtcNow();
                }
                await _context.SaveChangesAsync();
                _logger.LogWarning("SECURITY ALERT: Refresh token reuse detected for User {UserId}! Revoked all tokens.", storedToken.UserId);
                await LogAuditEventAsync(storedToken.UserId, "TOKEN_THEFT_DETECTED", $"Refresh token reuse/theft detected for token {refreshTokenStr}. All sessions revoked.");
                throw new AuthException(AuthErrorCodes.InvalidToken, "Token reuse detected.");
            }

            if (storedToken == null || !storedToken.IsActive) return null;

            var user = storedToken.User;
            var roles = await _identityRepository.GetUserRolesAsync(user.Id);
            var permissions = await _identityRepository.GetUserPermissionsAsync(user.Id);

            // Rotate Refresh Token
            var newRefreshTokenStr = _tokenService.GenerateRefreshToken();
            storedToken.RevokedAt = _timeProvider.GetUtcNow();
            storedToken.ReplacedByToken = newRefreshTokenStr;

            await SaveRefreshTokenAsync(user.Id, newRefreshTokenStr);

            var jwt = _tokenService.GenerateJwtToken(user, roles, permissions);

            _tokenService.SetTokenInsideCookie("access_token", jwt);
            _tokenService.SetTokenInsideCookie("refresh_token", newRefreshTokenStr);

            await LogAuditEventAsync(user.Id, "TOKEN_ROTATED", $"Token rotated successfully. New token issued.");

            return new AuthResponse(user.Id, user.Email, roles, permissions);
        }
        finally
        {
            await _cacheService.ReleaseLockAsync(lockKey, lockValue);
        }
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

    /// <summary>
    /// Registers a new user inside an EF Core transactional consistency boundary.
    /// Normalizes emails, handles idempotency retries, enforces password constraints,
    /// and schedules email verification via Outbox pattern.
    /// </summary>
    public async Task<bool> RegisterAsync(RegisterRequest request, string userAgent, string ipAddress, CancellationToken cancellationToken = default)
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
                return await ResendVerificationEmailAsync(resendRequest, cancellationToken);
            }
            // Idempotency: If already active, return success silently (enumeration prevention and retry friendly)
            if (existingUser.Status == UserStatus.ACTIVE)
            {
                _logger.LogInformation("[CorrelationID: {CorrelationId}] User registration is duplicate and already active. Returning silent idempotent success.", correlationId);
                return true;
            }

            throw new AuthException(AuthErrorCodes.EmailAlreadyExists, "The email address is already registered.");
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
                RoleId = userRole.Id,
                Email = normalizedEmail,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                Status = UserStatus.EMAIL_VERIFY_PENDING,
                CreatedAt = _timeProvider.GetUtcNow().UtcDateTime,
                UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime
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
                ExpiresAt = _timeProvider.GetUtcNow().AddHours(_envConfig.Auth.VerificationTokenDurationInHours),
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
            return true;
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
    public async Task<bool> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
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
            user.Status = UserStatus.ACTIVE;
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

            _logger.LogInformation("[CorrelationID: {CorrelationId}] Email successfully verified for user {UserId}.", correlationId, user.Id);
            _metrics.RecordVerification();
            return true;
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
                ExpiresAt = _timeProvider.GetUtcNow().AddHours(_envConfig.Auth.VerificationTokenDurationInHours),
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
                ExpiresAt = _timeProvider.GetUtcNow().AddMinutes(_envConfig.Auth.ResetPasswordTokenDurationInMinutes),
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
    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
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

            _logger.LogInformation("[CorrelationID: {CorrelationId}] Password reset successfully. Session refresh tokens globally revoked for user {UserId}.", correlationId, user.Id);
            _metrics.RecordPasswordReset();
            return true;
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

    private async Task SaveRefreshTokenAsync(Guid userId, string tokenStr)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = tokenStr,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
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

