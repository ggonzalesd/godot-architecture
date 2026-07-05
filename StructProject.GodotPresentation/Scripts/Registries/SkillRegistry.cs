using System.Collections.Generic;
using StructProject.Core.Entities.Skills;
using StructProject.GodotPresentation.Scripts.Data;

namespace StructProject.GodotPresentation.Scripts.Registries;

public sealed class SkillRegistry
{
  private readonly Dictionary<SkillKind, SkillConfig> _byKind = [];

  public void Register(SkillConfig config)
  {
    if (config == null) return;
    _byKind[config.Kind] = config;
  }

  public void RegisterRange(IEnumerable<SkillConfig> configs)
  {
    foreach (var c in configs) Register(c);
  }

  public SkillConfig? Get(SkillKind kind) => _byKind.TryGetValue(kind, out var c) ? c : null;

  public IReadOnlyDictionary<SkillKind, Core.Entities.Skills.SkillSnapshot> ToSnapshotDictionary()
  {
    var result = new Dictionary<SkillKind, Core.Entities.Skills.SkillSnapshot>();
    foreach (var (kind, cfg) in _byKind)
    {
      result[kind] = cfg.ToSnapshot();
    }
    return result;
  }
}
