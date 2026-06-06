using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using CVerify.API.Modules.Auth.Services;
using CVerify.API.Modules.Profiles.DTOs;
using CVerify.API.Modules.Profiles.Entities;
using CVerify.API.Modules.Shared.Domain.Entities;
using CVerify.API.Modules.Shared.Exceptions;
using CVerify.API.Modules.Shared.Persistence;

namespace CVerify.API.Modules.Profiles.Services;

public class CareerService : ICareerService
{
    private readonly ApplicationDbContext _context;

    public CareerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CareerPreferenceResponse> GetCareerPreferenceByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var career = await _context.CareerPreferences
            .FirstOrDefaultAsync(cp => cp.UserId == userId, cancellationToken);

        if (career == null)
        {
            // Verify if user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken);
            if (!userExists)
            {
                throw new ResourceNotFoundException(ProfileErrorCodes.ProfileNotFound, "User not found.");
            }

            // Create default career preferences
            career = new CareerPreference
            {
                UserId = userId,
                AvailableForHire = true,
                PreferredLanguage = "en",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.CareerPreferences.Add(career);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var skills = await _context.UserSkills
            .Where(us => us.UserId == userId)
            .Select(us => us.Skill)
            .ToListAsync(cancellationToken);

        var locations = await _context.UserPreferredLocations
            .Where(upl => upl.UserId == userId)
            .Select(upl => upl.Location)
            .ToListAsync(cancellationToken);

        var employmentPrefs = await _context.UserEmploymentPreferences
            .Where(uep => uep.UserId == userId)
            .Select(uep => uep.PreferenceName)
            .ToListAsync(cancellationToken);

        return MapToResponse(career, skills, locations, employmentPrefs);
    }

    public async Task<CareerPreferenceResponse> UpdateCareerPreferenceAsync(
        Guid userId, 
        UpdateCareerPreferenceRequest request, 
        CancellationToken cancellationToken = default)
    {
        var career = await _context.CareerPreferences
            .FirstOrDefaultAsync(cp => cp.UserId == userId, cancellationToken);

        if (career == null)
        {
            throw new ResourceNotFoundException(ProfileErrorCodes.ProfileNotFound, "Career preferences not found.");
        }

        // Concurrency Control
        if (career.Version != request.Version)
        {
            throw new ProfileException(ProfileErrorCodes.ProfileConcurrencyConflict, "Career preferences were modified by another process. Please reload and try again.");
        }

        // Validation: Min Salary <= Max Salary
        if (request.ExpectedSalaryMin.HasValue && request.ExpectedSalaryMax.HasValue && request.ExpectedSalaryMin.Value > request.ExpectedSalaryMax.Value)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(request.ExpectedSalaryMax), new[] { "Maximum salary must be greater than or equal to minimum salary." } }
            }, "Minimum expected salary cannot exceed the maximum expected salary.");
        }

        // Validate and normalize preference list tags
        var preferredWorkEnvironments = ValidateAndNormalizeTags(request.PreferredWorkEnvironments, nameof(request.PreferredWorkEnvironments));
        var workStyles = ValidateAndNormalizeTags(request.WorkStyles, nameof(request.WorkStyles));
        var companyValues = ValidateAndNormalizeTags(request.CompanyValues, nameof(request.CompanyValues));
        var preferredLocations = ValidateAndNormalizeTags(request.PreferredLocations, nameof(request.PreferredLocations));
        var employmentPreferences = ValidateAndNormalizeTags(request.EmploymentPreferences, nameof(request.EmploymentPreferences));
        var skills = ValidateAndNormalizeTags(request.Skills, nameof(request.Skills));
        var desiredJobPositions = ValidateAndNormalizeTags(request.DesiredJobPositions, nameof(request.DesiredJobPositions));

        // Update properties
        career.AvailableForHire = request.AvailableForHire;
        career.PreferredLanguage = request.PreferredLanguage;
        career.JobTitlePreferences = request.JobTitlePreferences;
        career.SalaryExpectations = request.SalaryExpectations;
        career.RemotePreference = request.RemotePreference;
        career.OpenToWorkStatus = request.OpenToWorkStatus;
        career.PreferredWorkEnvironments = JsonSerializer.Serialize(preferredWorkEnvironments);
        career.WorkStyles = JsonSerializer.Serialize(workStyles);
        career.CompanyValues = JsonSerializer.Serialize(companyValues);
        career.DesiredJobPositions = JsonSerializer.Serialize(desiredJobPositions);
        career.ExpectedSalaryMin = request.ExpectedSalaryMin;
        career.ExpectedSalaryMax = request.ExpectedSalaryMax;
        career.ExpectedSalaryCurrency = request.ExpectedSalaryCurrency;
        career.ExpectedSalaryType = request.ExpectedSalaryType;
        career.ExpectedSalaryNegotiable = request.ExpectedSalaryNegotiable;
        career.IsExpectedSalaryVisible = request.IsExpectedSalaryVisible;
        career.WorkPreferenceNotes = request.WorkPreferenceNotes;
        career.UpdatedAt = DateTimeOffset.UtcNow;

        // Sync Skills
        var existingSkills = await _context.UserSkills
            .Where(us => us.UserId == userId)
            .ToListAsync(cancellationToken);
        _context.UserSkills.RemoveRange(existingSkills);

        var finalSkills = new List<string>();
        foreach (var s in skills)
        {
            var skill = new UserSkill
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Skill = s,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _context.UserSkills.Add(skill);
            finalSkills.Add(skill.Skill);
        }

        // Sync Locations
        var existingLocations = await _context.UserPreferredLocations
            .Where(upl => upl.UserId == userId)
            .ToListAsync(cancellationToken);
        _context.UserPreferredLocations.RemoveRange(existingLocations);

        var finalLocations = new List<string>();
        foreach (var loc in preferredLocations)
        {
            var location = new UserPreferredLocation
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Location = loc,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _context.UserPreferredLocations.Add(location);
            finalLocations.Add(location.Location);
        }

        // Sync Employment Preferences
        var existingEmpPrefs = await _context.UserEmploymentPreferences
            .Where(uep => uep.UserId == userId)
            .ToListAsync(cancellationToken);
        _context.UserEmploymentPreferences.RemoveRange(existingEmpPrefs);

        var finalEmpPrefs = new List<string>();
        foreach (var ep in employmentPreferences)
        {
            var empPref = new UserEmploymentPreference
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                PreferenceName = ep,
                CreatedAt = DateTimeOffset.UtcNow
            };
            _context.UserEmploymentPreferences.Add(empPref);
            finalEmpPrefs.Add(empPref.PreferenceName);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ProfileException(ProfileErrorCodes.ProfileConcurrencyConflict, "A concurrency conflict occurred. Please reload and try again.", ex);
        }

        return MapToResponse(career, finalSkills, finalLocations, finalEmpPrefs);
    }

    private static CareerPreferenceResponse MapToResponse(
        CareerPreference career, 
        List<string> skills, 
        List<string> locations, 
        List<string> employmentPrefs)
    {
        var preferredWorkEnvironments = string.IsNullOrEmpty(career.PreferredWorkEnvironments)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(career.PreferredWorkEnvironments) ?? new List<string>();

        var workStyles = string.IsNullOrEmpty(career.WorkStyles)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(career.WorkStyles) ?? new List<string>();

        var companyValues = string.IsNullOrEmpty(career.CompanyValues)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(career.CompanyValues) ?? new List<string>();

        var desiredJobPositions = string.IsNullOrEmpty(career.DesiredJobPositions)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(career.DesiredJobPositions) ?? new List<string>();

        return new CareerPreferenceResponse(
            career.UserId,
            career.AvailableForHire,
            career.PreferredLanguage,
            career.JobTitlePreferences,
            career.SalaryExpectations,
            career.RemotePreference,
            career.OpenToWorkStatus,
            skills,
            locations,
            employmentPrefs,
            career.Version,
            preferredWorkEnvironments,
            workStyles,
            companyValues,
            desiredJobPositions,
            career.ExpectedSalaryMin,
            career.ExpectedSalaryMax,
            career.ExpectedSalaryCurrency,
            career.ExpectedSalaryType,
            career.ExpectedSalaryNegotiable,
            career.IsExpectedSalaryVisible,
            career.WorkPreferenceNotes
        );
    }

    private static List<string> ValidateAndNormalizeTags(List<string>? tags, string fieldName)
    {
        if (tags == null) return new List<string>();

        if (tags.Count > 20)
        {
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { fieldName, new[] { "Maximum of 20 items is allowed." } }
            }, "Preference list exceeds maximum items limit.");
        }

        var normalized = new List<string>();
        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { fieldName, new[] { "Preference tag cannot be empty or whitespace." } }
                }, "Invalid preference tag.");
            }

            var trimmed = tag.Trim();
            if (trimmed.Length > 100)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { fieldName, new[] { $"Preference tag '{trimmed}' exceeds maximum length of 100 characters." } }
                }, "Preference tag too long.");
            }

            if (normalized.Any(t => string.Equals(t, trimmed, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { fieldName, new[] { $"Duplicate preference tag '{trimmed}' is not allowed." } }
                }, "Duplicate preference tag.");
            }

            normalized.Add(trimmed);
        }

        return normalized;
    }
}
