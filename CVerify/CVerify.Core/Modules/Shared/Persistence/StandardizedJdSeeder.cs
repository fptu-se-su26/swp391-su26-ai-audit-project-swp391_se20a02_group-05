using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CVerify.API.Modules.Jd.Entities;
using CVerify.API.Modules.Shared.Configuration;

namespace CVerify.API.Modules.Shared.Persistence;

public static class StandardizedJdSeeder
{
    private static readonly Guid Tier1OrgId = Guid.Parse("01900000-0000-0000-0000-000000000001");

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static async Task SeedAsync(ApplicationDbContext context, SeedingSettings seeding, SeedingPolicy policy)
    {
        if (!policy.SeedDemoContent) return;

        var seedPath = seeding.JdSeedDataPath;
        if (!Path.IsPathRooted(seedPath))
        {
            var fromBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, seedPath);
            seedPath = File.Exists(fromBase) ? fromBase : Path.Combine(Directory.GetCurrentDirectory(), seedPath);
        }

        if (!File.Exists(seedPath)) return;

        var json = await File.ReadAllTextAsync(seedPath);
        var entries = JsonSerializer.Deserialize<List<JdSeedEntry>>(json, _jsonOpts);
        if (entries == null || entries.Count == 0) return;

        foreach (var entry in entries)
        {
            var normalizedId = $"jd-seed-{entry.JobTitle.ToLowerInvariant().Replace(" ", "-")}";

            var exists = await context.StandardizedJds
                .AnyAsync(j => j.Id == normalizedId && j.OwnerUserId == Tier1OrgId);

            if (exists) continue;

            var normalizedJd = BuildNormalizedJd(entry);
            var structuredJson = JsonSerializer.Serialize(normalizedJd, _jsonOpts);
            var humanText = BuildHumanReadableText(entry);

            var entity = new StandardizedJd
            {
                Id = normalizedId,
                OwnerUserId = Tier1OrgId,
                JobTitle = entry.JobTitle,
                Seniority = entry.Seniority,
                Department = entry.Department ?? string.Empty,
                EmploymentType = entry.EmploymentType ?? string.Empty,
                Location = entry.Location ?? string.Empty,
                WorkMode = entry.WorkMode ?? string.Empty,
                Industry = entry.Industry ?? string.Empty,
                HiringPriority = entry.HiringPriority ?? string.Empty,
                Currency = (entry.Currency ?? "VND").ToUpperInvariant(),
                SalaryMin = entry.SalaryMin,
                SalaryMax = entry.SalaryMax,
                StructuredJson = structuredJson,
                HumanReadableText = humanText,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            context.StandardizedJds.Add(entity);
        }

        await context.SaveChangesAsync();
    }

    private static object BuildNormalizedJd(JdSeedEntry e)
    {
        var yearsMin = 0;
        var yearsMax = 5;
        if (!string.IsNullOrEmpty(e.YearsExperience))
        {
            var parts = e.YearsExperience.Split('-');
            if (parts.Length == 2)
            {
                int.TryParse(parts[0], out yearsMin);
                int.TryParse(parts[1], out yearsMax);
            }
        }

        return new
        {
            jobTitle = e.JobTitle,
            department = e.Department ?? string.Empty,
            seniority = e.Seniority,
            employmentType = e.EmploymentType ?? "Full-time",
            location = e.Location ?? "Vietnam",
            workMode = e.WorkMode ?? "hybrid",
            workingModel = e.WorkMode ?? "hybrid",
            requiredSkills = e.RequiredSkills ?? new List<string>(),
            preferredSkills = e.PreferredSkills ?? new List<string>(),
            techStack = e.TechStack ?? new List<string>(),
            responsibilities = e.Responsibilities ?? new List<string>(),
            mustHave = e.MustHave ?? new List<string>(),
            niceToHave = e.NiceToHave ?? new List<string>(),
            experienceYearsMin = yearsMin,
            experienceYearsMax = yearsMax,
            educationRequirement = e.Education ?? "Bachelor's Degree or equivalent",
            languages = e.Languages ?? new List<string> { "English" },
            englishLevel = "Professional",
            salaryMin = e.SalaryMin,
            salaryMax = e.SalaryMax,
            currency = (e.Currency ?? "VND").ToUpperInvariant(),
            industry = e.Industry ?? "Software Development",
            hiringPriority = e.HiringPriority ?? "Medium",
            workHoursPerWeek = 40,
            benefits = new List<string>()
        };
    }

    private static string BuildHumanReadableText(JdSeedEntry e)
    {
        var sb = new global::System.Text.StringBuilder();
        sb.AppendLine($"# {e.JobTitle} — {e.Seniority}");
        sb.AppendLine($"**Department:** {e.Department} | **Type:** {e.EmploymentType} | **Location:** {e.Location} | **Mode:** {e.WorkMode}");
        sb.AppendLine($"**Salary:** {e.SalaryMin:N0}–{e.SalaryMax:N0} {e.Currency}");
        sb.AppendLine();
        if (e.Responsibilities?.Count > 0)
        {
            sb.AppendLine("## Responsibilities");
            foreach (var r in e.Responsibilities) sb.AppendLine($"- {r}");
            sb.AppendLine();
        }
        if (e.RequiredSkills?.Count > 0)
            sb.AppendLine($"**Required Skills:** {string.Join(", ", e.RequiredSkills)}");
        if (e.MustHave?.Count > 0)
        {
            sb.AppendLine("## Must Have");
            foreach (var m in e.MustHave) sb.AppendLine($"- {m}");
        }
        return sb.ToString().Trim();
    }

    private class JdSeedEntry
    {
        public string JobTitle { get; set; } = null!;
        public string? Department { get; set; }
        public string Seniority { get; set; } = null!;
        public string? EmploymentType { get; set; }
        public string? Location { get; set; }
        public string? WorkMode { get; set; }
        public List<string>? RequiredSkills { get; set; }
        public List<string>? PreferredSkills { get; set; }
        public List<string>? TechStack { get; set; }
        public List<string>? Responsibilities { get; set; }
        public List<string>? MustHave { get; set; }
        public List<string>? NiceToHave { get; set; }
        public string? YearsExperience { get; set; }
        public string? Education { get; set; }
        public decimal SalaryMin { get; set; }
        public decimal SalaryMax { get; set; }
        public string? Currency { get; set; }
        public List<string>? Languages { get; set; }
        public string? Industry { get; set; }
        public string? HiringPriority { get; set; }
    }
}
