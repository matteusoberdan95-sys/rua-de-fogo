namespace SangueNoAsfalto.Visual;

/// <summary>
/// Caminhos e disponibilidade dos pacotes de arte real em art/production/.
/// Nunca aponta para references/.
/// </summary>
public static class ProductionArtCatalog
{
    public const string CauaFramesPath = "res://art/production/characters/caua/caua_v0_frames.tres";
    public const string CauaManifestPath = "res://art/production/characters/caua/manifest.json";

    public static string GetSpriteFramesPath(ProductionCharacterId id) => id switch
    {
        ProductionCharacterId.Caua => CauaFramesPath,
        ProductionCharacterId.QuebraOsso => "res://art/production/characters/enemies/quebra-osso/quebra_osso_v0_frames.tres",
        ProductionCharacterId.Fast => "res://art/production/characters/enemies/fast/fast_v0_frames.tres",
        ProductionCharacterId.Brute => "res://art/production/characters/enemies/brute/brute_v0_frames.tres",
        ProductionCharacterId.Infected => "res://art/production/characters/enemies/infected/infected_v0_frames.tres",
        ProductionCharacterId.MiniBoss => "res://art/production/characters/enemies/mini-boss/mini_boss_v0_frames.tres",
        _ => string.Empty,
    };

    public static string GetCharacterFolder(ProductionCharacterId id) => id switch
    {
        ProductionCharacterId.Caua => "res://art/production/characters/caua/",
        ProductionCharacterId.QuebraOsso => "res://art/production/characters/enemies/quebra-osso/",
        ProductionCharacterId.Fast => "res://art/production/characters/enemies/fast/",
        ProductionCharacterId.Brute => "res://art/production/characters/enemies/brute/",
        ProductionCharacterId.Infected => "res://art/production/characters/enemies/infected/",
        ProductionCharacterId.MiniBoss => "res://art/production/characters/enemies/mini-boss/",
        _ => string.Empty,
    };

    public static bool HasProductionPack(ProductionCharacterId id)
    {
        string framesPath = GetSpriteFramesPath(id);
        if (string.IsNullOrEmpty(framesPath))
        {
            return false;
        }

        return ResourceLoader.Exists(framesPath);
    }

    public static SpriteFrames? LoadSpriteFrames(ProductionCharacterId id)
    {
        if (!HasProductionPack(id))
        {
            return null;
        }

        return ResourceLoader.Load<SpriteFrames>(GetSpriteFramesPath(id));
    }

    public static ProductionCharacterId FromLayeredPreset(LayeredPrototypePreset preset) => preset switch
    {
        LayeredPrototypePreset.Caua => ProductionCharacterId.Caua,
        LayeredPrototypePreset.QuebraOsso => ProductionCharacterId.QuebraOsso,
        LayeredPrototypePreset.Fast => ProductionCharacterId.Fast,
        LayeredPrototypePreset.Brute => ProductionCharacterId.Brute,
        LayeredPrototypePreset.Infected => ProductionCharacterId.Infected,
        LayeredPrototypePreset.MiniBoss => ProductionCharacterId.MiniBoss,
        _ => ProductionCharacterId.QuebraOsso,
    };
}
