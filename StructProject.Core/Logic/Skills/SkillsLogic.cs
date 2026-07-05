using System.Collections.Generic;
using StructProject.Core.Entities.Skills;

namespace StructProject.Core.Logic.Skills;

public record SkillSlotState(
  SkillKind Kind,
  float CooldownRemaining,
  float ActiveRemaining,
  bool TriggeredThisFrame
)
{
  public bool IsReady => CooldownRemaining <= 0f;
  public float CooldownFraction01 => CooldownRemaining <= 0f ? 1f : 1f - CooldownRemaining / 5f;
}

public record PlayerSkillsState(
  Dictionary<SkillKind, SkillSlotState> Slots,
  float TimeScale
)
{
  public static PlayerSkillsState Create(IEnumerable<(SkillKind, float)> cooldowns, float initialTimeScale = 1f)
  {
    var slots = new Dictionary<SkillKind, SkillSlotState>();
    foreach (var (kind, cd) in cooldowns)
    {
      slots[kind] = new SkillSlotState(kind, 0f, 0f, false);
    }
    return new PlayerSkillsState(slots, initialTimeScale);
  }
}

public class SkillsLogic
{
  public PlayerSkillsState Update(PlayerSkillsState state, double delta, IReadOnlyDictionary<SkillKind, SkillSnapshot> registry)
  {
    var next = state;
    var slots = new Dictionary<SkillKind, SkillSlotState>(state.Slots.Count);
    foreach (var (kind, slot) in state.Slots)
    {
      var snapshot = registry.TryGetValue(kind, out var s) ? s : default;
      var cd = MathF.Max(0f, slot.CooldownRemaining - (float)delta);
      var active = slot.ActiveRemaining > 0f ? MathF.Max(0f, slot.ActiveRemaining - (float)delta) : 0f;
      slots[kind] = slot with { CooldownRemaining = cd, ActiveRemaining = active };
    }
    next = next with { Slots = slots };

    var slowMax = 0f;
    foreach (var (kind, slot) in slots)
    {
      if (slot.ActiveRemaining > 0f && kind == SkillKind.SlowMotion)
      {
        if (registry.TryGetValue(kind, out var s))
        {
          slowMax = MathF.Max(slowMax, MathF.Min(1f, 0.4f + (1f - s.Power) * 0.5f));
        }
      }
    }
    next = next with { TimeScale = slowMax > 0 ? slowMax : 1f };
    return next;
  }

  public PlayerSkillsState Trigger(PlayerSkillsState state, SkillKind kind, IReadOnlyDictionary<SkillKind, SkillSnapshot> registry)
  {
    if (!registry.TryGetValue(kind, out var s)) return state;
    if (!state.Slots.TryGetValue(kind, out var slot) || slot.CooldownRemaining > 0f) return state;

    var slots = new Dictionary<SkillKind, SkillSlotState>(state.Slots);
    slots[kind] = slot with
    {
      CooldownRemaining = s.Cooldown,
      ActiveRemaining = s.Duration,
      TriggeredThisFrame = true
    };
    return state with { Slots = slots };
  }

  public PlayerSkillsState ConsumeTrigger(PlayerSkillsState state, SkillKind kind)
  {
    if (!state.Slots.TryGetValue(kind, out var slot)) return state;
    var slots = new Dictionary<SkillKind, SkillSlotState>(state.Slots);
    slots[kind] = slot with { TriggeredThisFrame = false };
    return state with { Slots = slots };
  }
}
