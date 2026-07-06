using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.DTOs;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.Interfaces;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AchievementService.CreateAchievementAsync — CVerify-21 (5 UTCIDs).
/// POST /api/v1/users/achievements [Authorize] — adds an academic/professional achievement.
/// </summary>
public sealed class CVerify21_AddAchievementTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storageService = new();
    private readonly AchievementService _sut;

    public CVerify21_AddAchievementTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://cdn.example.com/signed-url");

        _sut = new AchievementService(_context, _storageService.Object);
    }

    public void Dispose() => _context.Dispose();

    private static AcademicAchievementRequest BuildRequest(
        string title = "AWS Certified",
        string issuer = "Amazon",
        DateTimeOffset? issueDate = null,
        string description = "Cloud cert",
        string? credentialUrl = null,
        Guid? attachmentId = null) =>
        new(
            Title: title,
            Issuer: issuer,
            IssueDate: issueDate ?? new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero),
            Description: description,
            CredentialUrl: credentialUrl,
            AttachmentId: attachmentId);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify21_UTCID01_AddAchievement_AllRequiredFields_ReturnsAchievementResponse()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.CreateAchievementAsync(userId, BuildRequest(
            title: "AWS Certified",
            issuer: "Amazon",
            description: "Cloud certification",
            credentialUrl: "https://aws.amazon.com/cert/123"));

        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Title.Should().Be("AWS Certified");
        result.Issuer.Should().Be("Amazon");
        result.Description.Should().Be("Cloud certification");
        result.CredentialUrl.Should().Be("https://aws.amazon.com/cert/123");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify21_UTCID02_AddAchievement_RequiredFieldsOnly_ReturnsAchievementResponse()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.CreateAchievementAsync(userId, BuildRequest(
            title: "First Place Hackathon",
            issuer: "HackFPT",
            description: "Won first place",
            credentialUrl: null));

        result.Should().NotBeNull();
        result.Title.Should().Be("First Place Hackathon");
        result.CredentialUrl.Should().BeNull();
        result.Attachment.Should().BeNull();
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // title: null → service calls request.Title.Trim() → NullReferenceException
    [Fact]
    public async Task CVerify21_UTCID03_AddAchievement_NullTitle_ThrowsNullReference()
    {
        var userId = Guid.NewGuid();
        var request = new AcademicAchievementRequest(
            Title: null!,
            Issuer: "Amazon",
            IssueDate: DateTimeOffset.UtcNow,
            Description: "cert",
            CredentialUrl: null,
            AttachmentId: null);

        var act = async () => await _sut.CreateAchievementAsync(userId, request);

        await act.Should().ThrowAsync<Exception>("controller [Required] validates Title before reaching service");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // No JWT — controller [Authorize] returns 401.
    // Service level: accepts any userId without verifying JWT.
    [Fact]
    public async Task CVerify21_UTCID04_AddAchievement_NoJwtControllerLevel_ServiceCreatesForAnyUserId()
    {
        var ghostUserId = Guid.NewGuid(); // not in DB, no auth

        var result = await _sut.CreateAchievementAsync(ghostUserId, BuildRequest());

        result.Should().NotBeNull("service does not validate JWT — controller responsibility");
        result.UserId.Should().Be(ghostUserId);
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // Title: 200-char string (max boundary)
    [Fact]
    public async Task CVerify21_UTCID05_AddAchievement_MaxBoundaryTitle200Chars_ReturnsResponse()
    {
        var userId = Guid.NewGuid();
        var longTitle = new string('A', 200);

        var result = await _sut.CreateAchievementAsync(userId, BuildRequest(title: longTitle));

        result.Should().NotBeNull();
        result.Title.Should().HaveLength(200);
    }
}
