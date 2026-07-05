using System;
using System.Collections.Generic;
using StructProject.Core.Entities.Enemies;
using StructProject.Core.Entities.Pickups;

namespace StructProject.Core.Logic.Pickups;

public class DropLogic
{
  private readonly Random _random = new();

  public DropEvent Roll(int originX, int originY, int coinReward, DropTableSnapshot? table)
  {
    var kind = PickupKind.Coin;
    var duration = 0f;
    var amount = coinReward;
    var cost = 0;

    if (table != null && table.Entries.Count > 0)
    {
      var totalWeight = 0f;
      foreach (var entry in table.Entries) totalWeight += entry.Weight;

      var roll = (float)_random.NextDouble() * totalWeight;
      var cumulative = 0f;
      foreach (var entry in table.Entries)
      {
        cumulative += entry.Weight;
        if (roll <= cumulative)
        {
          kind = entry.Kind;
          break;
        }
      }
    }

    switch (kind)
    {
      case PickupKind.Coin:
        amount = Math.Max(1, coinReward);
        cost = amount;
        break;
      case PickupKind.Health:
        amount = 1;
        cost = 25;
        duration = 0f;
        break;
      case PickupKind.Shield:
        amount = 1;
        cost = 0;
        duration = 6f;
        break;
      case PickupKind.DamageUp:
        amount = 25;
        cost = 0;
        duration = 8f;
        break;
      case PickupKind.FireRateUp:
        amount = 50;
        cost = 0;
        duration = 8f;
        break;
      case PickupKind.SpeedUp:
        amount = 35;
        cost = 0;
        duration = 6f;
        break;
      case PickupKind.Magnet:
        amount = 200;
        cost = 0;
        duration = 5f;
        break;
    }

    return new DropEvent(
      originX + _random.Next(-20, 20),
      originY,
      kind,
      duration,
      amount,
      cost
    );
  }
}
