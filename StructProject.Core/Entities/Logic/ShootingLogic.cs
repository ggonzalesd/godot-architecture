using StructProject.Core.Entities.Models;
using StructProject.Core.Shared.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.Core.Entities.Logic;

public class ShootingLogic(
  IInputActions Inputs,
  ILogger Logger
)
{
  private const float CooldownSeconds = 0.25f;

  private float _cooldownRemaining = 0f;

  public int ShotsFired { get; private set; } = 0;

  public bool IsReady => _cooldownRemaining <= 0f;

  public Vec2 Update(double delta, Vec2 playerPos, PlayerShooter shooter)
  {
    _cooldownRemaining = MathF.Max(0f, _cooldownRemaining - (float)delta);

    var aim = ComputeAim(playerPos, Inputs.CursorPosition);

    if (Inputs.ShootPressed && _cooldownRemaining <= 0f)
    {
      _cooldownRemaining = CooldownSeconds;
      var origin = shooter.GetMuzzle();
      shooter.SpawnBullet(origin, aim);
      ShotsFired++;
      Logger.Log($"Shoot at {origin} dir {aim}");
    }

    return aim;
  }

  private static Vec2 ComputeAim(Vec2 playerPos, Vec2 cursorPos)
  {
    var dx = cursorPos.X - playerPos.X;
    var dy = cursorPos.Y - playerPos.Y;
    var length = MathF.Sqrt(dx * dx + dy * dy);
    if (length > 0.0001f)
    {
      return new Vec2(dx / length, dy / length);
    }
    return Vec2.Zero;
  }
}
