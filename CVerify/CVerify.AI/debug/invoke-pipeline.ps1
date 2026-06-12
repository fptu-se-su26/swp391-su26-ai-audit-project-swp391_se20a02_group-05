#Requires -Version 5.1
<#
.SYNOPSIS
    Debug helper for the CVerify.AI pipeline — invokes tasks and displays token usage.

.DESCRIPTION
    Sends a signed pipeline task request to the CVerify.AI FastAPI microservice and
    prints token accounting from the per-job token_debug.jsonl file.

    Prerequisites:
      - CVerify.AI running (docker compose up or uvicorn)
      - AI_DEBUG_TOKENS=true in CVerify.AI/.env
      - SHARED_SECRET set in CVerify.AI/.env (used to sign the HMAC header)

.PARAMETER JobId
    The GUID job identifier. A new one is generated if omitted.

.PARAMETER TaskType
    Pipeline task to invoke. Default: RepoStructure
    Valid v2 tasks: RepoStructure, CommitIntelligence, SkillExtraction, ArchitectureAnalysis,
                    CodeQuality, SecurityAnalysis, RepositoryClassification, RepositorySummary,
                    CommitDiff, CommitTimeline, CommitIntent, Complexity, GitBlame,
                    CloneDetection, AiGeneratedCode, Ownership, SkillGraph, TrustScore

.PARAMETER RepoUrl
    GitHub HTTPS URL of the repository to analyse.

.PARAMETER BaseUrl
    CVerify.AI base URL. Default: http://localhost:8000

.PARAMETER ShowTokens
    After task completes, tail the token_debug.jsonl and print a summary table.

.EXAMPLE
    .\invoke-pipeline.ps1 -RepoUrl https://github.com/some/repo -TaskType CommitDiff
    .\invoke-pipeline.ps1 -RepoUrl https://github.com/some/repo -TaskType TrustScore -ShowTokens
#>

param(
    [string]  $JobId     = [System.Guid]::NewGuid().ToString(),
    [string]  $TaskType  = "RepoStructure",
    [string]  $RepoUrl   = "https://github.com/octocat/Hello-World",
    [string]  $BaseUrl   = "http://localhost:8000",
    [switch]  $ShowTokens
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ── Load environment ────────────────────────────────────────────────────────
$EnvFile = Join-Path $PSScriptRoot ".." ".env"
if (Test-Path $EnvFile) {
    Get-Content $EnvFile | ForEach-Object {
        if ($_ -match '^\s*([^#=\s]+)\s*=\s*(.+)$') {
            [System.Environment]::SetEnvironmentVariable($Matches[1], $Matches[2].Trim('"').Trim("'"))
        }
    }
    Write-Host "[debug] Loaded .env from $EnvFile" -ForegroundColor DarkGray
}

$SharedSecret = $env:SHARED_SECRET
$ClientId     = if ($env:CLIENT_ID) { $env:CLIENT_ID } else { "cverify-core" }

if (-not $SharedSecret) {
    Write-Warning "SHARED_SECRET not set — request will be unsigned (server may reject it)"
    $SharedSecret = "dev-secret"
}

# ── Build HMAC-SHA256 signature ─────────────────────────────────────────────
function Get-HmacSignature([string]$Payload, [string]$Secret) {
    $keyBytes     = [System.Text.Encoding]::UTF8.GetBytes($Secret)
    $payloadBytes = [System.Text.Encoding]::UTF8.GetBytes($Payload)
    $hmac         = New-Object System.Security.Cryptography.HMACSHA256
    $hmac.Key     = $keyBytes
    $hashBytes    = $hmac.ComputeHash($payloadBytes)
    return [System.BitConverter]::ToString($hashBytes).Replace("-", "").ToLower()
}

# ── Assemble request body ────────────────────────────────────────────────────
$Timestamp = [DateTimeOffset]::UtcNow.ToUnixTimeSeconds().ToString()
$Nonce     = [System.Guid]::NewGuid().ToString("N")

$Body = @{
    job_id           = $JobId
    task_type        = $TaskType
    repo_url         = $RepoUrl
    encrypted_token  = ""       # leave empty for public repos
    correlation_id   = "debug-$($JobId.Substring(0,8))"
} | ConvertTo-Json -Depth 3

$SignaturePayload = "${ClientId}:${Timestamp}:${Nonce}:${Body}"
$Signature        = Get-HmacSignature -Payload $SignaturePayload -Secret $SharedSecret

$Headers = @{
    "Content-Type"     = "application/json"
    "X-Client-Id"      = $ClientId
    "X-Timestamp"      = $Timestamp
    "X-Nonce"          = $Nonce
    "X-Signature"      = $Signature
}

# ── Invoke the task ──────────────────────────────────────────────────────────
$Endpoint = "$BaseUrl/api/v1/repository/analyze/task"

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host " CVerify Pipeline Debug Invoker" -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host " Job ID   : $JobId"
Write-Host " Task     : $TaskType"
Write-Host " Repo     : $RepoUrl"
Write-Host " Endpoint : $Endpoint"
Write-Host "───────────────────────────────────────────────────────"

try {
    $Response = Invoke-RestMethod `
        -Uri       $Endpoint `
        -Method    POST `
        -Headers   $Headers `
        -Body      $Body `
        -TimeoutSec 300

    Write-Host " Status   : SUCCESS" -ForegroundColor Green
    Write-Host ""
    $Response | ConvertTo-Json -Depth 6 | Write-Host
} catch {
    $StatusCode = $_.Exception.Response.StatusCode.value__
    $Detail     = $_.ErrorDetails.Message
    Write-Host " Status   : FAILED ($StatusCode)" -ForegroundColor Red
    Write-Host " Detail   : $Detail"   -ForegroundColor Red
}

# ── Token debug summary ──────────────────────────────────────────────────────
if ($ShowTokens) {
    Write-Host ""
    Write-Host "───────────────────────────────────────────────────────"
    Write-Host " Token Debug Summary (token_debug.jsonl)" -ForegroundColor Yellow
    Write-Host "───────────────────────────────────────────────────────"

    # Find temp_clones dir relative to this script
    $TempClones = Join-Path $PSScriptRoot ".." "app" "pipelines" "repository" "temp_clones"
    $JobDir     = Join-Path $TempClones $JobId
    $DebugFile  = Join-Path $JobDir "token_debug.jsonl"

    if (-not (Test-Path $DebugFile)) {
        Write-Host " No token_debug.jsonl found at: $DebugFile" -ForegroundColor DarkYellow
        Write-Host " Make sure AI_DEBUG_TOKENS=true in .env and the job directory exists."
    } else {
        $TotalCost     = 0.0
        $TotalPrompt   = 0
        $TotalComplete = 0
        $TotalCacheR   = 0
        $TotalCacheW   = 0

        $Rows = Get-Content $DebugFile | ForEach-Object {
            $entry = $_ | ConvertFrom-Json
            $TotalCost     += [double]$entry.estimated_cost_usd
            $TotalPrompt   += [int]$entry.prompt_tokens
            $TotalComplete += [int]$entry.completion_tokens
            $TotalCacheR   += [int]$entry.cache_read_tokens
            $TotalCacheW   += [int]$entry.cache_write_tokens
            [PSCustomObject]@{
                Task           = $entry.task
                Prompt         = $entry.prompt_tokens
                Completion     = $entry.completion_tokens
                CacheRead      = $entry.cache_read_tokens
                CacheWrite     = $entry.cache_write_tokens
                Total          = $entry.total_tokens
                "Cost(USD)"    = ("{0:F6}" -f [double]$entry.estimated_cost_usd)
                "Dur(ms)"      = $entry.duration_ms
                Mismatch       = $entry.mismatch_flag
            }
        }

        $Rows | Format-Table -AutoSize

        Write-Host "───────────────────────────────────────────────────────"
        Write-Host (" TOTAL  prompt={0}  completion={1}  cache_read={2}  cache_write={3}  cost=`${4:F6}" -f `
            $TotalPrompt, $TotalComplete, $TotalCacheR, $TotalCacheW, $TotalCost) -ForegroundColor Cyan
        Write-Host "═══════════════════════════════════════════════════════"
    }
}
