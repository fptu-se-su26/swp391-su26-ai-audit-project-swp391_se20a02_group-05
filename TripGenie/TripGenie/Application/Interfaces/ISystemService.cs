using TripGenie.API.Application.DTOs;

namespace TripGenie.API.Application.Interfaces;

public interface ISystemService
{
    Task<DatabaseStatusResponse> CheckDatabaseStatusAsync();
}
