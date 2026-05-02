#Requires -Version 5.1
<#
.SYNOPSIS
    Scans hook input or repository files for obvious secret-like patterns.
.DESCRIPTION
    This script is intentionally conservative about output. It reports only the
    source and pattern class, never the matched value. Hook mode reads stdin and
    emits the JSON response expected by Copilot hooks. Repo mode scans tracked
    files when git is available, or workspace files with common generated and
    secret directories excluded.
.PARAMETER Mode
    Hook scans stdin and returns a hook decision. Repo scans repository files.
.PARAMETER Path
    Repository root or path to scan in Repo mode.
.PARAMETER MaxFileBytes
    Maximum file size to read in Repo mode.
.EXAMPLE
    powershell -NoProfile -ExecutionPolicy Bypass -File scripts/security/Test-SecretPatterns.ps1 -Mode Repo
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [ValidateSet('Hook', 'Repo')]
    [string]$Mode = 'Repo',

    [Parameter(Mandatory = $false)]
    [string]$Path = (Get-Location).Path,

    [Parameter(Mandatory = $false)]
    [ValidateRange(1024, 10485760)]
    [int]$MaxFileBytes = 1048576
)

$ErrorActionPreference = 'Stop'

#region Functions
function Get-SecretPattern {
    [CmdletBinding()]
    [OutputType([object[]])]
    param()

    return @(
        [pscustomobject]@{ Name = 'Private key block'; Pattern = '-----BEGIN (RSA |DSA |EC |OPENSSH |PGP )?PRIVATE KEY-----' },
        [pscustomobject]@{ Name = 'GitHub classic token'; Pattern = '\b(ghp|gho|ghu|ghs|ghr)_[A-Za-z0-9_]{30,}\b' },
        [pscustomobject]@{ Name = 'GitHub fine-grained token'; Pattern = 'github_pat_[A-Za-z0-9_]{20,}_[A-Za-z0-9_]{20,}' },
        [pscustomobject]@{ Name = 'OpenAI-style API key'; Pattern = '\bsk-[A-Za-z0-9_-]{20,}\b' },
        [pscustomobject]@{ Name = 'AWS access key id'; Pattern = '\bAKIA[0-9A-Z]{16}\b' },
        [pscustomobject]@{ Name = 'Azure storage connection string'; Pattern = 'DefaultEndpointsProtocol=https?;AccountName=[^;\s]+;AccountKey=[^;\s]+' },
        [pscustomobject]@{ Name = 'URL basic auth credential'; Pattern = 'https?://[^/\s:@]+:[^/\s:@]{8,}@' },
        [pscustomobject]@{ Name = 'Generic assigned secret'; Pattern = '(?i)\b(api[_-]?key|auth[_-]?token|access[_-]?token|refresh[_-]?token|secret|password|passwd|pwd|connection[_-]?string)\b\s*[:=]\s*["'']?[^"''\s]{16,}' }
    )
}

function Test-ContentForSecret {
    [CmdletBinding()]
    [OutputType([object[]])]
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Content,

        [Parameter(Mandatory = $true)]
        [string]$Source
    )

    $Findings = New-Object System.Collections.Generic.List[object]

    foreach ($SecretPattern in Get-SecretPattern) {
        if ([regex]::IsMatch($Content, $SecretPattern.Pattern)) {
            $Findings.Add([pscustomobject]@{
                Source = $Source
                PatternName = $SecretPattern.Name
            })
        }
    }

    return $Findings.ToArray()
}

function Test-SensitiveFileName {
    [CmdletBinding()]
    [OutputType([bool])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$RelativePath
    )

    $NormalizedPath = $RelativePath -replace '\\', '/'
    return ($NormalizedPath -match '(^|/)\.env(\.|$)' -or
        $NormalizedPath -match '(^|/)(secrets|credentials|private)/' -or
        $NormalizedPath -match '\.(pem|key|pfx|p12)$')
}

function Test-IgnoredScanPath {
    [CmdletBinding()]
    [OutputType([bool])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$RelativePath
    )

    $NormalizedPath = $RelativePath -replace '\\', '/'
    return ($NormalizedPath -match '(^|/)\.git/' -or
        $NormalizedPath -match '(^|/)\.godot/' -or
        $NormalizedPath -match '(^|/)\.import/' -or
        $NormalizedPath -match '(^|/)node_modules/' -or
        $NormalizedPath -match '(^|/)(bin|obj|build|dist|out|tmp|temp)/' -or
        $NormalizedPath -match '(^|/)\.copilot-tracking/agentic/runs/' -or
        $NormalizedPath -match '\.(png|jpg|jpeg|gif|webp|ico|pck|zip|tar|gz|7z|exe|dll|pdb|bin)$')
}

function Get-RelativeScanPath {
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$RootPath,

        [Parameter(Mandatory = $true)]
        [string]$FilePath
    )

    $ResolvedRoot = [System.IO.Path]::GetFullPath($RootPath).TrimEnd('\', '/')
    $ResolvedFile = [System.IO.Path]::GetFullPath($FilePath)

    if ($ResolvedFile.StartsWith($ResolvedRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $ResolvedFile.Substring($ResolvedRoot.Length).TrimStart('\', '/')
    }

    return $ResolvedFile
}

function Get-ScanFile {
    [CmdletBinding()]
    [OutputType([object[]])]
    param(
        [Parameter(Mandatory = $true)]
        [string]$RootPath
    )

    $GitCommand = Get-Command git -ErrorAction SilentlyContinue
    if ($GitCommand) {
        $GitRoot = git -C $RootPath rev-parse --show-toplevel 2>$null
        if ($LASTEXITCODE -eq 0 -and $GitRoot) {
            $TrackedFiles = git -C $GitRoot ls-files --cached --others --exclude-standard
            if ($LASTEXITCODE -eq 0) {
                foreach ($TrackedFile in $TrackedFiles) {
                    if ([string]::IsNullOrWhiteSpace($TrackedFile)) {
                        continue
                    }

                    [pscustomobject]@{
                        FullPath = Join-Path $GitRoot $TrackedFile
                        RelativePath = $TrackedFile
                    }
                }
                return
            }
        }
    }

    Get-ChildItem -Path $RootPath -File -Recurse -ErrorAction Stop | ForEach-Object {
        $RelativePath = Get-RelativeScanPath -RootPath $RootPath -FilePath $_.FullName
        [pscustomobject]@{
            FullPath = $_.FullName
            RelativePath = $RelativePath
        }
    }
}

function Write-HookDecision {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [bool]$Allowed,

        [Parameter(Mandatory = $true)]
        [string]$Reason
    )

    $Decision = if ($Allowed) { 'allow' } else { 'deny' }
    $Payload = [ordered]@{
        continue = $true
        hookSpecificOutput = [ordered]@{
            hookEventName = 'PreToolUse'
            permissionDecision = $Decision
            permissionDecisionReason = $Reason
        }
    }

    $Payload | ConvertTo-Json -Depth 5 -Compress
}
#endregion Functions

#region Main Execution
try {
    if ($Mode -eq 'Hook') {
        $InputContent = [Console]::In.ReadToEnd()
        $Findings = Test-ContentForSecret -Content $InputContent -Source 'hook-input'

        if ($Findings.Count -gt 0) {
            Write-HookDecision -Allowed $false -Reason 'Potential secret-like content detected in tool input. Remove sensitive values and use GitHub Secrets, ignored local config, or an OS credential store.'
            exit 0
        }

        Write-HookDecision -Allowed $true -Reason 'No obvious secret pattern detected.'
        exit 0
    }

    $RootPath = [System.IO.Path]::GetFullPath($Path)
    $AllFindings = New-Object System.Collections.Generic.List[object]

    foreach ($ScanFile in Get-ScanFile -RootPath $RootPath) {
        if (Test-IgnoredScanPath -RelativePath $ScanFile.RelativePath) {
            continue
        }

        if (Test-SensitiveFileName -RelativePath $ScanFile.RelativePath) {
            $AllFindings.Add([pscustomobject]@{
                Source = $ScanFile.RelativePath
                PatternName = 'Sensitive filename should not be tracked'
            })
            continue
        }

        if (-not (Test-Path -LiteralPath $ScanFile.FullPath -PathType Leaf)) {
            continue
        }

        $FileInfo = Get-Item -LiteralPath $ScanFile.FullPath -ErrorAction Stop
        if ($FileInfo.Length -gt $MaxFileBytes) {
            continue
        }

        $Content = Get-Content -LiteralPath $ScanFile.FullPath -Raw -ErrorAction Stop
        foreach ($Finding in Test-ContentForSecret -Content $Content -Source $ScanFile.RelativePath) {
            $AllFindings.Add($Finding)
        }
    }

    if ($AllFindings.Count -gt 0) {
        Write-Host 'Potential secret-like content detected:' -ForegroundColor Red
        foreach ($Finding in $AllFindings) {
            Write-Host (" - {0}: {1}" -f $Finding.Source, $Finding.PatternName) -ForegroundColor Red
        }
        exit 2
    }

    Write-Host 'No obvious secret patterns detected.' -ForegroundColor Green
    exit 0
}
catch {
    Write-Error -ErrorAction Continue "Secret pattern scan failed: $($_.Exception.Message)"
    exit 1
}
#endregion Main Execution