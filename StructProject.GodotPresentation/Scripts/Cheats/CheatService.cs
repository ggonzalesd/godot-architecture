using System.Collections.Generic;
using System.Linq;
using StructProject.GodotPresentation.Scripts.Containers;
using StructProject.Core.Logic.Player;

namespace StructProject.GodotPresentation.Scripts.Cheats;

public class CheatService
{
  public bool GodMode { get; private set; }
  public float? EnemySpeedMultiplier { get; private set; }
  public bool OneShot { get; private set; }
  public bool SkipMenu { get; private set; }
  public int? CoinsToAdd { get; private set; }
  public int? TargetWave { get; private set; }
  public string? WeaponId { get; private set; }

  public void Apply(IEnumerable<Infrastructure.Cheats.CheatCommand> commands, BaseContainer container)
  {
    foreach (var cmd in commands)
    {
      switch (cmd.Kind)
      {
        case Infrastructure.Cheats.CheatKind.God:
          GodMode = true;
          break;
        case Infrastructure.Cheats.CheatKind.Coins:
          if (int.TryParse(cmd.Args.FirstOrDefault(), out var n)) CoinsToAdd = n;
          break;
        case Infrastructure.Cheats.CheatKind.Wave:
          if (int.TryParse(cmd.Args.FirstOrDefault(), out var w)) TargetWave = w;
          break;
        case Infrastructure.Cheats.CheatKind.Weapon:
          WeaponId = cmd.Args.FirstOrDefault();
          break;
        case Infrastructure.Cheats.CheatKind.EnemySpeed:
          if (float.TryParse(cmd.Args.FirstOrDefault(), out var s)) EnemySpeedMultiplier = s;
          break;
        case Infrastructure.Cheats.CheatKind.SkipMenu:
          SkipMenu = true;
          break;
        case Infrastructure.Cheats.CheatKind.OneShot:
          OneShot = true;
          break;
      }
    }

    if (CoinsToAdd.HasValue)
    {
      container.SetInventory(container.Inventory with { Coins = container.Inventory.Coins + CoinsToAdd.Value });
      container.Logger.Log("CHEAT: coins", CoinsToAdd);
    }
    if (TargetWave.HasValue)
    {
      var newWave = TargetWave.Value;
      container.Logger.Log("CHEAT: target wave", newWave);
    }
    if (GodMode) container.Logger.Log("CHEAT: --god enabled");
    if (EnemySpeedMultiplier.HasValue) container.Logger.Log("CHEAT: enemy-speed", EnemySpeedMultiplier);
    if (OneShot) container.Logger.Log("CHEAT: --oneshot enabled");
    if (SkipMenu) container.Logger.Log("CHEAT: --skip-menu enabled");
    if (!string.IsNullOrEmpty(WeaponId)) container.Logger.Log("CHEAT: weapon", WeaponId);
  }
}
