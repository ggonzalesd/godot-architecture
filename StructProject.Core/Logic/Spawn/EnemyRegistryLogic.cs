using System.Collections.Generic;
using StructProject.Core.Entities.Enemies;

namespace StructProject.Core.Logic.Spawn;

public class EnemyRegistryLogic
{
  private readonly Dictionary<int, EnemySpawnEvent> _active = new();
  public int NextId { get; private set; } = 1;

  public int Register(EnemySpawnEvent evt)
  {
    var id = NextId++;
    _active[id] = evt;
    return id;
  }

  public IReadOnlyCollection<EnemySpawnEvent> Active => _active.Values;

  public EnemySpawnEvent? Get(int id)
  {
    return _active.TryGetValue(id, out var e) ? e : null;
  }

  public void Unregister(int id)
  {
    _active.Remove(id);
  }

  public void Clear()
  {
    _active.Clear();
  }
}
