namespace SangueNoAsfalto.Visual;

using System;
using SangueNoAsfalto.Enemies;

/// <summary>
/// Troca inimigos "bloco" (Polygon2D soltos) pelo rig 2D em camadas de <see cref="CharacterSpriteVisual"/>.
/// Imagens em art/ continuam apenas como referencia — nunca runtime.
/// </summary>
public static class EnemyLayeredVisual
{
    public static CharacterSpriteVisual AttachLayeredRig(
        SideScrollerEnemyController enemy,
        LayeredPrototypePreset preset)
    {
        HideBlockMeshes(enemy);

        if (enemy.GetNodeOrNull<CharacterSpriteVisual>("SpriteVisual") is CharacterSpriteVisual existing)
        {
            existing.EnsureLayeredRig(preset);
            return existing;
        }

        CharacterSpriteVisual visual = new()
        {
            Name = "SpriteVisual",
            Position = new Vector2(0f, -4f),
            UseLayeredPrototype = true,
            LayeredPreset = preset,
            SourceFacesRight = false,
        };

        enemy.AddChild(visual);
        enemy.MoveChild(visual, 1);
        visual.EnsureLayeredRig(preset);
        return visual;
    }

    public static LayeredPrototypePreset ResolvePreset(SideScrollerEnemyController enemy)
    {
        string name = enemy.Name.ToString();
        if (name.Contains("Fast", StringComparison.OrdinalIgnoreCase))
        {
            return LayeredPrototypePreset.Fast;
        }

        if (name.Contains("Brute", StringComparison.OrdinalIgnoreCase))
        {
            return LayeredPrototypePreset.Brute;
        }

        if (name.Contains("Infected", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Rain", StringComparison.OrdinalIgnoreCase))
        {
            return LayeredPrototypePreset.Infected;
        }

        if (name.Contains("MiniBoss", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Alpha", StringComparison.OrdinalIgnoreCase)
            || name.Contains("Boss", StringComparison.OrdinalIgnoreCase))
        {
            return LayeredPrototypePreset.MiniBoss;
        }

        return LayeredPrototypePreset.QuebraOsso;
    }

    public static void HideBlockMeshes(SideScrollerEnemyController enemy)
    {
        foreach (Node child in enemy.GetChildren())
        {
            if (child is Polygon2D poly && child.Name != "LaneShadow")
            {
                poly.Visible = false;
            }
        }
    }
}
