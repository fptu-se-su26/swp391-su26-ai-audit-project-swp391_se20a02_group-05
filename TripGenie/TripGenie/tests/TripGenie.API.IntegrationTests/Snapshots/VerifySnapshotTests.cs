using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TripGenie.API.Application.Interfaces;
using TripGenie.API.IntegrationTests.Fixtures;
using VerifyXunit;
using Xunit;

namespace TripGenie.API.IntegrationTests.Snapshots;

/// <summary>
/// Execution tests using Verify.Xunit snapshot comparison engine to validate HTML structures and contracts.
/// </summary>
public class VerifySnapshotTests : IClassFixture<SharedTestcontainerFixture>
{
    private readonly SharedTestcontainerFixture _fixture;
    private readonly IEmailTemplateService _templateService;

    /// <summary>
    /// Initializes a new instance of the <see cref="VerifySnapshotTests"/> class.
    /// </summary>
    public VerifySnapshotTests(SharedTestcontainerFixture fixture)
    {
        _fixture = fixture;
        var factory = new IntegrationTestApplicationFactory(_fixture);
        _templateService = factory.Services.GetRequiredService<IEmailTemplateService>();
    }

    [Fact]
    public async Task VerificationEmailTemplate_SnapshotTest()
    {
        // Arrange
        var model = new Dictionary<string, object>
        {
            { "full_name", "Luc Snapshot Tester" },
            { "verification_link", "https://tripgenie.ai/verify?token=verify_snapshot_123_abc" }
        };

        // Act
        var html = await _templateService.RenderTemplateAsync("VerificationEmail.html", model).ConfigureAwait(false);

        // Assert
        // Verifier saves dynamic HTML outputs and compares them to pre-recorded .received and .verified states
        await Verifier.Verify(html, "html").ConfigureAwait(false);
    }

    [Fact]
    public async Task ResetPasswordEmailTemplate_SnapshotTest()
    {
        // Arrange
        var model = new Dictionary<string, object>
        {
            { "full_name", "Luc Snapshot Tester" },
            { "reset_link", "https://tripgenie.ai/reset?token=reset_snapshot_123_abc" }
        };

        // Act
        var html = await _templateService.RenderTemplateAsync("ResetPasswordEmail.html", model).ConfigureAwait(false);

        // Assert
        await Verifier.Verify(html, "html").ConfigureAwait(false);
    }

    [Fact]
    public async Task WelcomeEmailTemplate_SnapshotTest()
    {
        // Arrange
        var model = new Dictionary<string, object>
        {
            { "full_name", "Luc Welcome Snapshot" }
        };

        // Act
        var html = await _templateService.RenderTemplateAsync("WelcomeEmail.html", model).ConfigureAwait(false);

        // Assert
        await Verifier.Verify(html, "html").ConfigureAwait(false);
    }
}
