using System.Security.Claims;
using TripGenie.API.Core.Entities;

namespace TripGenie.API.Application.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);
    string GenerateRefreshToken();
    void SetTokenInsideCookie(string tokenName, string tokenValue, DateTime? expires = null);
    void RemoveTokenFromCookie(string tokenName);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
