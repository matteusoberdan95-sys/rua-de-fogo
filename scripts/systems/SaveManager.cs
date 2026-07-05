namespace SangueNoAsfalto.Systems;

public static class SaveManager
{
    private const string SavePath = "user://save_game.json";
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static GameSave Current { get; private set; } = new();

    public static void Load()
    {
        string path = ProjectSettings.GlobalizePath(SavePath);
        if (!File.Exists(path))
        {
            Current = new GameSave();
            return;
        }

        try
        {
            Current = JsonSerializer.Deserialize<GameSave>(File.ReadAllText(path), JsonOptions) ?? new GameSave();
        }
        catch (Exception error)
        {
            GD.PushWarning($"Falha ao carregar save local: {error.Message}");
            Current = new GameSave();
        }
    }

    public static void Save()
    {
        string path = ProjectSettings.GlobalizePath(SavePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
        File.WriteAllText(path, JsonSerializer.Serialize(Current, JsonOptions));
    }

    public static void Reset()
    {
        Current = new GameSave();
        string path = ProjectSettings.GlobalizePath(SavePath);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
