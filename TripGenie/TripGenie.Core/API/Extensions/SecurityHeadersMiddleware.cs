using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TripGenie.API.Infrastructure.Configuration;

namespace TripGenie.API.API.Extensions;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly HashSet<string> ExcludedMutatingPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/auth/login",
        "/api/auth/register",
        "/api/auth/google",
        "/health",
        "/api/system/status"
    };

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Centralized Cache-Control and standard security headers for /api/auth endpoints
        context.Response.OnStarting(() =>
        {
            if (context.Request.Path.StartsWithSegments("/api/auth"))
            {
                context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
                context.Response.Headers["Pragma"] = "no-cache";
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["Referrer-Policy"] = "no-referrer";
            }
            return Task.CompletedTask;
        });

        // Get Configuration for CORS/Origin check
        var envConfig = context.RequestServices.GetRequiredService<EnvConfiguration>();
        var frontendUrl = envConfig.Auth.FrontendUrl?.TrimEnd('/');

        // 2. Generate and Set CSRF-TOKEN Cookie if not present
        if (!context.Request.Cookies.ContainsKey("CSRF-TOKEN"))
        {
            var csrfTokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(csrfTokenBytes);
            }
            var csrfToken = Convert.ToHexString(csrfTokenBytes).ToLower();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = false, // Must be readable by Axios on frontend
                Secure = true,    // Enforce Secure in production
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };
            context.Response.Cookies.Append("CSRF-TOKEN", csrfToken, cookieOptions);
        }

        // 3. Validate CSRF on mutating HTTP requests (POST, PUT, DELETE, PATCH)
        var method = context.Request.Method;
        if (HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsDelete(method) || HttpMethods.IsPatch(method))
        {
            var path = context.Request.Path.Value ?? "";

            // Check if the endpoint is explicitly excluded from CSRF checks
            bool isExcluded = ExcludedMutatingPaths.Any(excluded => path.Equals(excluded, StringComparison.OrdinalIgnoreCase));

            if (!isExcluded)
            {
                // Double Submit Cookie Check
                if (!context.Request.Headers.TryGetValue("X-CSRF-Token", out var headerCsrfToken) ||
                    !context.Request.Cookies.TryGetValue("CSRF-TOKEN", out var cookieCsrfToken) ||
                    string.IsNullOrEmpty(headerCsrfToken) ||
                    string.IsNullOrEmpty(cookieCsrfToken) ||
                    !string.Equals(headerCsrfToken, cookieCsrfToken, StringComparison.Ordinal))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\": \"CSRF token validation failed.\"}");
                    return;
                }

                // Origin & Referer Defense-in-Depth Validation
                var origin = context.Request.Headers["Origin"].ToString()?.TrimEnd('/');
                var referer = context.Request.Headers["Referer"].ToString()?.TrimEnd('/');

                bool isValidOrigin = false;
                if (!string.IsNullOrEmpty(frontendUrl))
                {
                    if (!string.IsNullOrEmpty(origin) && origin.Equals(frontendUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        isValidOrigin = true;
                    }
                    else if (!string.IsNullOrEmpty(referer) && referer.StartsWith(frontendUrl, StringComparison.OrdinalIgnoreCase))
                    {
                        isValidOrigin = true;
                    }
                }

                // If Origin/Referer present and doesn't match configured frontend, block the request
                if ((!string.IsNullOrEmpty(origin) || !string.IsNullOrEmpty(referer)) && !isValidOrigin)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\": \"Invalid request origin or referer.\"}");
                    return;
                }
            }
        }

        await _next(context);
    }
}
