# Cria pastas para assets finais da Vila Esperanca (Sprint 32)
$root = Join-Path (Join-Path $PSScriptRoot "..") "art\stage\vila-esperanca"
$dirs = @("props", "tiles", "landmarks")
foreach ($d in $dirs) {
    $path = Join-Path $root $d
    if (-not (Test-Path $path)) {
        New-Item -ItemType Directory -Path $path -Force | Out-Null
        Write-Host "Created $path"
    } else {
        Write-Host "Exists $path"
    }
}
Write-Host "Stage art folders ready. See docs/STAGE_ASSET_PIPELINE.md"
