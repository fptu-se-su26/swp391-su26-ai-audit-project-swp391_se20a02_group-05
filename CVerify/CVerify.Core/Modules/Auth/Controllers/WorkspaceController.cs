using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CVerify.API.Modules.Auth.DTOs;
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Domain.Enums;
using CVerify.API.Modules.Shared.Persistence;
using CVerify.API.Modules.Shared.Security.Authorization;
using CVerify.API.Modules.Shared.Storage.Constants;
using CVerify.API.Modules.Shared.Storage.Enums;
using CVerify.API.Modules.Shared.Storage.Interfaces;

namespace CVerify.API.Modules.Auth.Controllers;

[ApiController]
[Route("api/workspace")]
[Authorize]
public class WorkspaceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IOrganizationAuthorizationService _authorizationService;
    private readonly IStorageService _storageService;

    public WorkspaceController(
        ApplicationDbContext context,
        IOrganizationAuthorizationService authorizationService,
        IStorageService storageService)
    {
        _context = context;
        _authorizationService = authorizationService;
        _storageService = storageService;
    }

    [HttpGet("my-organizations")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LinkedOrganizationDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyOrganizations()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var actorTypeClaim = User.FindFirst("actor_type")?.Value;
        bool isBusiness = string.Equals(actorTypeClaim, "business", StringComparison.OrdinalIgnoreCase);

        if (isBusiness)
        {
            var org = await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == userId && o.DeletedAt == null);
            if (org != null)
            {
                return Ok(new List<LinkedOrganizationDto> { new LinkedOrganizationDto(org.Name, org.Username) });
            }
            return Ok(new List<LinkedOrganizationDto>());
        }

        var orgs = await _context.OrganizationMemberships
            .Where(om => om.UserId == userId && om.Status == "active")
            .Include(om => om.Organization)
            .Select(om => new LinkedOrganizationDto(om.Organization.Name, om.Organization.Username))
            .ToListAsync();

        return Ok(orgs);
    }

    [HttpGet("{organizationSlug}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkspaceDetailsDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkspaceDetails(string organizationSlug)
    {
        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Username.ToLower() == organizationSlug.ToLower() && o.DeletedAt == null);

        if (org == null)
        {
            return NotFound(new { message = "Organization not found" });
        }

        var workspaces = await _context.Workspaces
            .Where(w => w.OrganizationId == org.Id && w.DeletedAt == null)
            .Select(w => new WorkspaceDto(w.Id, w.DisplayName, w.Slug))
            .ToListAsync();

        var signedBannerUrl = await GetSignedUrlAsync(org.BannerUrl);
        var signedLogoUrl = await GetSignedUrlAsync(org.LogoUrl);

        var signedGalleryUrls = new List<string>();
        if (org.GalleryUrls != null)
        {
            foreach (var url in org.GalleryUrls)
            {
                var signed = await GetSignedUrlAsync(url);
                if (signed != null) signedGalleryUrls.Add(signed);
            }
        }

        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            // Anonymous visitor: return basic public profile details
            return Ok(MapToWorkspaceDetailsDto(
                org,
                null,
                new List<LinkedOrganizationDto>(),
                new List<string>(),
                workspaces,
                signedBannerUrl,
                signedLogoUrl,
                signedGalleryUrls,
                org.FollowerCount,
                false
            ));
        }

        var actorTypeClaim = User.FindFirst("actor_type")?.Value;
        bool isBusiness = string.Equals(actorTypeClaim, "business", StringComparison.OrdinalIgnoreCase);

        if (isBusiness)
        {
            if (org.Id != userId)
            {
                // Authenticated as a different business account (treat as anonymous public viewer)
                return Ok(MapToWorkspaceDetailsDto(
                    org,
                    null,
                    new List<LinkedOrganizationDto>(),
                    new List<string>(),
                    workspaces,
                    signedBannerUrl,
                    signedLogoUrl,
                    signedGalleryUrls,
                    org.FollowerCount,
                    false
                ));
            }

            var businessPermissions = new List<string>
            {
                "organization:profile:edit", "organization:settings:edit", "organization:workspace:view", "organization:roles:manage", "organization:roles:view",
                "organization:members:manage", "organization:members:view", "identity:verification:initiate", "identity:verification:approve",
                "identity:verification:reject", "evidence:graph:validate", "evidence:graph:comment", "analysis:repository:sync",
                "analysis:repository:run", "analysis:repository:configure", "trust:metric:view", "trust:flag:manage",
                "ai:interview:configure", "ai:interview:conduct", "ai:interview:evaluate", "candidate:trust:score",
                "candidate:trust:override", "organization:audit:view", "billing:invoice:view", "billing:subscription:manage"
            };

            return Ok(MapToWorkspaceDetailsDto(
                org,
                "OWNER",
                new List<LinkedOrganizationDto>(),
                businessPermissions,
                workspaces,
                signedBannerUrl,
                signedLogoUrl,
                signedGalleryUrls,
                org.FollowerCount,
                false
            ));
        }

        // Authorize membership using the centralized authorization service
        var isAuthorized = await _authorizationService.AuthorizeAsync(userId, org.Id, OrganizationPermissions.ViewWorkspace);
        if (!isAuthorized)
        {
            // Authenticated but not authorized to view workspace (treat as public viewer)
            var isFollowingPublic = await _context.OrganizationFollowers
                .AnyAsync(f => f.UserId == userId && f.OrganizationId == org.Id);
            return Ok(MapToWorkspaceDetailsDto(
                org,
                null,
                new List<LinkedOrganizationDto>(),
                new List<string>(),
                workspaces,
                signedBannerUrl,
                signedLogoUrl,
                signedGalleryUrls,
                org.FollowerCount,
                isFollowingPublic
            ));
        }

        // Fetch the user's role in this organization
        var membership = await _context.OrganizationMemberships
            .FirstOrDefaultAsync(om => om.OrganizationId == org.Id && om.UserId == userId);

        if (membership == null || membership.Status != "active")
        {
            // Fallback for safety (treat as public viewer)
            var isFollowingFallback = await _context.OrganizationFollowers
                .AnyAsync(f => f.UserId == userId && f.OrganizationId == org.Id);
            return Ok(MapToWorkspaceDetailsDto(
                org,
                null,
                new List<LinkedOrganizationDto>(),
                new List<string>(),
                workspaces,
                signedBannerUrl,
                signedLogoUrl,
                signedGalleryUrls,
                org.FollowerCount,
                isFollowingFallback
            ));
        }

        // Resolve dynamic permissions
        var userPerms = await _authorizationService.GetPermissionsAsync(userId, org.Id, HttpContext.RequestAborted);
        var allDbPermissions = await _context.Permissions
            .Select(p => p.Name)
            .ToListAsync(HttpContext.RequestAborted);

        var permissions = allDbPermissions
            .Where(p => PermissionEvaluator.HasPermission(userPerms, p, org.Id))
            .ToList();

        // Fetch other organizations the user belongs to for switching overview (Account Linking Overview)
        var linkedOrgs = await _context.OrganizationMemberships
            .Where(om => om.UserId == userId && om.OrganizationId != org.Id && om.Status == "active")
            .Include(om => om.Organization)
            .Select(om => new LinkedOrganizationDto(om.Organization.Name, om.Organization.Username))
            .ToListAsync();

        var isFollowingMember = await _context.OrganizationFollowers
            .AnyAsync(f => f.UserId == userId && f.OrganizationId == org.Id, HttpContext.RequestAborted);

        return Ok(MapToWorkspaceDetailsDto(
            org,
            membership.Role,
            linkedOrgs,
            permissions,
            workspaces,
            signedBannerUrl,
            signedLogoUrl,
            signedGalleryUrls,
            org.FollowerCount,
            isFollowingMember
        ));
    }

    [HttpPatch("{organizationSlug}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkspaceDetailsDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWorkspaceDetails(
        string organizationSlug,
        [FromBody] UpdateWorkspaceDetailsRequestDto dto,
        CancellationToken cancellationToken)
    {
        if (dto == null)
        {
            return BadRequest("Request payload is empty.");
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Username.ToLower() == organizationSlug.ToLower() && o.DeletedAt == null, cancellationToken);

        if (org == null)
        {
            return NotFound(new { message = "Organization not found" });
        }

        var actorTypeClaim = User.FindFirst("actor_type")?.Value;
        bool isBusiness = string.Equals(actorTypeClaim, "business", StringComparison.OrdinalIgnoreCase);

        if (isBusiness)
        {
            if (org.Id != userId)
            {
                return Forbid();
            }
        }
        else
        {
            var isAuthorized = await _authorizationService.AuthorizeAsync(userId, org.Id, OrganizationPermissions.EditProfile, cancellationToken: cancellationToken);
            if (!isAuthorized)
            {
                return Forbid();
            }
        }

        // Apply updates
        org.Description = dto.Description;
        org.CompanyType = dto.CompanyType;
        org.CompanySize = dto.CompanySize;
        org.BranchCount = dto.BranchCount;
        org.IndustryTags = dto.IndustryTags ?? new List<string>();
        org.BenefitTags = dto.BenefitTags ?? new List<string>();
        org.ContactName = dto.ContactName;
        org.ContactPhone = dto.ContactPhone;
        org.ContactEmail = dto.ContactEmail;
        org.City = dto.City;
        org.DetailAddress = dto.DetailAddress;
        org.GoogleMapsEmbedUrl = dto.GoogleMapsEmbedUrl;
        org.LinkedinUrl = dto.LinkedinUrl;
        org.FacebookUrl = dto.FacebookUrl;
        org.TwitterUrl = dto.TwitterUrl;
        org.Website = dto.Website;
        org.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Fetch same information to return the updated Details DTO
        var workspaces = await _context.Workspaces
            .Where(w => w.OrganizationId == org.Id && w.DeletedAt == null)
            .Select(w => new WorkspaceDto(w.Id, w.DisplayName, w.Slug))
            .ToListAsync(cancellationToken);

        var signedBannerUrl = await GetSignedUrlAsync(org.BannerUrl, cancellationToken);
        var signedLogoUrl = await GetSignedUrlAsync(org.LogoUrl, cancellationToken);

        var signedGalleryUrls = new List<string>();
        if (org.GalleryUrls != null)
        {
            foreach (var url in org.GalleryUrls)
            {
                var signed = await GetSignedUrlAsync(url, cancellationToken);
                if (signed != null) signedGalleryUrls.Add(signed);
            }
        }

        // Determine permissions and role just like GET endpoint
        string? userRole = null;
        var permissions = new List<string>();
        var linkedOrgs = new List<LinkedOrganizationDto>();

        if (isBusiness)
        {
            userRole = "OWNER";
            permissions = new List<string>
            {
                "organization:profile:edit", "organization:settings:edit", "organization:workspace:view", "organization:roles:manage", "organization:roles:view",
                "organization:members:manage", "organization:members:view", "identity:verification:initiate", "identity:verification:approve",
                "identity:verification:reject", "evidence:graph:validate", "evidence:graph:comment", "analysis:repository:sync",
                "analysis:repository:run", "analysis:repository:configure", "trust:metric:view", "trust:flag:manage",
                "ai:interview:configure", "ai:interview:conduct", "ai:interview:evaluate", "candidate:trust:score",
                "candidate:trust:override", "organization:audit:view", "billing:invoice:view", "billing:subscription:manage"
            };
        }
        else
        {
            var membership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(om => om.OrganizationId == org.Id && om.UserId == userId, cancellationToken);
            if (membership != null && membership.Status == "active")
            {
                userRole = membership.Role;
                var userPerms = await _authorizationService.GetPermissionsAsync(userId, org.Id, cancellationToken);
                var allDbPermissions = await _context.Permissions
                    .Select(p => p.Name)
                    .ToListAsync(cancellationToken);
                permissions = allDbPermissions
                    .Where(p => PermissionEvaluator.HasPermission(userPerms, p, org.Id))
                    .ToList();

                linkedOrgs = await _context.OrganizationMemberships
                    .Where(om => om.UserId == userId && om.OrganizationId != org.Id && om.Status == "active")
                    .Include(om => om.Organization)
                    .Select(om => new LinkedOrganizationDto(om.Organization.Name, om.Organization.Username))
                    .ToListAsync(cancellationToken);
            }
        }

        var responseDto = MapToWorkspaceDetailsDto(
            org,
            userRole,
            linkedOrgs,
            permissions,
            workspaces,
            signedBannerUrl,
            signedLogoUrl,
            signedGalleryUrls
        );

        return Ok(responseDto);
    }

    [HttpPost("{organizationSlug}/follow")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FollowToggleResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleFollowWorkspace(string organizationSlug, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        // Business accounts cannot follow organizations
        var actorTypeClaim = User.FindFirst("actor_type")?.Value;
        if (string.Equals(actorTypeClaim, "business", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized();
        }

        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Username.ToLower() == organizationSlug.ToLower() && o.DeletedAt == null, cancellationToken);

        if (org == null)
        {
            return NotFound(new { message = "Organization not found" });
        }

        var existing = await _context.OrganizationFollowers
            .FirstOrDefaultAsync(f => f.UserId == userId && f.OrganizationId == org.Id, cancellationToken);

        bool isFollowing;
        if (existing == null)
        {
            // Follow
            _context.OrganizationFollowers.Add(new OrganizationFollower
            {
                UserId = userId,
                OrganizationId = org.Id,
                FollowedAt = DateTimeOffset.UtcNow
            });
            org.FollowerCount = Math.Max(0, org.FollowerCount + 1);
            isFollowing = true;
        }
        else
        {
            // Unfollow
            _context.OrganizationFollowers.Remove(existing);
            org.FollowerCount = Math.Max(0, org.FollowerCount - 1);
            isFollowing = false;
        }

        org.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new FollowToggleResponseDto(org.FollowerCount, isFollowing));
    }

    private WorkspaceDetailsDto MapToWorkspaceDetailsDto(
        Organization org,
        string? userRole,
        List<LinkedOrganizationDto> linkedOrganizations,
        List<string> permissions,
        List<WorkspaceDto> workspaces,
        string? signedBannerUrl,
        string? signedLogoUrl,
        List<string> signedGalleryUrls,
        int followerCount = 0,
        bool isFollowing = false)
    {
        return new WorkspaceDetailsDto(
            org.Id,
            org.Name,
            org.Username,
            userRole,
            linkedOrganizations,
            permissions,
            workspaces,
            signedBannerUrl,
            signedLogoUrl,
            org.CompanyType,
            org.CompanySize,
            org.BranchCount,
            org.IndustryTags ?? new List<string>(),
            org.Description,
            org.BenefitTags ?? new List<string>(),
            signedGalleryUrls,
            org.ContactName,
            org.ContactPhone,
            org.ContactEmail,
            org.City,
            org.DetailAddress,
            org.GoogleMapsEmbedUrl,
            org.LinkedinUrl,
            org.FacebookUrl,
            org.TwitterUrl,
            org.Website,
            org.TaxCode,
            followerCount,
            isFollowing
        );
    }

    private async Task<string?> GetSignedUrlAsync(string? url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(url))
        {
            return null;
        }

        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
            url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return url;
        }

        try
        {
            return await _storageService.GetSignedUrlAsync(url, TimeSpan.FromHours(24), cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    [HttpGet("{organizationSlug}/members")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedMembersResponseDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkspaceMembers(
        string organizationSlug,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool publicOnly = false)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        Guid? userId = null;
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsedId))
        {
            userId = parsedId;
        }

        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Username.ToLower() == organizationSlug.ToLower() && o.DeletedAt == null);

        if (org == null)
        {
            return NotFound(new { message = "Organization not found" });
        }

        bool limitToPublic = userId == null || publicOnly;

        if (!limitToPublic)
        {
            var actorTypeClaim = User.FindFirst("actor_type")?.Value;
            bool isBusiness = string.Equals(actorTypeClaim, "business", StringComparison.OrdinalIgnoreCase);

            if (isBusiness)
            {
                if (org.Id != userId.Value)
                {
                    return Forbid();
                }
            }
            else
            {
                // Authorize permission using centralized authorization service
                var isAuthorized = await _authorizationService.AuthorizeAsync(userId.Value, org.Id, OrganizationPermissions.ViewMembers);
                if (!isAuthorized)
                {
                    return Forbid();
                }
            }
        }

        var baseQuery = (from om in _context.OrganizationMemberships.Where(om => om.OrganizationId == org.Id && om.Status == "active")
                         join up in _context.UserProfiles on om.UserId equals up.UserId into upGroup
                         from up in upGroup.DefaultIfEmpty()
                         select new { om, up }).AsNoTracking();

        if (limitToPublic)
        {
            baseQuery = baseQuery.Where(x => x.up != null && x.up.ProfileVisibility == "public");
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            baseQuery = baseQuery.Where(x => 
                x.om.User.FullName.ToLower().Contains(searchLower) ||
                x.om.User.Email.ToLower().Contains(searchLower)
            );
        }

        var totalCount = await baseQuery.CountAsync();
        var items = await baseQuery
            .OrderBy(x => x.om.User.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new {
                x.om.UserId,
                x.om.User.FullName,
                x.om.User.Email,
                x.om.Role,
                x.om.Status,
                Headline = x.up != null ? x.up.Headline : null,
                Username = x.up != null ? x.up.Username : null,
                x.om.User.AvatarUrl
            })
            .ToListAsync();

        var dtoList = new List<MemberDto>();
        foreach (var x in items)
        {
            var signedAvatar = await GetSignedUrlAsync(x.AvatarUrl);
            dtoList.Add(new MemberDto(
                x.UserId,
                x.FullName,
                x.Email,
                x.Role,
                x.Status,
                x.Headline,
                x.Username,
                signedAvatar
            ));
        }

        return Ok(new PaginatedMembersResponseDto(dtoList, totalCount, page, pageSize));
    }

    [HttpPost("{organizationSlug}/banner")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkspaceAvatarUploadResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadBanner(
        string organizationSlug,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File payload is empty or missing.");
        }

        if (file.Length > StorageConstants.MaxProfileSize)
        {
            return BadRequest($"File size exceeds the maximum allowed limit of {StorageConstants.MaxProfileSize / (1024 * 1024)}MB.");
        }

        if (!StorageConstants.AllowedImageTypes.Contains(file.ContentType))
        {
            return BadRequest($"MIME type '{file.ContentType}' is not supported. Only JPEG, PNG, WebP, and GIF are allowed.");
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Username.ToLower() == organizationSlug.ToLower() && o.DeletedAt == null, cancellationToken);

        if (org == null)
        {
            return NotFound(new { message = "Organization not found" });
        }

        var actorTypeClaim = User.FindFirst("actor_type")?.Value;
        bool isBusiness = string.Equals(actorTypeClaim, "business", StringComparison.OrdinalIgnoreCase);

        if (isBusiness)
        {
            if (org.Id != userId)
            {
                return Forbid();
            }
        }
        else
        {
            var isAuthorized = await _authorizationService.AuthorizeAsync(userId, org.Id, OrganizationPermissions.EditProfile, cancellationToken: cancellationToken);
            if (!isAuthorized)
            {
                return Forbid();
            }
        }

        // Delete old banner from storage if exists
        if (!string.IsNullOrEmpty(org.BannerUrl) && 
            !org.BannerUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && 
            !org.BannerUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await _storageService.DeleteFileAsync(org.BannerUrl, cancellationToken);
            }
            catch
            {
                // Log and ignore
            }
        }

        // Physical upload to R2
        using var fileStream = file.OpenReadStream();
        var uploadedFile = await _storageService.UploadFileAsync(
            fileStream,
            file.FileName,
            file.ContentType,
            StorageModule.Profile,
            null,
            cancellationToken);

        // Update organization record
        org.BannerUrl = uploadedFile.ObjectKey;
        org.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Generate signed URL
        var signedUrl = await _storageService.GetSignedUrlAsync(
            uploadedFile.ObjectKey,
            TimeSpan.FromHours(24),
            cancellationToken);

        return Ok(new WorkspaceAvatarUploadResponse(signedUrl));
    }

    [HttpPost("{organizationSlug}/avatar")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkspaceAvatarUploadResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadAvatar(
        string organizationSlug,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File payload is empty or missing.");
        }

        if (file.Length > StorageConstants.MaxProfileSize)
        {
            return BadRequest($"File size exceeds the maximum allowed limit of {StorageConstants.MaxProfileSize / (1024 * 1024)}MB.");
        }

        if (!StorageConstants.AllowedImageTypes.Contains(file.ContentType))
        {
            return BadRequest($"MIME type '{file.ContentType}' is not supported. Only JPEG, PNG, WebP, and GIF are allowed.");
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Username.ToLower() == organizationSlug.ToLower() && o.DeletedAt == null, cancellationToken);

        if (org == null)
        {
            return NotFound(new { message = "Organization not found" });
        }

        var actorTypeClaim = User.FindFirst("actor_type")?.Value;
        bool isBusiness = string.Equals(actorTypeClaim, "business", StringComparison.OrdinalIgnoreCase);

        if (isBusiness)
        {
            if (org.Id != userId)
            {
                return Forbid();
            }
        }
        else
        {
            var isAuthorized = await _authorizationService.AuthorizeAsync(userId, org.Id, OrganizationPermissions.EditProfile, cancellationToken: cancellationToken);
            if (!isAuthorized)
            {
                return Forbid();
            }
        }

        // Delete old logo from storage if exists
        if (!string.IsNullOrEmpty(org.LogoUrl) && 
            !org.LogoUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && 
            !org.LogoUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                await _storageService.DeleteFileAsync(org.LogoUrl, cancellationToken);
            }
            catch
            {
                // Log and ignore
            }
        }

        // Physical upload to R2
        using var fileStream = file.OpenReadStream();
        var uploadedFile = await _storageService.UploadFileAsync(
            fileStream,
            file.FileName,
            file.ContentType,
            StorageModule.Profile,
            null,
            cancellationToken);

        // Update organization record
        org.LogoUrl = uploadedFile.ObjectKey;
        org.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Generate signed URL
        var signedUrl = await _storageService.GetSignedUrlAsync(
            uploadedFile.ObjectKey,
            TimeSpan.FromHours(24),
            cancellationToken);

        return Ok(new WorkspaceAvatarUploadResponse(signedUrl));
    }
}
