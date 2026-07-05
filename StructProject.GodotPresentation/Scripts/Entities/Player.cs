using Godot;
using PlayerBody = StructProject.Core.Entities.Player.Body;
using PlayerShooter = StructProject.Core.Entities.Player.Shooter;
using PlayerBinding = StructProject.Core.Entities.Player.IBinding;
using JumpStateT = StructProject.Core.Entities.Player.JumpState;
using StructProject.Core.Shared.Models;
using StructProject.GodotPresentation.Scripts.Adapters.Players;
using StructProject.GodotPresentation.Scripts.Containers;
using PickupEntity = StructProject.GodotPresentation.Scripts.Entities.Pickup;

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

  [ExportGroup("Player Settings")]
  [Export]
  private float BulletSpeed { get; set; } = 900f;
  [Export]
  private float BulletRatio { get; set; } = 8f;

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

    var currentVelocity = Velocity;

    PlayerBody = PlayerBody with
    {
      Position = new Vec2(GlobalPosition.X, GlobalPosition.Y),
      Velocity = new Vec2(currentVelocity.X, currentVelocity.Y)
    };

    var powerUp = BaseContainer.Instance.PowerUp;
    PlayerShooter = PlayerShooter with
    {
      Speed = BulletSpeed,
      Ratio = BulletRatio * powerUp.ActiveFireRateMultiplier
    };

    BaseContainer.Instance.PlayerBodyLogic.Update(
      delta,
      PlayerBody,
      PlayerBinding,
      Jump
    );

    var aim = BaseContainer.Instance.Shooting.Update(
      delta,
      PlayerBody,
      PlayerShooter,
      PlayerBinding
    );

    CannotPivot.Rotation = Mathf.Atan2(aim.Y, aim.X);

    if (BaseContainer.Instance.Player.IsDead)
    {
      Visible = false;
      SetPhysicsProcess(false);
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
