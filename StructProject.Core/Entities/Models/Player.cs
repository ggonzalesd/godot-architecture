namespace StructProject.Core.Entities.Models;

public class Player(string name, int level, int health)
{
  public string Name { get; set; } = name;
  public int Level { get; set; } = level;
  public int Health { get; set; } = health;
  public Position Position { get; set; } = Position.Zero;

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
