using StructProject.Core.Entities.Enemies;
using StructProject.Core.Entities.Waves;
using StructProject.Core.Shared.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.Core.Logic.Waves;

public class WaveControllerLogic(ILogger logger)
{
  public event Action<int, WaveSnapshot>? OnWaveStarted;
  public event Action<int>? OnWaveCompleted;
  public event Action<EnemySpawnEvent>? OnEnemySpawn;
  public event Action? OnSequenceCompleted;

  private WaveSequenceSnapshot? _sequence;
  private int _currentWaveIndex = -1;
  private float _waveTime;
  private float _preWaveCooldown;
  private bool _betweenWaves;
  private int _loopsCompleted;
  private readonly List<ActiveSpawn> _activeSpawns = [];

  private class ActiveSpawn(WaveSpawn spawn)
  {
    public WaveSpawn Spawn { get; } = spawn;
    public float Elapsed { get; set; }
    public int SpawnedSoFar { get; set; }
  }

  public bool IsRunning => _sequence != null;
  public int CurrentWaveIndex => _currentWaveIndex;
  public int LoopsCompleted => _loopsCompleted;

  public void Start(WaveSequenceSnapshot sequence)
  {
    _sequence = sequence;
    _currentWaveIndex = -1;
    _waveTime = 0f;
    _betweenWaves = true;
    _preWaveCooldown = 1f;
    _activeSpawns.Clear();
    _loopsCompleted = 0;
    logger.Log("WaveController started sequence", sequence.DisplayName);
    AdvanceToNextWave();
  }

  public void Stop()
  {
    _sequence = null;
    _activeSpawns.Clear();
    _currentWaveIndex = -1;
    _betweenWaves = false;
  }

  public void Update(double delta)
  {
    if (_sequence == null) return;

    if (_betweenWaves)
    {
      _preWaveCooldown -= (float)delta;
      if (_preWaveCooldown <= 0f)
      {
        AdvanceToNextWave();
      }
      return;
    }

    _waveTime += (float)delta;
    var wave = _sequence.Waves[_currentWaveIndex];

    foreach (var spawn in _activeSpawns)
    {
      spawn.Elapsed += (float)delta;
      if (spawn.Elapsed >= spawn.Spawn.Delay)
      {
        var totalInterval = spawn.Spawn.Interval;
        var timeSinceReady = spawn.Elapsed - spawn.Spawn.Delay;
        var shouldHaveSpawned = (int)Math.Floor(timeSinceReady / totalInterval) + 1;
        while (spawn.SpawnedSoFar < spawn.Spawn.Count && spawn.SpawnedSoFar < shouldHaveSpawned)
        {
          Spawn(spawn.Spawn, wave.DifficultyMultiplier);
          spawn.SpawnedSoFar++;
        }
      }
    }

    if (_waveTime >= wave.Duration && AllSpawnsCompleted(wave))
    {
      CompleteCurrentWave();
    }
  }

  private void AdvanceToNextWave()
  {
    if (_sequence == null) return;

    if (_currentWaveIndex >= 0)
    {
      CompleteCurrentWave();
      return;
    }

    var nextIndex = _currentWaveIndex + 1;
    if (nextIndex >= _sequence.Waves.Count)
    {
      if (_sequence.LoopOnFinish)
      {
        _loopsCompleted++;
        nextIndex = 0;
      }
      else
      {
        OnSequenceCompleted?.Invoke();
        Stop();
        return;
      }
    }

    _currentWaveIndex = nextIndex;
    _waveTime = 0f;
    _betweenWaves = false;

    var wave = _sequence.Waves[nextIndex];
    _activeSpawns.Clear();
    foreach (var spawn in wave.Spawns)
    {
      _activeSpawns.Add(new ActiveSpawn(spawn));
    }

    logger.Log("Wave started", nextIndex + 1, wave.DisplayName);
    OnWaveStarted?.Invoke(nextIndex, wave);
  }

  private void CompleteCurrentWave()
  {
    if (_sequence == null || _currentWaveIndex < 0) return;

    var finishedWaveIndex = _currentWaveIndex;
    var nextWaveIndex = _currentWaveIndex + 1;

    logger.Log("Wave completed", finishedWaveIndex + 1);
    OnWaveCompleted?.Invoke(finishedWaveIndex);

    if (nextWaveIndex >= _sequence.Waves.Count)
    {
      if (_sequence.LoopOnFinish)
      {
        _loopsCompleted++;
        _currentWaveIndex = -1;
        _betweenWaves = true;
        _preWaveCooldown = _sequence.Waves[finishedWaveIndex].TimeBeforeNext;
        return;
      }

      OnSequenceCompleted?.Invoke();
      Stop();
      return;
    }

    _currentWaveIndex = -1;
    _betweenWaves = true;
    _preWaveCooldown = _sequence.Waves[finishedWaveIndex].TimeBeforeNext;
  }

  private bool AllSpawnsCompleted(WaveSnapshot wave)
  {
    return _activeSpawns.All(s => s.SpawnedSoFar >= s.Spawn.Count);
  }

  private void Spawn(WaveSpawn spawn, float difficultyMultiplier)
  {
    var enemy = spawn.Enemy;
    var hp = Math.Max(1, (int)(enemy.MaxHealth * difficultyMultiplier * spawn.HealthMultiplier));
    var x = spawn.Direction >= 0 ? 1200f : -1200f;
    var y = spawn.LaneY;

    OnEnemySpawn?.Invoke(new EnemySpawnEvent(
      enemy.Kind,
      x,
      y,
      hp,
      enemy.ContactDamage,
      enemy.ScoreValue,
      enemy.CoinDrop,
      spawn.Direction,
      enemy.Flies,
      enemy.MoveSpeed * difficultyMultiplier,
      enemy.ShootCooldown,
      enemy.ShootSpeed,
      enemy.ShootRange,
      enemy.ShootDamage
    ));
  }
}
