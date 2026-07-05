using System.Collections.Generic;
using Godot;
using StructProject.Core.Shared.Enums;
using StructProject.Core.Shared.Service;

namespace StructProject.GodotPresentation.Scripts.Adapters;

public class GodotInputActionsAdapter : IInputActions
{
  private readonly IReadOnlyDictionary<PlayerActions, string> _actionMap;

  public GodotInputActionsAdapter(IReadOnlyDictionary<PlayerActions, string>? actionMap = null)
  {
    _actionMap = actionMap ?? DefaultActionMap();
  }

  public bool IsActionPressed(PlayerActions action)
  {
    if (!_actionMap.TryGetValue(action, out var godotAction))
      return false;

    if (!InputMap.HasAction(godotAction))
      return false;

    return Input.IsActionPressed(godotAction);
  }

  private static IReadOnlyDictionary<PlayerActions, string> DefaultActionMap()
  {
    return new Dictionary<PlayerActions, string>
    {
      [PlayerActions.MOVE_FORWARD] = "move_forward",
      [PlayerActions.MOVE_BACKWARD] = "move_backward",
      [PlayerActions.MOVE_LEFT] = "move_left",
      [PlayerActions.MOVE_RIGHT] = "move_right",
      [PlayerActions.JUMP] = "jump",
      [PlayerActions.CROUCH] = "crouch",
      [PlayerActions.ATTACK] = "attack",
    };
  }
}