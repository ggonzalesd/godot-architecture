using System.Collections.Generic;
using StructProject.Core.Entities.Enemies;
using StructProject.GodotPresentation.Scripts.Data;

namespace StructProject.GodotPresentation.Scripts.Registries;

public sealed class EnemyKindRegistry
{
  private readonly Dictionary<EnemyKind, EnemyKindConfig> _byKind = [];

  public void Register(EnemyKindConfig config)
  {
    if (config == null) return;
    _byKind[config.Kind] = config;
  }

  public void RegisterRange(IEnumerable<EnemyKindConfig> configs)
  {
    foreach (var c in configs) Register(c);
  }

  public EnemyKindConfig? Get(EnemyKind kind) => _byKind.TryGetValue(kind, out var c) ? c : null;

  public IReadOnlyCollection<EnemyKindConfig> All => _byKind.Values;
}
