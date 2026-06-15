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

    private async Task<JobVacancyDto> MapToJobVacancyDtoAsync(JobVacancy job, CancellationToken cancellationToken)
    {
        var signedCoverUrl = await GetSignedUrlAsync(job.CoverUrl, cancellationToken) ?? job.CoverUrl;
        var signedImages = new List<string>();
        if (job.Images != null)
        {
            foreach (var img in job.Images)
            {
                var signedImg = await GetSignedUrlAsync(img, cancellationToken);
                if (signedImg != null) signedImages.Add(signedImg);
            }
        }

        return new JobVacancyDto(
            job.Id,
            job.OrganizationId,
            job.Title,
            job.Department,
            job.WorkplaceType,
            job.City,
            job.Type,
            job.Salary,
            job.SalaryMinMax,
            job.Headcount,
            job.Gender,
            job.Experience,
            job.Degree,
            job.Category,
            job.Description,
            job.Requirements,
            job.Benefits,
            job.Tags,
            job.Skills,
            signedCoverUrl,
            signedImages,
            job.IsActive,
            job.CreatedAt,
            job.UpdatedAt
        );
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

        List<Guid>? publicUserIds = null;
        if (limitToPublic)
        {
            publicUserIds = await _context.Database.SqlQueryRaw<Guid>(
                "SELECT user_id FROM user_profiles WHERE profile_visibility = 'public'"
            ).ToListAsync();
        }

        var query = _context.OrganizationMemberships
            .AsNoTracking()
            .Where(om => om.OrganizationId == org.Id && om.Status == "active");

        if (publicUserIds != null)
        {
            query = query.Where(om => publicUserIds.Contains(om.UserId));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(om => 
                om.User.FullName.ToLower().Contains(searchLower) ||
                om.User.Email.ToLower().Contains(searchLower)
            );
        }

        var totalCount = await query.CountAsync();
        var members = await query
            .Include(om => om.User)
            .OrderBy(om => om.User.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var memberUserIds = members.Select(m => m.UserId).ToList();
        var profileMap = new Dictionary<Guid, MemberProfileDataDto>();

        if (memberUserIds.Count > 0)
        {
            var profiles = await _context.Database.SqlQueryRaw<MemberProfileDataDto>(
                "SELECT user_id as \"UserId\", headline as \"Headline\", username as \"Username\" FROM user_profiles WHERE user_id = ANY({0})",
                memberUserIds.ToArray()
            ).ToListAsync();
            
            profileMap = profiles.ToDictionary(p => p.UserId);
        }

        var dtoList = new List<MemberDto>();
        foreach (var m in members)
        {
            profileMap.TryGetValue(m.UserId, out var prof);
            var signedAvatar = await GetSignedUrlAsync(m.User.AvatarUrl);
            dtoList.Add(new MemberDto(
                m.UserId,
                m.User.FullName,
                m.User.Email,
                m.Role,
                m.Status,
                prof?.Headline,
                prof?.Username,
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

    [HttpPost("{organizationSlug}/media/upload")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadMedia(
        string organizationSlug,
        [FromForm] List<IFormFile> files,
        CancellationToken cancellationToken)
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest("No files uploaded.");
        }

        if (files.Count > 5)
        {
            return BadRequest("Cannot upload more than 5 images at once.");
        }

        foreach (var file in files)
        {
            if (file.Length > StorageConstants.MaxProfileSize)
            {
                return BadRequest($"File '{file.FileName}' size exceeds the maximum allowed limit of {StorageConstants.MaxProfileSize / (1024 * 1024)}MB.");
            }

            if (!StorageConstants.AllowedImageTypes.Contains(file.ContentType))
            {
                return BadRequest($"MIME type '{file.ContentType}' is not supported for file '{file.FileName}'. Only JPEG, PNG, WebP, and GIF are allowed.");
            }
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
            var isMember = await _context.OrganizationMemberships
                .AnyAsync(om => om.OrganizationId == org.Id && om.UserId == userId && om.Status == "active", cancellationToken);
            
            if (!isMember)
            {
                return Forbid();
            }
        }

        var uploadedUrls = new List<string>();
        foreach (var file in files)
        {
            using var fileStream = file.OpenReadStream();
            var uploadedFile = await _storageService.UploadFileAsync(
                fileStream,
                file.FileName,
                file.ContentType,
                StorageModule.Profile,
                null,
                cancellationToken);

            var signedUrl = await _storageService.GetSignedUrlAsync(
                uploadedFile.ObjectKey,
                TimeSpan.FromDays(7),
                cancellationToken);

            uploadedUrls.Add(signedUrl);
        }

        return Ok(uploadedUrls);
    }

    [HttpPost("{organizationSlug}/posts")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorkspacePostDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePost(
        string organizationSlug,
        [FromBody] CreateWorkspacePostRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Content is required.");
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

        string authorRole = "Administrator";
        if (isBusiness)
        {
            if (org.Id != userId)
            {
                return Forbid();
            }
            authorRole = "OWNER";
        }
        else
        {
            var membership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(om => om.OrganizationId == org.Id && om.UserId == userId && om.Status == "active", cancellationToken);
            
            if (membership == null)
            {
                return Forbid();
            }
            authorRole = membership.Role;
        }

        var post = new WorkspacePost
        {
            Id = Guid.CreateVersion7(),
            OrganizationId = org.Id,
            CreatedByUserId = userId,
            Category = request.Category,
            Content = request.Content,
            Images = request.ImageUrls ?? request.Images ?? new List<string>(),
            Likes = 0,
            SharesCount = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.WorkspacePosts.Add(post);
        await _context.SaveChangesAsync(cancellationToken);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        var signedAvatar = user != null ? await GetSignedUrlAsync(user.AvatarUrl) : null;

        var dto = new WorkspacePostDto(
            post.Id,
            post.Category,
            post.Content,
            post.Images,
            post.Likes,
            post.SharesCount,
            post.CreatedAt,
            user?.FullName ?? "Manager",
            signedAvatar,
            authorRole
        );

        return Ok(dto);
    }

    [HttpGet("{organizationSlug}/posts")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WorkspacePostDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPosts(
        string organizationSlug,
        CancellationToken cancellationToken)
    {
        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Username.ToLower() == organizationSlug.ToLower() && o.DeletedAt == null, cancellationToken);

        if (org == null)
        {
            return NotFound(new { message = "Organization not found" });
        }

        bool isAuthorizedToSeeAuthor = false;
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            var actorTypeClaim = User?.FindFirst("actor_type")?.Value;
            bool isBusiness = string.Equals(actorTypeClaim, "business", StringComparison.OrdinalIgnoreCase);

            if (isBusiness)
            {
                if (org.Id == userId)
                {
                    isAuthorizedToSeeAuthor = true;
                }
            }
            else
            {
                isAuthorizedToSeeAuthor = await _context.OrganizationMemberships
                    .AnyAsync(om => om.OrganizationId == org.Id && om.UserId == userId && om.Status == "active", cancellationToken);
            }
        }

        var postsQuery = _context.WorkspacePosts
            .Where(wp => wp.OrganizationId == org.Id)
            .OrderByDescending(wp => wp.CreatedAt);

        var postsList = await postsQuery.ToListAsync(cancellationToken);

        if (postsList.Count == 0)
        {
            var mockUser = await _context.Users.FirstOrDefaultAsync(cancellationToken);
            var mockUserId = mockUser?.Id ?? Guid.NewGuid();
            var mockPosts = new List<WorkspacePost>
            {
                new WorkspacePost
                {
                    Id = Guid.CreateVersion7(),
                    OrganizationId = org.Id,
                    CreatedByUserId = mockUserId,
                    Category = "Engineering",
                    Content = "Chúng tôi vô cùng tự hào thông báo rằng quy trình đánh giá và xác thực lập trình viên trên CVerify đã chính thức tích hợp chữ ký mật mã hóa (cryptographic credential signatures)! Việc này giúp tự động hóa 100% quy trình kiểm thử năng lực thực tế từ kho lưu trữ mã nguồn của ứng viên.\n\nĐặc biệt, đại diện CVerify cùng đối tác đã ký kết biên bản ghi nhớ hợp tác chiến lược nhằm xây dựng cộng đồng kỹ sư công nghệ chất lượng cao, bảo mật và đáng tin cậy. Dưới đây là một số hình ảnh sự kiện ký kết và hoạt động triển khai thực tế của đội ngũ kỹ sư tại văn phòng Đà Nẵng.",
                    Images = new List<string>
                    {
                        "https://images.unsplash.com/photo-1542744173-8e7e53415bb0?q=80&w=800",
                        "https://images.unsplash.com/photo-1531538606174-0f90ff5dce83?q=80&w=800"
                    },
                    Likes = 88,
                    SharesCount = 14,
                    CreatedAt = DateTimeOffset.UtcNow.AddHours(-1)
                },
                new WorkspacePost
                {
                    Id = Guid.CreateVersion7(),
                    OrganizationId = org.Id,
                    CreatedByUserId = mockUserId,
                    Category = "Recruitment",
                    Content = "WE ARE HIRING! GIA NHẬP ĐỘI NGŨ CÔNG NGHỆ CỦA CHÚNG TÔI.\n\nNhằm mở rộng quy mô dự án và đáp ứng nhu cầu tăng trưởng trong giai đoạn mới, chúng tôi tìm kiếm các đồng nghiệp tài năng ở các vị trí:\n1. Senior Full-Stack Developer (.NET & React)\n2. Automated QA Engineer\n3. DevOps Engineer (Platform Team)\n\nChúng tôi mang đến môi trường làm việc Hybrid linh hoạt, chế độ đãi ngộ cạnh tranh, hỗ trợ thiết bị làm việc hiện đại hàng đầu cùng cơ hội phát triển bản thân vượt trội. Hãy truy cập ngay tab 'Jobs' để xem chi tiết mô tả công việc và ứng tuyển trực tiếp bằng hồ sơ đã xác thực nhé!",
                    Images = new List<string> { "https://images.unsplash.com/photo-1521737711867-e3b90473bd58?q=80&w=800" },
                    Likes = 42,
                    SharesCount = 5,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-2)
                }
            };
            _context.WorkspacePosts.AddRange(mockPosts);
            await _context.SaveChangesAsync(cancellationToken);
            postsList = await postsQuery.ToListAsync(cancellationToken);
        }

        var dtoList = new List<WorkspacePostDto>();
        foreach (var post in postsList)
        {
            string? authorName = null;
            string? authorAvatar = null;
            string? authorRole = null;

            if (isAuthorizedToSeeAuthor)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == post.CreatedByUserId, cancellationToken);
                var membership = await _context.OrganizationMemberships.FirstOrDefaultAsync(om => om.OrganizationId == org.Id && om.UserId == post.CreatedByUserId, cancellationToken);
                authorName = user?.FullName ?? "Manager";
                authorAvatar = user != null ? await GetSignedUrlAsync(user.AvatarUrl) : null;
                authorRole = membership?.Role ?? "OWNER";
            }

            dtoList.Add(new WorkspacePostDto(
                post.Id,
                post.Category,
                post.Content,
                post.Images,
                post.Likes,
                post.SharesCount,
                post.CreatedAt,
                authorName,
                authorAvatar,
                authorRole
            ));
        }

        return Ok(dtoList);
    }

    [HttpPost("{organizationSlug}/jobs")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JobVacancyDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateJob(
        string organizationSlug,
        [FromBody] CreateJobRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Job Title is required.");
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
            var membership = await _context.OrganizationMemberships
                .FirstOrDefaultAsync(om => om.OrganizationId == org.Id && om.UserId == userId && om.Status == "active", cancellationToken);
            
            if (membership == null)
            {
                return Forbid();
            }
        }

        var job = new JobVacancy
        {
            Id = Guid.CreateVersion7(),
            OrganizationId = org.Id,
            Title = request.Title,
            Department = request.Department,
            WorkplaceType = request.WorkplaceType,
            City = request.City,
            Type = request.Type,
            Salary = request.Salary,
            SalaryMinMax = request.SalaryMinMax,
            Headcount = request.Headcount,
            Gender = request.Gender,
            Experience = request.Experience,
            Degree = request.Degree,
            Category = request.Category,
            Description = request.Description ?? new List<string>(),
            Requirements = request.Requirements ?? new List<string>(),
            Benefits = request.Benefits ?? new List<string>(),
            Tags = request.Tags ?? new List<string>(),
            Skills = request.Skills ?? new List<string>(),
            CoverUrl = request.CoverUrl,
            Images = request.ImageUrls ?? request.Images ?? new List<string>(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.JobVacancies.Add(job);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = await MapToJobVacancyDtoAsync(job, cancellationToken);
        return Ok(dto);
    }

    [HttpGet("{organizationSlug}/jobs")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<JobVacancyDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobs(
        string organizationSlug,
        CancellationToken cancellationToken)
    {
        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Username.ToLower() == organizationSlug.ToLower() && o.DeletedAt == null, cancellationToken);

        if (org == null)
        {
            return NotFound(new { message = "Organization not found" });
        }

        var jobsList = await _context.JobVacancies
            .Where(jv => jv.OrganizationId == org.Id && jv.IsActive)
            .OrderByDescending(jv => jv.CreatedAt)
            .ToListAsync(cancellationToken);

        if (jobsList.Count == 0)
        {
            // Seed default mock jobs for this organization
            var mockJobs = new List<JobVacancy>
            {
                new JobVacancy
                {
                    Id = Guid.CreateVersion7(),
                    OrganizationId = org.Id,
                    Title = "Senior Full-Stack Developer (.NET & React)",
                    Department = "Engineering",
                    WorkplaceType = "Hybrid",
                    City = "Hà Nội",
                    Type = "Full-Time",
                    Salary = "$ 2,000 - 4,500 USD",
                    SalaryMinMax = "50 - 110 triệu",
                    Headcount = 3,
                    Gender = "Không yêu cầu",
                    Experience = "5+ năm kinh nghiệm",
                    Degree = "Đại học / Kỹ sư",
                    Category = "Phát triển phần mềm, Công nghệ thông tin",
                    Description = new List<string>
                    {
                        "Thiết kế và phát triển kiến trúc hệ thống backend microservices bằng .NET Core 8 và cơ sở dữ liệu PostgreSQL.",
                        "Xây dựng giao diện ứng dụng web Single Page Application (SPA) hiệu năng cao, mượt mà bằng React, TypeScript và quản lý trạng thái qua Zustand/Redux.",
                        "Tối ưu hóa các truy vấn SQL nâng cao và cấu hình bộ nhớ cache Redis phân tán.",
                        "Viết mã nguồn kiểm thử tự động (Unit Test / Integration Test) đảm bảo độ ổn định cao trước khi bàn giao hệ thống.",
                        "Tham gia hướng dẫn kỹ thuật, code review và hỗ trợ các thành viên junior trong đội ngũ."
                    },
                    Requirements = new List<string>
                    {
                        "Tốt nghiệp đại học chuyên ngành Công nghệ thông tin, Khoa học máy tính hoặc tương đương.",
                        "Tối thiểu 5 năm kinh nghiệm thực chiến phát triển ứng dụng web, có kiến thức sâu rộng về lập trình hướng đối tượng OOP và các Design Pattern.",
                        "Thành thạo ngôn ngữ C#, ASP.NET Core, Entity Framework Core và lập trình bất đồng bộ.",
                        "Kinh nghiệm làm việc sâu sắc với ReactJS, Hooks, state management và thư viện CSS như Tailwind/Vanilla CSS.",
                        "Kinh nghiệm thiết kế API RESTful chất lượng, hiểu biết tốt về CI/CD và Git."
                    },
                    Benefits = new List<string>
                    {
                        "Lương thưởng hấp dẫn lên tới $4,500 USD cùng tháng lương thứ 13 và thưởng hiệu suất cuối năm.",
                        "Được cung cấp đầy đủ trang thiết bị làm việc hiện đại cao cấp (MacBook Pro / Dell XPS và màn hình phụ).",
                        "Gói bảo hiểm chăm sóc sức khỏe cao cấp toàn diện cho bản thân và gia đình.",
                        "Hưởng 15 ngày phép có lương trong năm và chế độ nghỉ lễ tết theo luật lao động.",
                        "Tham gia các chương trình đào tạo kỹ năng chuyên sâu và chứng chỉ công nghệ quốc tế miễn phí."
                    },
                    Tags = new List<string> { "React", "TypeScript", ".NET Core", "C#", "Microservices" },
                    Skills = new List<string> { "C#", ".NET Core", "React", "TypeScript", "PostgreSQL", "Zustand" },
                    CoverUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?q=80&w=600",
                    Images = new List<string>
                    {
                        "https://images.unsplash.com/photo-1555066931-4365d14bab8c?q=80&w=600",
                        "https://images.unsplash.com/photo-1542744173-8e7e53415bb0?q=80&w=600",
                        "https://images.unsplash.com/photo-1517245386807-bb43f82c33c4?q=80&w=600"
                    },
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow.AddHours(-1),
                    UpdatedAt = DateTimeOffset.UtcNow.AddHours(-1)
                },
                new JobVacancy
                {
                    Id = Guid.CreateVersion7(),
                    OrganizationId = org.Id,
                    Title = "Automated Verification QA Engineer",
                    Department = "Quality Assurance",
                    WorkplaceType = "Remote",
                    City = "Đà Nẵng",
                    Type = "Contract",
                    Salary = "$ 1,200 - 2,500 USD",
                    SalaryMinMax = "30 - 62 triệu",
                    Headcount = 2,
                    Gender = "Không yêu cầu",
                    Experience = "3+ năm kinh nghiệm",
                    Degree = "Đại học / Cao đẳng",
                    Category = "Kiểm thử phần mềm, Quality Assurance",
                    Description = new List<string>
                    {
                        "Thiết kế, xây dựng và duy trì các kịch bản kiểm thử tự động (Automated Test Scripts) cho hệ thống xác thực cryptographic của CVerify.",
                        "Viết và tối ưu hóa các bộ test suite kiểm tra hiệu năng (Performance Test) và độ tin cậy của chuỗi dữ liệu băm.",
                        "Tích hợp các bài kiểm thử tự động vào hệ thống CI/CD thông qua GitHub Actions.",
                        "Phối hợp chặt chẽ với đội ngũ phát triển sản phẩm để tìm kiếm, phân tích và theo dõi các lỗi phát sinh.",
                        "Tạo các báo cáo kiểm thử chi tiết và đề xuất các giải pháp nâng cao chất lượng sản phẩm."
                    },
                    Requirements = new List<string>
                    {
                        "Tối thiểu 3 năm kinh nghiệm làm kỹ sư kiểm thử tự động (Auto QA).",
                        "Thành thạo ít nhất một trong các công cụ viết test tự động: Playwright, Cypress hoặc Selenium.",
                        "Có kinh nghiệm làm việc với ngôn ngữ lập trình JavaScript/TypeScript hoặc Python.",
                        "Có kiến thức căn bản về mật mã học, mã băm (hashing), chữ ký số là một lợi thế lớn.",
                        "Tư duy phân tích lỗi tốt, cẩn thận, tỉ mỉ và giao tiếp hiệu quả."
                    },
                    Benefits = new List<string>
                    {
                        "Mức lương thỏa thuận cạnh tranh cao tương xứng theo năng lực thực tế.",
                        "Làm việc từ xa (Remote) 100% giúp chủ động cân bằng thời gian và cuộc sống.",
                        "Được cung cấp gói ngân sách hỗ trợ nâng cấp thiết bị cá nhân hàng năm.",
                        "Tham gia hoạt động teambuilding thường niên cùng công ty tại các resort đẳng cấp.",
                        "Được tài trợ chi phí thi các chứng chỉ quốc tế chuyên ngành kiểm thử (ISTQB...)."
                    },
                    Tags = new List<string> { "Automation", "Playwright", "Cypress", "QA", "CI/CD" },
                    Skills = new List<string> { "Playwright", "Cypress", "QA Testing", "TypeScript", "CI/CD" },
                    CoverUrl = "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?q=80&w=600",
                    Images = new List<string>
                    {
                        "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?q=80&w=600",
                        "https://images.unsplash.com/photo-1531403009284-440f080d1e12?q=80&w=600"
                    },
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-2),
                    UpdatedAt = DateTimeOffset.UtcNow.AddDays(-2)
                },
                new JobVacancy
                {
                    Id = Guid.CreateVersion7(),
                    OrganizationId = org.Id,
                    Title = "Lead UI/UX Product Designer",
                    Department = "Design",
                    WorkplaceType = "On-site",
                    City = "Hà Nội",
                    Type = "Full-Time",
                    Salary = "$ 1,500 - 3,200 USD",
                    SalaryMinMax = "38 - 80 triệu",
                    Headcount = 1,
                    Gender = "Không yêu cầu",
                    Experience = "4+ năm kinh nghiệm",
                    Degree = "Đại học / Cao đẳng Mỹ thuật",
                    Category = "Thiết kế đồ họa, UI/UX Design",
                    Description = new List<string>
                    {
                        "Chịu trách nhiệm thiết kế giao diện (UI) và xây dựng trải nghiệm người dùng (UX) cho các hệ thống phần mềm của CVerify.",
                        "Xây dựng wireframe, prototype và sơ đồ luồng trải nghiệm người dùng (user flow) dựa trên hoạt động nghiên cứu hành vi khách hàng.",
                        "Tổ chức, thiết lập và mở rộng hệ thống thiết kế (Design System) của công ty trên Figma đảm bảo tính nhất quán cao.",
                        "Hợp tác chặt chẽ cùng Product Manager và Tech Lead để thẩm định thiết kế trước khi chuyển giao lập trình.",
                        "Thực hiện đo lường, phân tích hành vi và phản hồi từ người dùng thực tế để liên tục cải tiến sản phẩm."
                    },
                    Requirements = new List<string>
                    {
                        "Tối thiểu 4 năm kinh nghiệm thiết kế giao diện ứng dụng web dashboard, nền tảng SaaS phức tạp.",
                        "Kỹ năng sử dụng Figma xuất sắc (thành thạo Auto-layout, Variables, Components, Prototyping nâng cao).",
                        "Có tư duy logic tốt về trải nghiệm người dùng (UX), khả năng phân tích và giải quyết các bài toán thiết kế khó.",
                        "Có portfolio chất lượng cao trình bày chi tiết tư duy thiết kế qua các dự án thực tế.",
                        "Hiểu biết căn bản về HTML/CSS là lợi thế lớn giúp phối hợp ăn ý với đội ngũ frontend."
                    },
                    Benefits = new List<string>
                    {
                        "Mức lương cạnh tranh hấp dẫn cùng các phụ cấp ăn trưa, đi lại tại văn phòng.",
                        "Môi trường làm việc năng động, không gian văn phòng hạng A hiện đại và rộng rãi.",
                        "Thưởng hiệu suất công việc định kỳ và xét tăng lương định kỳ 2 lần/năm.",
                        "Chương trình khám sức khỏe tổng quát định kỳ hàng năm tại hệ thống bệnh viện quốc tế.",
                        "Hỗ trợ 100% chi phí tham gia các khóa học chuyên sâu nâng cao chuyên môn tự chọn."
                    },
                    Tags = new List<string> { "Figma", "UI/UX", "Product Design", "Design System", "Wireframing" },
                    Skills = new List<string> { "Figma", "UI/UX", "Product Design", "Design System", "Wireframing" },
                    CoverUrl = "https://images.unsplash.com/photo-1581291518633-83b4ebd1d83e?q=80&w=600",
                    Images = new List<string>
                    {
                        "https://images.unsplash.com/photo-1581291518633-83b4ebd1d83e?q=80&w=600",
                        "https://images.unsplash.com/photo-1586717791821-3f44a563fa4c?q=80&w=600"
                    },
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-5),
                    UpdatedAt = DateTimeOffset.UtcNow.AddDays(-5)
                }
            };

            _context.JobVacancies.AddRange(mockJobs);
            await _context.SaveChangesAsync(cancellationToken);

            jobsList = await _context.JobVacancies
                .Where(jv => jv.OrganizationId == org.Id && jv.IsActive)
                .OrderByDescending(jv => jv.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        var dtoList = new List<JobVacancyDto>();
        foreach (var job in jobsList)
        {
            dtoList.Add(await MapToJobVacancyDtoAsync(job, cancellationToken));
        }

        return Ok(dtoList);
    }
}
