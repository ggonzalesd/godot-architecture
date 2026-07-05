using System.Collections.Generic;

namespace StructProject.Core.Logic.Player;

public record PowerUpState(
  bool HasShield,
  float DamageMultiplier,
  float FireRateMultiplier,
  float SpeedMultiplier,
  float MagnetRadius,
  float RemainingShieldTime,
  float RemainingDamageTime,
  float RemainingFireRateTime,
  float RemainingSpeedTime,
  float RemainingMagnetTime
)
{
  public static PowerUpState Empty { get; } = new(
    HasShield: false,
    DamageMultiplier: 1f,
    FireRateMultiplier: 1f,
    SpeedMultiplier: 1f,
    MagnetRadius: 0f,
    RemainingShieldTime: 0f,
    RemainingDamageTime: 0f,
    RemainingFireRateTime: 0f,
    RemainingSpeedTime: 0f,
    RemainingMagnetTime: 0f
  );

  public float ActiveDamageMultiplier => RemainingDamageTime > 0f ? DamageMultiplier : 1f;
  public float ActiveFireRateMultiplier => RemainingFireRateTime > 0f ? FireRateMultiplier : 1f;
  public float ActiveSpeedMultiplier => RemainingSpeedTime > 0f ? SpeedMultiplier : 1f;
  public float ActiveMagnetRadius => RemainingMagnetTime > 0f ? MagnetRadius : 0f;
}

public class PowerUpLogic
{
  public PowerUpState Apply(PowerUpState state, StructProject.Core.Entities.Pickups.PickupKind kind, float amount, float duration)
  {
    return kind switch
    {
      StructProject.Core.Entities.Pickups.PickupKind.Shield => state with
      {
        HasShield = true,
        RemainingShieldTime = duration
      },
      StructProject.Core.Entities.Pickups.PickupKind.DamageUp => state with
      {
        DamageMultiplier = 1f + amount / 100f,
        RemainingDamageTime = duration
      },
      StructProject.Core.Entities.Pickups.PickupKind.FireRateUp => state with
      {
        FireRateMultiplier = 1f + amount / 100f,
        RemainingFireRateTime = duration
      },
      StructProject.Core.Entities.Pickups.PickupKind.SpeedUp => state with
      {
        SpeedMultiplier = 1f + amount / 100f,
        RemainingSpeedTime = duration
      },
      StructProject.Core.Entities.Pickups.PickupKind.Magnet => state with
      {
        MagnetRadius = amount,
        RemainingMagnetTime = duration
      },
      _ => state
    };
  }

  public PowerUpState Update(PowerUpState state, double delta)
  {
    var dt = (float)delta;
    var next = state;
    if (next.RemainingShieldTime > 0f)
    {
      next = next with { RemainingShieldTime = MathF.Max(0f, next.RemainingShieldTime - dt) };
      if (next.RemainingShieldTime <= 0f) next = next with { HasShield = false };
    }
    if (next.RemainingDamageTime > 0f)
      next = next with { RemainingDamageTime = MathF.Max(0f, next.RemainingDamageTime - dt) };
    if (next.RemainingFireRateTime > 0f)
      next = next with { RemainingFireRateTime = MathF.Max(0f, next.RemainingFireRateTime - dt) };
    if (next.RemainingSpeedTime > 0f)
      next = next with { RemainingSpeedTime = MathF.Max(0f, next.RemainingSpeedTime - dt) };
    if (next.RemainingMagnetTime > 0f)
      next = next with { RemainingMagnetTime = MathF.Max(0f, next.RemainingMagnetTime - dt) };

    return next;
  }
}

public record InventoryState(int Coins);

public class InventoryLogic
{
  public InventoryState AddCoins(InventoryState inv, int amount)
  {
    return inv with { Coins = inv.Coins + amount };
  }

  public InventoryState SpendCoins(InventoryState inv, int amount)
  {
    return inv with { Coins = Math.Max(0, inv.Coins - amount) };
  }
}
