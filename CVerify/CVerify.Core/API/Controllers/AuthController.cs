using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using CVerify.API.Application.DTOs;
using CVerify.API.Application.Exceptions;
using CVerify.API.Application.Interfaces;

namespace CVerify.API.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)
        {
            return Unauthorized(new { code = AuthErrorCodes.InvalidCredentials, message = "Invalid email or password" });
        }

        return Ok(response);
    }

    [HttpPost("google")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var response = await _authService.LoginWithGoogleAsync(request);
            if (response == null)
            {
                return Unauthorized(new { code = AuthErrorCodes.InvalidCredentials, message = "Google authentication failed" });
            }

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { code = AuthErrorCodes.InvalidCredentials, message = ex.Message });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken()
    {
        var response = await _authService.RefreshTokenAsync();
        if (response == null)
        {
            return Unauthorized(new { code = AuthErrorCodes.Unauthorized, message = "Invalid refresh token" });
        }

        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe()
    {
        var response = await _authService.GetMeAsync();
        if (response == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [EnableRateLimiting("RegisterLimit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResponse))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var userAgent = Request.Headers["User-Agent"].ToString();
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        var result = await _authService.RegisterAsync(request, userAgent, ipAddress, cancellationToken);
        return Ok(result);
    }

    [HttpPost("verify-email")]
    [AllowAnonymous]
    [EnableRateLimiting("VerifyEmailLimit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.VerifyEmailAsync(request, cancellationToken);
        if (result != null)
        {
            return Ok(result);
        }

        return BadRequest(new { message = "Email verification failed." });
    }

    [HttpPost("resend-verification")]
    [AllowAnonymous]
    [EnableRateLimiting("ResendVerificationLimit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResendVerificationEmailAsync(request, cancellationToken);
        if (result)
        {
            return Ok(new { message = "If the email is eligible, a new verification link has been sent." });
        }

        return BadRequest(new { message = "Failed to resend verification email." });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [EnableRateLimiting("ForgotPasswordLimit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ForgotPasswordAsync(request, cancellationToken);
        if (result)
        {
            return Ok(new { message = "If the email is registered, a password reset link has been sent." });
        }

        return BadRequest(new { message = "Forgot password request failed." });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [EnableRateLimiting("ResetPasswordLimit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResetPasswordAsync(request, cancellationToken);
        if (result != null)
        {
            return Ok(result);
        }

        return BadRequest(new { message = "Password reset failed." });
    }

    [HttpDelete("/api/users/me")]
    [HttpDelete("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteMe()
    {
        var result = await _authService.DeleteMeAsync();
        if (result)
        {
            return Ok(new { message = "Account successfully deleted." });
        }
        return BadRequest(new { message = "Account deletion failed." });
    }
}
