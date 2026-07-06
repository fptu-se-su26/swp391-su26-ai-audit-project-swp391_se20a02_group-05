using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Diagnostics;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security;
using CVerify.API.Modules.Shared.Storage.DTOs;
using CVerify.API.Modules.Shared.Storage.Enums;
using CVerify.API.Modules.Shared.Storage.Interfaces;
using CVerify.API.Modules.Shared.System.Services;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for ProfileService.UploadAvatarAsync — CVerify-09 (9 UTCIDs).
/// File size / MIME-type validation lives in the controller; the service just delegates to IStorageService.
/// </summary>
public sealed class CVerify09_UploadAvatarTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    private readonly Mock<ICacheService>         _cacheService         = new();
    private readonly Mock<IStorageService>       _storageService       = new();
    private readonly Mock<IUsernameService>      _usernameService      = new();
    private readonly Mock<IAppLogger>            _logger               = new();
    private readonly Mock<IProjectService>       _projectService       = new();
    private readonly Mock<ICvRepositoryIndexer>  _cvRepositoryIndexer  = new();

    private const string FakeObjectKey = "profiles/avatar-abc123.jpg";
    private const string FakeSignedUrl = "https://cdn.example.com/signed?key=profiles/avatar-abc123.jpg&exp=9999";

    private readonly ProfileService _sut;

    public CVerify09_UploadAvatarTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.UploadFileAsync(
                It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<StorageModule>(), It.IsAny<System.Collections.Generic.Dictionary<string, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StorageFileDto { ObjectKey = FakeObjectKey, MimeType = "image/jpeg", Size = 1024 });

        _storageService
            .Setup(s => s.GetSignedUrlAsync(FakeObjectKey, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(FakeSignedUrl);

        _sut = new ProfileService(
            _context,
            _cacheService.Object,
            _storageService.Object,
            _usernameService.Object,
            new FakeTimeProvider(),
            _logger.Object,
            _projectService.Object,
            _cvRepositoryIndexer.Object);
    }

    public void Dispose() => _context.Dispose();

    private async Task<User> SeedUserAsync(string? existingAvatarUrl = null)
    {
        var user = new User
        {
            Id              = Guid.NewGuid(),
            Email           = "user@example.com",
            FullName        = "Avatar User",
            Username        = "avataruser",
            Status          = UserStatus.ACTIVE,
            EmailVerifiedAt = DateTime.UtcNow,
            AvatarUrl       = existingAvatarUrl,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    private static MemoryStream MakeStream(int sizeBytes) =>
        new(new byte[sizeBytes]);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify09_UTCID01_UploadAvatar_JpegOneMb_ReturnsSignedUrlAndObjectKey()
    {
        var user = await SeedUserAsync();
        using var stream = MakeStream(1 * 1024 * 1024);

        var (signedUrl, objectKey) = await _sut.UploadAvatarAsync(user.Id, stream, "photo.jpg", "image/jpeg");

        signedUrl.Should().Be(FakeSignedUrl);
        objectKey.Should().Be(FakeObjectKey);
        var updated = await _context.Users.FindAsync(user.Id);
        updated!.AvatarUrl.Should().Be(FakeObjectKey);
        updated.AvatarSource.Should().Be(AvatarSource.Uploaded);
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify09_UTCID02_UploadAvatar_PngTwoMb_ReturnsSignedUrl()
    {
        var user = await SeedUserAsync();
        using var stream = MakeStream(2 * 1024 * 1024);

        var (signedUrl, _) = await _sut.UploadAvatarAsync(user.Id, stream, "photo.png", "image/png");

        signedUrl.Should().Be(FakeSignedUrl);
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify09_UTCID03_UploadAvatar_WebP500Kb_ReturnsSignedUrl()
    {
        var user = await SeedUserAsync();
        using var stream = MakeStream(500 * 1024);

        var (signedUrl, _) = await _sut.UploadAvatarAsync(user.Id, stream, "photo.webp", "image/webp");

        signedUrl.Should().Be(FakeSignedUrl);
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // 6 MB exceeds StorageConstants.MaxProfileSize (2 MB) → controller returns 400.
    // At service level: service does NOT validate file size — it passes through to storage.
    [Fact]
    public async Task CVerify09_UTCID04_UploadAvatar_SixMbOverLimit_ServicePassesToStorageWithoutSizeCheck()
    {
        var user = await SeedUserAsync();
        using var stream = MakeStream(6 * 1024 * 1024);

        var (signedUrl, _) = await _sut.UploadAvatarAsync(user.Id, stream, "photo.jpg", "image/jpeg");

        signedUrl.Should().Be(FakeSignedUrl, "service does not validate file size — controller responsibility");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // PDF MIME type → controller validates AllowedImageTypes → 400.
    // At service level: service passes content type straight to storage.
    [Fact]
    public async Task CVerify09_UTCID05_UploadAvatar_PdfMimeType_ServicePassesToStorageWithoutMimeCheck()
    {
        var user = await SeedUserAsync();
        using var stream = MakeStream(100 * 1024);

        var (signedUrl, _) = await _sut.UploadAvatarAsync(user.Id, stream, "document.pdf", "application/pdf");

        signedUrl.Should().Be(FakeSignedUrl, "service does not validate MIME type — controller responsibility");
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    // No file (null/empty) → controller returns 400 before calling service.
    // At service level with an empty stream: service delegates to mocked storage → succeeds.
    [Fact]
    public async Task CVerify09_UTCID06_UploadAvatar_EmptyStream_ServiceDelegatesToStorageMock()
    {
        var user = await SeedUserAsync();
        using var stream = MakeStream(0);

        var (signedUrl, _) = await _sut.UploadAvatarAsync(user.Id, stream, "empty.jpg", "image/jpeg");

        signedUrl.Should().Be(FakeSignedUrl, "controller guards against empty file; service itself does not");
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // At service level: non-existent userId → ResourceNotFoundException.
    [Fact]
    public async Task CVerify09_UTCID07_UploadAvatar_NoJwtControllerLevel_ServiceThrowsForUnknownUser()
    {
        using var stream = MakeStream(512);

        var act = async () => await _sut.UploadAvatarAsync(Guid.NewGuid(), stream, "photo.jpg", "image/jpeg");

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }

    // ── UTCID08 ───────────────────────────────────────────────────────────
    // Exactly 5 MB — boundary test (controller MaxProfileSize = 2 MB, so this would fail at controller).
    // At service level, service has no size check, so it succeeds.
    [Fact]
    public async Task CVerify09_UTCID08_UploadAvatar_ExactlyFiveMbBoundary_ServicePassesToStorage()
    {
        var user = await SeedUserAsync();
        using var stream = MakeStream(5 * 1024 * 1024);

        var (signedUrl, _) = await _sut.UploadAvatarAsync(user.Id, stream, "photo.jpg", "image/jpeg");

        signedUrl.Should().Be(FakeSignedUrl);
    }

    // ── UTCID09 ───────────────────────────────────────────────────────────
    [Fact]
    public async Task CVerify09_UTCID09_UploadAvatar_GifFile_ReturnsSignedUrl()
    {
        var user = await SeedUserAsync();
        using var stream = MakeStream(300 * 1024);

        var (signedUrl, _) = await _sut.UploadAvatarAsync(user.Id, stream, "anim.gif", "image/gif");

        signedUrl.Should().Be(FakeSignedUrl);
    }
}
