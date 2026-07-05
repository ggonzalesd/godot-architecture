using StructProject.Core.Shared.Models;

namespace StructProject.Core.Shared.Service;

public interface IInputActions
{
  public bool ShootPressed { get; }
  public Position CursorPosition { get; }
}
