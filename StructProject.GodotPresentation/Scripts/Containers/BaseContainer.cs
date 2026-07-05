using Godot;
using Microsoft.EntityFrameworkCore;
using StructProject.Core.Entities.Enemies;
using StructProject.Core.Logic.Enemies;
using StructProject.Core.Logic.Player;
using StructProject.Core.Logic.Spawn;
using StructProject.Core.Logic.Waves;
using StructProject.Core.Shared.Service;
using StructProject.Database.Context;
using StructProject.Database.Persistence;
using StructProject.GodotPresentation.Scripts.Adapters;
using StructProject.GodotPresentation.Scripts.Adapters.Spawn;
using StructProject.GodotPresentation.Scripts.Data;
using StructProject.GodotPresentation.Scripts.Registries;
using EnemyNode = StructProject.GodotPresentation.Scripts.Entities.Enemy;

namespace StructProject.GodotPresentation.Scripts.Containers;

[GlobalClass]
public partial class BaseContainer : Node
{
  [Export]
  public PackedScene BulletScene { get; private set; } = null!;

  [Export]
  public PackedScene EnemyScene { get; private set; } = null!;

  [Export]
  public Godot.Collections.Array<EnemyKindConfig> EnemyKinds { get; private set; } = [];

  [Export]
  public Resource? StartingWaveSequence { get; private set; }

  public ILogger Logger { get; private set; } = null!;
  public IInputActions Inputs { get; private set; } = null!;
  public IScoreService Score { get; } = new ScoreService();

  public IBulletSpawn BulletSpawn { get; private set; } = null!;
  public IBulletSpawn EnemyBulletSpawn { get; private set; } = null!;

  public BodyLogic PlayerBodyLogic { get; private set; } = null!;
  public ShootingLogic Shooting { get; private set; } = null!;
  public PlayerStateLogic PlayerLogic { get; private set; } = null!;
  public WaveControllerLogic WaveController { get; private set; } = null!;
  public static EnemyAiLogic EnemyAi { get; } = new();
  public EnemyRegistryLogic EnemyRegistry { get; } = new();
  public EnemyKindRegistry EnemyKindRegistry { get; private set; } = null!;

  public PlayerState Player { get; private set; } = null!;
  public StructProject.Core.Entities.Player.IBinding? PlayerBinding { get; private set; }

  public IDbContextFactory<GameDbContext> DbContextFactory { get; private set; } = null!;

  public event System.Action<int>? OnWaveIndexChanged;
  public event System.Action<int>? OnPlayerHealthChanged;
  public event System.Action<int>? OnPlayerScoreChanged;
  public event System.Action? OnPlayerDied;
  public event System.Action<int, int, int>? OnEnemyKilledEvent;

  private static BaseContainer? _instance;
  public static BaseContainer Instance => _instance ?? throw new System.InvalidOperationException("BaseContainer not initialized.");

  public override void _Ready()
  {
    _instance = this;

    Logger = new GodotLoggerAdapter();
    Inputs = new GodotInputActionsAdapter(GetViewport());
    PlayerLogic = new PlayerStateLogic();

    var origin = GetTree().CurrentScene;
    BulletSpawn = new GodotSpawnBullet(BulletScene, origin);
    EnemyBulletSpawn = new GodotSpawnBullet(BulletScene, origin);

    PlayerBodyLogic = new BodyLogic(Inputs);
    Shooting = new ShootingLogic(Inputs, Logger, BulletSpawn);

    Player = PlayerLogic.Spawn(maxHealth: 100);

    EnemyKindRegistry = new EnemyKindRegistry();
    EnemyKindRegistry.RegisterRange(EnemyKinds);

    WaveController = new WaveControllerLogic(Logger);
    WaveController.OnWaveStarted += (idx, _) => OnWaveIndexChanged?.Invoke(idx + 1);
    WaveController.OnEnemySpawn += HandleEnemySpawn;

    var dbPath = ProjectSettings.GlobalizePath("user://game.db");
    DbContextFactory = new GameDbContextFactory(dbPath);
    using (var ctx = DbContextFactory.CreateDbContext())
    {
      ctx.Database.Migrate();
    }
    Logger.Log($"Database initialized at {dbPath}");

    Score.OnScoreChanged += s => OnPlayerScoreChanged?.Invoke(s);

    if (StartingWaveSequence is WaveSequence waveSequence)
    {
      var snapshot = Mappers.WaveSequenceMapper.ToSnapshot(waveSequence);
      WaveController.Start(snapshot);
    }
  }

  public override void _Process(double delta)
  {
    WaveController?.Update(delta);
    Player = PlayerLogic.Update(Player, delta);
  }

  public void BindPlayer(StructProject.Core.Entities.Player.IBinding binding)
  {
    PlayerBinding = binding;
  }

  public void TryDamagePlayer(int amount)
  {
    var next = PlayerLogic.TakeDamage(Player, amount);
    if (next.CurrentHealth != Player.CurrentHealth)
    {
      Player = next;
      OnPlayerHealthChanged?.Invoke(Player.CurrentHealth);
      if (Player.IsDead)
      {
        OnPlayerDied?.Invoke();
      }
    }
  }

  public void HealPlayer(int amount)
  {
    var next = PlayerLogic.Heal(Player, amount);
    if (next.CurrentHealth != Player.CurrentHealth)
    {
      Player = next;
      OnPlayerHealthChanged?.Invoke(Player.CurrentHealth);
    }
  }

  public void UpgradePlayerMaxHealth(int amount)
  {
    Player = PlayerLogic.UpgradeMaxHealth(Player, amount);
    OnPlayerHealthChanged?.Invoke(Player.CurrentHealth);
  }

  public void OnEnemyKilled(int instanceId, int scoreValue, int coinDrop)
  {
    if (instanceId >= 0)
    {
      EnemyRegistry.Unregister(instanceId);
    }
    Score.AddScore(scoreValue);
    Logger.Log("Enemy killed", instanceId, "score", scoreValue);
    OnEnemyKilledEvent?.Invoke(instanceId, scoreValue, coinDrop);
  }

  private void HandleEnemySpawn(EnemySpawnEvent spawnEvent)
  {
    if (EnemyScene == null) return;
    var id = EnemyRegistry.Register(spawnEvent);
    var enemy = EnemyScene.Instantiate<EnemyNode>();
    enemy.GlobalPosition = new Vector2(spawnEvent.X, spawnEvent.Y);
    var instance = new EnemyInstance(
      spawnEvent,
      CurrentHealth: spawnEvent.Kind switch
      {
        EnemyKind.Tank => spawnEvent.Health * 4,
        _ => spawnEvent.Health
      },
      ShootCooldownRemaining: 1f
    );
    enemy.Initialize(id, instance);
    if (EnemyKindRegistry.Get(spawnEvent.Kind) is { } cfg)
    {
      enemy.Modulate = cfg.TintColor;
      var s = enemy.GetNodeOrNull<Sprite2D>("Visual");
      if (s != null)
      {
        s.Scale = cfg.Scale;
      }
    }
    GetTree().CurrentScene.AddChild(enemy);
  }
}
