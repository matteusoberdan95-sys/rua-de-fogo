namespace SangueNoAsfalto.World;

/// <summary>
/// Contexto compartilhado para montar props de cenario com animacao leve (neon, vento, reflexos).
/// </summary>
public sealed class StageAssetContext
{
    public List<CanvasItem> NeonItems { get; } = [];
    public List<Node2D> WindItems { get; } = [];
    public List<CanvasItem> WetHighlights { get; } = [];
    public List<CanvasItem> FlickerItems { get; } = [];
}
