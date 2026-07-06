namespace SangueNoAsfalto.World;

using SangueNoAsfalto.Combat;
using SangueNoAsfalto.Core;
using SangueNoAsfalto.Enemies;

/// <summary>
/// Prop da Vila Esperanca que quebra com golpes — madeira, vidro, lixo, placa.
/// </summary>
public partial class BreakableStageProp : Node2D
{
    public enum PropKind
    {
        Crate,
        FencePlank,
        TrashBag,
        StreetSign,
        KioskBottle,
    }

    [Export]
    public PropKind Kind { get; set; } = PropKind.Crate;

    [Export]
    public int MaxHealth { get; set; } = 38;

    private int _health;
    private Node2D? _visualRoot;
    private Area2D? _hurtArea;

    public override void _Ready()
    {
        _health = MaxHealth;
        BuildVisual();
        BuildHurtbox();
    }

    private void BuildVisual()
    {
        _visualRoot = new Node2D { Name = "Visual" };
        AddChild(_visualRoot);

        switch (Kind)
        {
            case PropKind.Crate:
                AddBox(_visualRoot, new Vector2(-26f, -18f), new Vector2(52f, 36f), new Color(0.19f, 0.14f, 0.095f));
                AddBox(_visualRoot, new Vector2(-22f, -22f), new Vector2(44f, 6f), new Color(0.24f, 0.18f, 0.12f));
                break;
            case PropKind.FencePlank:
                AddPoly(_visualRoot, new Color(0.18f, 0.13f, 0.09f), [
                    new Vector2(-8f, -48f), new Vector2(10f, -52f), new Vector2(12f, 42f), new Vector2(-10f, 46f)
                ]);
                break;
            case PropKind.TrashBag:
                AddPoly(_visualRoot, new Color(0.008f, 0.012f, 0.013f), [
                    new Vector2(-22f, 8f), new Vector2(-8f, -24f), new Vector2(18f, -16f), new Vector2(28f, 12f), new Vector2(6f, 22f)
                ]);
                break;
            case PropKind.StreetSign:
                AddBox(_visualRoot, new Vector2(-4f, -58f), new Vector2(8f, 58f), new Color(0.14f, 0.13f, 0.11f));
                AddBox(_visualRoot, new Vector2(-34f, -72f), new Vector2(68f, 22f), new Color(0.56f, 0.52f, 0.42f));
                break;
            case PropKind.KioskBottle:
                AddPoly(_visualRoot, new Color(0.12f, 0.42f, 0.14f, 0.85f), [
                    new Vector2(-6f, -18f), new Vector2(6f, -18f), new Vector2(8f, 8f), new Vector2(-8f, 8f)
                ]);
                AddPoly(_visualRoot, new Color(0.78f, 0.62f, 0.08f), [
                    new Vector2(-10f, 10f), new Vector2(10f, 10f), new Vector2(8f, 16f), new Vector2(-8f, 16f)
                ]);
                break;
        }
    }

    private void BuildHurtbox()
    {
        _hurtArea = new Area2D
        {
            Name = "HurtArea",
            CollisionLayer = 16,
            CollisionMask = 8,
        };
        AddChild(_hurtArea);

        RectangleShape2D shape = new()
        {
            Size = Kind switch
            {
                PropKind.StreetSign => new Vector2(72f, 90f),
                PropKind.FencePlank => new Vector2(24f, 96f),
                _ => new Vector2(56f, 48f),
            },
        };
        CollisionShape2D collision = new()
        {
            Position = Kind == PropKind.StreetSign ? new Vector2(0f, -24f) : Vector2.Zero,
            Shape = shape,
        };
        _hurtArea.AddChild(collision);
        _hurtArea.AreaEntered += OnAreaEntered;
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is not Hitbox hitbox || hitbox.Source is SideScrollerEnemyController)
        {
            return;
        }

        ApplyHit(hitbox.Damage, hitbox.Source is Node2D source2D ? source2D.GlobalPosition : GlobalPosition);
    }

    private void ApplyHit(int damage, Vector2 sourceGlobal)
    {
        _health -= Mathf.Max(damage / 2, 8);
        Vector2 dir = sourceGlobal.DirectionTo(GlobalPosition);
        CombatFeedback.PlayPropBreak(this, dir, Kind);
        Modulate = new Color(1f, 0.72f, 0.65f);
        GetTree().CreateTimer(0.06).Timeout += () =>
        {
            if (IsInstanceValid(this))
            {
                Modulate = Colors.White;
            }
        };

        if (_health > 0)
        {
            return;
        }

        QueueFree();
    }

    private static void AddBox(Node parent, Vector2 pos, Vector2 size, Color color)
    {
        ColorRect rect = new()
        {
            Position = pos,
            Size = size,
            Color = color,
        };
        parent.AddChild(rect);
    }

    private static void AddPoly(Node parent, Color color, Vector2[] points)
    {
        Polygon2D poly = new()
        {
            Color = color,
            Polygon = points,
        };
        parent.AddChild(poly);
    }
}
