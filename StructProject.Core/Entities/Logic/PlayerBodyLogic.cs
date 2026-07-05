using StructProject.Core.Entities.Models;
using StructProject.Core.Shared.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.Core.Entities.Logic;

public class PlayerBodyLogic(
  IInputActions Inputs
)
{
  public void Update(double delta, PlayerBody body)
  {
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
  }
}