using StructProject.Core.Shared.Models;

namespace StructProject.Core.Shared.Service;

public interface IInputActions
{
  public bool ShootPressed { get; }
  public bool MovementPressed { get; }
  public Vec2 Axis { get; }

  public Vec2 CursorPosition { get; }
}
