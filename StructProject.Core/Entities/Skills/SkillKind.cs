namespace StructProject.Core.Entities.Skills;

public enum SkillKind
{
  Dash,
  Bomb,
  SlowMotion
}

public record SkillSnapshot(
  SkillKind Kind,
  string DisplayName,
  float Cooldown,
  float Duration,
  float Power,
  int Cost
);
