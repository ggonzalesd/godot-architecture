using StructProject.Core.Shared.Enums;

namespace StructProject.Core.Shared.Service;

public interface IInputActions
{
  public bool IsActionPressed(PlayerActions action);
}
