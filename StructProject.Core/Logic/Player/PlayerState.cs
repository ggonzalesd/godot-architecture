namespace StructProject.Core.Logic.Player;

public record PlayerState(
  int MaxHealth,
  int CurrentHealth,
  float InvulnerableRemaining
)
{
  public bool IsDead => CurrentHealth <= 0;
  public bool CanTakeDamage => InvulnerableRemaining <= 0f;

  public float HealthFraction => MaxHealth <= 0 ? 0f : (float)CurrentHealth / MaxHealth;
}

public class PlayerStateLogic
{
  public PlayerState Spawn(int maxHealth) => new(maxHealth, maxHealth, 0f);

  public PlayerState Update(PlayerState state, double delta)
  {
    if (state.InvulnerableRemaining <= 0f) return state;
    return state with { InvulnerableRemaining = MathF.Max(0f, state.InvulnerableRemaining - (float)delta) };
  }

  public PlayerState TakeDamage(PlayerState state, int amount, float iFrameDuration = 1f)
  {
    if (!state.CanTakeDamage) return state;
    var nextHp = Math.Max(0, state.CurrentHealth - amount);
    return state with
    {
      CurrentHealth = nextHp,
      InvulnerableRemaining = nextHp > 0 ? iFrameDuration : 0f
    };
  }

  public PlayerState Heal(PlayerState state, int amount)
  {
    var nextHp = Math.Min(state.MaxHealth, state.CurrentHealth + amount);
    return state with { CurrentHealth = nextHp };
  }

  public PlayerState UpgradeMaxHealth(PlayerState state, int amount)
  {
    return state with
    {
      MaxHealth = state.MaxHealth + amount,
      CurrentHealth = state.CurrentHealth + amount
    };
  }
}
