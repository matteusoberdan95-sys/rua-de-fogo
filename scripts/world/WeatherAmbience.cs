namespace SangueNoAsfalto.World;

/// <summary>
/// SFX placeholder procedural para chuva e trovao (Sprint 33).
/// </summary>
public static class WeatherAmbience
{
    private const int MixRate = 22050;

    public static async void PlayThunder(Node parent)
    {
        float duration = 0.55f;
        int frameCount = Mathf.RoundToInt(MixRate * duration);

        AudioStreamGenerator stream = new()
        {
            MixRate = MixRate,
            BufferLength = duration + 0.05f,
        };

        AudioStreamPlayer player = new()
        {
            Stream = stream,
            VolumeDb = -4f,
            PitchScale = (float)GD.RandRange(0.88, 1.05),
        };

        parent.AddChild(player);
        player.Play();

        if (player.GetStreamPlayback() is AudioStreamGeneratorPlayback playback)
        {
            for (int i = 0; i < frameCount; i++)
            {
                float t = i / (float)MixRate;
                float envelope = 1f - i / (float)frameCount;
                envelope = envelope * envelope * envelope;
                float rumble = Mathf.Sin(Mathf.Tau * 42f * t) * 0.35f;
                float crack = i < frameCount * 0.08f ? GD.Randf() * 0.9f : 0f;
                float noise = GD.Randf() * 2f - 1f;
                float sample = (rumble + noise * 0.55f + crack) * envelope * 0.5f;
                playback.PushFrame(new Vector2(sample, sample));
            }
        }

        SceneTreeTimer timer = player.GetTree().CreateTimer(duration + 0.08f, processAlways: true);
        await player.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        player.QueueFree();
    }

    public static async void PlayRainTick(Node parent, float intensity)
    {
        if (intensity < 0.05f)
        {
            return;
        }

        float duration = 0.04f;
        int frameCount = Mathf.RoundToInt(MixRate * duration);

        AudioStreamGenerator stream = new()
        {
            MixRate = MixRate,
            BufferLength = duration + 0.02f,
        };

        AudioStreamPlayer player = new()
        {
            Stream = stream,
            VolumeDb = -22f - (1f - intensity) * 8f,
            PitchScale = (float)GD.RandRange(0.95, 1.15),
        };

        parent.AddChild(player);
        player.Play();

        if (player.GetStreamPlayback() is AudioStreamGeneratorPlayback playback)
        {
            for (int i = 0; i < frameCount; i++)
            {
                float envelope = 1f - i / (float)frameCount;
                float sample = (GD.Randf() * 2f - 1f) * envelope * 0.18f * intensity;
                playback.PushFrame(new Vector2(sample, sample));
            }
        }

        SceneTreeTimer timer = player.GetTree().CreateTimer(duration + 0.02f, processAlways: true);
        await player.ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
        player.QueueFree();
    }
}
