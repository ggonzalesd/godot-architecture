using Godot;
using StructProject.Core.Logic.Spawn;
using StructProject.Core.Shared.Models;

namespace StructProject.GodotPresentation.Scripts.Adapters.Spawn;

public class GodotSpawnBullet(
  PackedScene BulletScene,
  Node OriginNode
) : IBulletSpawn
{
  public void SpawnBullet(Vec2 origin, Vec2 direction, float speed, float lifetime)
  {
    var bullet = BulletScene.Instantiate<RigidBody2D>();
    bullet.GlobalPosition = new Vector2(
      x: (float)origin.X,
      y: (float)origin.Y
    );
    bullet.LinearVelocity = new Vector2(
      x: (float)direction.X,
      y: (float)direction.Y
    ) * speed;
    bullet.Rotation = Mathf.Atan2((float)direction.Y, (float)direction.X);

    OriginNode.AddChild(bullet);

    var timer = new Timer
    {
      WaitTime = lifetime,
      OneShot = true,
      Autostart = true
    };
    timer.Timeout += () => bullet.QueueFree();
    bullet.AddChild(timer);
  }
}
