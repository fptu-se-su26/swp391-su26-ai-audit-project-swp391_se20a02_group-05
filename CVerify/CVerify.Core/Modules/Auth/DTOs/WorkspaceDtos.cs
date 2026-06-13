using System.Collections.Generic;

namespace CVerify.API.Modules.Auth.DTOs;

public record MemberDto(
    Guid UserId,
    string Name,
    string Email,
    string Role,
    string Status,
    string? Headline = null,
    string? Username = null,
    string? AvatarUrl = null
);

public record LinkedOrganizationDto(
    string Name,
    string Slug
);

public record WorkspaceDto(
    Guid Id,
    string DisplayName,
    string Slug
);

public record WorkspaceAvatarUploadResponse(
    string AvatarUrl
);

public record WorkspaceDetailsDto(
    Guid OrganizationId,
    string OrganizationName,
    string OrganizationSlug,
    string? UserRole,
    List<LinkedOrganizationDto> LinkedOrganizations,
    List<string> Permissions,
    List<WorkspaceDto> Workspaces,
    string? BannerUrl = null,
    string? LogoUrl = null,
    string? CompanyType = null,
    string? CompanySize = null,
    int BranchCount = 0,
    List<string>? IndustryTags = null,
    string? Description = null,
    List<string>? BenefitTags = null,
    List<string>? GalleryUrls = null,
    string? ContactName = null,
    string? ContactPhone = null,
    string? ContactEmail = null,
    string? City = null,
    string? DetailAddress = null,
    string? GoogleMapsEmbedUrl = null,
    string? LinkedinUrl = null,
    string? FacebookUrl = null,
    string? TwitterUrl = null,
    string? Website = null,
    string? TaxCode = null,
    int FollowerCount = 0,
    bool IsFollowing = false
);

public record UpdateWorkspaceDetailsRequestDto(
    string? Description,
    string? CompanyType,
    string? CompanySize,
    int BranchCount,
    List<string> IndustryTags,
    List<string> BenefitTags,
    string? ContactName,
    string? ContactPhone,
    string? ContactEmail,
    string? City,
    string? DetailAddress,
    string? GoogleMapsEmbedUrl,
    string? LinkedinUrl,
    string? FacebookUrl,
    string? TwitterUrl,
    string? Website
);

public record PaginatedMembersResponseDto(
    List<MemberDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public record FollowToggleResponseDto(
    int FollowerCount,
    bool IsFollowing
);
