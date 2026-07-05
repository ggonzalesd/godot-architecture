using StructProject.Core.Entities.Enemies;
using StructProject.Core.Shared.Models;

namespace StructProject.Core.Logic.Enemies;

public enum HorizontalIntent
{
  None,
  Left,
  Right
}

public enum VerticalIntent
{
  None,
  Jump,
  Fly
}

public record EnemyAiOutput(
  HorizontalIntent Horizontal,
  VerticalIntent Vertical,
  bool WantShoot,
  int ShootDamage,
  float ShootSpeed,
  float ProjectileLifetime = 3f
)
{
  public static EnemyAiOutput Idle { get; } = new(
    HorizontalIntent.None,
    VerticalIntent.None,
    false,
    0,
    0f
  );
}

public class EnemyAiLogic
{
  public EnemyAiOutput Decide(EnemyInstance enemy, in Vec2 playerPos)
  {
    var dx = playerPos.X - enemy.Spawn.X;
    var absDx = MathF.Abs(dx);
    var horizontal = dx < 0 ? HorizontalIntent.Left : HorizontalIntent.Right;

    if (enemy.Spawn.Flies)
    {
      var vertical = enemy.Spawn.Y > playerPos.Y ? VerticalIntent.None : VerticalIntent.None;
      if (enemy.ShootCooldownRemaining <= 0f && absDx <= enemy.Spawn.ShootRange * 32f && enemy.Spawn.ShootDamage > 0)
      {
        return new EnemyAiOutput(horizontal, vertical, true, enemy.Spawn.ShootDamage, enemy.Spawn.ShootSpeed);
      }
      return new EnemyAiOutput(horizontal, vertical, false, 0, 0f);
    }

    if (absDx < 50f && enemy.ShootCooldownRemaining <= 0f && enemy.Spawn.ShootDamage > 0)
    {
      return new EnemyAiOutput(HorizontalIntent.None, VerticalIntent.None, true, enemy.Spawn.ShootDamage, enemy.Spawn.ShootSpeed);
    }

    if (absDx < 50f)
    {
      return new EnemyAiOutput(HorizontalIntent.None, VerticalIntent.Jump, false, 0, 0f);
    }

    if (enemy.ShootCooldownRemaining <= 0f && absDx <= enemy.Spawn.ShootRange * 32f && enemy.Spawn.ShootDamage > 0)
    {
      return new EnemyAiOutput(HorizontalIntent.None, VerticalIntent.None, true, enemy.Spawn.ShootDamage, enemy.Spawn.ShootSpeed);
    }

    return new EnemyAiOutput(horizontal, VerticalIntent.None, false, 0, 0f);
  }

  public EnemyInstance AdvanceCooldown(EnemyInstance enemy, double delta)
  {
    if (enemy.ShootCooldownRemaining <= 0f) return enemy;
    var next = MathF.Max(0f, enemy.ShootCooldownRemaining - (float)delta);
    return enemy with { ShootCooldownRemaining = next };
  }

  public EnemyInstance OnShotFired(EnemyInstance enemy)
  {
    return enemy with { ShootCooldownRemaining = enemy.Spawn.ShootCooldown };
  }

  public EnemyInstance ApplyDamage(EnemyInstance enemy, int damage)
  {
    return enemy with { CurrentHealth = Math.Max(0, enemy.CurrentHealth - damage) };
  }
}
