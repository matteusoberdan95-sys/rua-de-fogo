# Empacota vertical slice para testers (Sprint 34)
$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $projectRoot

Write-Host "=== Vertical Slice Pack ==="

Write-Host "Building C#..."
dotnet build "SangueNoAsfalto.csproj"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$outDir = Join-Path $projectRoot "build\vertical-slice-pack"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

Copy-Item -Force (Join-Path $projectRoot "build\demo\README.txt") (Join-Path $outDir "README.txt")
Copy-Item -Force (Join-Path $projectRoot "docs\VERTICAL_SLICE_QC.md") (Join-Path $outDir "QC_CHECKLIST.md")

$godot = Get-Command godot -ErrorAction SilentlyContinue
if ($godot) {
    New-Item -ItemType Directory -Force -Path (Join-Path $projectRoot "build\windows") | Out-Null
    Write-Host "Exporting Windows..."
    & godot --headless --export-release "Windows Desktop" (Join-Path $projectRoot "build\windows\SangueNoAsfalto.exe")
    if ($LASTEXITCODE -eq 0) {
        Copy-Item -Recurse -Force (Join-Path $projectRoot "build\windows\*") $outDir
        Write-Host "Export copied to $outDir"
    }
} else {
    Write-Host "Godot CLI not in PATH — export manually, then copy exe + DLLs to:"
    Write-Host "  $outDir"
}

Write-Host ""
Write-Host "Pack ready folder: $outDir"
Write-Host "Zip and share with testers + QC_CHECKLIST.md"
