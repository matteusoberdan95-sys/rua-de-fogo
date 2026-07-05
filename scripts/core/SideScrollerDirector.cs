namespace SangueNoAsfalto.Core;

public partial class SideScrollerDirector : Node
{
    private const string MainMenuScenePath = "res://scenes/ui/MainMenu.tscn";

    private enum Phase
    {
        EncounterOne,
        Checkpoint,
        EncounterTwo,
        EncounterThree,
        MiniBoss,
        MiniBossTwo,
        FinalBoss,
        Victory
    }

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
    public NodePath SpawnPointsPath { get; set; } = "../SpawnPoints";

    [Export]
    public NodePath PlayerPath { get; set; } = "../SideScrollerPlayer";

    [Export]
    public float TimeBetweenEncounters { get; set; } = 1.4f;

    [Export]
    public Vector2 StartPosition { get; set; } = new(-760f, 405f);

    [Export]
    public Vector2 CheckpointPosition { get; set; } = new(-80f, 405f);

    public int WaveNumber { get; private set; }

    public int TotalWaves => 6;

    public int EnemiesRemaining { get; private set; }

    public string StatusText { get; private set; } = "A rua acordou errada";

    public string ObjectiveText { get; private set; } = "Atravesse a Vila Esperanca";

    public bool HasCheckpoint { get; private set; }

    public bool IsGameOver => _gameOver;

    public bool IsCompleted => _completed;

    public bool ShowDebugHud => SaveManager.Current.ShowDebugHud;

    public bool AlternateControls => SaveManager.Current.AlternateControls;

    private static Phase _resumePhase = Phase.EncounterOne;
    private static bool _checkpointUnlocked;
    private readonly PackedScene?[] _emptyComposition = [];
    private Node2D? _spawnPoints;
    private Health? _playerHealth;
    private SideScrollerPlayerController? _player;
    private Phase _phase;
    private float _phaseTimer;
    private bool _phaseActive;
    private bool _completed;
    private bool _gameOver;
    private bool _f1WasDown;
    private bool _f2WasDown;
    private bool _f4WasDown;

    public override void _Ready()
    {
        AddToGroup("game_director");
        AddToGroup("side_director");
        SaveManager.Load();
        _checkpointUnlocked = SaveManager.Current.CheckpointUnlocked;
        InputBootstrap.ApplyAlternateControls(SaveManager.Current.AlternateControls);
        _spawnPoints = GetNodeOrNull<Node2D>(SpawnPointsPath);
        FindPlayer();
        ApplyRespawnState();
        StartPhase(_resumePhase);
    }

    public override void _Process(double delta)
    {
        if ((_gameOver || _completed) && Input.IsKeyPressed(Key.M))
        {
            _resumePhase = Phase.EncounterOne;
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
                _resumePhase = HasCheckpoint ? Phase.EncounterTwo : Phase.EncounterOne;
                _playerHealth = _player.GetNodeOrNull<Health>("Health");
                return;
            }

            _gameOver = true;
            _phaseActive = false;
            StatusText = HasCheckpoint
                ? "Caiu no asfalto. Aperte R para voltar ao checkpoint."
                : "Caiu no asfalto. Aperte R para voltar ao inicio.";
            return;
        }

        EnemiesRemaining = CountLivingEnemies();

        if (_phaseActive && EnemiesRemaining == 0)
        {
            _phaseActive = false;
            _phaseTimer = TimeBetweenEncounters;
            OnPhaseCleared();
        }

        if (!_phaseActive)
        {
            TickPhaseBreak((float)delta);
        }
    }

    private void TickPhaseBreak(float dt)
    {
        if (_completed || _gameOver)
        {
            return;
        }

        _phaseTimer -= dt;
        if (_phaseTimer > 0f)
        {
            return;
        }

        switch (_phase)
        {
            case Phase.Checkpoint:
                UnlockCheckpoint();
                StartPhase(Phase.EncounterTwo);
                break;
            case Phase.EncounterOne:
                StartPhase(Phase.Checkpoint);
                break;
            case Phase.EncounterTwo:
                StartPhase(Phase.EncounterThree);
                break;
            case Phase.EncounterThree:
                StartPhase(Phase.MiniBoss);
                break;
            case Phase.MiniBoss:
                StartPhase(Phase.MiniBossTwo);
                break;
            case Phase.MiniBossTwo:
                StartPhase(Phase.FinalBoss);
                break;
            case Phase.FinalBoss:
                CompleteRun();
                break;
        }
    }

    private void StartPhase(Phase phase)
    {
        _phase = phase;
        EnemiesRemaining = 0;

        switch (phase)
        {
            case Phase.EncounterOne:
                WaveNumber = 1;
                ObjectiveText = "Limpe a entrada da rua";
                StatusText = "Entrada da Vila Esperanca: sobreviva ao primeiro ataque.";
                SpawnComposition([EnemyScene, EnemyScene, EnemyScene]);
                break;
            case Phase.Checkpoint:
                WaveNumber = 1;
                ObjectiveText = "Alcance o altar improvisado";
                StatusText = "Respira. O altar virou checkpoint.";
                _phaseActive = false;
                _phaseTimer = 1.2f;
                break;
            case Phase.EncounterTwo:
                WaveNumber = 2;
                ObjectiveText = "Segure a rua contra os corredores";
                StatusText = "Reforcos rapidos na chuva. Nao deixa fecharem a lane.";
                SpawnComposition([EnemyScene, FastEnemyScene, EnemyScene, InfectedEnemyScene]);
                break;
            case Phase.EncounterThree:
                WaveNumber = 3;
                ObjectiveText = "Quebre a linha dos brutos";
                StatusText = "Um bruto chegou abrindo caminho no asfalto.";
                SpawnComposition([BruteEnemyScene, EnemyScene, EnemyScene, FastEnemyScene]);
                break;
            case Phase.MiniBoss:
                WaveNumber = 4;
                ObjectiveText = "Derrube o bruto da rua";
                StatusText = "O portao range. Tem algo grande vindo.";
                SpawnBoss(MiniBossScene);
                break;
            case Phase.MiniBossTwo:
                WaveNumber = 5;
                ObjectiveText = "Derrube o infectado da chuva";
                StatusText = "A chuva engrossou. Algo verde rasteja no meio da rua.";
                SpawnBoss(SecondMiniBossScene);
                break;
            case Phase.FinalBoss:
                WaveNumber = 6;
                ObjectiveText = "Sobreviva ao alfa da rua";
                StatusText = "A rua inteira parou. O alfa apareceu.";
                SpawnBoss(FinalBossScene);
                break;
            case Phase.Victory:
                CompleteRun();
                break;
        }
    }

    private void SpawnComposition(PackedScene?[] scenes)
    {
        PackedScene?[] composition = scenes.Length > 0 ? scenes : _emptyComposition;
        EnemiesRemaining = composition.Length;
        _phaseActive = true;

        for (int i = 0; i < composition.Length; i++)
        {
            SpawnEnemy(i, composition[i] ?? EnemyScene);
        }
    }

    private void SpawnEnemy(int index, PackedScene? scene)
    {
        if (scene is null || _spawnPoints is null || _spawnPoints.GetChildCount() == 0)
        {
            return;
        }

        Node2D spawnPoint = _spawnPoints.GetChild<Node2D>(index % _spawnPoints.GetChildCount());
        Node2D enemy = scene.Instantiate<Node2D>();
        enemy.GlobalPosition = spawnPoint.GlobalPosition + new Vector2(index * 14f, 0f);
        GetTree().CurrentScene?.AddChild(enemy);
    }

    private void SpawnBoss(PackedScene? bossScene)
    {
        if (bossScene is null || _spawnPoints is null || _spawnPoints.GetChildCount() == 0)
        {
            SpawnComposition([EnemyScene]);
            return;
        }

        Node2D spawnPoint = _spawnPoints.GetChild<Node2D>(1 % _spawnPoints.GetChildCount());
        Node2D boss = bossScene.Instantiate<Node2D>();
        boss.GlobalPosition = spawnPoint.GlobalPosition;
        GetTree().CurrentScene?.AddChild(boss);
        EnemiesRemaining = 1;
        _phaseActive = true;
    }

    private void OnPhaseCleared()
    {
        switch (_phase)
        {
            case Phase.EncounterOne:
                StatusText = "Entrada limpa. Avance ate o altar.";
                break;
            case Phase.EncounterTwo:
                StatusText = "A rua ficou quieta demais...";
                break;
            case Phase.EncounterThree:
                StatusText = "Os brutos cairam. O ar ficou pesado.";
                break;
            case Phase.MiniBoss:
                StatusText = "O bruto caiu. Trecho limpo.";
                break;
            case Phase.MiniBossTwo:
                StatusText = "O infectado da chuva desmanchou no asfalto.";
                break;
            case Phase.FinalBoss:
                StatusText = "O alfa caiu. A rua abriu caminho.";
                break;
        }
    }

    private void UnlockCheckpoint()
    {
        HasCheckpoint = true;
        _checkpointUnlocked = true;
        _resumePhase = Phase.EncounterTwo;
        SaveManager.Current.CheckpointUnlocked = true;
        SaveManager.Save();

        if (_player is not null)
        {
            _player.GlobalPosition = CheckpointPosition;
        }
    }

    private void CompleteRun()
    {
        _phase = Phase.Victory;
        _completed = true;
        _phaseActive = false;
        _resumePhase = Phase.EncounterOne;
        _checkpointUnlocked = false;
        SaveManager.Current.CheckpointUnlocked = false;
        SaveManager.Save();
        ObjectiveText = "Trecho limpo";
        StatusText = "Vitoria. A rua abriu caminho. Aperte R para jogar de novo.";
    }

    private void ReloadRun()
    {
        if (_gameOver && HasCheckpoint)
        {
            _resumePhase = Phase.EncounterTwo;
        }
        else
        {
            _resumePhase = Phase.EncounterOne;
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
            _resumePhase = Phase.EncounterOne;
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

        _player.GlobalPosition = _resumePhase >= Phase.EncounterTwo ? CheckpointPosition : StartPosition;
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
