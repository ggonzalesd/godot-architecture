using StructProject.Core.Entities.Models;
using StructProject.Core.Shared.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.Core.Entities.Logic;

public class PlayerLoopLogic(
  IInputActions Inputs,
  ILogger Logger,
  Player Player
)
{
  public void Update(double delta)
  {
    if (Inputs.ShootPressed)
    {
      Logger.Log($"has shot! ${Inputs.CursorPosition} | ${Player.GetPosition()}");
    }

    if (Inputs.MovementPressed)
    {
      var axis = Inputs.Axis;

      Player.ApplyVelocity(new Vec2(
        X: axis.X * 200f,
        Y: axis.Y * 200f
      ));
    }
    else
    {
      var velocity = Player.GetVelocity();

      var decayVector = new Vec2(
        X: velocity.X * MathF.Min(1f, (float)delta * 7f),
        Y: velocity.Y * MathF.Min(1f, (float)delta * 7f)
      );

      Player.ApplyVelocity(new Vec2(
        X: velocity.X - decayVector.X,
        Y: velocity.Y - decayVector.Y
      ));
    }
  }
}
