using StructProject.Core.Shared.Models;

namespace StructProject.Core.Entities.Player;

public record Body(
  Vec2 Position,
  Vec2 Velocity
)
{ }
