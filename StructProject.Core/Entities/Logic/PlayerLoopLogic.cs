using StructProject.Core.Entities.Models;
using StructProject.Core.Shared.Service;

namespace StructProject.Core.Entities.Logic;

public class PlayerLoopLogic(
  IInputActions Inputs,
  ILogger Logger,
  Player Player
)
{
  public void Update()
  {
    Player.Level += 1; // Example logic for leveling up the player

    if (Inputs.ShootPressed)
    {
      Logger.Log($"{Player.Name} has shot!");
      Player.Health -= 1; // Example logic for taking damage when shooting
    }
  }
}
