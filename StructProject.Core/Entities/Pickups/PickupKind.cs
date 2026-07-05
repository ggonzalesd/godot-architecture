namespace StructProject.Core.Entities.Pickups;

public enum PickupKind
{
  Coin,
  Health,
  Shield,
  DamageUp,
  FireRateUp,
  SpeedUp,
  Magnet
}

public record PickupSnapshot(
  PickupKind Kind,
  float Duration,
  int Amount,
  int Cost
);
