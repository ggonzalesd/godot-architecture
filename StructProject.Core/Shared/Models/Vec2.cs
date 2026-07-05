namespace StructProject.Core.Shared.Models;

public record Vec2(float X, float Y)
{
  public static Vec2 Zero { get; } = new(0f, 0f);
}
