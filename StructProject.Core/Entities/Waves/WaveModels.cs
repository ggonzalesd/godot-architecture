using StructProject.Core.Entities.Enemies;

namespace StructProject.Core.Entities.Waves;

public record WaveSpawn(
  EnemyKindSnapshot Enemy,
  float Delay,
  int Count,
  float Interval,
  float LaneY,
  int Direction,
  float HealthMultiplier
);

public record WaveSnapshot(
  string DisplayName,
  float Duration,
  float TimeBeforeNext,
  float DifficultyMultiplier,
  IReadOnlyList<WaveSpawn> Spawns
);

public record WaveSequenceSnapshot(
  string DisplayName,
  IReadOnlyList<WaveSnapshot> Waves,
  bool LoopOnFinish,
  float LoopGrowthFactor
);
