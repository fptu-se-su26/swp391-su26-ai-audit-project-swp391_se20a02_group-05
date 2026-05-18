using TripGenie.API.Core.Entities;

namespace TripGenie.API.Application.Interfaces;

public interface IAccountService
{
    Task HandleFailedLoginAsync(User user);
    Task ResetFailedAttemptsAsync(User user);
    bool IsAccountLocked(User user);
    bool IsAccountDisabled(User user);
}
