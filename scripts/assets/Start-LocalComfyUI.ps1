#!/usr/bin/env pwsh
# Copyright (c) Microsoft Corporation.
# SPDX-License-Identifier: MIT
#Requires -Version 7.0

<#
.SYNOPSIS
    Starts the local ComfyUI server for pixel art candidate generation.
.DESCRIPTION
    Runs the source-installed ComfyUI checkout from the ignored private local
    image generation folder. The server binds to localhost by default and keeps
    generated ComfyUI runtime outputs under ignored local folders.
.PARAMETER RepoRoot
    Root directory of the repository.
.PARAMETER Port
    Local HTTP port for ComfyUI.
.PARAMETER ComfyArgs
    Additional arguments to pass through to ComfyUI.
.PARAMETER StableMode
    Starts ComfyUI with conservative CUDA settings that avoid cudaMallocAsync
    and async offload. This is useful on Windows when longer batches crash after
    a few successful generations.
.EXAMPLE
    ./scripts/assets/Start-LocalComfyUI.ps1
.EXAMPLE
    ./scripts/assets/Start-LocalComfyUI.ps1 -StableMode
.EXAMPLE
    ./scripts/assets/Start-LocalComfyUI.ps1 -Port 8190 -ComfyArgs @('--lowvram')
.NOTES
    Runs via: pwsh ./scripts/assets/Start-LocalComfyUI.ps1
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$RepoRoot = (git rev-parse --show-toplevel 2>$null),

    [Parameter(Mandatory = $false)]
    [int]$Port = 8188,

    [Parameter(Mandatory = $false)]
    [string[]]$ComfyArgs = @(),

    [Parameter(Mandatory = $false)]
    [switch]$StableMode
)

$ErrorActionPreference = 'Stop'

#region Functions
function Resolve-RepoRoot {
    [CmdletBinding()]
    [OutputType([string])]
    param(
        [Parameter(Mandatory = $false)]
        [string]$CandidateRoot
    )

    if (-not [string]::IsNullOrWhiteSpace($CandidateRoot)) {
        return (Resolve-Path -Path $CandidateRoot).Path
    }

    return (Resolve-Path -Path (Join-Path $PSScriptRoot '../..')).Path
}
#endregion Functions

#region Main Execution
if ($MyInvocation.InvocationName -ne '.') {
    try {
        $ResolvedRepoRoot = Resolve-RepoRoot -CandidateRoot $RepoRoot
        $PythonPath = Join-Path $ResolvedRepoRoot 'private/local-imagegen/.venv/Scripts/python.exe'
        $ComfyRoot = Join-Path $ResolvedRepoRoot 'private/local-imagegen/ComfyUI'
        $ComfyMain = Join-Path $ComfyRoot 'main.py'
        $OutputDirectory = Join-Path $ResolvedRepoRoot 'private/local-imagegen/comfy-output'
        $InputDirectory = Join-Path $ResolvedRepoRoot 'private/local-imagegen/comfy-input'
        $TempDirectory = Join-Path $ResolvedRepoRoot 'private/local-imagegen/comfy-temp'

        if (-not (Test-Path -Path $PythonPath)) {
            throw "Python environment not found at $PythonPath"
        }

        if (-not (Test-Path -Path $ComfyMain)) {
            throw "ComfyUI entry point not found at $ComfyMain"
        }

        New-Item -ItemType Directory -Path $OutputDirectory, $InputDirectory, $TempDirectory -Force | Out-Null

        $LaunchArgs = @()
        if ($StableMode.IsPresent) {
            $LaunchArgs += @(
                '--disable-cuda-malloc',
                '--disable-async-offload',
                '--preview-method',
                'none'
            )
        }

        $LaunchArgs += $ComfyArgs

        Write-Host "Starting ComfyUI at http://127.0.0.1:$Port"
        & $PythonPath $ComfyMain `
            --listen 127.0.0.1 `
            --port $Port `
            --disable-auto-launch `
            --output-directory $OutputDirectory `
            --input-directory $InputDirectory `
            --temp-directory $TempDirectory `
            @LaunchArgs
        exit $LASTEXITCODE
    }
    catch {
        Write-Error -ErrorAction Continue "Start-LocalComfyUI failed: $($_.Exception.Message)"
        exit 1
    }
}
#endregion Main Execution