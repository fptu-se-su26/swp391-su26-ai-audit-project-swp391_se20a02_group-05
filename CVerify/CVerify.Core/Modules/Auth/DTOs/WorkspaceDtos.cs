using System.Collections.Generic;

namespace CVerify.API.Modules.Auth.DTOs;

public record MemberDto(
    string Name,
    string Email,
    string Role,
    string Status
);

public record LinkedOrganizationDto(
    string Name,
    string Slug
);

public record WorkspaceDetailsDto(
    string OrganizationName,
    string OrganizationSlug,
<<<<<<< Updated upstream
    string UserRole,
    List<LinkedOrganizationDto> LinkedOrganizations
=======
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
    string? TaxCode = null
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
>>>>>>> Stashed changes
);

public record PaginatedMembersResponseDto(
    List<MemberDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);
