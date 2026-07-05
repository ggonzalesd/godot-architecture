using Godot;
using PlayerBody = StructProject.Core.Entities.Player.Body;
using PlayerShooter = StructProject.Core.Entities.Player.Shooter;
using PlayerBinding = StructProject.Core.Entities.Player.Binding;
using StructProject.Core.Shared.Models;
using StructProject.GodotPresentation.Scripts.Containers;
using CorePosition = StructProject.Core.Shared.Models.Vec2;
using StructProject.GodotPresentation.Scripts.Adapters.Players;

namespace StructProject.GodotPresentation.Scripts.Entities;

public partial class Player : RigidBody2D
{
  private const float BulletLifetime = 3f;

  [ExportGroup("Inside Nodes")]
  [Export]
  private Node2D CannotPivot { get; set; } = null!;
  [Export]
  private Node2D SpawnPoint { get; set; } = null!;

  [ExportGroup("Outside Nodes")]
  [Export]
  private PackedScene BulletScene { get; set; } = null!;

  private PlayerBody PlayerBody = null!;
  private PlayerShooter PlayerShooter = null!;
  private PlayerBinding PlayerBinding = null!;

  [ExportGroup("Player Settings")]
  [Export]
  private float BulletSpeed { get; set; } = 600f;

  public override void _Ready()
  {
    PlayerBinding = new GodotPlayerBinding(
      rigidBody: this,
      spawnPoint: SpawnPoint
    );

    PlayerBody = new PlayerBody(
      Position: new CorePosition(X: GlobalPosition.X, Y: GlobalPosition.Y),
      Velocity: new CorePosition(X: LinearVelocity.X, Y: LinearVelocity.Y)
    );

    PlayerShooter = new PlayerShooter(
      Speed: BulletSpeed,
      SpawnBullet: SpawnBullet
    );
  }

  public override void _Process(double delta)
  {
    PlayerBody = PlayerBody with
    {
      Position = new CorePosition(X: GlobalPosition.X, Y: GlobalPosition.Y),
      Velocity = new CorePosition(X: LinearVelocity.X, Y: LinearVelocity.Y)
    };

    PlayerShooter = PlayerShooter with
    {
      Speed = BulletSpeed
    };

    BaseContainer.Instance.PlayerBodyLogic.Update(
      delta: delta,
      body: PlayerBody,
      binding: PlayerBinding
    );

    var aim = BaseContainer.Instance.Shooting.Update(
      delta: delta,
      body: PlayerBody,
      shooter: PlayerShooter,
      binding: PlayerBinding
    );

    CannotPivot.Rotation = Mathf.Atan2(aim.Y, aim.X);
  }

  private CorePosition GetMuzzle()
    => new(
      X: SpawnPoint.GlobalPosition.X,
      Y: SpawnPoint.GlobalPosition.Y
    );

  private void SpawnBullet(Vec2 origin, Vec2 direction)
  {
    var bullet = BulletScene.Instantiate<RigidBody2D>();
    bullet.GlobalPosition = new Vector2(
      x: (float)origin.X,
      y: (float)origin.Y
    );
    bullet.LinearVelocity = new Vector2(
      x: (float)direction.X,
      y: (float)direction.Y
    ) * BulletSpeed;
    bullet.Rotation = Mathf.Atan2((float)direction.Y, (float)direction.X);

    GetTree().CurrentScene.AddChild(bullet);

    var timer = new Timer
    {
      WaitTime = BulletLifetime,
      OneShot = true,
      Autostart = true
    };
    timer.Timeout += () => bullet.QueueFree();
    bullet.AddChild(timer);
  }
}