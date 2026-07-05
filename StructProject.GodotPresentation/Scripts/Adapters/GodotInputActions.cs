using Godot;
using StructProject.Core.Shared.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.GodotPresentation.Scripts.Adapters;

public class GodotInputActionsAdapter(Viewport viewport) : IInputActions
{
  public bool ShootPressed => Input.IsActionPressed("attack");

  public Position CursorPosition
  {
    get
    {
      var mousePosition = viewport.GetMousePosition();
      return new Position(
        X: mousePosition.X,
        Y: mousePosition.Y
      );
    }
  }
}