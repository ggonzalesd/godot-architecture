using StructProject.Core.Shared.Models;

namespace StructProject.Core.Entities.Player;

public record Shooter(
  float Speed,
  Action<Vec2, Vec2> SpawnBullet
)
{ }
