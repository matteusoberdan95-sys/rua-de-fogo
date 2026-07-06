namespace SangueNoAsfalto.Visual;

/// <summary>
/// Escolhe arte de producao quando disponivel; caso contrario mantem CharacterSpriteVisual procedural.
/// </summary>
public static class ActorVisualResolver
{
    public static IActorVisual Resolve(
        Node actor,
        bool useProductionArt,
        ProductionCharacterId characterId,
        LayeredPrototypePreset fallbackPreset = LayeredPrototypePreset.Caua)
    {
        CharacterSpriteVisual? fallback = actor.GetNodeOrNull<CharacterSpriteVisual>("SpriteVisual");
        ArtCharacterVisual? production = actor.GetNodeOrNull<ArtCharacterVisual>("ProductionArtVisual");

        if (useProductionArt && production is not null && production.TryActivate(fallback, characterId))
        {
            return production;
        }

        if (production is not null)
        {
            production.Visible = false;
        }

        if (fallback is not null)
        {
            fallback.Visible = true;
            fallback.SetProcess(true);
            fallback.EnsureLayeredRig(fallbackPreset);
            return fallback;
        }

        if (production is not null)
        {
            return production;
        }

        return fallback!;
    }
}
