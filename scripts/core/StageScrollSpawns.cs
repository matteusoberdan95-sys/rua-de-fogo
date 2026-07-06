namespace SangueNoAsfalto.Core;

/// <summary>
/// Spawn por progresso na fase — estilo Final Fight / Sunset Riders.
/// TriggerX = posicao X absoluta na rua quando o jogador ativa o spawn.
/// </summary>
public static class StageScrollSpawns
{
    public enum SpawnKind
    {
        Grunt,
        Fast,
        Brute,
        Infected,
        MiniBoss,
        RainBoss,
        AlphaBoss,
        Checkpoint,
        StatusOnly,
    }

    public readonly record struct Entry(
        float TriggerX,
        SpawnKind Kind,
        float LaneY = 405f,
        float SpawnOffsetX = 620f,
        bool EnterFromLeft = false,
        string Status = "");

    public const float StageEndX = 3180f;

    public static IReadOnlyList<Entry> BuildVilaEsperancaRun()
    {
        return
        [
            new(-740f, SpawnKind.StatusOnly, Status: "A rua acordou errada. Avance."),
            new(-560f, SpawnKind.Grunt, 405f),
            new(-420f, SpawnKind.Grunt, 392f),
            new(-280f, SpawnKind.Fast, 438f),
            new(-140f, SpawnKind.Grunt, 405f),
            new(-10f, SpawnKind.Grunt, 418f, Status: "Primeiro contato."),
            new(120f, SpawnKind.Fast, 405f),
            new(250f, SpawnKind.Grunt, 438f),
            new(380f, SpawnKind.Infected, 405f),
            new(480f, SpawnKind.Checkpoint, 405f, Status: "Altar improvisado — checkpoint."),
            new(560f, SpawnKind.Grunt, 405f, -620f, true),
            new(680f, SpawnKind.Fast, 438f),
            new(820f, SpawnKind.Grunt, 405f),
            new(960f, SpawnKind.Infected, 418f),
            new(1100f, SpawnKind.Fast, 392f, -620f, true, "Corredores na chuva."),
            new(1240f, SpawnKind.Grunt, 405f),
            new(1380f, SpawnKind.Infected, 438f),
            new(1520f, SpawnKind.Fast, 405f),
            new(1660f, SpawnKind.Grunt, 418f),
            new(1800f, SpawnKind.Brute, 405f, Status: "Bruto abrindo caminho."),
            new(1940f, SpawnKind.Grunt, 392f),
            new(2080f, SpawnKind.Fast, 438f),
            new(2220f, SpawnKind.Infected, 405f),
            new(2360f, SpawnKind.Grunt, 418f),
            new(2500f, SpawnKind.MiniBoss, 405f, Status: "O portao range. Algo grande vem."),
            new(2640f, SpawnKind.Grunt, 438f),
            new(2780f, SpawnKind.Infected, 405f),
            new(2900f, SpawnKind.Fast, 392f, Status: "A tempestade engrossou."),
            new(3020f, SpawnKind.RainBoss, 405f, Status: "Infectado da chuva."),
            new(3120f, SpawnKind.AlphaBoss, 405f, Status: "O alfa da rua apareceu."),
            new(StageEndX, SpawnKind.StatusOnly, Status: "Fim da Vila Esperanca."),
        ];
    }
}
