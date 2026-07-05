using System.Collections.Generic;
using StructProject.Core.Logic.Player;
using StructProject.Core.Entities.Pickups;
using StructProject.Core.Logic.Pickups;

namespace StructProject.Testing.Tests;

public static class PlayerStateTests
{
  public static System.Action<string>? HandlesIFrames()
  {
    var logic = new PlayerStateLogic();
    var state = logic.Spawn(100);
    var next = logic.TakeDamage(state, 30);
    if (next.CurrentHealth != 70) return m => m = $"expected 70 hp, got {next.CurrentHealth}";
    var same = logic.TakeDamage(next, 30);
    if (same.CurrentHealth != 70) return m => m = "expected i-frame to block second hit";
    return null;
  }
}

public static class PowerUpTests
{
  public static System.Action<string>? AppliesAndExpires()
  {
    var logic = new PowerUpLogic();
    var state = PowerUpState.Empty;
    var withShield = logic.Apply(state, PickupKind.Shield, 1, 6f);
    if (!withShield.HasShield) return m => m = "shield not applied";
    var expired = logic.Update(withShield, 7.0);
    if (expired.HasShield) return m => m = "shield did not expire";
    return null;
  }

  public static System.Action<string>? MagnetAttractionFloats()
  {
    var logic = new PowerUpLogic();
    var withMagnet = logic.Apply(PowerUpState.Empty, PickupKind.Magnet, 250f, 5f);
    if (withMagnet.ActiveMagnetRadius <= 0f) return m => m = "magnet radius not set";
    return null;
  }
}

public static class PickupTests
{
  public static System.Action<string>? RollsWeighted()
  {
    var logic = new DropLogic();
    var kindCounts = new Dictionary<PickupKind, int>();
    for (var i = 0; i < 50; i++)
    {
      var table = new DropTableSnapshot(new List<DropEntry>
      {
        new DropEntry(PickupKind.Coin, 0.7f),
        new DropEntry(PickupKind.Health, 0.3f)
      });
      var evt = logic.Roll(0, 0, coinReward: 1, table);
      if (kindCounts.ContainsKey(evt.Kind)) kindCounts[evt.Kind]++;
      else kindCounts[evt.Kind] = 1;
    }
    var coinCount = kindCounts.ContainsKey(PickupKind.Coin) ? kindCounts[PickupKind.Coin] : 0;
    if (coinCount == 0) return m => m = "no coins rolled";
    return null;
  }
}

public static class CheatTests
{
  public static System.Action<string>? RecognisesFlags()
  {
    var parser = new StructProject.Infrastructure.Cheats.CheatParser();
    var args = new[] { "--god", "--coins", "100", "--wave", "3", "--oneshot", "--unknown" };
    var cmds = parser.Parse(args);
    if (cmds.Count != 4) return m => m = $"expected 4 cheats, got {cmds.Count}";
    return null;
  }
}
