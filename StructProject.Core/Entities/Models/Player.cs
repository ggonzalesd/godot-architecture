using StructProject.Core.Shared.Models;

namespace StructProject.Core.Entities.Models;

public class Player(string name, int level, int health, Func<Position> getPosition)
{
  public string Name { get; set; } = name;
  public int Level { get; set; } = level;
  public int Health { get; set; } = health;

  // Function Getter Position Type
  public Func<Position> GetPosition { get; set; } = getPosition;

  public void TakeDamage(int damage)
  {
    Health -= damage;
    if (Health < 0)
    {
      Health = 0;
    }
  }

  public void Heal(int amount)
  {
    Health += amount;
  }
}
