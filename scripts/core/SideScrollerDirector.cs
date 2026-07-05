using Godot;
using SangueNoAsfalto.Core;
using SangueNoAsfalto.Player;

namespace SangueNoAsfalto.Core;

public partial class SideScrollerDirector : Node
{
    private enum Phase
    {
        EncounterOne,
        Checkpoint,
        EncounterTwo,
        MiniBoss,
        Victory
    }

    [Export]
    public PackedScene? EnemyScene { get; set; }

    [Export]
    public PackedScene? MiniBossScene { get; set; }

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

    public int TotalWaves => 3;

    public int EnemiesRemaining { get; private set; }

    public string StatusText { get; private set; } = "A rua acordou errada";

    public string ObjectiveText { get; private set; } = "Atravesse a Vila Esperanca";

    public bool HasCheckpoint { get; private set; }

    public bool IsGameOver => _gameOver;

    public bool IsCompleted => _completed;

    private static Phase _resumePhase = Phase.EncounterOne;
    private static bool _checkpointUnlocked;
    private readonly int[] _encounterSizes = { 3, 4 };
    private Node2D? _spawnPoints;
    private Health? _playerHealth;
    private SideScrollerPlayerController? _player;
    private Phase _phase;
    private float _phaseTimer;
    private bool _phaseActive;
    private bool _completed;
    private bool _gameOver;

    public override void _Ready()
    {
        AddToGroup("game_director");
        AddToGroup("side_director");
        _spawnPoints = GetNodeOrNull<Node2D>(SpawnPointsPath);
        FindPlayer();
        ApplyRespawnState();
        StartPhase(_resumePhase);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("restart") || Input.IsKeyPressed(Key.R))
        {
            ReloadRun();
            return;
        }

        if (_gameOver || _completed)
        {
            return;
        }

        FindPlayer();
        if (_playerHealth is not null && _playerHealth.CurrentHealth <= 0)
        {
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
                StartPhase(Phase.MiniBoss);
                break;
            case Phase.MiniBoss:
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
                SpawnEncounter(_encounterSizes[0]);
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
                ObjectiveText = "Segure a rua ate o monstro aparecer";
                StatusText = "Reforcos na chuva. Nao deixa fecharem a lane.";
                SpawnEncounter(_encounterSizes[1]);
                break;
            case Phase.MiniBoss:
                WaveNumber = 3;
                ObjectiveText = "Derrube o bruto da rua";
                StatusText = "O portao range. Tem algo grande vindo.";
                SpawnMiniBoss();
                break;
            case Phase.Victory:
                CompleteRun();
                break;
        }
    }

    private void SpawnEncounter(int enemyCount)
    {
        EnemiesRemaining = enemyCount;
        _phaseActive = true;
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy(i);
        }
    }

    private void SpawnEnemy(int index)
    {
        if (EnemyScene is null || _spawnPoints is null || _spawnPoints.GetChildCount() == 0)
        {
            return;
        }

        Node2D spawnPoint = _spawnPoints.GetChild<Node2D>(index % _spawnPoints.GetChildCount());
        Node2D enemy = EnemyScene.Instantiate<Node2D>();
        enemy.GlobalPosition = spawnPoint.GlobalPosition + new Vector2(index * 14f, 0f);
        GetTree().CurrentScene?.AddChild(enemy);
    }

    private void SpawnMiniBoss()
    {
        if (MiniBossScene is null || _spawnPoints is null || _spawnPoints.GetChildCount() == 0)
        {
            SpawnEncounter(1);
            return;
        }

        Node2D spawnPoint = _spawnPoints.GetChild<Node2D>(1 % _spawnPoints.GetChildCount());
        Node2D miniBoss = MiniBossScene.Instantiate<Node2D>();
        miniBoss.GlobalPosition = spawnPoint.GlobalPosition;
        GetTree().CurrentScene?.AddChild(miniBoss);
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
            case Phase.MiniBoss:
                StatusText = "O bruto caiu. Trecho limpo.";
                break;
        }
    }

    private void UnlockCheckpoint()
    {
        HasCheckpoint = true;
        _checkpointUnlocked = true;
        _resumePhase = Phase.EncounterTwo;

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
        }

        GetTree().ReloadCurrentScene();
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
