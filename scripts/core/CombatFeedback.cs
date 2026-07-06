namespace SangueNoAsfalto.Core;

using SangueNoAsfalto.Combat;
using SangueNoAsfalto.Visual;

public static class CombatFeedback
{
    private const float HitPauseTimeScale = 0.06f;
    private const float FlashDuration = 0.14f;
    private static bool _hitPauseRunning;

    public static void PlayHit(Node2D target, Node2D source, int damage)
    {
        Vector2 impactDirection = source.GlobalPosition.DirectionTo(target.GlobalPosition);
        if (impactDirection.LengthSquared() < 0.01f)
        {
            impactDirection = Vector2.Right;
        }

        float weight = Mathf.Clamp(damage / 28f, 0.65f, 2.8f);

        FlashTarget(target, impactDirection, damage);
        SpawnBlood(target, impactDirection, damage);
        SpawnImpactSpark(target, impactDirection, damage);
        SpawnImpactShockwave(target, impactDirection, damage);
        SpawnPainCallout(target, damage);
        PlayImpactSound(target.GetParent(), damage);
        HitPause(target.GetTree(), weight);
        ShakeCamera(source, weight);
    }

    public static void PlayFinisher(Node2D target, Node2D source, ImprovisedWeaponKind weapon)
    {
        Vector2 impactDirection = source.GlobalPosition.DirectionTo(target.GlobalPosition);
        if (impactDirection.LengthSquared() < 0.01f)
        {
            impactDirection = Vector2.Right;
        }

        SpawnFinisherBurst(target, impactDirection, weapon);
        SpawnBlood(target, impactDirection, 80);
        SpawnGroundBlood(target.GetParent(), target.GlobalPosition, impactDirection, 80);
        PlayImpactSound(target.GetParent(), 80);
        FinisherHitPause(target.GetTree());
        ZoomFinisherCamera(source);
    }

    private static void SpawnFinisherBurst(Node2D target, Vector2 direction, ImprovisedWeaponKind weapon)
    {
        Node? parent = target.GetParent();
        if (parent is null)
        {
            return;
        }

        Color burstColor = weapon switch
        {
            ImprovisedWeaponKind.Hammer => new Color(0.92f, 0.22f, 0.12f, 0.95f),
            ImprovisedWeaponKind.Knife => new Color(0.78f, 0.04f, 0.06f, 0.95f),
            _ => new Color(0.88f, 0.38f, 0.14f, 0.92f),
        };

        for (int ring = 0; ring < 3; ring++)
        {
            Polygon2D burst = new()
            {
                Color = burstColor with { A = 0.85f - ring * 0.18f },
                Polygon =
                [
                    new Vector2(-22f, -8f),
                    new Vector2(28f, -12f),
                    new Vector2(42f, 0f),
                    new Vector2(24f, 12f),
                    new Vector2(-20f, 10f),
                ],
                Scale = Vector2.One * (1.2f + ring * 0.45f),
                ZIndex = 14,
            };

            parent.AddChild(burst);
            burst.GlobalPosition = target.GlobalPosition + new Vector2(direction.X * (18f + ring * 8f), -18f + ring * 4f);
            burst.Rotation = direction.Angle();

            Tween tween = burst.CreateTween();
            tween.TweenProperty(burst, "scale", burst.Scale * 1.8f, 0.08f + ring * 0.02f);
            tween.Parallel().TweenProperty(burst, "modulate:a", 0f, 0.14f + ring * 0.03f);
            tween.TweenCallback(Callable.From(burst.QueueFree));
        }
    }

    private static async void FinisherHitPause(SceneTree? tree)
    {
        if (_hitPauseRunning || tree is null)
        {
            return;
        }

        _hitPauseRunning = true;
        double originalTimeScale = Engine.TimeScale;

        try
        {
            Engine.TimeScale = 0.04f;
            SceneTreeTimer timer = tree.CreateTimer(0.12f, processAlways: true, ignoreTimeScale: true);
            await timer.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }
        finally
        {
            Engine.TimeScale = originalTimeScale;
            _hitPauseRunning = false;
        }
    }

    private static async void ZoomFinisherCamera(Node2D source)
    {
        Camera2D? camera = source.GetNodeOrNull<Camera2D>("Camera2D");
        if (camera is null)
        {
            return;
        }

        Vector2 originalZoom = camera.Zoom;
        Tween zoomIn = camera.CreateTween();
        zoomIn.TweenProperty(camera, "zoom", originalZoom * 1.14f, 0.06f);
        await zoomIn.ToSignal(zoomIn, Tween.SignalName.Finished);

        Tween zoomOut = camera.CreateTween();
        zoomOut.TweenProperty(camera, "zoom", originalZoom, 0.22f);
    }

    public static void PlayParry(Node2D defender, Node2D attacker)
    {
        Vector2 direction = defender.GlobalPosition.DirectionTo(attacker.GlobalPosition);
        SpawnImpactSpark(defender, direction, 36);
        HitPause(defender.GetTree());
    }

    public static void PlayPostureKill(Node2D target, Node2D source)
    {
        Vector2 impactDirection = source.GlobalPosition.DirectionTo(target.GlobalPosition);
        if (impactDirection.LengthSquared() < 0.01f)
        {
            impactDirection = Vector2.Right;
        }

        SpawnFinisherBurst(target, impactDirection, ImprovisedWeaponKind.Rebar);
        SpawnBlood(target, impactDirection, 95);
        SpawnGroundBlood(target.GetParent(), target.GlobalPosition, impactDirection, 95);
        FinisherHitPause(target.GetTree());
        ZoomFinisherCamera(source);
    }

    private static void FlashTarget(Node2D target, Vector2 direction, int damage)
    {
        CharacterSpriteVisual? visual = target.GetNodeOrNull<CharacterSpriteVisual>("SpriteVisual");
        if (visual is not null)
        {
            visual.PlayHitReaction(direction, Mathf.Clamp(damage / 22f, 0.7f, 3.2f));
            return;
        }

        target.Modulate = new Color(1f, 0.18f, 0.18f, 1f);

        Tween tween = target.CreateTween();
        tween.TweenProperty(target, "modulate", Colors.White, FlashDuration);
    }

    private static void SpawnBlood(Node2D target, Vector2 direction, int damage)
    {
        Node? parent = target.GetParent();
        if (parent is null)
        {
            return;
        }

        int splatCount = Mathf.Clamp(Mathf.CeilToInt(damage / 8f), 4, 9);
        for (int i = 0; i < splatCount; i++)
        {
            Polygon2D droplet = new()
            {
                Color = new Color(0.58f, 0.005f, 0.015f, 0.95f),
                Polygon =
                [
                    new Vector2(-7f, -3f),
                    new Vector2(7f, -4f),
                    new Vector2(11f, 1f),
                    new Vector2(3f, 6f),
                    new Vector2(-8f, 4f)
                ],
                Scale = Vector2.One * (0.9f + i * 0.12f),
                ZIndex = 8
            };

            parent.AddChild(droplet);
            float sideNoise = (i - splatCount * 0.5f) * 5f;
            droplet.GlobalPosition = target.GlobalPosition + new Vector2(direction.X * (14f + i * 5f), -20f + sideNoise);
            droplet.Rotation = direction.Angle() + sideNoise * 0.03f;

            Tween tween = droplet.CreateTween();
            tween.TweenProperty(droplet, "position", droplet.Position + new Vector2(direction.X * (28f + i * 3f), 24f + i * 4f), 0.28f);
            tween.Parallel().TweenProperty(droplet, "modulate:a", 0f, 0.42f);
            tween.TweenCallback(Callable.From(droplet.QueueFree));
        }

        SpawnGroundBlood(parent, target.GlobalPosition, direction, damage);
    }

    private static void SpawnGroundBlood(Node parent, Vector2 origin, Vector2 direction, int damage)
    {
        Polygon2D puddle = new()
        {
            Color = new Color(0.33f, 0f, 0.012f, 0.72f),
            Polygon =
            [
                new Vector2(-18f, -4f),
                new Vector2(5f, -9f),
                new Vector2(24f, -3f),
                new Vector2(31f, 6f),
                new Vector2(6f, 12f),
                new Vector2(-24f, 7f)
            ],
            Scale = Vector2.One * Mathf.Clamp(damage / 22f, 1.1f, 2.3f),
            ZIndex = 3
        };

        parent.AddChild(puddle);
        puddle.GlobalPosition = origin + new Vector2(direction.X * 20f, 22f);
        puddle.Rotation = direction.Angle() * 0.18f;

        Tween tween = puddle.CreateTween();
        tween.TweenInterval(1.15f);
        tween.TweenProperty(puddle, "modulate:a", 0f, 1.8f);
        tween.TweenCallback(Callable.From(puddle.QueueFree));
    }

    private static void SpawnImpactSpark(Node2D target, Vector2 direction, int damage)
    {
        Node? parent = target.GetParent();
        if (parent is null)
        {
            return;
        }

        float scale = damage >= 45 ? 1.35f : 1f;
        Polygon2D spark = new()
        {
            Color = new Color(1f, 0.82f, 0.28f, 0.95f),
            Polygon =
            [
                new Vector2(-4f, -3f),
                new Vector2(26f, -7f),
                new Vector2(38f, 0f),
                new Vector2(25f, 7f),
                new Vector2(-4f, 3f)
            ],
            Scale = new Vector2(Mathf.Sign(direction.X == 0f ? 1f : direction.X) * scale, scale),
            ZIndex = 10
        };

        parent.AddChild(spark);
        spark.GlobalPosition = target.GlobalPosition + new Vector2(direction.X * 20f, -16f);
        spark.Rotation = direction.Angle();

        Tween tween = spark.CreateTween();
        tween.TweenProperty(spark, "scale", spark.Scale * (damage >= 45 ? 1.65f : 1.45f), 0.06f);
        tween.Parallel().TweenProperty(spark, "modulate:a", 0f, 0.09f);
        tween.TweenCallback(Callable.From(spark.QueueFree));
    }

    private static async void PlayImpactSound(Node? parent, int damage)
    {
        if (parent is null)
        {
            return;
        }

        const int mixRate = 22050;
        const float duration = 0.11f;
        int frameCount = Mathf.RoundToInt(mixRate * duration);
        float frequency = damage >= 45 ? 92f : damage >= 28 ? 118f : 148f;
        float volume = damage >= 38 ? -6f : -9f;

        AudioStreamGenerator stream = new()
        {
            MixRate = mixRate,
            BufferLength = duration
        };

        AudioStreamPlayer player = new()
        {
            Stream = stream,
            VolumeDb = volume,
            PitchScale = damage >= 45 ? 0.78f : damage >= 28 ? 0.88f : 0.96f
        };

        parent.AddChild(player);
        player.Play();

        if (player.GetStreamPlayback() is AudioStreamGeneratorPlayback playback)
        {
            for (int i = 0; i < frameCount; i++)
            {
                float t = i / (float)mixRate;
                float envelope = 1f - i / (float)frameCount;
                float noise = GD.Randf() * 2f - 1f;
                float body = Mathf.Sin(Mathf.Tau * frequency * t);
                float sample = (body * 0.45f + noise * 0.55f) * envelope * 0.38f;
                playback.PushFrame(new Vector2(sample, sample));
            }
        }

        SceneTreeTimer timer = player.GetTree().CreateTimer(duration + 0.05f, processAlways: true);
        await player.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        player.QueueFree();
    }

    private static void SpawnImpactShockwave(Node2D target, Vector2 direction, int damage)
    {
        Node? parent = target.GetParent();
        if (parent is null)
        {
            return;
        }

        Polygon2D ring = new()
        {
            Color = new Color(1f, 0.92f, 0.78f, 0.55f),
            Polygon = MakeRingPolygon(12f + damage * 0.08f),
            ZIndex = 11,
        };

        parent.AddChild(ring);
        ring.GlobalPosition = target.GlobalPosition + new Vector2(direction.X * 16f, -18f);

        Tween tween = ring.CreateTween();
        tween.TweenProperty(ring, "scale", Vector2.One * (1.6f + damage / 40f), 0.1f);
        tween.Parallel().TweenProperty(ring, "modulate:a", 0f, 0.14f);
        tween.TweenCallback(Callable.From(ring.QueueFree));
    }

    private static Vector2[] MakeRingPolygon(float radius)
    {
        const int segments = 10;
        Vector2[] points = new Vector2[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = i / (float)segments * Mathf.Tau;
            points[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius * 0.55f);
        }

        return points;
    }

    private static void SpawnPainCallout(Node2D target, int damage)
    {
        Node? parent = target.GetParent();
        if (parent is null)
        {
            return;
        }

        string text = damage switch
        {
            >= 55 => "CRACK!",
            >= 38 => "AU!",
            >= 24 => "ugh!",
            _ => "hf",
        };

        Label callout = new()
        {
            Text = text,
            ZIndex = 20,
        };
        callout.AddThemeFontSizeOverride("font_size", damage >= 38 ? 20 : 15);
        callout.AddThemeColorOverride("font_color", damage >= 38
            ? new Color(1f, 0.32f, 0.22f)
            : new Color(1f, 0.78f, 0.62f));
        callout.AddThemeColorOverride("font_outline_color", Colors.Black);
        callout.AddThemeConstantOverride("outline_size", 4);

        parent.AddChild(callout);
        callout.GlobalPosition = target.GlobalPosition + new Vector2(-18f, -58f - GD.Randf() * 12f);

        Tween tween = callout.CreateTween();
        tween.TweenProperty(callout, "position", callout.Position + new Vector2(GD.Randf() * 16f - 8f, -28f), 0.38f);
        tween.Parallel().TweenProperty(callout, "modulate:a", 0f, 0.42f);
        tween.TweenCallback(Callable.From(callout.QueueFree));
    }

    private static void ShakeCamera(Node2D source, float weight)
    {
        Camera2D? camera = source.GetNodeOrNull<Camera2D>("Camera2D");
        if (camera is null)
        {
            return;
        }

        Vector2 baseOffset = camera.Offset;
        float mag = 3.5f + weight * 4f;
        Vector2 kick = new Vector2((GD.Randf() * 2f - 1f) * mag, (GD.Randf() * 2f - 1f) * mag * 0.35f);

        Tween tween = camera.CreateTween();
        tween.TweenProperty(camera, "offset", baseOffset + kick, 0.04f);
        tween.TweenProperty(camera, "offset", baseOffset - kick * 0.35f, 0.05f);
        tween.TweenProperty(camera, "offset", baseOffset, 0.07f);
    }

    private static async void HitPause(SceneTree? tree, float weight = 1f)
    {
        if (_hitPauseRunning || tree is null)
        {
            return;
        }

        _hitPauseRunning = true;
        double originalTimeScale = Engine.TimeScale;
        float duration = Mathf.Clamp(0.05f + weight * 0.028f, 0.05f, 0.11f);

        try
        {
            Engine.TimeScale = HitPauseTimeScale;
            SceneTreeTimer timer = tree.CreateTimer(duration, processAlways: true, ignoreTimeScale: true);
            await timer.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }
        finally
        {
            Engine.TimeScale = originalTimeScale;
            _hitPauseRunning = false;
        }
    }
}
