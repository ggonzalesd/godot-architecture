using StructProject.Core.Shared.Models;

namespace StructProject.Core.Entities.Models;

public record PlayerShooter(
  Func<Vec2> GetMuzzle,
  Action<Vec2, Vec2> SpawnBullet
)
{ }
