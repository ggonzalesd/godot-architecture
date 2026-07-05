using StructProject.Core.Logic.Player;

namespace StructProject.Core.Logic.Meta;

public enum MetaUpgradeKind
{
  MaxHealth,
  Damage,
  Speed
}

public record MetaUpgrade(
  MetaUpgradeKind Kind,
  string DisplayName,
  int Level,
  int MaxLevel,
  int CostBase,
  int CostMultiplier,
  int ValuePerLevel
);

public record MetaProgressionState(System.Collections.Generic.Dictionary<MetaUpgradeKind, MetaUpgrade> Upgrades)
{
  public static MetaProgressionState Default { get; } = new(new System.Collections.Generic.Dictionary<MetaUpgradeKind, MetaUpgrade>
  {
    [MetaUpgradeKind.MaxHealth] = new(MetaUpgradeKind.MaxHealth, "Vitality", 0, 5, 30, 30, 5),
    [MetaUpgradeKind.Damage]    = new(MetaUpgradeKind.Damage,    "Power",    0, 5, 40, 40, 10),
    [MetaUpgradeKind.Speed]     = new(MetaUpgradeKind.Speed,     "Agility",  0, 5, 30, 30, 5)
  });
}

public class MetaProgressionLogic
{
  public int GetCost(MetaUpgrade upgrade) =>
    upgrade.CostBase + upgrade.Level * upgrade.CostMultiplier;

  public bool CanAfford(InventoryState inv, MetaUpgrade upgrade) =>
    inv.Coins >= GetCost(upgrade);

  public (InventoryState inv, MetaProgressionState prog) Purchase(
    InventoryState inv, MetaProgressionState prog, MetaUpgradeKind kind)
  {
    if (!prog.Upgrades.TryGetValue(kind, out var upgrade)) return (inv, prog);
    if (upgrade.Level >= upgrade.MaxLevel) return (inv, prog);
    var cost = GetCost(upgrade);
    if (inv.Coins < cost) return (inv, prog);

    var nextInv = new InventoryLogic().SpendCoins(inv, cost);
    var nextUpgrade = upgrade with { Level = upgrade.Level + 1 };
    var nextUpgrades = new System.Collections.Generic.Dictionary<MetaUpgradeKind, MetaUpgrade>(prog.Upgrades)
    {
      [kind] = nextUpgrade
    };
    return (nextInv, prog with { Upgrades = nextUpgrades });
  }
}
