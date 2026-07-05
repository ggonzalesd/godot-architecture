using Godot;
using StructProject.Core.Logic.Spawn;
using StructProject.Core.Shared.Models;
using BulletEntity = StructProject.GodotPresentation.Scripts.Entities.Bullet;

namespace StructProject.GodotPresentation.Scripts.Adapters.Spawn;

public class GodotSpawnBullet(
  PackedScene BulletScene,
  Node OriginNode
) : IBulletSpawn
{
  public void SpawnBullet(Vec2 origin, Vec2 direction, float speed, float lifetime)
  {
    var bullet = BulletScene.Instantiate<BulletEntity>();
    bullet.GlobalPosition = new Vector2(
      x: (float)origin.X,
      y: (float)origin.Y
    );
    bullet.Velocity = new Vector2(
      x: (float)direction.X,
      y: (float)direction.Y
    ) * speed;
    bullet.LifetimeRemaining = lifetime;
    bullet.Rotation = Mathf.Atan2((float)direction.Y, (float)direction.X);

    OriginNode.AddChild(bullet);
  }
}
