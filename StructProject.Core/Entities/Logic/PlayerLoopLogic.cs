using StructProject.Core.Entities.Models;
using StructProject.Core.Shared.Enums;
using StructProject.Core.Shared.Service;

namespace StructProject.Core.Entities.Logic;

public class PlayerLoopLogic
{
  public required IInputActions Inputs { get; init; }
  public required ILogger Logger { get; init; }
  public required Player Player { get; init; }

  public void Update()
  {
    Player.Level += 1; // Example logic for leveling up the player

    if (Inputs.IsActionPressed(PlayerActions.MOVE_FORWARD))
    {
      Player.Health -= 1; // Example logic for taking damage when moving forward
    }

    // Example logic for player loop
    if (Player.Health <= 0)
    {
      Logger.Log($"{Player.Name} has been defeated!");
    }
    else
    {
      Logger.Log($"{Player.Name} is at level {Player.Level} with {Player.Health} health.");
    }
  }
}
