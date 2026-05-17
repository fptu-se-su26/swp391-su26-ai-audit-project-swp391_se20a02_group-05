using TripGenie.API.Application.Interfaces;
using TripGenie.API.Infrastructure.Persistence;
using TripGenie.API.Application.DTOs;

namespace TripGenie.API.Infrastructure.Services;

public class SystemService : ISystemService
{
    private readonly ApplicationDbContext _context;

    public SystemService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DatabaseStatusResponse> CheckDatabaseStatusAsync()
    {
        try
        {
            // check whether the database connection is available
            bool isConnected = await _context.Database.CanConnectAsync();

            return new DatabaseStatusResponse
            {
                Success = isConnected,
                Database = isConnected ? "Connected" : "Disconnected",
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception)
        {
            return new DatabaseStatusResponse
            {
                Success = false,
                Database = "Error",
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
