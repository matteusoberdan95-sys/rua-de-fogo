param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
)

$spritesDir = Join-Path $Root "art\production\characters\caua\sprites"
$template = @'
[remap]

importer="texture"
type="CompressedTexture2D"
uid="uid://{UID}"
path="res://.godot/imported/{BASENAME}-{HASH}.ctex"
metadata={
"vram_texture": false
}

[deps]

source_file="res://art/production/characters/caua/sprites/{BASENAME}"
dest_files=["res://.godot/imported/{BASENAME}-{HASH}.ctex"]

[params]

compress/mode=0
compress/high_quality=false
compress/lossy_quality=0.7
compress/uastc_level=0
compress/rdo_quality_loss=0.0
compress/hdr_compression=1
compress/normal_map=0
compress/channel_pack=0
mipmaps/generate=false
mipmaps/limit=-1
roughness/mode=0
roughness/src_normal=""
process/channel_remap/red=0
process/channel_remap/green=1
process/channel_remap/blue=2
process/channel_remap/alpha=3
process/fix_alpha_border=true
process/premult_alpha=false
process/normal_map_invert_y=false
process/hdr_as_srgb=false
process/hdr_clamp_exposure=false
process/size_limit=0
detect_3d/compress_to=1
'@

Get-ChildItem $spritesDir -Filter "*.png" | ForEach-Object {
    $base = $_.Name
    $hash = ([System.BitConverter]::ToString([System.Security.Cryptography.MD5]::Create().ComputeHash([Text.Encoding]::UTF8.GetBytes($base))).Replace("-", "").ToLower().Substring(0, 16))
    $uid = "uid://" + ([guid]::NewGuid().ToString("N"))
    $content = $template.Replace("{BASENAME}", $base).Replace("{HASH}", $hash).Replace("{UID}", $uid.Substring(5))
    $importPath = Join-Path $spritesDir ($base + ".import")
    Set-Content -Path $importPath -Value $content -Encoding UTF8
    Write-Host "import: $base"
}

Write-Host "Import sidecars criados. Abra o Godot para reimportar."
