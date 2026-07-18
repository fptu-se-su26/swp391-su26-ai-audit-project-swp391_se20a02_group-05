
using System;
using System.Collections.Generic;
using CVerify.API.Modules.Shared.Domain.Entities;

namespace CVerify.API.Modules.Admin.DTOs;

public record UserListItemDto(
    Guid Id,
    string Email,
    string FullName,
    string Status,
    DateTimeOffset? LastLoginAt,
    List<string> Roles,
    int SessionVersion,
    DateTimeOffset CreatedAt
);

public record UpdateUserDto(
    string Status,
    List<string> Roles
);

public record RoleListItemDto(
    Guid Id,
    string Name,
    string DisplayName,
    string? Description,
    bool IsSystem,
    bool IsActive,
    Guid? ParentRoleId,
    List<string> Permissions,
    uint Version
);

public record CreateOrUpdateRoleDto(
    string Name,
    string DisplayName,
    string? Description,
    Guid? ParentRoleId,
    List<string> Permissions,
    uint? Version
);

public record AuditLogListItemDto(
    Guid Id,
    string? UserEmail,
    string EventType,
    string Description,
    string? IpAddress,
    string? UserAgent,
    DateTimeOffset CreatedAt
);

/// <summary>
/// A monitoring event pushed in by an internal service (e.g. CVerify.AI) over the
/// HMAC-authenticated ingest endpoint. Recorded as an audit log and surfaced to admins.
/// </summary>
public record MonitoringEventIngestDto(
    string EventType,
    string Message,
    string? Severity,
    string? Source,
    string? CorrelationId,
    Dictionary<string, object>? Details,
    DateTimeOffset? OccurredAt
);

/// <summary>
/// Realtime payload broadcast to the "admins" SignalR group so the admin UI can raise a toast.
/// </summary>
public record AdminMonitoringAlertDto(
    Guid Id,
    string EventType,
    string Severity,
    string Source,
    string Message,
    DateTimeOffset CreatedAt
);


public record AdminMemberListItemDto(
    Guid Id,
    Guid UserId,
    string Email,
    string FullName,
    string Status,
    DateTimeOffset? LastLoginAt,
    int SessionVersion,
    DateTimeOffset JoinedAt,
    List<AdminMemberRoleDto> Roles
);

public record AdminMemberRoleDto(
    Guid RoleId,
    string Name,
    string DisplayName,
    string ScopeType,
    Guid ScopeId
);

public record InviteAdminDto(
    string Email,
    List<Guid> RoleIds
);

public record UpdateAdminMemberDto(
    string Status,
    List<Guid> RoleIds
);

public record AdminInvitationListItemDto(
    Guid Id,
    string InviteeEmail,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? AcceptedAt,
    Guid? InvitedByUserId,
    string InvitedByUserEmail,
    List<AdminInvitationRoleDto> Roles
);

public record AdminInvitationRoleDto(
    Guid RoleId,
    string Name,
    string DisplayName
);

public record AcceptInvitationDto(
    string Token
);

