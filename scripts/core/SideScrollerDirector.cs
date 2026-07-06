namespace SangueNoAsfalto.Core;

public partial class SideScrollerDirector : Node
{
    private const string MainMenuScenePath = "res://scenes/ui/MainMenu.tscn";
    private const float SpawnStaggerSec = 0.45f;

    [Export]
    public PackedScene? EnemyScene { get; set; }

    [Export]
    public PackedScene? MiniBossScene { get; set; }

    [Export]
    public PackedScene? FastEnemyScene { get; set; }

    [Export]
    public PackedScene? BruteEnemyScene { get; set; }

    [Export]
    public PackedScene? InfectedEnemyScene { get; set; }

    [Export]
    public PackedScene? SecondMiniBossScene { get; set; }

    [Export]
    public PackedScene? FinalBossScene { get; set; }

    [Export]
    public NodePath PlayerPath { get; set; } = "../SideScrollerPlayer";

    [Export]
    public NodePath WeatherControllerPath { get; set; } = "../WeatherController";

    [Export]
    public NodePath TimeOfDayControllerPath { get; set; } = "../TimeOfDayController";

    [Export]
    public Vector2 StartPosition { get; set; } = new(-760f, 405f);

    [Export]
    public Vector2 CheckpointPosition { get; set; } = new(480f, 405f);

    [Export]
    public float StageEndX { get; set; } = StageScrollSpawns.StageEndX;

    [Export]
    public float StageExitX { get; set; } = StageScrollSpawns.StageExitX;

    public int WaveNumber { get; private set; }

    public int TotalWaves => _spawnEntries.Count;

    public int EnemiesRemaining { get; private set; }

    public string StageTitle { get; private set; } = "VILA ESPERANCA";

    public string StageTagline { get; private set; } = "A rua nao esquece. Ela so espera.";

    public string StatusText { get; private set; } = "A rua acordou errada";

    public void SetClimateHint(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            StatusText = text;
        }
    }

    public string ObjectiveText { get; private set; } = "Atravesse a Vila Esperanca";

    public bool HasCheckpoint { get; private set; }

    public bool IsGameOver => _gameOver;

    public bool IsCompleted => _completed;

    public bool ShowDebugHud => SaveManager.Current.ShowDebugHud;

    public bool AlternateControls => SaveManager.Current.AlternateControls;

    private static bool _checkpointUnlocked;
    private readonly List<StageScrollSpawns.Entry> _spawnEntries = [];
    private readonly Queue<(StageScrollSpawns.Entry Entry, float SpawnAt)> _spawnQueue = new();
    private int _nextEntryIndex;
    private int _spawnedFightCount;
    private SideScrollerPlayerController? _player;
    private Health? _playerHealth;
    private WeatherController? _weather;
    private TimeOfDayController? _timeOfDay;
    private StageClimateDirector? _climate;
    private bool _completed;
    private bool _gameOver;
    private bool _f1WasDown;
    private bool _f2WasDown;
    private bool _f4WasDown;
    private float _queueClock;
    private bool _finalWaveSpawned;
    private bool _runCompletePending;

    public override void _Ready()
    {
        AddToGroup("game_director");
        AddToGroup("side_director");
        SaveManager.Load();
        _checkpointUnlocked = SaveManager.Current.CheckpointUnlocked;
        InputBootstrap.ApplyAlternateControls(SaveManager.Current.AlternateControls);
        _weather = GetNodeOrNull<WeatherController>(WeatherControllerPath);
        _timeOfDay = GetNodeOrNull<TimeOfDayController>(TimeOfDayControllerPath);
        _climate = GetNodeOrNull<StageClimateDirector>("../StageClimateDirector");
        _climate ??= GetTree().GetFirstNodeInGroup("climate_director") as StageClimateDirector;
        _spawnEntries.AddRange(StageScrollSpawns.BuildVilaEsperancaRun());
        FindPlayer();
        ApplyRespawnState();
        if (_weather is not null)
        {
            _weather.AutoCycle = false;
        }

        if (_timeOfDay is not null)
        {
            _timeOfDay.AutoCycle = false;
        }

        ObjectiveText = "Atravesse a Vila Esperanca";
    }

    public override void _Process(double delta)
    {
        if ((_gameOver || _completed) && Input.IsKeyPressed(Key.M))
        {
            GetTree().ChangeSceneToFile(MainMenuScenePath);
            return;
        }

        if (Input.IsActionJustPressed("restart") || Input.IsKeyPressed(Key.R))
        {
            ReloadRun();
            return;
        }

        HandlePrototypeSettings();

        if (_gameOver || _completed)
        {
            return;
        }

        FindPlayer();
        if (_playerHealth is not null && _playerHealth.CurrentHealth <= 0)
        {
            Vector2 respawnPosition = HasCheckpoint ? CheckpointPosition : StartPosition;
            if (_player?.TryUseContinue(respawnPosition) == true)
            {
                StatusText = "Voce gastou um continue e voltou cambaleando.";
                _playerHealth = _player.GetNodeOrNull<Health>("Health");
                return;
            }

            _gameOver = true;
            StatusText = HasCheckpoint
                ? "Caiu no asfalto. Aperte R para voltar ao checkpoint."
                : "Caiu no asfalto. Aperte R para voltar ao inicio.";
            return;
        }

        if (_player is null)
        {
            return;
        }

        float playerX = _player.GlobalPosition.X;
        EnemiesRemaining = CountLivingEnemies();
        WaveNumber = Mathf.Clamp(_spawnedFightCount, 0, _spawnEntries.Count);

        while (_nextEntryIndex < _spawnEntries.Count && playerX >= _spawnEntries[_nextEntryIndex].TriggerX)
        {
            StageScrollSpawns.Entry entry = _spawnEntries[_nextEntryIndex];
            _nextEntryIndex++;
            HandleStageEntry(entry);
        }

        _queueClock += (float)delta;
        while (_spawnQueue.Count > 0 && _queueClock >= _spawnQueue.Peek().SpawnAt)
        {
            SpawnStageEnemy(_spawnQueue.Dequeue().Entry);
        }

        ApplyClimateProgress(playerX);
        EnemiesRemaining = CountLivingEnemies();

        if (_runCompletePending)
        {
            if (playerX >= StageExitX || EnemiesRemaining == 0)
            {
                CompleteRun();
            }
            else
            {
                ObjectiveText = "Avance ate o portao de saida";
            }

            return;
        }

        if (_nextEntryIndex >= _spawnEntries.Count
            && EnemiesRemaining == 0
            && _finalWaveSpawned)
        {
            if (playerX >= StageExitX)
            {
                CompleteRun();
            }
            else
            {
                _runCompletePending = true;
                ObjectiveText = "Chefe derrotado — siga ate o portao";
                StatusText = "A saida da Vila Esperanca esta a frente.";
            }
        }
    }

    private float ComputeSpawnX(StageScrollSpawns.Entry entry)
    {
        if (_player is null)
        {
            return entry.TriggerX + 480f;
        }

        const float minSpawnX = -880f;
        const float maxSpawnX = 3280f;
        const float fightLead = 460f;

        float playerX = _player.GlobalPosition.X;
        float anchoredAhead = entry.TriggerX + fightLead;
        float anchoredBehind = entry.TriggerX - fightLead;

        float spawnX = entry.EnterFromLeft
            ? Mathf.Min(playerX - 240f, anchoredBehind)
            : Mathf.Max(playerX + 260f, anchoredAhead);

        if (entry.TriggerX >= 2000f)
        {
            spawnX = entry.EnterFromLeft ? anchoredBehind : anchoredAhead;
        }

        return Mathf.Clamp(spawnX, minSpawnX, maxSpawnX);
    }

    private void HandleStageEntry(StageScrollSpawns.Entry entry)
    {
        if (!string.IsNullOrEmpty(entry.Status))
        {
            StatusText = entry.Status;
        }

        switch (entry.Kind)
        {
            case StageScrollSpawns.SpawnKind.StatusOnly:
                return;
            case StageScrollSpawns.SpawnKind.Checkpoint:
                UnlockCheckpoint();
                return;
            default:
                _spawnQueue.Enqueue((entry, _queueClock + SpawnStaggerSec * (_spawnQueue.Count % 2)));
                break;
        }
    }

    private void SpawnStageEnemy(StageScrollSpawns.Entry entry)
    {
        PackedScene? scene = ResolveScene(entry.Kind);
        if (scene is null || _player is null)
        {
            return;
        }

        Node2D enemy = scene.Instantiate<Node2D>();
        float spawnX = ComputeSpawnX(entry);
        float spawnY = Mathf.Clamp(entry.LaneY, 268f, 465f);
        enemy.GlobalPosition = new Vector2(spawnX, spawnY);
        enemy.Modulate = new Color(1f, 1f, 1f, 0.35f);
        GetTree().CurrentScene?.AddChild(enemy);

        Tween fade = enemy.CreateTween();
        fade.TweenProperty(enemy, "modulate:a", 1f, 0.28f);

        if (entry.Kind is StageScrollSpawns.SpawnKind.MiniBoss
            or StageScrollSpawns.SpawnKind.RainBoss
            or StageScrollSpawns.SpawnKind.AlphaBoss)
        {
            enemy.SetMeta("stage_boss_kind", (int)entry.Kind);
            _climate?.NotifyBossSpawned(entry.Kind);
        }

        if (entry.Kind is not StageScrollSpawns.SpawnKind.StatusOnly
            and not StageScrollSpawns.SpawnKind.Checkpoint)
        {
            _spawnedFightCount++;
            if (entry.Kind is StageScrollSpawns.SpawnKind.RainBoss or StageScrollSpawns.SpawnKind.AlphaBoss)
            {
                _finalWaveSpawned = true;
            }
        }
    }

    private PackedScene? ResolveScene(StageScrollSpawns.SpawnKind kind)
    {
        return kind switch
        {
            StageScrollSpawns.SpawnKind.Grunt => EnemyScene,
            StageScrollSpawns.SpawnKind.Fast => FastEnemyScene ?? EnemyScene,
            StageScrollSpawns.SpawnKind.Brute => BruteEnemyScene ?? EnemyScene,
            StageScrollSpawns.SpawnKind.Infected => InfectedEnemyScene ?? EnemyScene,
            StageScrollSpawns.SpawnKind.MiniBoss => MiniBossScene ?? BruteEnemyScene ?? EnemyScene,
            StageScrollSpawns.SpawnKind.RainBoss => SecondMiniBossScene ?? InfectedEnemyScene ?? EnemyScene,
            StageScrollSpawns.SpawnKind.AlphaBoss => FinalBossScene ?? BruteEnemyScene ?? EnemyScene,
            _ => null,
        };
    }

    private void ApplyClimateProgress(float playerX)
    {
        // Clima/horario por ato: StageClimateDirector (Sprint 33).
        _ = playerX;
    }

    private void UnlockCheckpoint()
    {
        HasCheckpoint = true;
        _checkpointUnlocked = true;
        SaveManager.Current.CheckpointUnlocked = true;
        SaveManager.Save();
        StatusText = "Checkpoint ativo no altar improvisado.";
    }

    private void CompleteRun()
    {
        _completed = true;
        _checkpointUnlocked = false;
        SaveManager.Current.CheckpointUnlocked = false;
        SaveManager.Save();
        ObjectiveText = "Fase concluida";
        StatusText = "Vila Esperanca: trecho limpo. Aperte R para jogar de novo.";
    }

    private void ReloadRun()
    {
        if (!_gameOver && !_completed)
        {
            return;
        }

        if (_gameOver && !HasCheckpoint)
        {
            _checkpointUnlocked = false;
            SaveManager.Current.CheckpointUnlocked = false;
            SaveManager.Save();
        }

        GetTree().ReloadCurrentScene();
    }

    private void HandlePrototypeSettings()
    {
        if (ConsumeKeyPress(Key.F1, ref _f1WasDown))
        {
            SaveManager.Current.ShowDebugHud = !SaveManager.Current.ShowDebugHud;
            SaveManager.Save();
        }

        if (ConsumeKeyPress(Key.F2, ref _f2WasDown))
        {
            SaveManager.Current.AlternateControls = !SaveManager.Current.AlternateControls;
            InputBootstrap.ApplyAlternateControls(SaveManager.Current.AlternateControls);
            SaveManager.Save();
        }

        if (ConsumeKeyPress(Key.F4, ref _f4WasDown))
        {
            SaveManager.Reset();
            _checkpointUnlocked = false;
            GetTree().ReloadCurrentScene();
        }
    }

    private static bool ConsumeKeyPress(Key key, ref bool wasDown)
    {
        bool isDown = Input.IsKeyPressed(key) || Input.IsPhysicalKeyPressed(key);
        bool justPressed = isDown && !wasDown;
        wasDown = isDown;
        return justPressed;
    }

    private void ApplyRespawnState()
    {
        HasCheckpoint = _checkpointUnlocked;
        if (_player is null)
        {
            return;
        }

        if (_checkpointUnlocked)
        {
            _player.GlobalPosition = CheckpointPosition;
            float resumeX = CheckpointPosition.X - 40f;
            while (_nextEntryIndex < _spawnEntries.Count && _spawnEntries[_nextEntryIndex].TriggerX < resumeX)
            {
                _nextEntryIndex++;
            }
        }
        else
        {
            _player.GlobalPosition = StartPosition;
        }
    }

    private int CountLivingEnemies()
    {
        int count = 0;
        foreach (Node node in GetTree().GetNodesInGroup("enemy"))
        {
            if (!node.IsQueuedForDeletion())
            {
                count++;
            }
        }

        return count;
    }

    private void FindPlayer()
    {
        if (_player is not null && _playerHealth is not null)
        {
            return;
        }

        _player ??= GetNodeOrNull<SideScrollerPlayerController>(PlayerPath);
        _player ??= GetTree().GetFirstNodeInGroup("side_player") as SideScrollerPlayerController;

        if (_player is not null)
        {
            _playerHealth = _player.GetNodeOrNull<Health>("Health");
        }
    }
}
