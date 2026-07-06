using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Xunit;
using CVerify.API.Modules.Auth.Controllers;
using CVerify.API.Modules.Auth.DTOs;
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Storage.DTOs;
using CVerify.API.Modules.Shared.Storage.Enums;
using CVerify.API.Modules.Shared.Storage.Interfaces;

namespace CVerify.API.UnitTests.Auth;

/// <summary>
/// Unit tests for WorkspaceController.UploadBanner — CVerify-84 (5 UTCIDs).
/// POST /api/workspace/{organizationSlug}/banner [Authorize] — uploads a banner image.
/// NOTE: Excel says "Max 5MB"; the real limit is StorageConstants.MaxProfileSize = 2 MB, so the
/// oversized boundary case uses a 3 MB file.
/// </summary>
public sealed class CVerify84_UploadWorkspaceBannerTests : IDisposable
{
    private const long OneMb = 1024 * 1024;
    private readonly ApplicationDbContext _context;
    private readonly Mock<IOrganizationAuthorizationService> _authService = new();
    private readonly Mock<IStorageService> _storageService = new();

    public CVerify84_UploadWorkspaceBannerTests()
    {
        _context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options);

        _storageService
            .Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<StorageModule>(), It.IsAny<Dictionary<string, string>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StorageFileDto { Bucket = "b", ObjectKey = "orgs/banner-key", MimeType = "image/jpeg", Size = OneMb });

        _storageService
            .Setup(s => s.GetSignedUrlAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("https://signed.example/banner.jpg");

        _authService
            .Setup(a => a.AuthorizeAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false); // default: not authorized
    }

    public void Dispose() => _context.Dispose();

    private WorkspaceController BuildController(ClaimsPrincipal? user = null)
    {
        var ctrl = new WorkspaceController(_context, _authService.Object, _storageService.Object);
        ctrl.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user ?? new ClaimsPrincipal() },
        };
        return ctrl;
    }

    private async Task<Organization> SeedOrgAsync(string slug)
    {
        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = $"Org {slug}",
            Username = slug,
            TaxCode = $"TAX-{slug}",
            Email = $"{slug}@org.com",
            Status = "active",
        };
        _context.Organizations.Add(org);
        await _context.SaveChangesAsync();
        return org;
    }

    private static ClaimsPrincipal BusinessOwner(Guid orgId) =>
        new(new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, orgId.ToString()),
            new("actor_type", "business"),
        }, "Test"));

    private static ClaimsPrincipal Member(Guid userId) =>
        new(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) }, "Test"));

    private static IFormFile BuildFile(long length, string contentType, string fileName = "banner.jpg")
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.Length).Returns(length);
        mock.Setup(f => f.ContentType).Returns(contentType);
        mock.Setup(f => f.FileName).Returns(fileName);
        mock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[16]));
        return mock.Object;
    }

    // ── UTCID01 ── valid 1MB jpeg by owner → 200 OK { bannerUrl } ─────────
    [Fact]
    public async Task CVerify84_UTCID01_UploadBanner_ValidImage_ReturnsOkWithUrl()
    {
        var org = await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(BusinessOwner(org.Id));

        var response = await ctrl.UploadBanner("acme-corp", BuildFile(OneMb, "image/jpeg"), CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<WorkspaceAvatarUploadResponse>();
    }

    // ── UTCID02 ── oversized (3MB > 2MB limit) → 400 BadRequest ───────────
    [Fact]
    public async Task CVerify84_UTCID02_UploadBanner_TooLarge_ReturnsBadRequest()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(BusinessOwner(Guid.NewGuid()));

        var response = await ctrl.UploadBanner("acme-corp", BuildFile(3 * OneMb, "image/jpeg"), CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID03 ── unsupported type (image/bmp) → 400 BadRequest ──────────
    [Fact]
    public async Task CVerify84_UTCID03_UploadBanner_UnsupportedType_ReturnsBadRequest()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(BusinessOwner(Guid.NewGuid()));

        var response = await ctrl.UploadBanner("acme-corp", BuildFile(OneMb, "image/bmp", "banner.bmp"), CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    // ── UTCID04 ── valid image, member without permission → 403 Forbidden ─
    [Fact]
    public async Task CVerify84_UTCID04_UploadBanner_MemberNoPermission_ReturnsForbid()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(Member(Guid.NewGuid())); // authService default => false

        var response = await ctrl.UploadBanner("acme-corp", BuildFile(OneMb, "image/jpeg"), CancellationToken.None);

        response.Should().BeOfType<ForbidResult>();
    }

    // ── UTCID05 ── valid image, no JWT → 401 Unauthorized ─────────────────
    [Fact]
    public async Task CVerify84_UTCID05_UploadBanner_NoJwt_ReturnsUnauthorized()
    {
        await SeedOrgAsync("acme-corp");
        var ctrl = BuildController(); // anonymous

        var response = await ctrl.UploadBanner("acme-corp", BuildFile(OneMb, "image/jpeg"), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedResult>();
    }
}
