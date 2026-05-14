using Microsoft.EntityFrameworkCore;

namespace TripGenie.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add DbSets here as the project grows
}
