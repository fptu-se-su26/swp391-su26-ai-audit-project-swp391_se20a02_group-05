using TripGenie.API.DTOs;

namespace TripGenie.API.Services;

public interface ISystemService
{
    Task<DatabaseStatusResponse> CheckDatabaseStatusAsync();
}
