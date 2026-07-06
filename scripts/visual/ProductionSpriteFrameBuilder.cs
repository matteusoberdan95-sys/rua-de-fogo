namespace SangueNoAsfalto.Visual;

using System.Text.Json;
using GodotFileAccess = Godot.FileAccess;

/// <summary>
/// Monta SpriteFrames a partir de PNGs + manifest.json em art/production/.
/// </summary>
public static class ProductionSpriteFrameBuilder
{
    private sealed class ManifestAnim
    {
        public string Pattern { get; set; } = "{name}_{index}.png";
        public int Frames { get; set; } = 1;
        public float Fps { get; set; } = 8f;
        public bool Loop { get; set; }
    }

    private sealed class CharacterManifest
    {
        public string Character { get; set; } = string.Empty;
        public string Version { get; set; } = "v0";
        public string SpriteFrames { get; set; } = "auto";
        public string SpritesFolder { get; set; } = "sprites";
        public float DisplayScale { get; set; } = 1f;
        public float PivotY { get; set; } = 132f;
        public Dictionary<string, ManifestAnim> Animations { get; set; } = new();
    }

    public static SpriteFrames? BuildFromManifest(ProductionCharacterId id)
    {
        CharacterManifest? manifest = ReadManifest(id);
        if (manifest is null || manifest.Animations.Count == 0)
        {
            return null;
        }

        string folder = ProductionArtCatalog.GetCharacterFolder(id);
        string spritesDir = $"{folder}{manifest.SpritesFolder.TrimEnd('/')}/";
        SpriteFrames frames = new();

        foreach (KeyValuePair<string, ManifestAnim> entry in manifest.Animations)
        {
            string animName = entry.Key;
            ManifestAnim anim = entry.Value;
            frames.AddAnimation(animName);

            int loaded = 0;
            for (int i = 1; i <= anim.Frames; i++)
            {
                string fileName = anim.Pattern
                    .Replace("{name}", animName, StringComparison.Ordinal)
                    .Replace("{index}", i.ToString("D2"), StringComparison.Ordinal)
                    .Replace("{character}", manifest.Character, StringComparison.Ordinal);
                string texturePath = $"{spritesDir}{fileName}";

                Texture2D? texture = LoadTexture(texturePath);
                if (texture is null)
                {
                    continue;
                }

                frames.AddFrame(animName, texture, 1.0f);
                loaded++;
            }

            if (loaded == 0)
            {
                frames.RemoveAnimation(animName);
                continue;
            }

            frames.SetAnimationSpeed(animName, anim.Fps);
        }

        return frames.GetAnimationNames().Length > 0 ? frames : null;
    }

    public static float ReadDisplayScale(ProductionCharacterId id)
    {
        return ReadManifest(id)?.DisplayScale ?? 1f;
    }

    public static Vector2 ReadSpriteOffset(ProductionCharacterId id)
    {
        float pivotY = ReadManifest(id)?.PivotY ?? 132f;
        return new Vector2(0f, -pivotY + 80f);
    }

    private static CharacterManifest? ReadManifest(ProductionCharacterId id)
    {
        string manifestPath = $"{ProductionArtCatalog.GetCharacterFolder(id)}manifest.json";
        if (!GodotFileAccess.FileExists(manifestPath))
        {
            return null;
        }

        using GodotFileAccess file = GodotFileAccess.Open(manifestPath, GodotFileAccess.ModeFlags.Read);
        return JsonSerializer.Deserialize<CharacterManifest>(file.GetAsText());
    }

    private static Texture2D? LoadTexture(string resourcePath)
    {
        if (ResourceLoader.Exists(resourcePath))
        {
            return ResourceLoader.Load<Texture2D>(resourcePath);
        }

        string absolutePath = ProjectSettings.GlobalizePath(resourcePath);
        if (!System.IO.File.Exists(absolutePath))
        {
            return null;
        }

        Image image = Image.LoadFromFile(absolutePath);
        if (image.IsEmpty())
        {
            return null;
        }

        ImageTexture texture = ImageTexture.CreateFromImage(image);
        return texture;
    }
}
