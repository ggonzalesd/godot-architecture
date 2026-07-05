using Godot;
using PlayerBody = StructProject.Core.Entities.Player.Body;
using PlayerShooter = StructProject.Core.Entities.Player.Shooter;
using PlayerBinding = StructProject.Core.Entities.Player.IBinding;
using JumpStateT = StructProject.Core.Entities.Player.JumpState;
using StructProject.Core.Shared.Models;
using StructProject.GodotPresentation.Scripts.Containers;
using CorePosition = StructProject.Core.Shared.Models.Vec2;
using StructProject.GodotPresentation.Scripts.Adapters.Players;

namespace StructProject.GodotPresentation.Scripts.Entities;

public partial class Player : CharacterBody2D
{

  [ExportGroup("Inside Nodes")]
  [Export]
  private Node2D CannotPivot { get; set; } = null!;
  [Export]
  private Node2D SpawnPoint { get; set; } = null!;

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
    PlayerBinding = new GodotPlayerBinding(
      body: this,
      spawnPoint: SpawnPoint
    );

    PlayerBody = new PlayerBody(
      Position: new CorePosition(X: GlobalPosition.X, Y: GlobalPosition.Y),
      Velocity: new CorePosition(X: Velocity.X, Y: Velocity.Y)
    );

    PlayerShooter = new PlayerShooter(
      Speed: BulletSpeed,
      Ratio: BulletRatio,
      Aim: CorePosition.Zero
    );

    Jump = new JumpStateT();
  }

  public override void _PhysicsProcess(double delta)
  {
    var currentVelocity = Velocity;

    PlayerBody = PlayerBody with
    {
      Position = new CorePosition(X: GlobalPosition.X, Y: GlobalPosition.Y),
      Velocity = new CorePosition(X: currentVelocity.X, Y: currentVelocity.Y)
    };

    PlayerShooter = PlayerShooter with
    {
      Speed = BulletSpeed,
      Ratio = BulletRatio
    };

    BaseContainer.Instance.PlayerBodyLogic.Update(
      delta: delta,
      body: PlayerBody,
      binding: PlayerBinding,
      jump: Jump
    );

    var aim = BaseContainer.Instance.Shooting.Update(
      delta: delta,
      body: PlayerBody,
      shooter: PlayerShooter,
      binding: PlayerBinding
    );

    CannotPivot.Rotation = Mathf.Atan2(aim.Y, aim.X);
  }
}
