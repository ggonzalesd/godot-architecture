using StructProject.Core.Shared.Models;

namespace StructProject.Core.Entities.Models;

public record PlayerBody(
  Func<Vec2> GetPosition,
  Func<Vec2> GetVelocity,
  Action<Vec2> ApplyVelocity,
  Action<Vec2> ApplyForce
)
{ }
