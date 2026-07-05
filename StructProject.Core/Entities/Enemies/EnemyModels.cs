namespace StructProject.Core.Entities.Enemies;

public record EnemyKindSnapshot(
  EnemyKind Kind,
  string DisplayName,
  int MaxHealth,
  float MoveSpeed,
  int ContactDamage,
  int ScoreValue,
  int CoinDrop,
  bool Flies,
  float ShootCooldown,
  float ShootSpeed,
  float ShootRange,
  int ShootDamage
);

public record EnemySpawn(
  EnemyKind Kind,
  float Delay,
  int Count,
  float Interval,
  float LaneY,
  int Direction,
  float HealthMultiplier
);

public record EnemySpawnEvent(
  EnemyKind Kind,
  float X,
  float Y,
  int Health,
  int ContactDamage,
  int ScoreValue,
  int CoinDrop,
  int Direction,
  bool Flies,
  float MoveSpeed,
  float ShootCooldown,
  float ShootSpeed,
  float ShootRange,
  int ShootDamage
);
