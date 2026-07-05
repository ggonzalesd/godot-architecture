using System.Collections.Generic;
using StructProject.Core.Entities.Enemies;

namespace StructProject.Core.Logic.Spawn;

public interface IEnemySpawner
{
  void SpawnFromEvent(EnemySpawnEvent spawnEvent);
  IReadOnlyCollection<EnemySpawnEvent> ActiveEnemies { get; }
  void ClearAll();
}
