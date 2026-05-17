namespace TripGenie.API.Application.DTOs;

public record LoginRequest(string Email, string Password);

public record AuthResponse(Guid Id, string Email, IEnumerable<string> Roles, IEnumerable<string> Permissions);

public record UserProfileResponse(Guid Id, string Email, IEnumerable<string> Roles, IEnumerable<string> Permissions);
