using StructProject.Core.Entities.Player;
using StructProject.Core.Logic.Spawn;
using StructProject.Core.Shared.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.Core.Logic.Player;

public class ShootingLogic(
  IInputActions Inputs,
  ILogger Logger,
  IBulletSpawn BulletSpawn
)
{
  public Vec2 Update(double delta, Body body, Shooter shooter, IBinding binding)
  {
    shooter.CooldownRemaining = MathF.Max(0f, shooter.CooldownRemaining - (float)delta);

    shooter.Aim = ComputeAim(body.Position, Inputs.CursorPosition);

    if (Inputs.ShootPressed && shooter.IsReady)
    {
      shooter.CooldownRemaining = 1f / MathF.Max(0.0001f, shooter.Ratio);
      var origin = binding.GetMuzzle();
      BulletSpawn.SpawnBullet(origin, shooter.Aim, shooter.Speed, lifetime: 3f);
      shooter.ShotsFired++;
      Logger.Log($"Shoot at {origin} dir {shooter.Aim}");
    }

    return shooter.Aim;
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
