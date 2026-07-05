using System;
using Godot;
using PlayerBody = StructProject.Core.Entities.Player.Body;
using PlayerShooter = StructProject.Core.Entities.Player.Shooter;
using PlayerBinding = StructProject.Core.Entities.Player.IBinding;
using JumpStateT = StructProject.Core.Entities.Player.JumpState;
using SkillKindT = StructProject.Core.Entities.Skills.SkillKind;
using StructProject.Core.Shared.Models;
using StructProject.GodotPresentation.Scripts.Adapters.Players;
using StructProject.GodotPresentation.Scripts.Containers;
using PickupEntity = StructProject.GodotPresentation.Scripts.Entities.Pickup;
using EnemyEntity = StructProject.GodotPresentation.Scripts.Entities.Enemy;

namespace StructProject.GodotPresentation.Scripts.Entities;

public partial class Player : CharacterBody2D
{

  [ExportGroup("Inside Nodes")]
  [Export]
  private Node2D CannotPivot { get; set; } = null!;
  [Export]
  private Node2D SpawnPoint { get; set; } = null!;
  [Export]
  private Area2D HurtArea { get; set; } = null!;
  [Export]
  private Area2D PickupArea { get; set; } = null!;

  private PlayerBody PlayerBody = null!;
  private PlayerShooter PlayerShooter = null!;
  private PlayerBinding PlayerBinding = null!;
  private JumpStateT Jump = null!;

  private float _dashTimeRemaining;
  private float _bombFlashRemaining;
  private Vector2 _dashDir = Vector2.Right;
  private float _damageFlashTimer;

  [ExportGroup("Player Settings")]
  [Export]
  private float BulletSpeed { get; set; } = 900f;
  [Export]
  private float BulletRatio { get; set; } = 8f;

  [ExportGroup("Skill Tuning")]
  [Export]
  private float DashSpeed { get; set; } = 720f;
  [Export]
  private float DashDuration { get; set; } = 0.2f;
  [Export]
  private int BombDamage { get; set; } = 80;
  [Export]
  private float BombRadius { get; set; } = 240f;

  public override void _Ready()
  {
    CallDeferred(MethodName.InitializePlayer);
  }

  private void InitializePlayer()
  {
    PlayerBinding = new GodotPlayerBinding(this, SpawnPoint);

    PlayerBody = new PlayerBody(
      Position: new Vec2(GlobalPosition.X, GlobalPosition.Y),
      Velocity: new Vec2(Velocity.X, Velocity.Y)
    );

    PlayerShooter = new PlayerShooter(
      Speed: BulletSpeed,
      Ratio: BulletRatio,
      Aim: Vec2.Zero
    );

    Jump = new JumpStateT();

    PickupArea.AreaEntered += OnPickupAreaEntered;

    BaseContainer.Instance.BindPlayer(PlayerBinding);
  }

  public override void _PhysicsProcess(double delta)
  {
    if (PlayerBinding == null) return;

    var dt = (float)delta;
    var container = BaseContainer.Instance;

    var currentVelocity = Velocity;

    PlayerBody = PlayerBody with
    {
      Position = new Vec2(GlobalPosition.X, GlobalPosition.Y),
      Velocity = new Vec2(currentVelocity.X, currentVelocity.Y)
    };

    var powerUp = container.PowerUp;
    var fireRateMult = powerUp.ActiveFireRateMultiplier;
    var speedMult = powerUp.ActiveSpeedMultiplier;

    PlayerShooter = PlayerShooter with
    {
      Speed = BulletSpeed,
      Ratio = BulletRatio * fireRateMult
    };

    container.PlayerBodyLogic.Update(
      dt * container.SkillsState.TimeScale,
      PlayerBody,
      PlayerBinding,
      Jump
    );

    var dashDirection = Vector2.Zero;
    if (_dashTimeRemaining > 0f)
    {
      _dashTimeRemaining -= dt;
      dashDirection = _dashDir * DashSpeed;
      Velocity = new Vector2(dashDirection.X, dashDirection.Y);
    }
    else if (container.SkillsState.Slots.TryGetValue(SkillKindT.Dash, out var dashSlot)
             && dashSlot.TriggeredThisFrame)
    {
      var aimVec = container.PlayerBinding.GetPosition() - new Vec2(GlobalPosition.X, GlobalPosition.Y);
      var len = MathF.Sqrt(aimVec.X * aimVec.X + aimVec.Y * aimVec.Y);
      _dashDir = len > 0.001f ? new Vector2(aimVec.X / len, aimVec.Y / len) : Vector2.Right;
      _dashTimeRemaining = DashDuration;
      container.ConsumeSkillTrigger(SkillKindT.Dash);
    }

    if (container.SkillsState.Slots.TryGetValue(SkillKindT.Bomb, out var bombSlot) && bombSlot.TriggeredThisFrame)
    {
      ApplyBomb();
      container.ConsumeSkillTrigger(SkillKindT.Bomb);
    }

    if (_dashTimeRemaining <= 0f)
    {
      var v = Velocity;
      v = new Vector2(v.X * speedMult, v.Y);
      Velocity = v;
    }

    MoveAndSlide();

    var aim = container.Shooting.Update(
      dt,
      PlayerBody,
      PlayerShooter,
      PlayerBinding
    );

    CannotPivot.Rotation = Mathf.Atan2(aim.Y, aim.X);

    if (container.Player.IsDead)
    {
      Visible = false;
      SetPhysicsProcess(false);
    }
  }

  public void TakeVisualDamageFlash(float duration)
  {
    _damageFlashTimer = MathF.Max(_damageFlashTimer, duration);
  }

  public override void _Process(double delta)
  {
    if (_damageFlashTimer > 0f)
    {
      _damageFlashTimer -= (float)delta;
      Modulate = new Color(1f, 0.5f, 0.5f, 1f);
    }
    else
    {
      Modulate = new Color(1f, 1f, 1f, 1f);
    }
    if (_bombFlashRemaining > 0f) _bombFlashRemaining -= (float)delta;
  }

  private void ApplyBomb()
  {
    var origin = new Vector2(GlobalPosition.X, GlobalPosition.Y);
    _bombFlashRemaining = 0.3f;
    var enemies = GetTree().GetNodesInGroup("enemies");
    foreach (var node in enemies)
    {
      if (node is EnemyEntity enemy)
      {
        var pos = enemy.GlobalPosition;
        var dx = pos.X - origin.X;
        var dy = pos.Y - origin.Y;
        if (MathF.Sqrt(dx * dx + dy * dy) <= BombRadius)
        {
          enemy.ApplyDamage(BombDamage);
        }
      }
    }
  }

  private void OnPickupAreaEntered(Area2D area)
  {
    if (area is PickupEntity pickup)
    {
      BaseContainer.Instance.CollectPickup(pickup);
      pickup.QueueFree();
    }
  }
}
