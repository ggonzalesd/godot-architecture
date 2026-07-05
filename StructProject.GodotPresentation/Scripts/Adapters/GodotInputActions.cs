using Godot;
using StructProject.Core.Shared.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.GodotPresentation.Scripts.Adapters;

public class GodotInputActionsAdapter(Viewport viewport) : IInputActions
{
  public bool ShootPressed => Input.IsActionPressed("attack");

  public Vec2 CursorPosition
  {
    get
    {
      var mousePosition = viewport.GetMousePosition();
      return new Vec2(
        X: mousePosition.X,
        Y: mousePosition.Y
      );
    }
  }

  public bool MovementPressed =>
    Input.IsActionPressed("move_forward")
    || Input.IsActionPressed("move_backward")
    || Input.IsActionPressed("move_left")
    || Input.IsActionPressed("move_right");

  public Vec2 Axis
  {
    get
    {
      var axisVector = new Vector2(
        x: Input.GetAxis("move_left", "move_right"),
        y: Input.GetAxis("move_forward", "move_backward")
      ).Normalized();

      return new Vec2(
        X: axisVector.X,
        Y: axisVector.Y
      );
    }
  }
}