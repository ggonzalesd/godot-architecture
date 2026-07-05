using System.Collections.Generic;
using Godot;
using Microsoft.EntityFrameworkCore;
using StructProject.Core.Entities.Enemies;
using StructProject.Core.Entities.Pickups;
using StructProject.Core.Entities.Skills;
using StructProject.Core.Logic.Enemies;
using StructProject.Core.Logic.Pickups;
using StructProject.Core.Logic.Player;
using StructProject.Core.Logic.Skills;
using StructProject.Core.Logic.Spawn;
using StructProject.Core.Logic.Waves;
using StructProject.Core.Shared.Service;
using StructProject.Database.Context;
using StructProject.Database.Persistence;
using StructProject.GodotPresentation.Scripts.Adapters;
using StructProject.GodotPresentation.Scripts.Adapters.Spawn;
using StructProject.GodotPresentation.Scripts.Data;
using StructProject.GodotPresentation.Scripts.Entities;
using StructProject.GodotPresentation.Scripts.Persistence;
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
  public PackedScene PickupScene { get; private set; } = null!;

  [Export]
  public Godot.Collections.Array<EnemyKindConfig> EnemyKinds { get; private set; } = [];

  [Export]
  public Godot.Collections.Array<PickupConfig> PickupKinds { get; private set; } = [];

  [Export]
  public Godot.Collections.Array<SkillConfig> Skills { get; private set; } = [];

  [Export]
  public Resource? StartingWaveSequence { get; private set; }

  public ILogger Logger { get; private set; } = null!;
  public IInputActions Inputs { get; private set; } = null!;
  public IScoreService Score { get; } = new ScoreService();
  public PersistenceService Persistence { get; private set; } = null!;

  public IBulletSpawn BulletSpawn { get; private set; } = null!;
  public IBulletSpawn EnemyBulletSpawn { get; private set; } = null!;

  public BodyLogic PlayerBodyLogic { get; private set; } = null!;
  public ShootingLogic Shooting { get; private set; } = null!;
  public PlayerStateLogic PlayerLogic { get; private set; } = null!;
  public PowerUpLogic PowerUpLogic { get; } = new();
  public InventoryLogic InventoryLogic { get; } = new();
  public DropLogic DropRoller { get; } = new();

  public WaveControllerLogic WaveController { get; private set; } = null!;
  public static EnemyAiLogic EnemyAi { get; } = new();
  public EnemyRegistryLogic EnemyRegistry { get; } = new();

  public EnemyKindRegistry EnemyKindRegistry { get; private set; } = null!;
  public PickupRegistry PickupRegistry { get; private set; } = null!;
  public SkillRegistry SkillRegistry { get; private set; } = null!;
  public SkillsLogic SkillsLogic { get; } = new();

  public PlayerState Player { get; private set; } = null!;
  public PowerUpState PowerUp { get; private set; } = PowerUpState.Empty;
  public InventoryState Inventory { get; private set; } = new InventoryState(0);
  public PlayerSkillsState SkillsState { get; private set; } = new(new Dictionary<SkillKind, SkillSlotState>(), 1f);

  public StructProject.Core.Entities.Player.IBinding? PlayerBinding { get; private set; }

  public IDbContextFactory<GameDbContext> DbContextFactory { get; private set; } = null!;

  public event System.Action<int>? OnWaveIndexChanged;
  public event System.Action<int>? OnPlayerHealthChanged;
  public event System.Action<int>? OnPlayerScoreChanged;
  public event System.Action<int>? OnCoinsChanged;
  public event System.Action? OnPlayerDied;
  public event System.Action<int, int, int>? OnEnemyKilledEvent;
  public event System.Action<PlayerSkillsState>? OnSkillsStateChanged;

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

    PickupRegistry = new PickupRegistry();
    PickupRegistry.RegisterRange(PickupKinds);

    SkillRegistry = new SkillRegistry();
    SkillRegistry.RegisterRange(Skills);
    InitializeSkills();

    WaveController = new WaveControllerLogic(Logger);
    WaveController.OnWaveStarted += (idx, _) => OnWaveIndexChanged?.Invoke(idx + 1);
    WaveController.OnEnemySpawn += HandleEnemySpawn;

    var dbPath = ProjectSettings.GlobalizePath("user://game.db");
    DbContextFactory = new GameDbContextFactory(dbPath);
    try
    {
      using (var ctx = DbContextFactory.CreateDbContext())
      {
        ctx.Database.Migrate();
      }
    }
    catch (System.Exception ex)
    {
      Logger.Error("Migration failed, falling back to EnsureCreated", ex.Message);
      using (var ctx = DbContextFactory.CreateDbContext())
      {
        ctx.Database.EnsureCreated();
      }
    }
    Logger.Log($"Database initialized at {dbPath}");

    Persistence = new PersistenceService((StructProject.Database.Persistence.GameDbContextFactory)DbContextFactory);

    WaveController.OnWaveCompleted += idx =>
    {
      _ = AutoSaveAsync();
    };

    OnPlayerDied += () =>
    {
      _ = AutoSaveAsync(submit: true);
    };

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
    PowerUp = PowerUpLogic.Update(PowerUp, delta);

    if (Inputs.DashPressed) TryTriggerSkill(SkillKind.Dash);
    if (Inputs.BombPressed) TryTriggerSkill(SkillKind.Bomb);
    if (Inputs.SlowPressed) TryTriggerSkill(SkillKind.SlowMotion);

    var prev = SkillsState;
    SkillsState = SkillsLogic.Update(SkillsState, delta, SkillRegistry.ToSnapshotDictionary());
    if (!ReferenceEquals(prev, SkillsState))
    {
      Engine.TimeScale = SkillsState.TimeScale;
      OnSkillsStateChanged?.Invoke(SkillsState);
    }
  }

  private void InitializeSkills()
  {
    var slots = new Dictionary<SkillKind, SkillSlotState>();
    foreach (var kind in System.Enum.GetValues<SkillKind>())
    {
      slots[kind] = new SkillSlotState(kind, 0f, 0f, false);
    }
    SkillsState = new PlayerSkillsState(slots, 1f);
  }

  public void TryTriggerSkill(SkillKind kind)
  {
    SkillsState = SkillsLogic.Trigger(SkillsState, kind, SkillRegistry.ToSnapshotDictionary());
    OnSkillsStateChanged?.Invoke(SkillsState);
  }

  public void ConsumeSkillTrigger(SkillKind kind)
  {
    SkillsState = SkillsLogic.ConsumeTrigger(SkillsState, kind);
    OnSkillsStateChanged?.Invoke(SkillsState);
  }

  public void BindPlayer(StructProject.Core.Entities.Player.IBinding binding)
  {
    PlayerBinding = binding;
  }

  public void TryDamagePlayer(int amount)
  {
    if (PowerUp.HasShield && amount > 0)
    {
      PowerUp = PowerUp with { HasShield = false, RemainingShieldTime = 0f };
      amount = 0;
    }
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

  public void OnEnemyKilled(int instanceId, int scoreValue, int coinReward)
  {
    var spawn = EnemyRegistry.Get(instanceId);
    if (instanceId >= 0)
    {
      EnemyRegistry.Unregister(instanceId);
    }
    Score.AddScore(scoreValue);
    Inventory = InventoryLogic.AddCoins(Inventory, coinReward);
    OnCoinsChanged?.Invoke(Inventory.Coins);
    Logger.Log("Enemy killed", instanceId, "score", scoreValue);
    OnEnemyKilledEvent?.Invoke(instanceId, scoreValue, coinReward);

    if (PickupScene != null && spawn is { } s)
    {
      var cfg = EnemyKindRegistry.Get(s.Kind);
      var table = cfg?.DropTable?.ToSnapshot();
      var drop = DropRoller.Roll((int)s.X, (int)s.Y, coinReward, table);
      SpawnPickup(drop);
    }
  }

  private void SpawnPickup(DropEvent evt)
  {
    if (PickupScene == null) return;
    var pickup = PickupScene.Instantiate<Pickup>();
    pickup.Position = new Vector2(evt.X, evt.Y);
    pickup.Initialize(evt.Kind, evt.Duration, evt.Amount);
    GetTree().CurrentScene.AddChild(pickup);
  }

  public void CollectPickup(Pickup pickup)
  {
    var kind = pickup.Kind;
    var amount = pickup.Amount;
    var duration = pickup.Duration;

    switch (kind)
    {
      case PickupKind.Coin:
        Inventory = InventoryLogic.AddCoins(Inventory, amount);
        OnCoinsChanged?.Invoke(Inventory.Coins);
        break;
      case PickupKind.Health:
        HealPlayer(amount);
        break;
      case PickupKind.Shield:
      case PickupKind.DamageUp:
      case PickupKind.FireRateUp:
      case PickupKind.SpeedUp:
      case PickupKind.Magnet:
        PowerUp = PowerUpLogic.Apply(PowerUp, kind, amount, duration);
        break;
    }

    Logger.Log("Pickup collected", kind);
  }

  private async System.Threading.Tasks.Task AutoSaveAsync(bool submit = false)
  {
    try
    {
      if (submit)
      {
        await Persistence.SubmitScoreAsync("Player", Score.CurrentScore, WaveController.CurrentWaveIndex + 1);
      }
      await Persistence.AutoSaveRunAsync(Score.CurrentScore, WaveController.CurrentWaveIndex + 1, Inventory.Coins);
    }
    catch (System.Exception ex)
    {
      Logger.Error("AutoSaveAsync failed", ex.Message);
    }
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
