$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $projectRoot

Write-Host "Building C# project..."
dotnet build "SangueNoAsfalto.csproj"
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$godot = Get-Command godot -ErrorAction SilentlyContinue
if (-not $godot) {
    Write-Host "Godot CLI not found in PATH."
    Write-Host "Export manually via Project > Export > Windows Desktop."
    Write-Host "Target path: build/windows/SangueNoAsfalto.exe"
    exit 0
}

New-Item -ItemType Directory -Force -Path "build/windows" | Out-Null

Write-Host "Exporting Windows demo..."
& godot --headless --export-release "Windows Desktop" "build/windows/SangueNoAsfalto.exe"
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Host "Demo export complete: build/windows/SangueNoAsfalto.exe"
Write-Host "Copy build/demo/README.txt into the export folder before zipping for testers."
