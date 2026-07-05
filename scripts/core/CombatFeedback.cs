namespace SangueNoAsfalto.Core;

public static class CombatFeedback
{
    private const float HitPauseDuration = 0.045f;
    private const float HitPauseTimeScale = 0.08f;
    private const float FlashDuration = 0.11f;
    private static bool _hitPauseRunning;

    public static void PlayHit(Node2D target, Node2D source, int damage)
    {
        Vector2 impactDirection = source.GlobalPosition.DirectionTo(target.GlobalPosition);
        if (impactDirection.LengthSquared() < 0.01f)
        {
            impactDirection = Vector2.Right;
        }

        FlashTarget(target);
        SpawnBlood(target, impactDirection, damage);
        SpawnImpactSpark(target, impactDirection);
        PlayImpactSound(target.GetParent(), damage);
        HitPause(target.GetTree());
    }

    private static void FlashTarget(Node2D target)
    {
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

    private static void SpawnImpactSpark(Node2D target, Vector2 direction)
    {
        Node? parent = target.GetParent();
        if (parent is null)
        {
            return;
        }

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
            Scale = new Vector2(Mathf.Sign(direction.X == 0f ? 1f : direction.X), 1f),
            ZIndex = 10
        };

        parent.AddChild(spark);
        spark.GlobalPosition = target.GlobalPosition + new Vector2(direction.X * 20f, -16f);
        spark.Rotation = direction.Angle();

        Tween tween = spark.CreateTween();
        tween.TweenProperty(spark, "scale", spark.Scale * 1.45f, 0.06f);
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
        const float duration = 0.085f;
        int frameCount = Mathf.RoundToInt(mixRate * duration);
        float frequency = damage >= 45 ? 118f : 156f;

        AudioStreamGenerator stream = new()
        {
            MixRate = mixRate,
            BufferLength = duration
        };

        AudioStreamPlayer player = new()
        {
            Stream = stream,
            VolumeDb = -8f,
            PitchScale = damage >= 45 ? 0.86f : 1f
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

    private static async void HitPause(SceneTree? tree)
    {
        if (_hitPauseRunning || tree is null)
        {
            return;
        }

        _hitPauseRunning = true;
        double originalTimeScale = Engine.TimeScale;

        try
        {
            Engine.TimeScale = HitPauseTimeScale;
            SceneTreeTimer timer = tree.CreateTimer(HitPauseDuration, processAlways: true, ignoreTimeScale: true);
            await timer.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        }
        finally
        {
            Engine.TimeScale = originalTimeScale;
            _hitPauseRunning = false;
        }
    }
}
