using System.Collections.Generic;
using StructProject.Core.Entities.Enemies;
using StructProject.Core.Entities.Waves;
using StructProject.GodotPresentation.Scripts.Data;

namespace StructProject.GodotPresentation.Scripts.Mappers;

public static class WaveSequenceMapper
{
  public static WaveSequenceSnapshot ToSnapshot(WaveSequence sequence)
  {
    var waves = new List<WaveSnapshot>(sequence.Waves.Count);
    foreach (var wave in sequence.Waves)
    {
      waves.Add(ToSnapshot(wave));
    }

    return new WaveSequenceSnapshot(
      sequence.DisplayName,
      waves,
      sequence.LoopOnFinish,
      sequence.LoopGrowthFactor
    );
  }

  public static WaveSnapshot ToSnapshot(WaveData wave)
  {
    var spawns = new List<WaveSpawn>(wave.Spawns.Count);
    foreach (var spawn in wave.Spawns)
    {
      if (spawn.Enemy == null) continue;
      spawns.Add(new WaveSpawn(
        spawn.Enemy.ToSnapshot(),
        spawn.Delay,
        spawn.Count,
        spawn.Interval,
        spawn.LaneY,
        spawn.Direction,
        spawn.HealthMultiplier
      ));
    }

    return new WaveSnapshot(
      wave.DisplayName,
      wave.Duration,
      wave.TimeBeforeNext,
      wave.DifficultyMultiplier,
      spawns
    );
  }
}
