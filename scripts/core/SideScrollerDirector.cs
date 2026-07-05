namespace SangueNoAsfalto.Core;

public partial class SideScrollerDirector : Node
{
    private const string MainMenuScenePath = "res://scenes/ui/MainMenu.tscn";

    private enum Phase
    {
        IntroWalk,
        EncounterOne,
        EncounterOneB,
        ApproachCheckpoint,
        Checkpoint,
        EncounterTwo,
        EncounterTwoB,
        MidBreather,
        EncounterThree,
        EncounterThreeB,
        StreetMiniBoss,
        RainBuildup,
        RainMiniBoss,
        AlphaBuildup,
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
    public NodePath WeatherControllerPath { get; set; } = "../WeatherController";

    [Export]
    public NodePath TimeOfDayControllerPath { get; set; } = "../TimeOfDayController";

    [Export]
    public float TimeBetweenEncounters { get; set; } = 2.8f;

    [Export]
    public Vector2 StartPosition { get; set; } = new(-760f, 405f);

    [Export]
    public Vector2 CheckpointPosition { get; set; } = new(-80f, 405f);

    public int WaveNumber { get; private set; }

    public int TotalWaves => 10;

    public int EnemiesRemaining { get; private set; }

    public string StageTitle { get; private set; } = "VILA ESPERANCA";

    public string StageTagline { get; private set; } = "A rua nao esquece. Ela so espera.";

    public string StatusText { get; private set; } = "A rua acordou errada";

    public string ObjectiveText { get; private set; } = "Atravesse a Vila Esperanca";

    public bool HasCheckpoint { get; private set; }

    public bool IsGameOver => _gameOver;

    public bool IsCompleted => _completed;

    public bool ShowDebugHud => SaveManager.Current.ShowDebugHud;

    public bool AlternateControls => SaveManager.Current.AlternateControls;

    private static Phase _resumePhase = Phase.IntroWalk;
    private static bool _checkpointUnlocked;
    private readonly PackedScene?[] _emptyComposition = [];
    private Node2D? _spawnPoints;
    private Health? _playerHealth;
    private SideScrollerPlayerController? _player;
    private WeatherController? _weather;
    private TimeOfDayController? _timeOfDay;
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
        _weather = GetNodeOrNull<WeatherController>(WeatherControllerPath);
        _timeOfDay = GetNodeOrNull<TimeOfDayController>(TimeOfDayControllerPath);
        FindPlayer();
        ApplyRespawnState();

        Phase startPhase = _checkpointUnlocked ? Phase.EncounterTwo : _resumePhase;
        if (startPhase == Phase.EncounterOne)
        {
            startPhase = Phase.IntroWalk;
        }

        StartPhase(startPhase);
    }

    public override void _Process(double delta)
    {
        if ((_gameOver || _completed) && Input.IsKeyPressed(Key.M))
        {
            _resumePhase = Phase.IntroWalk;
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
            case Phase.IntroWalk:
                StartPhase(Phase.EncounterOne);
                break;
            case Phase.EncounterOne:
                StartPhase(Phase.EncounterOneB);
                break;
            case Phase.EncounterOneB:
                StartPhase(Phase.ApproachCheckpoint);
                break;
            case Phase.ApproachCheckpoint:
                StartPhase(Phase.Checkpoint);
                break;
            case Phase.Checkpoint:
                UnlockCheckpoint();
                StartPhase(Phase.EncounterTwo);
                break;
            case Phase.EncounterTwo:
                StartPhase(Phase.EncounterTwoB);
                break;
            case Phase.EncounterTwoB:
                StartPhase(Phase.MidBreather);
                break;
            case Phase.MidBreather:
                StartPhase(Phase.EncounterThree);
                break;
            case Phase.EncounterThree:
                StartPhase(Phase.EncounterThreeB);
                break;
            case Phase.EncounterThreeB:
                StartPhase(Phase.StreetMiniBoss);
                break;
            case Phase.StreetMiniBoss:
                StartPhase(Phase.RainBuildup);
                break;
            case Phase.RainBuildup:
                StartPhase(Phase.RainMiniBoss);
                break;
            case Phase.RainMiniBoss:
                StartPhase(Phase.AlphaBuildup);
                break;
            case Phase.AlphaBuildup:
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
        ApplyActAtmosphere(phase);

        switch (phase)
        {
            case Phase.IntroWalk:
                WaveNumber = 0;
                ObjectiveText = "Entre na Vila Esperanca";
                StatusText = "O asfalto esta molhado. Avance pela rua.";
                BeginIntermission(38f);
                break;
            case Phase.EncounterOne:
                WaveNumber = 1;
                ObjectiveText = "Limpe a entrada da rua";
                StatusText = "Primeiro contato. Nao deixe fecharem a lane.";
                SpawnComposition([EnemyScene, EnemyScene, EnemyScene]);
                break;
            case Phase.EncounterOneB:
                WaveNumber = 2;
                ObjectiveText = "Segure a entrada";
                StatusText = "Reforcos chegando pelos fundos.";
                SpawnComposition([EnemyScene, FastEnemyScene, EnemyScene]);
                break;
            case Phase.ApproachCheckpoint:
                WaveNumber = 2;
                ObjectiveText = "Alcance o altar improvisado";
                StatusText = "A rua abriu. Va ate o altar.";
                BeginIntermission(28f);
                break;
            case Phase.Checkpoint:
                WaveNumber = 2;
                ObjectiveText = "Ative o checkpoint";
                StatusText = "Respira. O altar virou ponto seguro.";
                BeginIntermission(4f);
                break;
            case Phase.EncounterTwo:
                WaveNumber = 3;
                ObjectiveText = "Segure a rua contra os corredores";
                StatusText = "Corredores na chuva. Troque de lane rapido.";
                SpawnComposition([EnemyScene, FastEnemyScene, EnemyScene, InfectedEnemyScene]);
                break;
            case Phase.EncounterTwoB:
                WaveNumber = 4;
                ObjectiveText = "Quebre a segunda linha";
                StatusText = "Mais infectados entrando pelo meio da rua.";
                SpawnComposition([FastEnemyScene, FastEnemyScene, InfectedEnemyScene, EnemyScene, EnemyScene]);
                break;
            case Phase.MidBreather:
                WaveNumber = 4;
                ObjectiveText = "Recupere folego";
                StatusText = "Trecho quieto. Use os pickups se precisar.";
                BeginIntermission(32f);
                break;
            case Phase.EncounterThree:
                WaveNumber = 5;
                ObjectiveText = "Quebre a linha dos brutos";
                StatusText = "Brutos abrindo caminho no asfalto.";
                SpawnComposition([BruteEnemyScene, EnemyScene, EnemyScene, FastEnemyScene]);
                break;
            case Phase.EncounterThreeB:
                WaveNumber = 6;
                ObjectiveText = "Segure o corredor central";
                StatusText = "Linha mista. Cuidado com o bruto.";
                SpawnComposition([BruteEnemyScene, InfectedEnemyScene, EnemyScene, FastEnemyScene, EnemyScene]);
                break;
            case Phase.StreetMiniBoss:
                WaveNumber = 7;
                ObjectiveText = "Derrube o bruto da rua";
                StatusText = "O portao range. Algo grande vem.";
                SpawnBoss(MiniBossScene);
                break;
            case Phase.RainBuildup:
                WaveNumber = 7;
                ObjectiveText = "Atravesse a chuva";
                StatusText = "A tempestade engrossou. Prepare-se.";
                BeginIntermission(24f);
                break;
            case Phase.RainMiniBoss:
                WaveNumber = 8;
                ObjectiveText = "Derrube o infectado da chuva";
                StatusText = "Algo verde rasteja no meio da rua.";
                SpawnBoss(SecondMiniBossScene);
                break;
            case Phase.AlphaBuildup:
                WaveNumber = 8;
                ObjectiveText = "Chegue ao fim do trecho";
                StatusText = "A rua inteira parou. O cheiro de ferro veio.";
                BeginIntermission(26f);
                break;
            case Phase.FinalBoss:
                WaveNumber = 9;
                ObjectiveText = "Sobreviva ao alfa da rua";
                StatusText = "O alfa apareceu. Termine a fase.";
                SpawnBoss(FinalBossScene);
                break;
            case Phase.Victory:
                CompleteRun();
                break;
        }
    }

    private void BeginIntermission(float duration)
    {
        _phaseActive = false;
        _phaseTimer = duration;
    }

    private void ApplyActAtmosphere(Phase phase)
    {
        if (_weather is not null)
        {
            _weather.AutoCycle = false;
            _weather.SetState(phase switch
            {
                Phase.IntroWalk or Phase.EncounterOne or Phase.EncounterOneB or Phase.ApproachCheckpoint => WeatherController.WeatherState.Drizzle,
                Phase.EncounterTwo or Phase.EncounterTwoB or Phase.MidBreather => WeatherController.WeatherState.HeavyRain,
                Phase.EncounterThree or Phase.EncounterThreeB or Phase.StreetMiniBoss => WeatherController.WeatherState.HeavyRain,
                Phase.RainBuildup or Phase.RainMiniBoss => WeatherController.WeatherState.Thunderstorm,
                Phase.AlphaBuildup or Phase.FinalBoss => WeatherController.WeatherState.Thunderstorm,
                _ => WeatherController.WeatherState.Drizzle
            });
        }

        if (_timeOfDay is not null)
        {
            _timeOfDay.AutoCycle = false;
            _timeOfDay.SetState(phase switch
            {
                Phase.IntroWalk or Phase.EncounterOne or Phase.EncounterOneB => TimeOfDayController.TimeOfDayState.Sunset,
                Phase.ApproachCheckpoint or Phase.Checkpoint or Phase.EncounterTwo or Phase.EncounterTwoB => TimeOfDayController.TimeOfDayState.Night,
                Phase.MidBreather or Phase.EncounterThree or Phase.EncounterThreeB => TimeOfDayController.TimeOfDayState.Night,
                Phase.StreetMiniBoss or Phase.RainBuildup or Phase.RainMiniBoss => TimeOfDayController.TimeOfDayState.Night,
                Phase.AlphaBuildup or Phase.FinalBoss => TimeOfDayController.TimeOfDayState.Night,
                _ => TimeOfDayController.TimeOfDayState.Sunset
            });
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
                StatusText = "Entrada segura por agora.";
                break;
            case Phase.EncounterOneB:
                StatusText = "Entrada limpa. Siga em frente.";
                break;
            case Phase.EncounterTwo:
                StatusText = "Primeira linha quebrada.";
                break;
            case Phase.EncounterTwoB:
                StatusText = "A rua ficou quieta demais...";
                break;
            case Phase.EncounterThree:
                StatusText = "Os brutos cairam.";
                break;
            case Phase.EncounterThreeB:
                StatusText = "O corredor abriu. Algo grande vem.";
                break;
            case Phase.StreetMiniBoss:
                StatusText = "O bruto caiu.";
                break;
            case Phase.RainMiniBoss:
                StatusText = "O infectado desmanchou no asfalto.";
                break;
            case Phase.FinalBoss:
                StatusText = "O alfa caiu. A fase terminou.";
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
        WaveNumber = 10;
        _resumePhase = Phase.IntroWalk;
        _checkpointUnlocked = false;
        SaveManager.Current.CheckpointUnlocked = false;
        SaveManager.Save();
        ObjectiveText = "Fase concluida";
        StatusText = "Vila Esperanca: trecho limpo. Aperte R para jogar de novo.";
    }

    private void ReloadRun()
    {
        if (_gameOver && HasCheckpoint)
        {
            _resumePhase = Phase.EncounterTwo;
        }
        else if (_completed)
        {
            _resumePhase = Phase.IntroWalk;
        }
        else
        {
            _resumePhase = Phase.IntroWalk;
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
            _resumePhase = Phase.IntroWalk;
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
