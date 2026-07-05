using Godot;
using StructProject.Core.Entities.Models;
using StructProject.Core.Shared.Models;
using StructProject.GodotPresentation.Scripts.Containers;
using CorePosition = StructProject.Core.Shared.Models.Vec2;

namespace StructProject.GodotPresentation.Scripts.Entities;

public partial class Player : RigidBody2D
{
  private const float BulletSpeed = 600f;
  private const float BulletLifetime = 3f;

  [ExportGroup("Inside Nodes")]
  [Export]
  private Node2D CannotPivot { get; set; } = null!;
  [Export]
  private Node2D SpawnPoint { get; set; } = null!;

  [ExportGroup("Outside Nodes")]
  [Export]
  private PackedScene BulletScene { get; set; } = null!;

  private PlayerBody _body = null!;
  private PlayerShooter _shooter = null!;

  public override void _Ready()
  {
    _body = new PlayerBody(
      GetPosition: () => new CorePosition(
        X: Position.X,
        Y: Position.Y
      ),
      GetVelocity: () => new CorePosition(
        X: LinearVelocity.X,
        Y: LinearVelocity.Y
      ),
      ApplyForce: (force) => ApplyCentralForce(
        force: new Vector2(force.X, force.Y)
      ),
      ApplyVelocity: (velocity) => LinearVelocity = new Vector2(
        x: velocity.X,
        y: velocity.Y
      )
    );

    _shooter = new PlayerShooter(
      GetMuzzle: GetMuzzle,
      SpawnBullet: SpawnBullet
    );
  }

  public override void _Process(double delta)
  {
    var container = BaseContainer.Instance;

    container.PlayerBodyLogic.Update(
      delta: delta,
      body: _body
    );

    var aim = container.Shooting.Update(
      delta: delta,
      playerPos: _body.GetPosition(),
      shooter: _shooter
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