using StructProject.Core.Entities.Models;
using StructProject.Core.Shared.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.Core.Entities.Logic;

public class PlayerLoopLogic(
  IInputActions Inputs,
  ILogger Logger
)
{
  public Vec2 AimingDirection { get; private set; } = Vec2.Zero;

  public void Update(double delta, PlayerBody body)
  {
    if (Inputs.ShootPressed)
    {
      Logger.Log($"has shot! ${Inputs.CursorPosition} | ${body.GetPosition()}");
    }

    if (Inputs.MovementPressed)
    {
      var axis = Inputs.Axis;

      body.ApplyVelocity(new Vec2(
        X: axis.X * 200f,
        Y: axis.Y * 200f
      ));
    }
    else
    {
      var velocity = body.GetVelocity();

      var decayVector = new Vec2(
        X: velocity.X * MathF.Min(1f, (float)delta * 7f),
        Y: velocity.Y * MathF.Min(1f, (float)delta * 7f)
      );

      body.ApplyVelocity(new Vec2(
        X: velocity.X - decayVector.X,
        Y: velocity.Y - decayVector.Y
      ));
    }

    var playerPos = body.GetPosition();
    var cursor = Inputs.CursorPosition;
    var dx = cursor.X - playerPos.X;
    var dy = cursor.Y - playerPos.Y;
    var length = MathF.Sqrt(dx * dx + dy * dy);
    if (length > 0.0001f)
    {
      AimingDirection = new Vec2(dx / length, dy / length);
    }
  }
}
