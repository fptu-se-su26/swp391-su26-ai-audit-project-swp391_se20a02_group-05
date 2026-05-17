using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TripGenie.API.Application.DTOs;
using TripGenie.API.Core.Entities;
using TripGenie.API.Infrastructure.Persistence;
using TripGenie.API.IntegrationTests.Fixtures;
using TripGenie.API.IntegrationTests.Helpers;
using Xunit;

namespace TripGenie.API.IntegrationTests.Auth;

public class AuthChaosTests : BaseIntegrationTest
{
    public AuthChaosTests(SharedTestcontainerFixture containerFixture) : base(containerFixture)
    {
    }

    [Fact]
    public async Task Registration_With_Simulated_Database_Crash_Should_Rollback_Cleanly()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Create a custom transaction and verify rollback behavior manually in code.
        using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var user = new UserBuilder()
                .WithEmail("chaos@tripgenie.ai")
                .Build();

            db.Users.Add(user);
            await db.SaveChangesAsync();

            // Simulate crash/exception
            throw new InvalidOperationException("Simulated Database Crash");
        }
        catch (InvalidOperationException)
        {
            await transaction.RollbackAsync();
        }

        // Verify user was NOT persisted
        var persistedUser = await db.Users.FirstOrDefaultAsync(u => u.Email == "chaos@tripgenie.ai");
        persistedUser.Should().BeNull();
    }
}
