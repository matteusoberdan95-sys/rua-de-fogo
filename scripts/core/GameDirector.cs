namespace SangueNoAsfalto.Core;

public partial class GameDirector : Node
{
    [Export]
    public PackedScene? EnemyScene { get; set; }

    [Export]
    public NodePath SpawnPointsPath { get; set; } = "../SpawnPoints";

    [Export]
    public float TimeBetweenWaves { get; set; } = 2f;

    public int WaveNumber { get; private set; }

    public int TotalWaves => _waveSizes.Length;

    public int EnemiesRemaining { get; private set; }

    public string StatusText { get; private set; } = "Sobreviva";

    private readonly int[] _waveSizes = { 2, 3, 5 };
    private Node2D? _spawnPoints;
    private Health? _playerHealth;
    private int _currentWaveIndex = -1;
    private float _nextWaveTimer;
    private bool _waveActive;
    private bool _completed;
    private bool _gameOver;

    public override void _Ready()
    {
        AddToGroup("game_director");
        _spawnPoints = GetNodeOrNull<Node2D>(SpawnPointsPath);
        FindPlayerHealth();
        StartNextWave();
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.R))
        {
            GetTree().ReloadCurrentScene();
            return;
        }

        if (_gameOver || _completed)
        {
            return;
        }

        FindPlayerHealth();
        if (_playerHealth is not null && _playerHealth.CurrentHealth <= 0)
        {
            _gameOver = true;
            _waveActive = false;
            StatusText = "Morreu. Aperte R para tentar de novo.";
            return;
        }

        EnemiesRemaining = CountLivingEnemies();

        if (_waveActive && EnemiesRemaining == 0)
        {
            _waveActive = false;
            _nextWaveTimer = TimeBetweenWaves;
        }

        if (!_waveActive)
        {
            TickWaveBreak((float)delta);
        }
    }

    private void TickWaveBreak(float dt)
    {
        if (_currentWaveIndex >= _waveSizes.Length - 1)
        {
            _completed = true;
            StatusText = "Vitoria. Aperte R para jogar de novo.";
            return;
        }

        _nextWaveTimer -= dt;
        StatusText = $"Proxima onda em {Mathf.CeilToInt(_nextWaveTimer)}";

        if (_nextWaveTimer <= 0f)
        {
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        _currentWaveIndex++;
        WaveNumber = _currentWaveIndex + 1;

        int enemyCount = _waveSizes[_currentWaveIndex];
        EnemiesRemaining = enemyCount;
        StatusText = $"Onda {WaveNumber}/{TotalWaves}";
        _waveActive = true;

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
        enemy.GlobalPosition = spawnPoint.GlobalPosition;
        GetTree().CurrentScene?.AddChild(enemy);
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

    private void FindPlayerHealth()
    {
        if (_playerHealth is not null)
        {
            return;
        }

        if (GetTree().GetFirstNodeInGroup("player") is PlayerController player)
        {
            _playerHealth = player.GetNodeOrNull<Health>("Health");
        }
    }
}
