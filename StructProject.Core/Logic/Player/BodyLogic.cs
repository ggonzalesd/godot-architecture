using StructProject.Core.Entities.Player;
using StructProject.Core.Shared.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.Core.Logic.Player;

public class BodyLogic(
  IInputActions Inputs
)
{
  public void Update(double delta, Body body, IBinding binding)
  {
    if (Inputs.MovementPressed)
    {
      var axis = Inputs.Axis;

      binding.ApplyVelocity(new Vec2(
        X: axis.X * 200f,
        Y: axis.Y * 200f
      ));
    }
    else
    {

      var decayVector = new Vec2(
        X: body.Velocity.X * MathF.Min(1f, (float)delta * 7f),
        Y: body.Velocity.Y * MathF.Min(1f, (float)delta * 7f)
      );

      binding.ApplyVelocity(new Vec2(
        X: body.Velocity.X - decayVector.X,
        Y: body.Velocity.Y - decayVector.Y
      ));
    }
  }
}
