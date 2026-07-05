using System.Collections.Generic;
using StructProject.Core.Entities.Enemies;

namespace StructProject.Core.Entities.Pickups;

public record DropEntry(PickupKind Kind, float Weight);

public record DropTableSnapshot(IReadOnlyList<DropEntry> Entries);

public record DropEvent(
  int X,
  int Y,
  PickupKind Kind,
  float Duration,
  int Amount,
  int Cost
);
