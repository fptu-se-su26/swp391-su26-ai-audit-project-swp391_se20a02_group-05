using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using TripGenie.API.Application.Interfaces;

namespace TripGenie.API.Infrastructure.Persistence;

public class IdentityRepository : IIdentityRepository
{
    private readonly IDbConnection _dbConnection;

    public IdentityRepository(ApplicationDbContext context)
    {
        _dbConnection = context.Database.GetDbConnection();
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(Guid userId)
    {
        const string sql = @"
            SELECT r.name 
            FROM roles r
            JOIN users u ON r.id = u.role_id
            WHERE u.id = @UserId";
        
        return await _dbConnection.QueryAsync<string>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId)
    {
        const string sql = @"
            SELECT DISTINCT p.name 
            FROM permissions p
            JOIN role_permissions rp ON p.id = rp.permission_id
            JOIN users u ON rp.role_id = u.role_id
            WHERE u.id = @UserId";
        
        return await _dbConnection.QueryAsync<string>(sql, new { UserId = userId });
    }
}
