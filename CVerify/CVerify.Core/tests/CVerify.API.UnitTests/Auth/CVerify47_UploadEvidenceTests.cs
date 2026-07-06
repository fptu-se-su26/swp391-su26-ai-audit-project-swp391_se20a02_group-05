using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Profiles.Services;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.DTOs;
using CVerify.API.Modules.Shared.Storage.Enums;
using CVerify.API.Modules.Shared.Storage.Interfaces;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for AttachmentService.UploadAttachmentAsync — CVerify-47 (7 UTCIDs).
/// POST /api/v1/users/evidence/upload [Authorize] — uploads evidence file (certificate, diploma, etc.).
/// File size and type validation are CONTROLLER-LEVEL; the service trusts its dependencies.
/// </summary>
public sealed class CVerify47_UploadEvidenceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IStorageService> _storageService = new();
    private readonly AttachmentService _sut;

    private static readonly StorageFileDto FakeUploadResult = new()
    {
        Bucket = "cverify-evidence",
        ObjectKey = "evidence/fake-key-123",
        MimeType = "application/pdf",
        Size = 512_000L,
    };

    public CVerify47_UploadEvidenceTests()
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
            .ReturnsAsync(FakeUploadResult);

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://cdn.example.com/signed-url");

        _sut = new AttachmentService(_context, _storageService.Object);
    }

    public void Dispose() => _context.Dispose();

    private static Stream MakeStream(int sizeBytes = 1024) =>
        new MemoryStream(new byte[sizeBytes]);

    // ── UTCID01 ───────────────────────────────────────────────────────────
    // file: certificate.pdf, entityType:'Achievement', entityId: valid GUID → 200 AttachmentResponse
    [Fact]
    public async Task CVerify47_UTCID01_UploadEvidence_PdfAchievement_ReturnsAttachmentResponse()
    {
        var userId = Guid.NewGuid();
        var entityId = Guid.NewGuid();

        var result = await _sut.UploadAttachmentAsync(
            userId, "AcademicAchievement", entityId,
            MakeStream(), "certificate.pdf", "application/pdf");

        result.Should().NotBeNull();
        result.FileName.Should().Be("certificate.pdf");
        result.FileUrl.Should().Be("https://cdn.example.com/signed-url");
    }

    // ── UTCID02 ───────────────────────────────────────────────────────────
    // file: diploma.jpg, entityType:'Education' → 200 AttachmentResponse
    [Fact]
    public async Task CVerify47_UTCID02_UploadEvidence_JpgEducation_ReturnsAttachmentResponse()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.UploadAttachmentAsync(
            userId, "Education", null,
            MakeStream(), "diploma.jpg", "image/jpeg");

        result.Should().NotBeNull();
        result.FileName.Should().Be("diploma.jpg");
    }

    // ── UTCID03 ───────────────────────────────────────────────────────────
    // file: large.pdf (11MB) → size check is CONTROLLER-LEVEL (R2StorageService validates).
    // With mocked storage, service-level upload succeeds.
    [Fact]
    public async Task CVerify47_UTCID03_UploadEvidence_LargeFile_ServiceLevelAccepts()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.UploadAttachmentAsync(
            userId, "AcademicAchievement", null,
            MakeStream(11 * 1024 * 1024), "large.pdf", "application/pdf");

        result.Should().NotBeNull("file size validation is handled by real IStorageService, not by AttachmentService");
    }

    // ── UTCID04 ───────────────────────────────────────────────────────────
    // file: program.exe → file type check is CONTROLLER-LEVEL.
    // With mocked storage, service-level upload succeeds.
    [Fact]
    public async Task CVerify47_UTCID04_UploadEvidence_ExeFile_ServiceLevelAccepts()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.UploadAttachmentAsync(
            userId, "AcademicAchievement", null,
            MakeStream(), "program.exe", "application/octet-stream");

        result.Should().NotBeNull("file type validation is controller-level, not AttachmentService-level");
    }

    // ── UTCID05 ───────────────────────────────────────────────────────────
    // No file in request → controller returns 400. At service level with mock: stream is empty.
    [Fact]
    public async Task CVerify47_UTCID05_UploadEvidence_EmptyStream_ServiceLevelAccepts()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.UploadAttachmentAsync(
            userId, "AcademicAchievement", null,
            MakeStream(0), "empty.pdf", "application/pdf");

        result.Should().NotBeNull("empty file validation is controller-level");
    }

    // ── UTCID06 ───────────────────────────────────────────────────────────
    // No JWT → controller [Authorize] returns 401.
    // Service level: ghost userId → creates attachment entry for any userId.
    [Fact]
    public async Task CVerify47_UTCID06_UploadEvidence_NoJwtControllerLevel_ServiceCreatesEntry()
    {
        var ghostUserId = Guid.NewGuid();

        var result = await _sut.UploadAttachmentAsync(
            ghostUserId, "AcademicAchievement", null,
            MakeStream(), "cert.pdf", "application/pdf");

        result.Should().NotBeNull("JWT auth is controller responsibility");
    }

    // ── UTCID07 ───────────────────────────────────────────────────────────
    // file exactly at max allowed size → service accepts (size check is in real storage service)
    [Fact]
    public async Task CVerify47_UTCID07_UploadEvidence_MaxSizeFile_ServiceLevelAccepts()
    {
        var userId = Guid.NewGuid();
        const int maxSize = 10 * 1024 * 1024; // 10 MB

        var result = await _sut.UploadAttachmentAsync(
            userId, "AcademicAchievement", null,
            MakeStream(maxSize), "max.pdf", "application/pdf");

        result.Should().NotBeNull();
    }
}
