using System.Collections.Generic;
using StructProject.Core.Entities.Pickups;
using StructProject.GodotPresentation.Scripts.Data;

namespace StructProject.GodotPresentation.Scripts.Registries;

public sealed class PickupRegistry
{
  private readonly Dictionary<PickupKind, PickupConfig> _byKind = [];

  public void Register(PickupConfig config)
  {
    if (config == null) return;
    _byKind[config.Kind] = config;
  }

  public void RegisterRange(IEnumerable<PickupConfig> configs)
  {
    foreach (var c in configs) Register(c);
  }

  public PickupConfig? Get(PickupKind kind)
  {
    return _byKind.TryGetValue(kind, out var c) ? c : null;
  }

  public IReadOnlyCollection<PickupConfig> All => _byKind.Values;
}
