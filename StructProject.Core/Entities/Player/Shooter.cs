using StructProject.Core.Shared.Models;

namespace StructProject.Core.Entities.Player;

public record Shooter(
  float Speed,
  float Ratio,
  Vec2 Aim
)
{
  public float CooldownRemaining = 0f;

  public bool IsReady => CooldownRemaining <= 0f;

  public int ShotsFired { get; set; } = 0;

  public Vec2 Aim { get; set; } = Aim;
}
