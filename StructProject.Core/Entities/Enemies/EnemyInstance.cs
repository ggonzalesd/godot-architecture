using StructProject.Core.Entities.Enemies;
using StructProject.Core.Shared.Models;

namespace StructProject.Core.Entities.Enemies;

public record EnemyInstance(
  EnemySpawnEvent Spawn,
  int CurrentHealth,
  float ShootCooldownRemaining
)
{
  public bool IsDead => CurrentHealth <= 0;

  public float DistanceTo(in Vec2 pos)
  {
    var dx = pos.X - Spawn.X;
    var dy = pos.Y - Spawn.Y;
    return MathF.Sqrt(dx * dx + dy * dy);
  }
}
